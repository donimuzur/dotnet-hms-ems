using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.Utils
{
    public static class ConvertHelper
    {

        public static decimal ConvertToDecimalOrZero(string value)
        {
            try
            {
                return GetDecimal(value);
               }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static bool IsNumeric(string value)
        {
            try
            {
                var result = GetDecimal(value);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static decimal GetDecimal(string value)
        {
            return Decimal.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        public static decimal GetDecimal(int value)
        {
            return GetDecimal(value.ToString());
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

        public static string ConvertDecimalToString(decimal? value, string formatDecimalValue = "f2")
        {
            return value.HasValue ? value.Value.ToString(formatDecimalValue) : string.Empty;
        }

        public static string ConvertDateToString(DateTime? value, string formatDate)
        {
            return value.HasValue ? value.Value.ToString(formatDate) : string.Empty;
        }

        public static string ConvertDateToStringddMMMyyyy(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("dd MMM yyyy") : string.Empty;
        }
    }
}
