﻿using System;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class Lack1GetByParamInput
    {
        public string NppbKcId { get; set; }
        public string Poa { get; set; }
        /// <summary>
        /// only if Lack1Level is Plant
        /// </summary>
        public string PlantId { get; set; }
        public string Creator { get; set; }
        public DateTime? SubmissionDate { get; set; }

        /// <summary>
        /// optional if want to sorting from query
        /// </summary>
        public string SortOrderColumn { get; set; }

        public Enums.Lack1Level Lack1Level { get; set; }

        public bool IsOpenDocumentOnly { get; set; }

    }
    
    public class Lack1SaveInput
    {
        public Lack1Dto Lack1 { get; set; }
        public string UserId { get; set; }
        public Enums.ActionType WorkflowActionType { get; set; }
    }

    public class Lack1WorkflowDocumentInput
    {
        public long DocumentId { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public string Comment { get; set; }
        public Enums.ActionType ActionType { get; set; }
        public string DocumentNumber { get; set; }

        public Lack1WorkflowDocumentData AdditionalDocumentData { get; set; }

    }

    public class Lack1WorkflowDocumentData
    {
        public DateTime DecreeDate { get; set; }
        public Lack1DocumentDto Lack1Document { get; set; }
    }

    public class Lack1GetLatestSaldoPerPeriodInput
    {
        public int MonthTo { get; set; }
        public int YearTo { get; set; }
        public string NppbkcId { get; set; }
    }

    public class Lack1GetByPeriodParamInput
    {
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public string NppbkcId { get; set; }
    }

    public class Lack1GenerateDataParamInput
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }
        public string NppbkcId { get; set; }
        public string ReceivedPlantId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string SupplierPlantId { get; set; }
        public string ExcisableGoodsType { get; set; }
        public string ExcisableGoodsTypeDesc { get; set; }
        public decimal WasteAmount { get; set; }
        public string WasteAmountUom { get; set; }
        public decimal ReturnAmount { get; set; }
        public string ReturnAmountUom { get; set; }

        public Enums.Lack1Level Lack1Level { get; set; }

        public string Noted { get; set; }

    }

    public class Lack1GetLatestLack1ByParamInput
    {
        public string CompanyCode { get; set; }
        public Enums.Lack1Level? Lack1Level { get; set; }
        public string NppbkcId { get; set; }
        public string ExcisableGoodsType { get; set; }
        public string SupplierPlantId { get; set; }
        public string ReceivedPlantId { get; set; }
        public DateTime PeriodTo { get; set; }
    }

}
