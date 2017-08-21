using System.ComponentModel.DataAnnotations;

namespace Sampoerna.EMS.Website.Models.Shared
{
    public class UserModel
    {
        public string UserId { set; get; }
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public string Email { set; get; }
        public string Address { set; get; }
        public string FullName { set; get; }
    }
}