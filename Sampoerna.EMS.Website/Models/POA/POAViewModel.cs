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

        [Display(Name = "ID Card"), Required, StringLength(22)]
        public string PoaIdCard { get; set; }

        
        [Display(Name = "POA Code"), Required, StringLength(12)]
        public string PoaCode { get; set; }

      
        [Display(Name = "User Name")]
        public USER User { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        [Required]
        [StringLength(50)]
        public string PoaPrintedName { get; set; }

        
        [Display(Name = "Address"),Required, StringLength(100)]
        public string PoaAddress { get; set; }

        [Display(Name = "Phone Number"), Required, StringLength(13)]
        [RegularExpression("([1-9][0-9]*)")] 
        public string PoaPhone { get; set; }

        [Required(ErrorMessage = "Title")]
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

        [Required(ErrorMessage = "Please Insert POA Code")]
        [Display(Name = "ID Card")]
        [StringLength(22, ErrorMessage = "Max Lenght : 22")]
        public string PoaIdCard { get; set; }

        [Required(ErrorMessage = "Please Insert POA Code")]
        [Display(Name = "POA Code")]
        [StringLength(22, ErrorMessage = "Max Lenght : 22")]
        public string PoaCode { get; set; }

        [Required(ErrorMessage = "Please choose User Name")]
        [Display(Name = "User Name")]
        public USER User { get; set; }

        [Required(ErrorMessage = "Please Insert Data Address")]
        [StringLength(50, ErrorMessage = "Max length : 50")]
        [Display(Name = "Printed Name")]
        public string PoaPrintedName { get; set; }

        [Required(ErrorMessage = "Please Insert Data Address")]
        [Display(Name = "Address")]
        [StringLength(100, ErrorMessage = "Max length 100")]
        public string PoaAddress { get; set; }

        [Display(Name = "Phone Number")]
        [StringLength(15, ErrorMessage = "Please Insert the Number Min 10, Max 15.", MinimumLength = 10)]
        [RegularExpression("([0-9][1-9]*)", ErrorMessage = "type data must number") ] 
        public string PoaPhone { get; set; }

        [Required(ErrorMessage = "Please Insert Data")]
        [Display(Name = "Title")]
        [StringLength(50, ErrorMessage = "Max Lenght : 50")]
        public string Title { get; set; }

        public bool isNewData { get; set; }
    }
}