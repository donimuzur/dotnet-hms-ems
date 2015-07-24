﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class Lack1GetByParamInput
    {
        public string NppbKcId { get; set; }
        public string Poa { get; set; }
        public string PlantId { get; set; }
        public int? ReportedOn { get; set; }
        public string Creator { get; set; }

        /// <summary>
        /// optional if want to sorting from query
        /// </summary>
        public string SortOrderColumn { get; set; }
    }

    public class Lack1GetListByNppbkcParam : Lack1GetByParamInput
    {
        
    }

    public class Lack1GetListByPlantParam : Lack1GetByParamInput
    {
        
    }
}
