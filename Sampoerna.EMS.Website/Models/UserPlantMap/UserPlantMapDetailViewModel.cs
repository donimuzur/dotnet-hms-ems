using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Website.Models.UserPlantMap
{
    public class UserPlantMapIndexViewModel : BaseModel
    {
        public List<UserPlantMapDto> UserPlantMaps { get; set; }

    }
}