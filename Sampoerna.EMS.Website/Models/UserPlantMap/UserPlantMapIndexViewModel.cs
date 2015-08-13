using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Website.Models.UserPlantMap
{
    public class UserPlantMapDetailViewModel : BaseModel
    {
        public UserPlantMapDto UserPlantMap { get; set; }

        public SelectList Users { get; set; }

        public List<PlantDto> Plants { get; set; }

        public List<T001W> ExistingPlant { get; set; }

        public MultiSelectList Nppbkcs { get; set; } 
    }
}