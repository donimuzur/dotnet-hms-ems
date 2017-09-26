using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.ManufacturingLicense
{
    public class LicenseRequestViewModel : MasterViewModel
    {
        public LicenseRequestViewModel() : base()
        {
             
        }

        public List<LicenseRequestModel> LicenseRequestDocuments { set; get; }
        public SelectList LastApprovedStatusList { get; set; }
        public SelectList FormNumList { set; get; }
        public SelectList CompTypeList { set; get; }
        public SelectList KPPBCList { set; get; }
        public SelectList CompanyList { set; get; }
        public SelectList ProdTypeList { set; get; }  
        public LicenseRequestFilterModel Filter { set; get; }
    }

    public class LicenseRequestFilterModel
    {   
        public string FormNum { set; get; }
        public string CompType { set; get; }
        public string KPPBC { set; get; }
        public string Company { set; get; }
        public string ProdType { set; get; }
        
        public int LastApprovedStatus { set; get; }
        public int StatusFilter { set; get; }

    }

}