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
        public string CREATED_BY { get; set; }
        public string MODIFIED_BY { get; set; }
        public bool? IS_DELETED { get; set; }
        public string ORT01 { get; set; }
        public string STRAS { get; set; }
    }
}
