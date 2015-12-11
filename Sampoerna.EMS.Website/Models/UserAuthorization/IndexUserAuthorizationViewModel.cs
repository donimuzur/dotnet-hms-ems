using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.UserAuthorization
{
    public class IndexUserAuthorizationViewModel : BaseModel
    {
        public IndexUserAuthorizationViewModel()
        {
            Detail = new List<DetailIndexUserAuthorization>();
        }

        public List<DetailIndexUserAuthorization> Detail { get; set; }

        
    }

    public class DetailIndexUserAuthorization
    {
        public string Brole { get; set; }
        public string BroleDescription { get; set; }
    }
}