using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.ManufacturingLicense
{
    public class InterviewRequestViewModel : MasterViewModel
    {
        public InterviewRequestViewModel() : base()
        {
            
        }
        public List<InterviewRequestModel> InterviewRequestDocuments { set; get; }
        public SelectList YearList { set; get; }
        public SelectList KppbcList { set; get; }
        public SelectList PoaList { set; get; }
        public SelectList CreatorList { set; get; }        
        public SelectList CompanyType { set; get; }
        public InterviewRequestFilterModel Filter { set; get; }
        public string FromMenu { set; get; }
        public bool IsCompleted { set; get; }
        public string CurrentUser { set; get; }
    }

    public class InterviewRequestFilterModel
    {
        public int Year { set; get; }
        public string POA { set; get; }
        public string Creator { set; get; }
        public string KPPBC { set; get; }
        public string CompanyType { set; get; }
    }
}