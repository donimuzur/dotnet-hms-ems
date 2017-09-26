﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
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
        public POAViewDetailModel()
        {
            PoaSKFile = new List<HttpPostedFileBase>();
            PoaSk = new List<POA_SK>();
        }

        public string PoaId { get; set; }

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
        public string UserId { get; set; }
        [Required]
        public string ManagerId { get; set; }
        [Required]
        public string PoaPrintedName { get; set; }

        [Required]
        public string PoaAddress { get; set; }

        [Display(Name = "Phone Number")]
        public string PoaPhone { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        [Display(Name = "Title")]
        [StringLength(50, ErrorMessage = "Max Lenght : 50")]
        public string Title { get; set; }

        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$")]
        public string Email { get; set; }

        public bool IsFromSAP { get; set; }

        public string Is_Active { get; set; }

        public List<HttpPostedFileBase> PoaSKFile { get; set; }

        public List<POA_SK> PoaSk { get; set; } 

        public bool IsExciser { get; set; }
    }
}