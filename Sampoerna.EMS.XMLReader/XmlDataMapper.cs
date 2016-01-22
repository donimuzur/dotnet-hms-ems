using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Configuration;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Voxteneo.WebComponents.Logger;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.DAL;
using Voxteneo.WebCompoments.NLogLogger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.XMLReader
{
    public class XmlDataMapper
    {
        public XElement _xmlData = null;
        public string _xmlName = null;
        public ILogger logger;
        public IUnitOfWork uow;
        public List<String> Errors;
        private int _errorCount;
        public string lastField = null;
    
        public XmlDataMapper(string xmlName)
        {
            
            _xmlName = xmlName;
            _xmlData = ReadXMLFile();
            logger = new NLogLogger();
            uow = new SqlUnitOfWork(logger);
            Errors = new List<string>();
            logger.Info(String.Format("Processing file {0}", _xmlName));
        }

        public void AddError(int error)
        {
            this._errorCount += error;
        }

        private XElement ReadXMLFile()
        {
            
            if (_xmlName == null)
                return null;
            if (!File.Exists(_xmlName))
                return null;
            
            return XElement.Load(_xmlName);
        }



        public XElement GetElement(string elementName)
        {
            if (_xmlData == null)
                return null;
            
             return _xmlData.Element(elementName);
        }
        public IEnumerable<XElement> GetElements(string elementName)
        {
            if (_xmlData == null)
                return null;

            return _xmlData.Elements(elementName);
        }

        public string NoInsert<T>(List<T> items) where T : class
        {
            return MoveFile();
           
        }

        public MovedFileOutput InsertToDatabase<T>(List<T> items) where T : class
        {
            var repo = uow.GetGenericRepository<T>();
            var xmlRepo = uow.GetGenericRepository<XML_LOGS>();
            var xmllogs = GetXmlLogs(_xmlName);
            var errorCount = 0;
            var itemToInsert = 0;
            var fileName = string.Empty;
            var needMoved = true;
            try
            {
                var existingData = repo.Get();
                foreach (var item in existingData)
                {
                    if (item is PRODUCTION || item is INVENTORY_MOVEMENT)
                        needMoved = true;
                    if (item is LFA1)
                        continue;

                    //if (item is USER)
                    //{
                    //    var is_active = item.GetType().GetProperty("IS_ACTIVE");
                    //    if (is_active != null)
                    //    {
                    //        item.GetType().GetProperty("IS_ACTIVE").SetValue(item, 0);
                    //        repo.Update(item);
                    //    }
                    //}

                    var isFromSap = item.GetType().GetProperty("IS_FROM_SAP") != null && (bool)item.GetType().GetProperty("IS_FROM_SAP").GetValue(item);
                    if (!isFromSap)
                        continue;


                    var is_deleted = item.GetType().GetProperty("IS_DELETED");
                    if (is_deleted != null)
                    {
                        item.GetType().GetProperty("IS_DELETED").SetValue(item, true);
                        repo.Update(item);
                        //uow.SaveChanges();
                    }

                    
                    

                }

                

                

                if (Errors.Count == 0)
                {
                    
                    foreach (var item in items)
                    {
                        itemToInsert++;
                        repo.InsertOrUpdate(item);


                    }

                    if (xmllogs != null)
                    {
                        xmllogs.STATUS = Enums.XmlLogStatus.Success;
                    }

                    
                }
                else
                {
                    
                    if (xmllogs == null)
                    {
                        xmllogs = new XML_LOGS();
                        xmllogs.XML_FILENAME = _xmlName.Split('\\')[_xmlName.Split('\\').Length - 1];
                        xmllogs.XML_LOGS_DETAILS = new List<XML_LOGS_DETAILS>();
                        
                        
                        xmllogs.CREATED_BY = "PI";
                        xmllogs.CREATED_DATE = DateTime.Now;

                    }
                    foreach (var error in Errors)
                    {
                        XML_LOGS_DETAILS detailError = new XML_LOGS_DETAILS();
                        logger.Warn(error);
                        detailError.ERROR_TIME = DateTime.Now;
                        detailError.LOGS = error;
                        xmllogs.XML_LOGS_DETAILS.Add(detailError);

                    }
                    xmllogs.STATUS = Enums.XmlLogStatus.Error;
                    xmllogs.LAST_ERROR_TIME = DateTime.Now;
                    xmlRepo.InsertOrUpdate(xmllogs);
                    //uow.SaveChanges();
                    
                }
                uow.SaveChanges();
                
              
            }
            catch (Exception ex)
            {
                errorCount++;
                logger.Warn(ex.Message);
                this.Errors.Add(ex.Message);
                //uow.RevertChanges();
            }
            //if (errorCount == 0 && itemToInsert > 0)
            //{
            //    fileName = MoveFile();
            //    return fileName;
            //}
            if (errorCount == 0 && Errors.Count == 0)
            {
                fileName = MoveFile();
                return new MovedFileOutput(fileName);
            }
            
            fileName = MoveFile(true, needMoved);
            Errors.Insert(0, String.Format("Last field read : {0}", lastField));
            return new MovedFileOutput(fileName, true, Errors);
            
            

            

        }
        public void InsertOrUpdate<T>(T entity) where T: class 
        {
            var repo = uow.GetGenericRepository<T>();
            try
            {
                repo.InsertOrUpdate(entity);
                uow.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                uow.RevertChanges();
                
            }
            


        }
        public void InsertToDatabase<T>(T data) where T : class
        {
            var repo = uow.GetGenericRepository<T>();

            try
            {
                
               repo.InsertOrUpdate(data);

                
                uow.SaveChanges();
            }

            catch (Exception ex)
            {
                logger.Error(ex.Message);
                uow.RevertChanges();
            }

            MoveFile();
        }
        
        public string MoveFile(bool isError=false,bool isNeedMoving = true)
        {
            var filenameMoved = string.Empty;
            try
            {
                string sourcePath = _xmlName;
                string archievePath;
                if (!isError)
                {
                    archievePath = ConfigurationManager.AppSettings["XmlArchievePath"];
                }
                else
                {
                    archievePath = ConfigurationManager.AppSettings["XmlErrorPath"];
                }
                var sourcefileName = Path.GetFileName(sourcePath);
                var destPath = Path.Combine(archievePath, sourcefileName);
                if (File.Exists(destPath))
                    return null;
                if(isNeedMoving)
                    File.Move(sourcePath, destPath);
                return sourcefileName;
            }
            catch (Exception ex)
            {
                
                throw;
            }
          
        }

        public DateTime? GetDate(string valueStr)
        {
            lastField = String.Format("input = {0}", valueStr);
            if (valueStr.Length == 8)
            {
                var year = Convert.ToInt32(valueStr.Substring(0, 4));
                var month = Convert.ToInt32(valueStr.Substring(4, 2));
                var date = Convert.ToInt32(valueStr.Substring(6, 2));
                return new DateTime(year, month, date);
            }
            return null;
        }
        public DateTime? GetDateDotSeparator(string valueStr)
        {
            lastField = String.Format("input = {0}", valueStr);
            if (!string.IsNullOrEmpty(valueStr) && valueStr.Length == 10)
            {
                var year = Convert.ToInt32(valueStr.Substring(6, 4));
                var month = Convert.ToInt32(valueStr.Substring(3, 2));
                var date = Convert.ToInt32(valueStr.Substring(0, 2));
                return new DateTime(year, month, date);
            }
            return null;
        }
        public string GetElementValue(XElement element)
        {
            
            if (element == null)
                return null;
            //logger.Debug(String.Format("processing field : {0} value = {1}", element.Name.LocalName, element.Value));
            lastField = String.Format("{0} value = {1}", element.Name.LocalName, element.Value);
            if (element.Value == "/")
                return null;
            return element.Value;

           
        }
        public string GetRomanNumeralValue(XElement element)
        {
            if (element == null)
                return null;
            if (element.Value == "/")
                return null;
            string romanValue = null ;
              
            switch (element.Value)
            {
                  case  "01":
                    romanValue = "I";
                    break;
                    
                case  "02":
                    romanValue = "II";
                    break;
                    
                case  "03":
                    romanValue = "III";
                    break;
                    
                case  "04":
                    romanValue = "IV";
                    break;
                    
                case  "05":
                        romanValue = "V";
                        break;
                        
                case  "06":
                        romanValue = "VI";
                        break;
                        
                 case  "07":
                        romanValue = "VII";
                        break;
                        
            case  "08":
                        romanValue = "VIII";
                        break;
                        
             case  "09":
                        romanValue = "IX";
                        break;
                        
              case  "10":
                        romanValue = "X";
                        break;
               
                    

            }
            return romanValue;


        }

        public string GetInnerException(Exception ex)
        {
            string result = "";

            if (ex.InnerException != null)
            {
                result = GetInnerException(ex.InnerException);
            }
            else
            {
                result = ex.Message;
            }

            return result;
        }


        public XML_LOGS GetXmlLogs(string filename)
        {
            var xmlLogs = uow.GetGenericRepository<XML_LOGS>()
                .Get(x=> x.XML_FILENAME == filename,null,"XML_LOGS_DETAILS").FirstOrDefault();

            return xmlLogs;
        }

    }

    
}
