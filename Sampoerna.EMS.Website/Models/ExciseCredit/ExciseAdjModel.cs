using Sampoerna.EMS.Website.Models.FinanceRatio;
using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.ExciseCredit
{
    public class ExciseAdjModel : MasterModel
    {
        public ExciseAdjModel() : base()
        {
        
        }
        public string BRAND_CE { get; set; }
        public string PRODUCT_CODE { get; set; }
        public decimal OLD_TARIFF { get; set; }
        public decimal NEW_TARIFF { get; set; }
        public decimal INCREASE_TARIFF { get; set; }
        public decimal CK1_AMOUNT { get; set; }
        public decimal WEIGHTED_INCREASE { get; set; }
        public long EXCISE_CREDIT_ID { get; set; }

    }
}