using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.ChangeRequest
{
    public class ChangeRequestViewModel : MasterViewModel
    {
        public ChangeRequestViewModel() : base()
        {
            //this.ListFinanceRatios = new List<FinanceRatioModel>();
            this.ViewModel = new ChangeRequestModel();
            this.FilterInput = new ChangeRequestFilterModel();

    }

        /// <summary>
        /// Property to support form data binding reperesenting Business Object of Finance Ratio
        ///
        /// </summary>
        public ChangeRequestModel ViewModel
        {
            set; get;
        }

        public Enums.UserRole CurrentRole { get; set; }
        public List<ChangeRequestModel> ChangeRequestDocuments { set; get; }

        public SelectList NppbkcList { set; get; }
        public SelectList PoaList { set; get; }
        public SelectList CreatorList { set; get; }
        public SelectList YearList { set; get; }
        public SelectList DocumentTypeList { set; get; }

        public ChangeRequestFilterModel FilterInput { set; get; }

        public List<string> UpdateNotes { set; get; }

        public int FilterLastApprovedStatus { set; get; }
    }

    public class ChangeRequestFilterModel
    {
        public int Year { set; get; }
        public string POA { set; get; }
        public string Creator { set; get; }
        public string NPPBKC { set; get; }
        public string DocumentType { set; get; }

        public int LastApprovedStatus { set; get; }
    }

}