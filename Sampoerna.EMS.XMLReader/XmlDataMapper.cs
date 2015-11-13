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
                        needMoved = false;
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

                foreach (var item in items)
                {
                    itemToInsert++;
                    repo.InsertOrUpdate(item);
                    
                    
                }

                if (Errors.Count == 0)
                {
                    uow.SaveChanges();    
                }
                
              
            }
            catch (Exception ex)
            {
                errorCount++;
                logger.Error(ex.ToString());
                this.Errors.Add(ex.Message);
                //uow.RevertChanges();
            }
            //if (errorCount == 0 && itemToInsert > 0)
            //{
            //    fileName = MoveFile();
            //    return fileName;
            //}
            if (errorCount == 0 && itemToInsert > 0 && Errors.Count == 0)
            {
                fileName = MoveFile();
                return new MovedFileOutput(fileName);
            }
            fileName = MoveFile(true,needMoved);
            return new MovedFileOutput(fileName, true);

            

        }
        public void InsertOrUpdate<T>(T entity) where T: class 
        {
            var repo = uow.GetGenericRepository<T>();

            repo.InsertOrUpdate(entity);
            uow.SaveChanges();


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
                logger.Error(ex.ToString());
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
            if (valueStr.Length == 10)
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
            logger.Debug(String.Format("processing field : {0} value = {1}", element.Name.LocalName, element.Value));
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

    }

    public class FileStatus
    {
        public string FileName { get; set; }
        public bool IsError { get; set; }

        public FileStatus(string fileName, bool isError = false)
        {
            this.FileName = fileName;
            this.IsError = isError;
        }
    }
}
