﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
namespace Sampoerna.EMS.XMLReader
{
    public class XmlCK5DataWriter     {
        public void CreateXML(CK5 ck5)
        {
           //ZAIDM_EX_MATERIAL material1 = new ZAIDM_EX_MATERIAL();
           //material1.STICKER_CODE = "wwewe";
           //material1.SKEP_NP = "dsdsdsd";
           //material1.MARKET_ID = 123;

           //ZAIDM_EX_MATERIAL material2 = new ZAIDM_EX_MATERIAL();
           //material2.STICKER_CODE = "xxxxx";
           //material2.SKEP_NP = "dsdsdsd";
           //material2.MARKET_ID = 124;
           //CK5 ck5 = new CK5();
           //ck5.CK5_ID = 101;
           ////ck5.CK5_NUMBER = "CK5OOOP0023";
           //ck5.CK5_TYPE = Sampoerna.EMS.Core.Enums.CK5Type.Domestic;
           //CK5_MATERIAL ck5Materialmaterial1 = new CK5_MATERIAL();
           //ck5Materialmaterial1.CK5 = ck5;
           //ck5Materialmaterial1.ZAIDM_EX_MATERIAL = material1;
           //CK5_MATERIAL ck5Materialmaterial2 = new CK5_MATERIAL();
           //ck5Materialmaterial2.CK5 = ck5;
           //ck5Materialmaterial2.ZAIDM_EX_MATERIAL = material2;

           // var listMaterial = new List<CK5_MATERIAL>();
           // listMaterial.Add(ck5Materialmaterial1);
           // listMaterial.Add(ck5Materialmaterial2);

            using (XmlWriter writer = XmlWriter.Create(@"H:\ck5_test.xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("ZAIDM_CK5_01");
                writer.WriteStartElement("IDOC");
                writer.WriteAttributeString("BEGIN", "1");
                
               
                writer.WriteStartElement("ZAIDM_CK5_HDR");
                writer.WriteAttributeString("SEGMENT", "1");

                writer.WriteElementString("CK5_NUMBER", ck5.REGISTRATION_NUMBER);
                writer.WriteElementString("CK5_PROCS_TYP", ck5.CK5_TYPE.ToString());
               
                //foreach (var item in listMaterial)  // <-- This is new
                //{
                //    writer.WriteStartElement("ITEM");
                //    writer.WriteElementString("STICKER_CODE", item.ZAIDM_EX_MATERIAL.STICKER_CODE);
                //    writer.WriteElementString("SKEP_NP", item.ZAIDM_EX_MATERIAL.SKEP_NP);
                //    writer.WriteEndElement();


                //}

                //ZAIDM_CK5_HDR
                writer.WriteEndElement();
                //IDOC
                writer.WriteEndElement();
                //ZAIDM_CK5_01
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

        }
    }
}
