using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class Lack10GetByParamInput
    {
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string PlantId { get; set; }
        public string NppbkcId { get; set; }
        public string ShortOrderColumn { get; set; }
        public bool IsOpenDocument { get; set; }
        public List<string> ListNppbkc { get; set; }
        public List<string> ListUserPlant { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class Lack10GetWasteDataInput
    {
        public string CompanyId { get; set; }
        public string PlantId { get; set; }
        public string NppbkcId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public bool IsNppbkc { get; set; }
    }

    public class Lack10WorkflowDocumentInput
    {
        public long DocumentId { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public string Comment { get; set; }
        public Enums.ActionType ActionType { get; set; }
        public string DocumentNumber { get; set; }
        public Lack10WorkflowDocumentData AdditionalDocumentData { get; set; }
    }

    public class Lack10WorkflowDocumentData
    {
        public DateTime DecreeDate { get; set; }
        public List<Lack10DecreeDocDto> Lack10DecreeDoc { get; set; }
    }

    public class Lack10UpdateSubmissionDate
    {
        public long Id { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public DateTime? DecreeDate { get; set; }
    }

    public class Lack10GetSummaryReportByParamInput
    {
        public string Lack10No { get; set; }

        public string NppbkcId { get; set; }

        public bool isForExport { get; set; }

        public List<string> ListNppbkc { get; set; }
        public List<string> ListUserPlant { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public string Poa { get; set; }
        public string Creator { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
