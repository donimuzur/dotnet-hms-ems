﻿
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class CK5GetByParamInput
    {
        public string DocumentNumber { get; set; }

        public string POA { get; set; }

        public string NPPBKCOrigin { get; set; }

        public string NPPBKCDestination { get; set; }

        public string Creator { get; set; }

        public string SortOrderColumn { get; set; }

        public Enums.CK5Type Ck5Type { get; set; }

        public string UserId { get; set; }

        public Enums.UserRole UserRole { get; set; }

        public List<string> ListUserPlant { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }
    }

    public class CK5SaveInput
    {
        public CK5Dto Ck5Dto { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; } 
        public List<CK5MaterialDto> Ck5Material { get; set; } 
    }

    public class CK5SaveListInput
    {
        public List<CK5FileDocumentDto> ListCk5UploadDocumentDto { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
    }

    public class CK5WorkflowDocumentInput
    {
        public long DocumentId { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public string Comment { get; set; }
        public Enums.ActionType ActionType { get; set; }
        public string DocumentNumber { get; set; }

        public CK5WorkflowDocumentData AdditionalDocumentData { get; set; }

        public string SealingNumber { get; set; }
        public DateTime? SealingDate { get; set; }
        public DateTime? GIDate { get; set; }

        public string UnSealingNumber { get; set; }
        public DateTime? UnSealingDate { get; set; }
        public DateTime? GRDate { get; set; }

        public Enums.CK5Type Ck5Type { get; set; }

        public bool IsModified { get; set; }

        public DateTime? GiDate { get; set; }
        public DateTime? GrDate { get; set; }
        public string DnNumber { get; set; }
        public string MatDoc { get; set; }

        public List<CK5MaterialDto> Ck5Material { get; set; } 
    }

    public class CK5WorkflowDocumentData
    {
        public string RegistrationNumber { get; set; }
        public DateTime RegistrationDate { get; set; }
        public List<CK5_FILE_UPLOADDto> Ck5FileUploadList { get; set; }

        public string Back1Number { get; set; }
        public DateTime? Back1Date { get; set; }
    }

    public class CK5WorkflowHistoryInput
    {
        public long DocumentId { get; set; }
        public string DocumentNumber { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public Enums.ActionType ActionType { get; set; }
        public string Comment { get; set; }
        public bool IsModified { get; set; }
    }

    public class CK5GetSummaryReportByParamInput
    {
        public string CompanyCodeSource { get; set; }

        public string CompanyCodeDest { get; set; }
        
        public string NppbkcIdSource { get; set; }
        
        public string NppbkcIdDest { get; set; }
        
        public string PlantSource { get; set; }
        
        public string PlantDest { get; set; }
        
        public DateTime? DateFrom { get; set; }
        
        public DateTime? DateTo { get; set; }
        
        public int Month { get; set; }
        
        public int Year { get; set; }

        public Enums.CK5Type Ck5Type { get; set; }

        public List<string> ListUserPlant { get; set; }

        public string UserId { get; set; }

        public string MaterialNumber { get; set; }
        public string MaterialDescription { get; set; }
        public string Poa { get; set; }
        public string Creator { get; set; }
        public Enums.UserRole UserRole { get; set; }
    }

    public class Ck5GetForLack1ByParamInput
    {
        public string CompanyCode { get; set; }
        public string NppbkcId { get; set; }
        public string ReceivedPlantId { get; set; }
        public Enums.Lack1Level Lack1Level { get; set; }
        public int ExGroupTypeId { get; set; }
        public string SupplierPlantId { get; set; }
        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }

        public bool IsExcludeSameNppbkcId { get; set; }

        public List<string> StoNumberList { get; set; }

        public List<int> Pbck1DecreeIdList { get; set; }

    }

    public class Ck5GetForLack2ByParamInput
    {
        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }
        public string SourcePlantId { get; set; }
        public int ExGroupTypeId { get; set; }
        public string CompanyCode { get; set; }
        public string NppbkcId { get; set; }

        public bool isSameNppbkcAllowed { get; set; }
    }

    public class Back1DataOutput
    {
        public string Back1Number { get; set; }
        public DateTime? Back1Date { get; set; }
   }

    public class CK5MarketReturnGetSummaryReportByParamInput
    {
        public string FaCode { get; set; }
        public string Poa { get; set; }
        public string Creator { get; set; }
        public string Pbck3No { get; set; }
        public string Ck2No { get; set; }

        public string Brand { get; set; }
        public string Content { get; set; }
        public string Hje { get; set; }
        public string Tariff { get; set; }
        public string Ck5MarketReturnQty { get; set; }
        public string FiscalYear { get; set; }
        public string ExciseValue { get; set; }
        public string Pbck3Status { get; set; }
        public string Ck2Value { get; set; }

        public List<string> ListUserPlant { get; set; }

        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
    }

    public class EditCompletedDocumentCk5Input
    {
        public long DocumentId { get; set; }
        public string Brand { get; set; }
        public string Content { get; set; }
        public string Hje { get; set; }
        public string Tariff { get; set; }
        public string Ck5MarketReturnQty { get; set; }
        public string FiscalYear { get; set; }
        public string ExciseValue { get; set; }
        public string Pbck3Status { get; set; }
        public string Ck2Value { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }

        public List<CK5_FILE_UPLOADDto> Ck5FileUploadList { get; set; }

        public string REGISTRATION_NUMBER { get; set; }
        public DateTime? REGISTRATION_DATE { get; set; }

        public Enums.ExciseSettlement EX_SETTLEMENT_ID { get; set; }
        public Enums.ExciseStatus EX_STATUS_ID { get; set; }
        public Enums.RequestType REQUEST_TYPE_ID { get; set; }
        public Enums.CarriageMethod? CARRIAGE_METHOD_ID { get; set; }

        public string INVOICE_NUMBER { get; set; }
        public DateTime? INVOICE_DATE { get; set; }

        public string SEALING_NOTIF_NUMBER { get; set; }
        public DateTime? SEALING_NOTIF_DATE { get; set; }

        public string UNSEALING_NOTIF_NUMBER { get; set; }
        public DateTime? UNSEALING_NOTIF_DATE { get; set; }

        public DateTime? GI_DATE { get; set; }
        public DateTime? GR_DATE { get; set; }

        public bool IsCk5Waste { get; set; }
        public bool IsCk5Manual { get; set; }

        public bool IS_REDUCE_PBCK1 { get; set; }
        public bool IS_LAB { get; set; }

        public List<CK5MaterialDto> Ck5MaterialDtos { get; set; }
    }

    public class GetMatdocListInput
    {
        public long Ck5Id { get; set; }
        public string PlantId { get; set; }
        public string MaterialId { get; set; }

    }

    public class Ck5GetForLack1DetailTis
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string PlantReceiverFrom { get; set; }
        public string PlantReceiverTo { get; set; }
    }

    public class Ck5GetForLack1DetailEa
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string PlantReceiverFrom { get; set; }
        public string PlantReceiverTo { get; set; }
    }
}
