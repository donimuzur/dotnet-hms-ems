using System;
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

        public Service()
        {
            
            xmlfiles = Directory.GetFiles(inboundPath).OrderBy(x => x).ToArray();
        }

        private IXmlDataReader XmlReaderFactory(string xmlfile)
        {
            if (xmlfile.Contains("COY"))
            {
                return new XmlCompanyDataMapper(xmlfile);
            }
            if (xmlfile.Contains("T001K"))
            {
                return new XmlT001KDataMapper(xmlfile);
            }
            if (xmlfile.Contains("UOM"))
            {
                return new XmlUoMDataMapper(xmlfile);
            }
            if (xmlfile.Contains("NPPBKC"))
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
            else if (xmlfile.Contains("T001W"))
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

        public List<string> Run()
        {
            var errorList = new List<string>();
            foreach (var xmlfile in xmlfiles)
            {
                try
                {
                    IXmlDataReader reader = XmlReaderFactory(xmlfile);
                  
                    if (reader != null)
                    {
                        reader.InsertToDatabase();
                    }
                }
                catch (Exception ex)
                {
                    var error = ex.Message;
                    if (ex.InnerException != null)
                    {
                        error = ex.InnerException.Message;
                    }
                    errorList.Add(string.Format("<b>File: {0} </b> -> Error : {1}", xmlfile, error));
                    continue;
                }
            }
            return errorList;
        }

     
       
        
    }
}
