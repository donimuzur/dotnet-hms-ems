using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class Pbck4GetByParamInput
    {
        public string NppbkcId { get; set; }

        public string PlantId { get; set; }

        public DateTime? ReportedOn { get; set; }

        public string Poa { get; set; }

        public string Creator { get; set; }

        public string SortOrderColumn { get; set; }

        public bool IsCompletedDocument { get; set; }

        public string UserId { get; set; }

        public Enums.UserRole UserRole { get; set; }
    }

    public class Pbck4SaveInput
    {
        public Pbck4Dto Pbck4Dto { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public List<Pbck4ItemDto> Pbck4Items { get; set; }
    }

    public class Pbck4WorkflowHistoryInput
    {
        public long DocumentId { get; set; }
        public string DocumentNumber { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public Enums.ActionType ActionType { get; set; }
        public string Comment { get; set; }
        public bool IsModified { get; set; }
    }

    public class Pbck4WorkflowDocumentInput
    {
        public long DocumentId { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public string Comment { get; set; }
        public Enums.ActionType ActionType { get; set; }
        public string DocumentNumber { get; set; }

        public Pbck4WorkflowDocumentData AdditionalDocumentData { get; set; }

        public List<Pbck4ItemDto> UploadItemDto { get; set; }

        public Enums.DocumentStatusGov GovStatusInput { get; set; }
        public bool IsModified { get; set; }
    }

    public class Pbck4WorkflowDocumentData
    {

        public string Back1No { get; set; }
        public DateTime? Back1Date { get; set; }

        public string Ck3No { get; set; }
        public DateTime? Ck3Date { get; set; }
        public string Ck3OfficeValue { get; set; }

        public List<PBCK4_DOCUMENTDto> Back1FileUploadList { get; set; }
        public List<PBCK4_DOCUMENTDto> Ck3FileUploadList { get; set; }
        //public string RegistrationNumber { get; set; }
        //public DateTime RegistrationDate { get; set; }
        //public List<CK5_FILE_UPLOADDto> Ck5FileUploadList { get; set; }
    }

    public class Pbck4GetSummaryReportByParamInput
    {
        public string Pbck4No { get; set; }

        public int? YearFrom { get; set; }

        public int? YearTo  { get; set; }

        public string PlantId { get; set; }

    }

    public class GetListCk1ByPlantAndFaCodeInput
    {
        public string NppbkcId { get; set; }
      
        public string PlantId { get; set; }

        public string FaCode { get; set; }

    }

}
