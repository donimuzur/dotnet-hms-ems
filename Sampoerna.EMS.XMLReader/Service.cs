using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
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


        public void PoaRunning()
        {
            var files = xmlfiles.Where(x => x.Contains("POA-"));
            foreach (var xmlfile in files)
            {
                reader = new XmlPoaDataMapper(xmlfile);
                reader.InsertToDatabase();
                
            }
        }
        public void PoaMapRunning()
        {
            var files = xmlfiles.Where(x => x.Contains("POAMAP"));
           
            foreach (var xmlfile in files)
            {
                
                reader = new XmlPoaMapDataMapper(xmlfile);
                reader.InsertToDatabase();
                
            }
        }
        public void CompanyRunning()
        {
            
            var files = xmlfiles.Where(x => x.Contains("COY"));
           
            foreach (var xmlfile in files)
            {
                
                reader = new XmlCompanyDataMapper(xmlfile);
                reader.InsertToDatabase();
                
            }
        }
        public void KPPBCRunning()
        {

            var files = xmlfiles.Where(x => x.Contains("KPPBC"));

            foreach (var xmlfile in files)
            {

                reader = new XmlKPPBCDataMapper(xmlfile);
                reader.InsertToDatabase();

            }
        }
        public void NPPBKCRunning()
        {

            var files = xmlfiles.Where(x => x.Contains("NPPBKC"));

            foreach (var xmlfile in files)
            {

                reader = new XmlNPPBKCDataMapper(xmlfile);
                reader.InsertToDatabase();

            }
        }
        public void VendorRunning()
        {

            var files = xmlfiles.Where(x => x.Contains("VENDOR"));

            foreach (var xmlfile in files)
            {

                reader = new XmlVendorDataMapper(xmlfile);
                reader.InsertToDatabase();

            }
        }
        public void UoMRunning()
        {

            var files = xmlfiles.Where(x => x.Contains("UOM"));

            foreach (var xmlfile in files)
            {

                reader = new XmlUoMDataMapper(xmlfile);
                reader.InsertToDatabase();

            }
        }

        public void MarketRunning()
        {

            var files = xmlfiles.Where(x => x.Contains("MARKET"));

            foreach (var xmlfile in files)
            {

                reader = new XmlMarketDataMapper(xmlfile);
                reader.InsertToDatabase();

            }
        }

        public void SeriesRunning()
        {

            var files = xmlfiles.Where(x => x.Contains("SERIES"));

            foreach (var xmlfile in files)
            {

                reader = new XmlSeriesDataMapper(xmlfile);
                reader.InsertToDatabase();

            }
        }

        public void ProdTypeRunning()
        {

            var files = xmlfiles.Where(x => x.Contains("PRODTYP"));

            foreach (var xmlfile in files)
            {

                reader = new XmlProdTypeDataMapper(xmlfile);
                reader.InsertToDatabase();

            }
        }

        public void PCodeRunning()
        {

            var files = xmlfiles.Where(x => x.Contains("PCODE"));

            foreach (var xmlfile in files)
            {

                reader = new XmlPCodeDataMapper(xmlfile);
                reader.InsertToDatabase();

            }
        }

        public void PlantRunning()
        {

            var files = xmlfiles.Where(x => x.Contains("PLANT-"));

            foreach (var xmlfile in files)
            {

                reader = new XmlPlantDataMapper(xmlfile);
                reader.InsertToDatabase();

            }
        }

        public void BrandRunning()
        {

            var files = xmlfiles.Where(x => x.Contains("BRANDREG"));

            foreach (var xmlfile in files)
            {

                reader = new XmlBrandDataMapper(xmlfile);
                reader.InsertToDatabase();

            }
        }

        public void MaterialRunning()
        {

            var files = xmlfiles.Where(x => x.Contains("MATERIAL"));

            foreach (var xmlfile in files)
            {

                reader = new XmlMaterialDataMapper(xmlfile);
                reader.InsertToDatabase();

            }
        }

        public void GoodTypeRunning()
        {

            var files = xmlfiles.Where(x => x.Contains("GOODT"));

            foreach (var xmlfile in files)
            {

                reader = new XmlGoodsTypeDataMapper(xmlfile);
                reader.InsertToDatabase();

            }
        }
        
    }
}
