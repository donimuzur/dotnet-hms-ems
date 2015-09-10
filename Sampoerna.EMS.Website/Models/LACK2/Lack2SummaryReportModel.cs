using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.LACK2
{
    public class Lack2SummaryReportModel
    : BaseModel
    {

        public Lack2SummaryReportModel()
        {

        }
        public SelectList CompanyList { get; set; }
        public SelectList PlantList { get; set; }

        public SelectList NppbkcList { get; set; }

        public SelectList GoodsTypeList { get; set; }

        public SelectList PeriodMonthList { get; set; }

        public SelectList PeriodYearList { get; set; }

        public SelectList CreatorList { get; set; }
        public SelectList POAList { get; set; }
      
        

    }
        public class Lack2SummaryDetailModel
        {

        }
}