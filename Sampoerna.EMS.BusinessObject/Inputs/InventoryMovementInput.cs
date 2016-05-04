﻿using System;
using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.Inputs
{

    public class InvMovementGetForLack1UsageMovementByParamInput
    {
        public InvMovementGetForLack1UsageMovementByParamInput()
        {
            PlantIdList = new List<string>();
            MvtCodeList = new List<string>();
            StoReceiverNumberList = new List<string>();
        }
        public List<string> PlantIdList { get; set; }
        public List<string> MvtCodeList { get; set; }
        public List<string> StoReceiverNumberList { get; set; }
        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }
        public string NppbkcId { get; set; }
    }

    public class InvMovementGetUsageByParamInput
    {
        public List<string> PlantIdList { get; set; }
        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }
        public string NppbkcId { get; set; }
        public bool IsTisToTis { get; set; }
        public bool IsEtilAlcohol { get; set; }
    }

    public class InvMovementGetReceivingByParamInput : InvMovementGetUsageByParamInput
    {
    }

    public class GetUsageByBatchAndPlantIdInPeriodParamInput
    {
        public string Batch { get; set; }
        public string PlantId { get; set; }
        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }
    }

    public class GetReceivingByOrderAndPlantIdInPeriodParamInput
    {
        public string Ordr { get; set; }
        public string PlantId { get; set; }
        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }
    }

    public class InvGetReceivingByParamZaapShiftRptInput
    {
        public string Ordr { get; set; }
        public string PlantId { get; set; }

        public string FaCode { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

}
