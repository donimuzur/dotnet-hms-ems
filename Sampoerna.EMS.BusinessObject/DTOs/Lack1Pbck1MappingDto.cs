using System;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack1Pbck1MappingDto
    {
        public long LACK1_PBCK1_MAPPING_ID { get; set; }
        public int LACK1_ID { get; set; }
        public int PBCK1_ID { get; set; }
        public string PBCK1_NUMBER { get; set; }
        public DateTime? DECREE_DATE { get; set; }
        public string DisplayDecreeDate { get; set; }
    }
}
