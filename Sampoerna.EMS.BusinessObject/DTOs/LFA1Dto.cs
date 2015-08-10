using System;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class LFA1Dto
    {
        public string LIFNR { get; set; }
        public string NAME1 { get; set; }
        public string NAME2 { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
    }
}
