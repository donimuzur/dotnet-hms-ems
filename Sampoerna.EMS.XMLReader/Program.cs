using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.XMLReader
{
    class Program
    {
        
         static void Main(string[] args)
        {

            Service svc = new Service();
            svc.Run(true);
           /* XmlCK5DataWriter rt= new XmlCK5DataWriter();
             var ck5 = new CK5();
             ck5.REGISTRATION_NUMBER = "8100000011";
             ck5.CK5_TYPE = Enums.CK5Type.Domestic;
             ck5.SOURCE_PLANT_ID = "ID01";
             ck5.DEST_PLANT_ID = "ID02";
             ck5.GI_DATE = DateTime.Now;
             ck5.GR_DATE = DateTime.Now.AddDays(-1);
             ck5.CK5_MATERIAL = new List<CK5_MATERIAL>();
               ck5.CK5_MATERIAL.Add(new CK5_MATERIAL()
               {
                   LINE_ITEM = 1,
                   BRAND = "22.8011",
                   CONVERTED_QTY = 1000,
                   CONVERTED_UOM = "G"
                   
               });
               ck5.CK5_MATERIAL.Add(new CK5_MATERIAL()
               {
                   LINE_ITEM = 10,
                   BRAND = "22.8022",
                   CONVERTED_QTY = 1,
                   CONVERTED_UOM = "G"

               });
             rt.CreateXML(ck5, @"H:\ck5_file.xml");*/
             Console.ReadLine();
        }
    }
}
