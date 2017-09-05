using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.ExciseCredit
{
    public class ExciseCreditCk1Model
    {
        public ExciseCreditCk1Model()
        {

        }

        public IEnumerable<ExciseCreditCK1DetailModel> Details
        { set; get; }
        public double AverageQuantity3
        { set; get; }
        public double AverageQuantity6
        { set; get; }
        public double AverageAmount3
        { set; get; }
        public double AverageAmount6
        { set; get; }
        public string StartMonth { set; get; }
        public string EndMonth { set; get; }
    }
}