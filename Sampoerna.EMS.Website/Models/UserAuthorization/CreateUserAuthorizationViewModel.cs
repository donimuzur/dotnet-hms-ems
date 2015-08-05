using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Math;

namespace Sampoerna.EMS.Website.Models.UserAuthorization
{
    public class CreateUserAuthorizationViewModel : BaseModel
    {
        public string Brole { get; set; }

        public SelectList BroleList { get; set; }

        public string BrolDescription { get; set; }

    }
}