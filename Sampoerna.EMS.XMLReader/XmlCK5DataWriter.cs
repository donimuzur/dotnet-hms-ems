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

        public void CreateXML(CK5 ck5, string filePath)
        {
            using (XmlWriter writer = XmlWriter.Create(filePath))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("ZAIDM_CK5_01");
                writer.WriteStartElement("IDOC");
                writer.WriteAttributeString("BEGIN", "1");
                
               
                writer.WriteStartElement("ZIA_CK5_HDR");
                writer.WriteAttributeString("SEGMENT", "1");

                writer.WriteElementString("CK5_NUMBER", ck5.REGISTRATION_NUMBER);
                writer.WriteElementString("CK5_PROCS_TYP", ck5.CK5_TYPE.ToString());
                writer.WriteElementString("SOURCE_PLANT", ck5.SOURCE_PLANT_ID);
                writer.WriteElementString("DEST_PLANT", ck5.DEST_PLANT_ID);
                writer.WriteElementString("CREATOR_ID", SetNullValue(ck5.CREATED_BY));
                writer.WriteElementString("STO_NUMBER", SetNullValue(ck5.STO_SENDER_NUMBER));
                writer.WriteElementString("STOB_NUMBER", SetNullValue(ck5.STOB_NUMBER));
                writer.WriteElementString("STO_REC_NUMBER", SetNullValue(ck5.STO_RECEIVER_NUMBER));
                writer.WriteElementString("GI_DATE", SetNullValue(GetDateFormat(ck5.GI_DATE)));
                writer.WriteElementString("GR_DATE", SetNullValue(GetDateFormat(ck5.GR_DATE)));

                
                foreach (var item in ck5.CK5_MATERIAL)
                {
                    writer.WriteStartElement("ZIA_CK5_ITM");
                    writer.WriteAttributeString("SEGMENT", "1");
                    writer.WriteElementString("CK5_NUMBER", ck5.REGISTRATION_NUMBER);
                    writer.WriteElementString("ITEM_NUMBER",GetLinesItem(item.LINE_ITEM));
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
