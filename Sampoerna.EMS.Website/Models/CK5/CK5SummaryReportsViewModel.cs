﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Wordprocessing;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.CK5
{
    public class CK5SummaryReportsViewModel : BaseModel
    {
        public CK5SummaryReportsViewModel()
        {
            SearchView = new CK5SearchSummaryReportsViewModel();
            DetailsList = new List<CK5SummaryReportsItem>();
        }

        public CK5SearchSummaryReportsViewModel SearchView { get; set; }
        public List<CK5SummaryReportsItem> DetailsList { get; set; }

        public CK5ExportSummaryReportsViewModel ExportModel { get; set; }

        public Enums.CK5Type Ck5Type { get; set; }
    }

    public class CK5SearchSummaryReportsViewModel
    {
        public Enums.CK5Type Ck5Type { get; set; }

        public string CompanyCodeSource { get; set; }
        public SelectList CompanyCodeSourceList { get; set; }

        public string CompanyCodeDest { get; set; }
        public SelectList CompanyCodeDestList { get; set; }

        public string NppbkcIdSource { get; set; }
        public SelectList NppbkcIdSourceList { get; set; }

        public string NppbkcIdDest { get; set; }
        public SelectList NppbkcIdDestList { get; set; }

        public string PlantSource { get; set; }
        public SelectList PlantSourceList { get; set; }

        public string PlantDest { get; set; }
        public SelectList PlantDestList { get; set; }

        public DateTime? DateFrom { get; set; }
        public SelectList DateFromList { get; set; }

        public DateTime? DateTo { get; set; }
        public SelectList DateToList { get; set; }

       
      

    }

    public class CK5SummaryReportsItem
    {
        public long Ck5Id { get; set; }
        
        #region domestic

        public string ExciseStatus { get; set; }

        public string Pbck1Number { get; set; }

        public string SubmissionDate { get; set; }

        public string SealingNotifDate { get; set; }

        public string SealingNotifNumber { get; set; }

        public string UnSealingNotifDate { get; set; }

        public string UnSealingNotifNumber { get; set; }

        public string Lack1Number { get; set; }

        public string Lack2Number { get; set; }
        
        #endregion

    }

    public class CK5ExportSummaryReportsViewModel : CK5SearchSummaryReportsViewModel
    {
        public bool ExciseStatus { get; set; }

        public bool DocumentNumber { get; set; }

        public bool SubmissionDate { get; set; }

        public bool SealingNotifDate { get; set; }

        public bool SealingNotifNumber { get; set; }

        public bool UnSealingNotifDate { get; set; }

        public bool UnSealingNotifNumber { get; set; }

        public bool Lack1Number { get; set; }

        public bool Lack2Number { get; set; }
    }

    public class CK5ExportSummaryReportsTypeExportViewModel : CK5SearchSummaryReportsViewModel
    {
        public bool ExciseStatus { get; set; }

        public bool DocumentNumber { get; set; }

        public bool SubmissionDate { get; set; }

        public bool SealingNotifDate { get; set; }

        public bool SealingNotifNumber { get; set; }

        public bool UnSealingNotifDate { get; set; }

        public bool UnSealingNotifNumber { get; set; }

        public bool Lack1Number { get; set; }

        public bool Lack2Number { get; set; }
    }

}