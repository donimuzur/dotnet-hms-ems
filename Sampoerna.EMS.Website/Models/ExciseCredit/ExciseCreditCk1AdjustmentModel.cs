using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.ExciseCredit
{
    public class ExciseCreditCk1AdjustmentModel
    {
        public ExciseCreditCk1AdjustmentModel()
        {

        }

        public IEnumerable<ExciseCreditCK1DetailAdjustmentModel> Details
        { set; get; }
        public double AverageQuantity12
        { set; get; }
        public double AverageAmount12
        { set; get; }
        public string StartMonth { set; get; }
        public string EndMonth { set; get; }
    }
}