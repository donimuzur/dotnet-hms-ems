using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Sampoerna.EMS.Contract;

namespace Sampoerna.EMS.XMLReader
{
    class Program
    {
         static void Main(string[] args)
        {
            var timer = new Stopwatch();
            timer.Start();
            //IXmlDataReader xmlData = new XmlAreaDataMapper();
            //IXmlDataReader xmlData = new XmlPoaDataMapper();
            //IXmlDataReader xmlData = new XmlMarketDataMapper();
             //IXmlDataReader xmlData = new XmlPlantDataMapper();
            //IXmlDataReader xmlData = new XmlPoaMapDataMapper();
            // IXmlDataReader xmlData = new XmlUserDataMapper();
            // IXmlDataReader xmlData = new XmlSeriesDataMapper();
             //IXmlDataReader xmlData = new XmlProdTypeDataMapper();
             //IXmlDataReader xmlData = new XmlKPPBCDataMapper();
             //IXmlDataReader xmlData = new XmlPCodeDataMapper();
             //IXmlDataReader xmlData = new XmlGoodsTypeDataMapper();
             string RootPath= ConfigurationManager.AppSettings["XmlFolderPath"];
        
             var xmlfiles = Directory.GetFiles(RootPath);

             foreach (var xmlfile in xmlfiles)
             {
                 if (xmlfile.Contains("MATERIAL"))
                 {
                     IXmlDataReader xmlData = new XmlMaterialDataMapper(xmlfile);
                     xmlData.InsertToDatabase();
                 }

             }
             //IXmlDataWriter xmlWriter = new XmlCK5DataWriter();
            //xmlWriter.CreateXML();
            timer.Stop();
            Console.WriteLine(timer.ElapsedMilliseconds);
            Console.ReadLine();
        }
    }
}
