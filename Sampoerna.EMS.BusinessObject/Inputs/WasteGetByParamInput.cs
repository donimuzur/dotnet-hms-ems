﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class WasteGetByParamInput
    {
        public string WasteDate { get; set; }
        public string Company { get; set; }
        public string Plant { get; set; }
        public string ShortOrderColumn { get; set; }
    }
}
