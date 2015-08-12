﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.Contract;

namespace Sampoerna.EMS.XMLReader
{
    public class Service
    {
        private IXmlDataReader reader = null;
        private readonly string inboundPath = ConfigurationManager.AppSettings["XmlInboundPath"];
        private string[] xmlfiles = null;
        public List<string> filesMoved;
        public Service()
        {
            
            xmlfiles = Directory.GetFiles(inboundPath).OrderBy(x => x).ToArray();
            filesMoved = new List<string>();
        }

        private IXmlDataReader XmlReaderFactory(string xmlfile)
        {
            if (xmlfile.Contains("POA"))
            {
                return new XmlPoaDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("COY"))
            {
                return new XmlCompanyDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("PLANTV"))
            {
                return new XmlT001KDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("UOM"))
            {
                return new XmlUoMDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("NPPBKC"))
            {
                return new XmlNPPBKCDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("KPPBC"))
            {
                return new XmlKPPBCDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("VENDOR"))
            {
                return new XmlVendorDataMapper(xmlfile);
            }
           
            else if (xmlfile.Contains("MARKET"))
            {
                return  new XmlMarketDataMapper(xmlfile);
            }
             else if (xmlfile.Contains("PRODTYP"))
             {
                 return new XmlProdTypeDataMapper(xmlfile);
             }
             else if (xmlfile.Contains("PCODE"))
             {
                 return new XmlPCodeDataMapper(xmlfile);
             }
             else if (xmlfile.Contains("SERIES"))
             {
                 return new XmlSeriesDataMapper(xmlfile);
             }
            else if (xmlfile.Contains("PLANT-"))
            {
                return new XmlPlantDataMapper(xmlfile);
            }
             else if (xmlfile.Contains("GOODTYP"))
             {
                 return new XmlGoodsTypeDataMapper(xmlfile);
             }
            else if (xmlfile.Contains("BRANDREG"))
            {
                return new XmlBrandDataMapper(xmlfile);
            }
            else if (xmlfile.Contains("MATERIAL"))
            {
                return new XmlMaterialDataMapper(xmlfile);
            }
            return null;
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
            if (!isComplete)
                return null;
           
            return orderedXmlFiles;
        }

        public List<string> Run()
        {
            var errorList = new List<string>();
            var orderedFile = OrderFile();
            
            
            if (orderedFile != null)
            {
                foreach (var xmlfile in orderedFile)
                {
                    try
                    {
                        IXmlDataReader reader = XmlReaderFactory(xmlfile);

                        if (reader != null)
                        {
                            filesMoved.Add(reader.InsertToDatabase());
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
            return errorList;
        }

     
       
        
    }
}
