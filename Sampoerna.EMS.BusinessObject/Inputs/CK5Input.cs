
using System;
using System.Collections.Generic;
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
        public List<CK5Dto> Ck5Dto { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public List<CK5MaterialDto> Ck5Material { get; set; }
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
    }

    public class CK5WorkflowDocumentData
    {
        public string RegistrationNumber { get; set; }
        public DateTime RegistrationDate { get; set; }
        public List<CK5_FILE_UPLOADDto> Ck5FileUploadList { get; set; } 
    }

    public class CK5WorkflowHistoryInput
    {
        public long DocumentId { get; set; }
        public string DocumentNumber { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public Enums.ActionType ActionType { get; set; }
        public string Comment { get; set; }
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
}
