using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.ExciseCredit
{
    public class ExciseCreditCK1DetailModel
    {
        public long Id { set; get; }
        public long ExciseCreditId { set; get; }
        public int MonthPeriod { set; get; }
        public int YearPeriod { set; get; }
        public string PeriodDisplay { set; get; }
        public string ProductTypeCode { set; get; }
        public DateTime CK1Date { set; get; }
        public string CK1Number { set; get; }
        public int OrderQuantity { set; get; }
        public decimal Amount { set; get; }
    }
}