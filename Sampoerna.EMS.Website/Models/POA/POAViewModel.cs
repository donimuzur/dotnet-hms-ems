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
            Details = new List<POAViewDetailModel>();
        }
        public List<POAViewDetailModel> Details { get; set; }

    }


    public class POAFormModel : BaseModel
    {


        public IEnumerable<SelectListItem> Users { get; set; }
        public IEnumerable<SelectListItem> Managers { get; set; }

        public POAViewDetailModel Detail { get; set; }

        
    }

    public class POAViewDetailModel
    {


        public int PoaId { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        [Display(Name = "ID Card")]
        [StringLength(22, ErrorMessage = "Max Lenght : 22")]
        public string PoaIdCard { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        [Display(Name = "POA Code")]
        [StringLength(22, ErrorMessage = "Max Lenght : 22")]
        public string PoaCode { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        [Display(Name = "User Name")]
        public USER User { get; set; }

        public USER Manager { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        public int UserId { get; set; }
        public int ManagerId { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        [StringLength(50, ErrorMessage = "Max length : 50")]
        [Display(Name = "Printed Name")]
        public string PoaPrintedName { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        [Display(Name = "Address")]
        [StringLength(100, ErrorMessage = "Max length 100")]
        public string PoaAddress { get; set; }

        [Display(Name = "Phone Number")]
        [StringLength(15, ErrorMessage = "Please Insert the Number Min 10, Max 15.", MinimumLength = 10)]
        [RegularExpression("([0-9][1-9]*)", ErrorMessage = "type data must number")]
        public string PoaPhone { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        [Display(Name = "Title")]
        [StringLength(50, ErrorMessage = "Max Lenght : 50")]
        public string Title { get; set; }
        public string Email { get; set; }

        public bool IsFromSAP { get; set; }

        public string Is_Deleted { get; set; }
    }
}