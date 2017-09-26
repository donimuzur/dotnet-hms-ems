using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.CustomService.Core
{
    public static class ConversionHelper
    {
        public static int ? ToInt32OrDefault(string value)
        {
            if (String.IsNullOrEmpty(value))
                return new Nullable<int>();

            int result = 0;
            if (!Int32.TryParse(value, out result))
            {
                throw new Exception("NotANumberException thrown at Sampoerna.EMS.CustomService.Core.ConversionHelper. See inner exception for more details", new NotFiniteNumberException("Not a number!"));
            }

            return result;

        }

        public static long? ToInt64OrDefault(string value)
        {
            if (String.IsNullOrEmpty(value))
                return new Nullable<long>();

            long result = 0;
            if (!Int64.TryParse(value, out result))
            {
                throw new Exception("NotANumberException thrown at Sampoerna.EMS.CustomService.Core.ConversionHelper. See inner exception for more details", new NotFiniteNumberException("Not a number!"));
            }

            return result;
        }
    }
}
