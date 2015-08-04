using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.UserAuthorization
{
    public class UserAuthorizationViewModel : BaseModel
    {
        public UserAuthorizationViewModel()
        {
            Detail = new List<DetailUserAuthorization>();
        }

        public List<DetailUserAuthorization> Detail { get; set; }

        
    }

    public class DetailUserAuthorization
    {
        public string Brole { get; set; }
        public string BroleDescription { get; set; }
    }
}