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
    public class XmlPBCK4DataWriter
    {
        private string GetDateFormat(DateTime? date)
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
            return result.ToString("yyyyMMdd");
        }

      
        private string SetNullValue(object value)
        {
            if (value == null)
            {
                return null;
            }
            return value.ToString();
        }

      
        public void CreatePbck4Xml(Pbck4XmlDto pbck4XmlDto)
        {
            using (XmlWriter writer = XmlWriter.Create(pbck4XmlDto.GeneratedXmlPath))
            {

               

                writer.WriteStartElement("ZAIDM_CK2_CK3");
                writer.WriteStartElement("IDOC");
                writer.WriteAttributeString("BEGIN", "1");

                writer.WriteStartElement("Z1A_CK2_CK3");
                writer.WriteAttributeString("SEGMENT", "1");
                writer.WriteElementString("PBCK_NO", pbck4XmlDto.PbckNo);
                writer.WriteElementString("NPPBKC_ID", pbck4XmlDto.NppbckId);
                writer.WriteElementString("COMPN_TYPE", pbck4XmlDto.CompType);
                writer.WriteElementString("COMPN_NO", pbck4XmlDto.CompNo);
                writer.WriteElementString("COMPN_DATE", GetDateFormat(pbck4XmlDto.CompnDate));
                writer.WriteElementString("COMPN_VALUE", pbck4XmlDto.CompnValue);
                writer.WriteElementString("DEL_FLAG", pbck4XmlDto.DeleteFlag);
             
               
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
