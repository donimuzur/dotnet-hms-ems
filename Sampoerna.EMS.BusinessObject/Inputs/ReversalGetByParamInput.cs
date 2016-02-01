﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class ReversalGetByParamInput
    {
        public string DateProduction { get; set; }
        public string PlantId { get; set; }
        public string ShortOrderColumn { get; set; }
        public string UserId { get; set; }
    }

    public class ReversalCreateParamInput
    {
        public int ZaapShiftId { get; set; }
        public Decimal ReversalQty { get; set; }
    }
}
