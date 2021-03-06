﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class WasteGetByParamInput
    {
        public string WasteProductionDate { get; set; }

        public DateTime? BeginingProductionDate { get; set; }
        public DateTime? EndProductionDate { get; set; }

        public string Company { get; set; }
        public string Plant { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }

        public string FaCode { get; set; }
        public string ShortOrderColumn { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public List<string> ListUserPlants { get; set; }
    }

    public class GetWasteDailyProdByParamInput
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string PlantFrom { get; set; }
        public string PlantTo { get; set; }
    }
}
