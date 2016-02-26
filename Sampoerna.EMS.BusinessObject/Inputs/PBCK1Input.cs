﻿using System;
using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class Pbck1GetByParamInput
    {
        public string NppbkcId { get; set; }
        public string Poa { get; set; }
        public Enums.PBCK1Type? Pbck1Type { get; set; }

        public string GoodTypeId { get; set; }
        public string Creator { get; set; }
        public int? Year { get; set; }
        
        /// <summary>
        /// optional if want to sorting from query
        /// </summary>
        public string SortOrderColumn { get; set; }

        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
    }

    public class Pbck1GetOpenDocumentByParamInput : Pbck1GetByParamInput
    {
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
    }

    public class Pbck1GetCompletedDocumentByParamInput : Pbck1GetByParamInput
    {
    }

    public class Pbck1GetSummaryReportByParamInput
    {
        public string NppbkcId { get; set; }
        public string CompanyCode { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        /// <summary>
        /// optional if want to sorting from query
        /// </summary>
        public string SortOrderColumn { get; set; }

        public string pbck1Number { get; set; }
        public string Poa { get; set; }
        public string Creator { get; set; }
    }

    public class Pbck1GetMonitoringUsageByParamInput
    {
        public string NppbkcId { get; set; }
        public string CompanyCode { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public string SupNppbkc { get; set; }
        public string SupKppbc { get; set; }
        public string SupPlant { get; set; }
        public string SupCompany { get; set; }
        public string Poa { get; set; }
        public string Creator { get; set; }
        /// <summary>
        /// optional if want to sorting from query
        /// </summary>
        public string SortOrderColumn { get; set; }
    }

    public class Pbck1SaveInput 
    {
        public Pbck1Dto Pbck1 { get; set; }
        public string UserId { get; set; }
        public Enums.ActionType WorkflowActionType { get; set; }
    }
    
    public class Pbck1WorkflowDocumentInput
    {
        public long DocumentId { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public string Comment { get; set; }
        public Enums.ActionType ActionType { get; set; }
        public string DocumentNumber { get; set; }
        public Enums.DocumentStatus DocumentStatus { get; set; }
        public Pbck1WorkflowDocumentData AdditionalDocumentData { get; set; }
        
    }

    public class Pbck1WorkflowDocumentData
    {
        public decimal QtyApproved { get; set; }
        public DateTime? DecreeDate { get; set; }
        public List<Pbck1DecreeDocDto> Pbck1DecreeDoc { get; set; }
    }

    public class Pbck1UpdateReportedOn
    {
        public long Id { get; set; }
        public DateTime? ReportedOn { get; set; }
    }

    public class Pbck1GetDataForLack1ParamInput
    {
        public string CompanyCode { get; set; }
        public string NppbkcId { get; set; }
        public string ExcisableGoodsTypeId { get; set; }
        public string SupplierPlantId { get; set; }
        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }
    }

    public class Pbck1GetSupplierPlantByParamInput
    {
        public string NppbkcId { get; set; }
        public string ExciseableGoodsTypeId { get; set; }
    }

    public class Pbck1ReferenceSearchInput {
        public string NppbkcId { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public string SupllierNppbkcId { get; set; }
        public string SupplierPlantWerks { get; set; }
        public string SupplierPlant { get; set; }
        public string GoodTypeId { get; set; }
    }

    public class Pbck1GetMonitoringMutasiByParamInput
    {
        /// <summary>
        /// optional if want to sorting from query
        /// </summary>
        public string SortOrderColumn { get; set; }

        public string pbck1Number { get; set; }
        public int? yearFrom { get; set; }
        public int? yearTo { get; set; }
        public string supPlant { get; set; }
        public string supComp { get; set; }
        public string oriNppbkc { get; set; }
        public string oriKppbc { get; set; }
        public string poa { get; set; }
        public string creator { get; set; }
    }

    public class Pbck1GetProductionLack1TisToTisParamInput
    {
        public string NppbkcId { get; set; }
        public string ExcisableGoodsTypeId { get; set; }
        public string SupplierPlantId { get; set; }
        public string SupplierPlantNppbkcId { get; set; }
        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }
    }

}