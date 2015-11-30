using System;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack1IncomeDetailDto
    {
        public long LACK1_INCOME_ID { get; set; }
        public int LACK1_ID { get; set; }
        public long CK5_ID { get; set; }
        public decimal AMOUNT { get; set; }
        public string REGISTRATION_NUMBER { get; set; }
        public DateTime? REGISTRATION_DATE { get; set; }
    }
}
