using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.XMLReader
{
    public class XmlDataWriter
    {
        protected string GetDateFormat(DateTime? date)
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

        protected string GetTimeFormat(DateTime? date)
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

        protected string GetLinesItem(int? line)
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

        protected string SetNullValue(object value)
        {
            if (value == null)
            {
                return null;
            }
            return value.ToString();
        }

        protected string SetNullToSlash(object value)
        {
            if (value == null)
            {
                return "/";
            }
            return value.ToString();
        }

        protected string SetBoolToX(bool? value)
        {
            if (value.HasValue && value.Value)
            {
                return "X";
            }
            
            return "/";
            

            
        }

        public void MoveTempToOutbound(string oldPath, string newPath)
        {
            try
            {
                File.Move(oldPath, newPath);
            }
            catch (Exception ex)
            {
                Exception ex1 = new Exception(String.Format("Failed to move xml file to outbound folder. Cause : {0}", ex.Message));

                throw ex1;
            }


        }

        
    }
}
