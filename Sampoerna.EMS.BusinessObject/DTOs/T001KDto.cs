using System;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class T001KDto
    {
        public string BWKEY { get; set; }
        public string BUKRS { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string MODIFIED_BY { get; set; }
        public bool? IS_DELETED { get; set; }
    }
}
