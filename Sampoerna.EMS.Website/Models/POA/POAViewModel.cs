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
        public int? UserId { get; set; }
        [Required]
        public int? ManagerId { get; set; }
        [Required]
        public string PoaPrintedName { get; set; }

        [Required]
        public string PoaAddress { get; set; }

        [Display(Name = "Phone Number")]
        [Required]
        public string PoaPhone { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        [Display(Name = "Title")]
        [StringLength(50, ErrorMessage = "Max Lenght : 50")]
        public string Title { get; set; }

        [Required]
        public string Email { get; set; }

        public bool IsFromSAP { get; set; }

        public string Is_Deleted { get; set; }
    }
}