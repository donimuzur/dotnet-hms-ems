using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Website.Helpers;

namespace Sampoerna.EMS.Website.Models.HeaderFooter
{

    public class HeaderFooterViewModel : BaseModel
    {
        public List<HeaderFooterItem> Details { get; set; }
    }

    public class HeaderFooterItemViewModel : BaseModel
    {

        public HeaderFooterDetailItem Detail { get; set; }

        public SelectList CompanyList { get; set; }

        //in our viewmodel we will be posting files....
        //use Data Annotations to validate properties
        //[Required(ErrorMessage = "please fill this field")]
        [Display(Name = "Header Image")]
        [ValidateFile(MaximumSize = 100)]
        public HttpPostedFileBase HeaderImageFile { get; set; }

        public string SubmitType { get; set; }

        public int idxToPreview { get; set; }

    }

    public class HeaderFooterItem
    {
        public int HEADER_FOOTER_ID { get; set; }
        public string FORM_NAME { get; set; }
        
        [Required(ErrorMessage = "please fill this field")]
        [Display(Name = "Company Code")]
        public long? COMPANY_ID { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        public string HEADER_IMAGE_PATH { get; set; }
        public string HEADER_IMAGE_PATH_BEFOREEDIT { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        [Display(Name = "Company Footer")]
        public string FOOTER_CONTENT { get; set; }
        public bool? IS_ACTIVE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string COMPANY_NAME { get; set; }
        public string COMPANY_NPWP { get; set; }
        public bool IsHeaderHide { get; set; }
    }

    public class HeaderFooterDetailItem : HeaderFooterItem
    {
        public List<HeaderFooterMapItem> HeaderFooterMapList { get; set; }
    }

}