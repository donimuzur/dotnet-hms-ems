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
                return result += "000" + lineValid;
            if (line < 100)
                return result += "00" + lineValid;
            if (line < 1000)
                return result += "0" + lineValid;
            return null;
        }

        public void CreateXML(CK5 ck5, string fileName)
        {
            var outboundFolder = ConfigurationManager.AppSettings["XmlOutboundPath"];
            using (XmlWriter writer = XmlWriter.Create(Path.Combine(outboundFolder, fileName)
                ))
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
                writer.WriteElementString("CREATOR_ID", ck5.CREATED_BY);
                writer.WriteElementString("STO_NUMBER", ck5.STO_SENDER_NUMBER);
                writer.WriteElementString("STOB_NUMBER", ck5.STOB_NUMBER);
                writer.WriteElementString("STO_REC_NUMBER", ck5.STO_RECEIVER_NUMBER);
                writer.WriteElementString("GI_DATE", GetDateFormat(ck5.GI_DATE));
                writer.WriteElementString("GR_DATE", GetDateFormat(ck5.GR_DATE));

                
                foreach (var item in ck5.CK5_MATERIAL)
                {
                    writer.WriteStartElement("ZIA_CK5_ITM");
                    writer.WriteAttributeString("SEGMENT", "1");
                    writer.WriteElementString("CK5_NUMBER", ck5.REGISTRATION_NUMBER);
                    writer.WriteElementString("ITEM_NUMBER",GetLinesItem(item.LINE_ITEM));
                    writer.WriteElementString("MATERIAL", item.BRAND);
                    writer.WriteElementString("MENGE", item.CONVERTED_QTY.ToString());
                    writer.WriteElementString("MEINS", item.CONVERTED_UOM.ToString());
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
