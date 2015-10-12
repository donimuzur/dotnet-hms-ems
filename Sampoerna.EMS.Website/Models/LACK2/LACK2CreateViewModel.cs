﻿using System;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.LACK2
{
    public class LACK2CreateViewModel : BaseModel
    {
        #region Field
        public string Lack2Number { get; set; }
        public int? PeriodMonth { get; set; }
        public int? PeriodYear { get; set; }
        public string SourcePlantId { get; set; }
        public string ExcisableGoodsType { get; set; }
        public string CompanyCode { get; set; }
        public string NppbkcId { get; set; }
        public string UserId { get; set; }
        public DateTime? SubmissionDate { get; set; }
        
        #endregion

        #region View Purpose

        public SelectList CompanyCodesDDL { get; set; }
       
        public SelectList NPPBKCDDL { get; set; }
        
        public SelectList SendingPlantDDL { get; set; }
        
        public SelectList ExcisableGoodsTypeDDL { get; set; }

        public SelectList GovStatusDDL { get; set; }

        public SelectList MonthList { get; set; }

        public SelectList YearList { get; set; }

        #endregion

    }
}