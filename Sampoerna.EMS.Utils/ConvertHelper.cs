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
    }
}
