﻿using System.Collections.Generic;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Website.Models.UserPlantMap
{
    public class UserPlantMapDetailViewModel : BaseModel
    {
        public UserPlantMapDto UserPlantMap { get; set; }

        public SelectList Users { get; set; }

        public List<PlantDto> Plants { get; set; }

        public List<string> SelectedNppbkc { get; set; }

        public MultiSelectList Nppbkcs { get; set; }

    }
}