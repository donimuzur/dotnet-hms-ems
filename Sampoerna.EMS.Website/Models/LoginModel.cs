using System.ComponentModel.DataAnnotations;

namespace Sampoerna.EMS.Website.Models
{
    public class LoginModel : BaseModel
    {
        [Required]
        [Display(Name = "Username Login")]
        public string Username { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

    }
}