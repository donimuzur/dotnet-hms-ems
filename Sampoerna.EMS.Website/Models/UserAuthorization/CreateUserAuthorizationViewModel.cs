using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Math;
using Sampoerna.EMS.BusinessObject.DTOs;
namespace Sampoerna.EMS.Website.Models.UserAuthorization
{
    public class EditUserAuthorizationViewModel : BaseModel
    {
        public UserAuthorizationDto RoleAuthorizationDto { get; set; }

        public List<PageDto> Pages { get; set; }
        

    }

  
}