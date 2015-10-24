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
        public List<UserPlantMapDetail> UserPlantMaps { get; set; }

        public string IsActive { get; set; }

    }

    public class UserPlantMapDetail
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public string PlantId { get; set; }

        public string UserName { get; set; }

        public string PlantName { get; set; }

        public string IsActive { get; set; }
    }
}