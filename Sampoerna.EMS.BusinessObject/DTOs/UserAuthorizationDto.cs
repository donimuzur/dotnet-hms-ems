using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class UserAuthorizationDto
    {
        public UserAuthorizationDto()
        {
            PageMaps = new List<PageMapDto>();
        }

        public string Brole { get; set; }
        public string BroleDescription { get; set; }
        public List<BRoleMapDto> BRoleMaps { get; set; }
        public List<PageMapDto> PageMaps { get; set; } 

    }

    public class BRoleMapDto
    {
        public int Id { get; set; }
        public UserDto User { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
       
    }
    public class BRoleDto
    {
        public string Id { get; set; }
        public string  Description { get; set; }

        

    }

    public class UserDto
    {
        public string UserId { get; set; }

        public string UserFirstName { get; set; }

        public string UserLastName { get; set; }

        public string UserEmail { get; set; }
    }

    public class PageMapDto
    {


        public int Id { get; set; }

        public PageDto Page { get; set; }

        public string Brole { get; set; }
        
    }

    public class PageDto
    {

        public int Id { get; set; }

        public string PageName { get; set; }

        public bool IsChecked { get; set; }
    }
}
