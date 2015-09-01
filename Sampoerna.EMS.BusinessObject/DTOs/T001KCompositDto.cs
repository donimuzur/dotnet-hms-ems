using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    /// <summary>
    /// use for Dropdownlist source or grid
    /// make it light, mean no need lot of field from T00K table
    /// </summary>
    public class T001KCompositDto
    {
        public string BWKEY { get; set; }
        public string BUKRS { get; set; }
        public string DROPDOWNTEXTFIELD { get; set; }
    }
}
