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

        
    }
}
