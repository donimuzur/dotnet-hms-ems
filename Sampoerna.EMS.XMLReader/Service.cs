using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Newtonsoft.Json;

namespace Sampoerna.EMS.XMLReader
{
    public class Service
    {
        private IXmlDataReader reader = null;
        private readonly string inboundPath = ConfigurationManager.AppSettings["XmlInboundPath"];
        private readonly string schedulerjsonPath = ConfigurationManager.AppSettings["SchedulerConfigJson"];
        private SchedulerConfigJson configjson = new SchedulerConfigJson();
        private string[] xmlfiles = null;
        public List<MovedFileOutput> filesMoved;
        public Service()
        {
            
            xmlfiles = new DirectoryInfo(inboundPath).GetFiles().OrderBy(x => x.LastWriteTime).Select(x => x.FullName).ToArray();
            

            
            filesMoved = new List<MovedFileOutput>();
        }

        private IXmlDataReader XmlReaderFactoryDaily(string xmlfile)
        {
            configjson = (SchedulerConfigJson)JsonConvert.DeserializeObject(File.ReadAllText(schedulerjsonPath), typeof(SchedulerConfigJson));
            if (xmlfile.Contains("POA") && configjson.IsRead.POA)
            {
                if (xmlfile.Contains("POAMAP") && configjson.IsRead.POAMAP)
                {
                    return new XmlPoaMapDataMapper(xmlfile);
                }
                
                return new XmlPoaDataMapper(xmlfile);
                

            }
            else if (xmlfile.Contains("BRANDREG") && configjson.IsRead.BRANDREG)
            {
                return new XmlBrandDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("CK5") && configjson.IsRead.CK5)
            {
                return new XmlCk5DataMapper(xmlfile);
            }
            else if (xmlfile.Contains("InvMovement") && configjson.IsRead.InvMovement)
            {
                return new XmlMovementDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("BOMMAP") && configjson.IsRead.BOMMAP)
            {
                return new XmlBOMDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("BLOCKSTOCK") && configjson.IsRead.BLOCKSTOCK)
            {
                return new XmlBlockStockDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("PRDOUTPUT") && configjson.IsRead.PRDOUTPUT)
            {
                return new XmlProdOutputDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("CK1") && configjson.IsRead.CK1)
            {
                return new XmlCK1DataMapper(xmlfile);
            }
            else if (xmlfile.Contains("PAYMENT") && configjson.IsRead.PAYMENT)
            {
                return new XmlPaymentDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("USER") && configjson.IsRead.USER)
            {
                return new XmlUserDataMapper(xmlfile);
            }

            return null;
        }
        private IXmlDataReader XmlReaderFactoryMonthly(string xmlfile)
        {
            configjson = (SchedulerConfigJson)JsonConvert.DeserializeObject(File.ReadAllText(schedulerjsonPath), typeof(SchedulerConfigJson));
            if (xmlfile.Contains("COY") && configjson.IsRead.COY)
            {
                return new XmlCompanyDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("PLANTV") && configjson.IsRead.T001K)
            {
                return new XmlT001KDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("UOM") && configjson.IsRead.UOM)
            {
                return new XmlUoMDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("NPPBKC") && configjson.IsRead.NPPBKC)
            {
                return new XmlNPPBKCDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("KPPBC") && configjson.IsRead.KPPBC)
            {
                return new XmlKPPBCDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("VENDOR") && configjson.IsRead.VENDOR)
            {
                return new XmlVendorDataMapper(xmlfile);
            }

            else if (xmlfile.Contains("MARKET") && configjson.IsRead.MARKET)
            {
                return new XmlMarketDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("PRODTYP") && configjson.IsRead.PRODTYP)
            {
                return new XmlProdTypeDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("PCODE") && configjson.IsRead.PCODE)
            {
                return new XmlPCodeDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("SERIES") && configjson.IsRead.SERIES)
            {
                return new XmlSeriesDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("PLANT-") && configjson.IsRead.T001W)
            {
                return new XmlPlantDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("GOODTYP") && configjson.IsRead.GOODTYP)
            {
                return new XmlGoodsTypeDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("MATERIAL") && configjson.IsRead.MATERIAL)
            {
                return new XmlMaterialDataMapper(xmlfile);
            }
            

            return null;
        }

        private void DailyOnceFactory()
        {
            
        }

        private void SendMailQuotaPending()
        {
            
        }

