﻿using System;
using System.Collections.Generic;
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

        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }

        public List<string> DocumentNumberList { get; set; }
        public List<string> ListNppbkc { get; set; }
        public List<string> ListUserPlant { get; set; }

    }
    
    public class Lack1WorkflowDocumentInput
    {
        public int? DocumentId { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public string Comment { get; set; }
        public Enums.ActionType ActionType { get; set; }
        public string DocumentNumber { get; set; }
        public bool IsModified { get; set; }

        public Lack1WorkflowDocumentData AdditionalDocumentData { get; set; }

    }

    public class Lack1WorkflowDocumentData
    {
        public DateTime? DecreeDate { get; set; }
        public List<Lack1DocumentDto> Lack1Document { get; set; }
    }

    public class Lack1GetLatestSaldoPerPeriodInput
    {
        public int MonthTo { get; set; }
        public int YearTo { get; set; }
        public string NppbkcId { get; set; }
        public string SupplierPlantWerks { get; set; }
        public string ExcisableGoodsType { get; set; }

        public bool isImport { get; set; }
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

        /// <summary>
        /// Get from LACK-1 Form selection criteria
        /// todo: add logic to parse Supplier Plant ID and Supplier Plant NPPBKC Id from LACK-1 Form selection criteria, please provide it :)
        /// </summary>
        public string SupplierPlantNppbkcId { get; set; }
        public string ExcisableGoodsType { get; set; }
        public string ExcisableGoodsTypeDesc { get; set; }
        public decimal? WasteAmount { get; set; }
        public string WasteAmountUom { get; set; }
        public decimal? ReturnAmount { get; set; }
        public string ReturnAmountUom { get; set; }

        public Enums.Lack1Level Lack1Level { get; set; }

        public string Noted { get; set; }

        public int ExGroupTypeId { get; set; }

        public bool IsCreateNew { get; set; }

        public int Lack1Id { get; set; }

        /// <summary>
        /// Set to TRUE if the report for TIS to TIS LACK-1 report
        /// </summary>
        public bool IsTisToTis { get; set; }

        public bool IsSupplierNppbkcImport { get; set; }
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
        public int ExcludeLack1Id { get; set; }
    }

    public class Lack1GetBySelectionCriteriaParamInput
    {
        public string CompanyCode { get; set; }
        public string NppbkcId { get; set; }
        public string ExcisableGoodsType { get; set; }
        public string ReceivingPlantId { get; set; }
        public string SupplierPlantId { get; set; }
        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }
        public Enums.Lack1Level Lack1Level { get; set; }

        /// <summary>
        /// set to TRUE for TIS to TIS LACK-1 Report
        /// </summary>
        public bool IsTisToTis { get; set; }
    }

    public class Lack1CreateParamInput : Lack1GenerateDataParamInput
    {
        public string UserId { get; set; }
    }

    public class Lack1RegenerateParamInput : Lack1GenerateDataParamInput
    {
        public string Lack1Number { get; set; }
        public int Lack1Id { get; set; }
        public string UserId { get; set; }
        public Enums.ActionType WorkflowActionType { get; set; }
    }

    public class Lack1GetSummaryReportByParamInput
    {
        public string CompanyCode { get; set; }
        public string NppbkcId { get; set; }
        public string ReceivingPlantId { get; set; }
        public string ExcisableGoodsType { get; set; }
        public string SupplierPlantId { get; set; }
        public int? PeriodMonth { get; set; }
        public int? PeriodYear { get; set; }

        public Enums.DocumentStatus? DocumentStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }
        public string Creator { get; set; }
        public string Approver { get; set; }
        public List<string> ListNppbkc { get; set; }
        public List<string> ListUserPlant { get; set; }
        public Enums.UserRole UserRole { get; set; }
    }

    public class Lack1GetDetailReportByParamInput
    {
        public string CompanyCode { get; set; }
        public string NppbkcId { get; set; }
        public string ReceivingPlantId { get; set; }
        public string ExcisableGoodsType { get; set; }
        public string SupplierPlantId { get; set; }
        public string Poa { get; set; }
        public string Creator { get; set; }
        public int? PeriodMonthFrom { get; set; }
        public int? PeriodYearFrom { get; set; }
        public int? PeriodMonthTo { get; set; }
        public int? PeriodYearTo { get; set; }
        public Enums.Lack1Level? Lack1Level { get; set; }
        public List<string> ListNppbkc { get; set; }
        public List<string> ListUserPlant { get; set; }
        public Enums.UserRole UserRole { get; set; }
    }

    public class Lack1GetPbck1RealizationListParamInput
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int Year { get; set; }
        public string NppbkcId { get; set; }
        public string SupplierPlantId { get; set; }
        public string ExcisableGoodsTypeId { get; set; }
    }

    public class Lack1GetReconciliationByParamInput
    {
        public string NppbkcId { get; set; }
        public string PlantId { get; set; }
        public string ExGoodType { get; set; }
        public List<string> ListNppbkc { get; set; }
        public List<string> ListUserPlant { get; set; }
        public Enums.UserRole UserRole { get; set; }
    }


    public class Lack1GetDetailTisByParamInput
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string PlantReceiverFrom { get; set; }
        public string PlantReceiverTo { get; set; }
    }

    public class Lack1GetDailyProdByParamInput
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string PlantFrom { get; set; }
        public string PlantTo { get; set; }

        public List<string> ListNppbkc { get; set; }
        public List<string> ListUserPlant { get; set; }
        public Enums.UserRole UserRole { get; set; }
    }

    public class Lack1GetPrimaryResultsByParamInput
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string PlantFrom { get; set; }
        public string PlantTo { get; set; }

        public List<string> ListNppbkc { get; set; }
        public List<string> ListUserPlant { get; set; }
        public Enums.UserRole UserRole { get; set; }
    }

    public class Lack1CFUsageVsFAByParamInput
    {
        public string BeginingPlant { get; set; }
    
        public string EndPlant {get; set;}
        public DateTime BeginingPostingDate { get; set; }
    
        public DateTime EndPostingDate { get; set; }

        public bool IsSummary { get; set; }
    }

    public class Lack1GetDetailEaByParamInput
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string PlantReceiverFrom { get; set; }
        public string PlantReceiverTo { get; set; }
    }
}
