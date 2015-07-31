using System;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Pbck1QuotaDto
    {
        public long PBCK1_QUOTA_ID { get; set; }
        public int PBCK1_ID { get; set; }
        public long CK5_ID { get; set; }
        public Enums.CK5TransType CK5_TRANS_TYPE { get; set; }
        public decimal CK5_GRAND_TOTAL_EX { get; set; }
        public decimal PBCK1_QTY_APPROVED { get; set; }
        public decimal PREV_REMAINING_QUOTA { get; set; }
        public decimal RECEIVED_AMOUNT { get; set; }
        public decimal TOTAL_REMAINING_QUOTA { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }

        public CK5Dto CK5 { get; set; }
        public Pbck1Dto PBCK1 { get; set; }
    }
}
