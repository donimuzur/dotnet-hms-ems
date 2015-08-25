using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using NLog.Internal;
using NLog.Targets.Wrappers;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace Sampoerna.EMS.XMLReader
{
    public class XmlCK5DataWriter
    {

        private string GetDateFormat(DateTime? date)
        {
            var result = DateTime.MinValue;
            if (date == null)
                return null;
            else
            {
                result =Convert.ToDateTime(date);
            }
            //var monthFormat = result.Month < 10 ? "0" + result.Month : result.Month.ToString();
            //return string.Format("{0}{1}{2}", result.Year, monthFormat, result.Day);
            return result.ToString("yyyyMMdd");
        }

        private string GetTimeFormat(DateTime? date)
        {
            var result = DateTime.MinValue;
            if (date == null)
                return null;
            else
            {
                result = Convert.ToDateTime(date);
            }
            //var monthFormat = result.Month < 10 ? "0" + result.Month : result.Month.ToString();
            //return string.Format("{0}{1}{2}", result.Year, monthFormat, result.Day);
            return result.ToString("hhmmss");
        }

        private string GetLinesItem(int? line)
        {
            if (line == null)
                return null;

            var lineValid = Convert.ToInt32(line);
            var result = string.Empty;
            if (line < 10)
                return result += "0000" + lineValid;
            if (line < 100)
                return result += "000" + lineValid;
            if (line < 1000)
                return result += "00" + lineValid;
            if (line < 10000)
                return result += "0" + lineValid;
            return null;
        }

        private string SetNullValue(object value)
        {
            if (value == null)
            {
                return "/";
            }
            return value.ToString();
        }

        private string MappingCk5Type(Sampoerna.EMS.Core.Enums.CK5Type type)
        {
            switch (type)
            {
                 case Enums.CK5Type.Domestic:
                    return "01";
                 case Enums.CK5Type.Intercompany:
                    return "02";
                 case Enums.CK5Type.ImporterToPlant:
                    return "03";
                 case Enums.CK5Type.PortToImporter:
                    return "04";
                 case Enums.CK5Type.Export:
                    return "05";
            }
            return null;
            
        }

    
        public void CreateCK5Xml(CK5XmlDto ck5XmlDto)
        {
            using (XmlWriter writer = XmlWriter.Create(ck5XmlDto.Ck5PathXml))
            {

               

                writer.WriteStartElement("ZAIDM_CK5_01");
                writer.WriteStartElement("IDOC");
                writer.WriteAttributeString("BEGIN", "1");

                writer.WriteStartElement("EDI_DC40");
                writer.WriteAttributeString("SEGMENT", "1");
                writer.WriteElementString("TABNAM", SetNullValue(null));
                writer.WriteElementString("MANDT", SetNullValue(null));
                writer.WriteElementString("DOCNUM", SetNullValue(null));
                writer.WriteElementString("DOCREL", SetNullValue(null));
                writer.WriteElementString("STATUS", SetNullValue(null));
                writer.WriteElementString("DIRECT", SetNullValue(null));
                writer.WriteElementString("OUTMOD", SetNullValue(null));
                writer.WriteElementString("IDOCTYP", "ZAIDM_CK5_01");
                writer.WriteElementString("MESTYP", "ZAIDM_EMS_CK5");
                writer.WriteElementString("MESCOD", "EMS");
                writer.WriteElementString("SNDPOR", SetNullValue(null));
                writer.WriteElementString("SNDPRT", SetNullValue(null));
                writer.WriteElementString("SNDPRN", SetNullValue(null));
                writer.WriteElementString("CREDAT", GetDateFormat(DateTime.Now));
                writer.WriteElementString("CRETIM", GetTimeFormat(DateTime.Now));
                writer.WriteElementString("SERIAL", SetNullValue(null));
               
                writer.WriteEndElement();
              
                writer.WriteStartElement("ZIA_CK5_HDR");
                writer.WriteAttributeString("SEGMENT", "1");

                writer.WriteElementString("CK5_NUMBER", ck5XmlDto.SUBMISSION_NUMBER);
                writer.WriteElementString("CK5_PROCS_TYP", MappingCk5Type(ck5XmlDto.CK5_TYPE));
                writer.WriteElementString("STATUS", "01");
                writer.WriteElementString("SOURCE_PLANT", ck5XmlDto.SOURCE_PLANT_ID);
                writer.WriteElementString("DEST_PLANT", ck5XmlDto.DEST_PLANT_ID);
                writer.WriteElementString("CREATOR_ID", SetNullValue(ck5XmlDto.CREATED_BY));
                writer.WriteElementString("STO_NUMBER", SetNullValue(ck5XmlDto.STO_SENDER_NUMBER));
                writer.WriteElementString("STOB_NUMBER", SetNullValue(ck5XmlDto.STOB_NUMBER));
                writer.WriteElementString("STO_REC_NUMBER", SetNullValue(ck5XmlDto.STO_RECEIVER_NUMBER));
                writer.WriteElementString("GI_DATE", SetNullValue(GetDateFormat(ck5XmlDto.GI_DATE)));
                writer.WriteElementString("GR_DATE", SetNullValue(GetDateFormat(ck5XmlDto.GR_DATE)));

                var lineItem = 1;
                foreach (var item in ck5XmlDto.Ck5Material)
                {
                    writer.WriteStartElement("ZIA_CK5_ITM");
                    writer.WriteAttributeString("SEGMENT", "1");
                    writer.WriteElementString("CK5_NUMBER", ck5XmlDto.SUBMISSION_NUMBER);
                    writer.WriteElementString("ITEM_NUMBER", lineItem.ToString());
                    writer.WriteElementString("MATERIAL", item.BRAND);
                    writer.WriteElementString("MENGE", SetNullValue(item.CONVERTED_QTY.ToString()));
                    writer.WriteElementString("MEINS", SetNullValue(item.CONVERTED_UOM));
                    writer.WriteElementString("DELIVERY_NOTE", SetNullValue(item.NOTE));
                    writer.WriteElementString("GI_OPN_QTY", SetNullValue(null));
                    writer.WriteElementString("GI_ACC_QTY", SetNullValue(null));
                    writer.WriteElementString("GR_ACC_QTY", SetNullValue(null));
                    writer.WriteElementString("STOB_GI_OPN_QTY", SetNullValue(null));
                    writer.WriteElementString("STOB_GI_ACC_QTY", SetNullValue(null));
                    writer.WriteElementString("STOR_GI_ACC_QTY", SetNullValue(null));
                    writer.WriteElementString("STOR_GR_OPN_QTY", SetNullValue(null));
                    writer.WriteElementString("STOR_GR_ACC_QTY", SetNullValue(null));
                    writer.WriteElementString("GI_MAT_DOC", SetNullValue(null));
                    writer.WriteElementString("GI_ZEILE", SetNullValue(null));
                    writer.WriteElementString("GI_YEAR", SetNullValue(null));
                    writer.WriteElementString("GI_DATE", SetNullValue(null));
                    writer.WriteElementString("GR_MAT_DOC", SetNullValue(null));
                    writer.WriteElementString("GR_ZEILE", SetNullValue(null));
                    writer.WriteElementString("GR_YEAR", SetNullValue(null));
                    writer.WriteElementString("GR_DATE", SetNullValue(null));
                    writer.WriteElementString("STOB_GI_MAT_DOC", SetNullValue(null));
                    writer.WriteElementString("STOB_GI_ZEILE", SetNullValue(null));
                    writer.WriteElementString("STOB_GI_YEAR", SetNullValue(null));
                    writer.WriteElementString("STOB_GI_DATE", SetNullValue(null));
                    writer.WriteElementString("STOR_GI_MAT_DOC", SetNullValue(null));
                    writer.WriteElementString("STOR_GI_ZEILE", SetNullValue(null));
                    writer.WriteElementString("STOR_GI_YEAR", SetNullValue(null));
                    writer.WriteElementString("STOR_GI_DATE", SetNullValue(null));
                    writer.WriteElementString("STOR_GR_MAT_DOC", SetNullValue(null));
                    writer.WriteElementString("STOR_GR_ZEILE", SetNullValue(null));
                    writer.WriteElementString("STOR_GR_YEAR", SetNullValue(null));
                    writer.WriteElementString("STOR_GR_DATE", SetNullValue(null));


                    //ZAIDM_CK5_ITM
                    writer.WriteEndElement();
                    lineItem++;
                }


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
