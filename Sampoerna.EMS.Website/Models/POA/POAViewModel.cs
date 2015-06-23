using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Website.Models.POA
{
    public class POAViewModel : BaseModel
    {
        public POAViewModel()
        {
            Details = new List<ZAIDM_EX_POA>();
        }
        public List<ZAIDM_EX_POA> Details { get; set; }

        public int PoaId { get; set; }

        [Display(Name = "ID Card")]
        public string PoaIdCard { get; set; }

        [Display(Name = "POA Code")]
        public string PoaCode { get; set; }

        [Display(Name = "User Name")]
        public USER User { get; set; }


        [Display(Name = "Printed Name")]
        public string PoaPrintedName { get; set; }

        [Display(Name = "Address")]
        public string PoaAddress { get; set; }

        [Display(Name = "Phone")]
        public string PoaPhone { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        public bool isNewData { get; set; }
    }


    public class POAFormModel :BaseModel
    {
        

        public IEnumerable<SelectListItem> Users { get; set; }
        
        public POAViewDetailModel Detail { get; set; }
    }

    public class POAViewDetailModel 
    {
       

        public int PoaId { get; set; }

        [Display(Name = "ID Card")]
        public string PoaIdCard { get; set; }

        [Display(Name = "POA Code")]
        public string PoaCode { get; set; }

        [Display(Name = "User Name")]
        public USER User { get; set; }

        
        [Display(Name = "Printed Name")]
        public string PoaPrintedName { get; set; }

        [Display(Name = "Address")]
        public string PoaAddress { get; set; }

        [Display(Name = "Phone")]
        public string PoaPhone { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        public bool isNewData { get; set; }
    }
}