        private List<string> OrderFile()
        {
            bool isComplete = true;
            var orderedXmlFiles =new List<string>();
           
            var filesVendor = xmlfiles.Where(x => x.Contains("VENDOR"));
            orderedXmlFiles.AddRange(filesVendor);
            
            var filesSeries = xmlfiles.Where(x => x.Contains("SERIES"));
            orderedXmlFiles.AddRange(filesSeries);

            var filesMarket = xmlfiles.Where(x => x.Contains("MARKET"));
            orderedXmlFiles.AddRange(filesMarket);

            var filesGoodType = xmlfiles.Where(x => x.Contains("GOODTYP"));
            orderedXmlFiles.AddRange(filesGoodType);

            var filesProdType = xmlfiles.Where(x => x.Contains("PRODTYP"));
            orderedXmlFiles.AddRange(filesProdType);

            var filesPCode = xmlfiles.Where(x => x.Contains("PCODE"));
            orderedXmlFiles.AddRange(filesPCode);

            var filesPlant = xmlfiles.Where(x => x.Contains("PLANT-"));
            orderedXmlFiles.AddRange(filesPlant);

            var filesCompany = xmlfiles.Where(x => x.Contains("COY"));
            orderedXmlFiles.AddRange(filesCompany);

            var filesT001K = xmlfiles.Where(x => x.Contains("PLANTV-"));
            orderedXmlFiles.AddRange(filesT001K);

            var filesUom = xmlfiles.Where(x => x.Contains("UOM"));
            orderedXmlFiles.AddRange(filesUom);

            var filesKppbc = xmlfiles.Where(x => x.Contains("KPPBC"));
            orderedXmlFiles.AddRange(filesKppbc);

            var filesNppbkc = xmlfiles.Where(x => x.Contains("NPPBKC"));
            orderedXmlFiles.AddRange(filesNppbkc);

            var filesMaterial = xmlfiles.Where(x => x.Contains("MATERIAL"));
            orderedXmlFiles.AddRange(filesMaterial);

            var filesBrandReg = xmlfiles.Where(x => x.Contains("BRANDREG"));
            orderedXmlFiles.AddRange(filesBrandReg);

            var filesPOA = xmlfiles.Where(x => x.Contains("POA"));
            orderedXmlFiles.AddRange(filesPOA);

            var filesUser = xmlfiles.Where(x => x.Contains("USER"));
           
            orderedXmlFiles.AddRange(filesUser);

            if (filesVendor.Count() == 0)
                isComplete = false;
            if (filesSeries.Count() == 0)
                isComplete = false;
            if (filesMarket.Count() == 0)
                isComplete = false;
              
            if (filesGoodType.Count() == 0)
                isComplete = false;
            if (filesPCode.Count() == 0)
                isComplete = false;
            if (filesProdType.Count() == 0)
                isComplete = false;
            if (filesUom.Count() == 0)
                isComplete = false;
            if (filesPlant.Count() == 0)
                isComplete = false;
            if (filesCompany.Count() == 0)
                isComplete = false;
            if (ConfigurationManager.AppSettings["FileComplete"] != null)
            {
                if (ConfigurationManager.AppSettings["FileComplete"] == "1")
                {
                    if (!isComplete)
                        return null;
                }
            }

            return orderedXmlFiles;
        }

        public List<MovedFileOutput> Run(SchedulerEnums.Schedule isDaily)
        {
            var errorList = new List<string>();
            //var movedFileList = new List<MovedFileOutput>();
            var orderedFile = new List<string>();
            
            if (isDaily != SchedulerEnums.Schedule.Daily)
            {
                orderedFile = OrderFile();
                if (isDaily == SchedulerEnums.Schedule.DailyOnce)
                {
                    //Daily Once
                    var ck1Check = new BrandCk1CheckService();
                    ck1Check.BrandCheckProcessCk1();

                    var ck5Check = new BrandCk5CheckService();
                    ck5Check.BrandCheckProcessCk5();
                }
            }
            else
            {
                orderedFile = xmlfiles == null ? null : xmlfiles.ToList();
                var schedulerEmail = new SchedulerEmailService();
                schedulerEmail.CheckAndSendEmailQuotaMonitoring();
            }

            IXmlDataReader reader =null;
             
            if (orderedFile != null)
            {
                foreach (var xmlfile in orderedFile)
                {
                    try
                    {
                        if (isDaily == SchedulerEnums.Schedule.Daily)
                        {
                            reader = XmlReaderFactoryDaily(xmlfile);
                            
                        }
                        else if (isDaily == SchedulerEnums.Schedule.MonthLy)
                        {
                            reader = XmlReaderFactoryMonthly(xmlfile);
                        }
                        else
                        {
                            
                        }

                        if (reader != null)
                        {
                            var fileIsMoved = reader.InsertToDatabase();
                            if (!string.IsNullOrEmpty(fileIsMoved.FileName) && fileIsMoved.IsError)
                            {
                                filesMoved.Add(fileIsMoved);
                                //movedFileList.Add();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var error = ex.ToString();

                        errorList.Add(string.Format("<b>File: {0} </b> -> Error : {1}", xmlfile, error));
                        continue;
                    }
                }
            }
            if (reader != null)
            {
                errorList.AddRange(reader.GetErrorList());
                if (errorList.Count > 0)
                {
                    var generalError = new MovedFileOutput("General error", true, errorList);
                    filesMoved.Add(generalError);
                }
            }
            return filesMoved;
        }

     
       
        
    }
}
