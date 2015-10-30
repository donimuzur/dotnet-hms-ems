
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

        public Enums.CK5Type Ck5Type { get; set; }
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

    }

    public class Ck5GetForLack2ByParamInput
    {
        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }
        public string SourcePlantId { get; set; }
        public int ExGroupTypeId { get; set; }
        public string CompanyCode { get; set; }
        public string NppbkcId { get; set; }
    }

    public class Back1DataOutput
    {
        public string Back1Number { get; set; }
        public DateTime? Back1Date { get; set; }
   }
}
