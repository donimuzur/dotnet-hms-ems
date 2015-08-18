using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.Utils
{
    public static class ConvertHelper
    {

        public static bool IsNumeric(string value)
        {
            try
            {
                var result = Convert.ToDecimal(value);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static DateTime? StringToDateTimeCk5FileDocuments(string value)
        {
            DateTime? result = null;

            if (!IsNumeric(value))
                return null;

            //format ddMMyyyy
            if (value.Length != 8)
                return null;

            try
            {
                return new DateTime(Convert.ToInt32(value.Substring(4, 4)),
                    Convert.ToInt32(value.Substring(2, 2)),
                    Convert.ToInt32(value.Substring(0, 2)));

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool IsEnumsObject<T>(Object enumsType, int enumsValue)
        {
            //T one = (T)Enum.Parse(typeof(T), o.ToString());
            //return one;
            if (typeof(T).IsEnumDefined(enumsValue))
                return true;
            return false;
        }
    }
}
