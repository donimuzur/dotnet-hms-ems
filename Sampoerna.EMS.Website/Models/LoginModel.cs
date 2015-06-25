using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;

namespace Sampoerna.EMS.Website.Models
{
    public class LoginModel 
    {
        [Required]
        [Display(Name = "Username Login")]
        public string Username { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

    }

    public class LoginFormModel : BaseModel
    {
        public IEnumerable<SelectListItem> Users { get; set; }
        public  LoginModel Login { get; set; }
    }
}