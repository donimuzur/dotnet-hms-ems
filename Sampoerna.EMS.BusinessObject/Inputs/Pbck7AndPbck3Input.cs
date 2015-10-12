using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class Pbck7AndPbck3Input
    {
        public string NppbkcId { get; set; }
        public string PlantId { get; set; }
        public string Pbck7Date { get; set; }
        public string Pbck3Date { get; set; }
        public string Poa { get; set; }
        public string Creator { get; set; }
        public string ShortOrderColum { get; set; }
        public Enums.Pbck7Type Pbck7AndPvck3Type { get; set; }
      


    }


    public class Pbck7SummaryInput
    {
        public string NppbkcId { get; set; }
        public string PlantId { get; set; }
        public int? From { get; set; }
        public int? To { get; set; }
        public string Pbck7Number { get; set; }
        public string ShortOrderColum { get; set; }
       




    }
    public class Pbck3SummaryInput
    {
        public string NppbkcId { get; set; }
        public string PlantId { get; set; }
        public int? From { get; set; }
        public int? To { get; set; }
        public string Pbck3Number { get; set; }
        public string ShortOrderColum { get; set; }

    }

    public class InsertPbck3FromCk5MarketReturnInput
    {
        public long Ck5Id { get; set; }
        public string NppbkcId { get; set; }
        public string UserId { get; set; }
    }

    public class Pbck7Pbck3SaveInput
    {
        public Pbck7AndPbck3Dto Pbck7Pbck3Dto { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public List<Pbck7ItemUpload> Pbck7Pbck3Items { get; set; }
        public Enums.FormType FormType { get; set; }
    }

    public class Pbck7Pbck3WorkflowHistoryInput
    {
        public long DocumentId { get; set; }
        public string DocumentNumber { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public Enums.ActionType ActionType { get; set; }
        public string Comment { get; set; }
        public Enums.FormType FormType { get; set; }
    }

    public class Pbck7Pbck3WorkflowDocumentInput
    {
        public long DocumentId { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public string Comment { get; set; }
        public Enums.ActionType ActionType { get; set; }
        public string DocumentNumber { get; set; }

        public Pbck4WorkflowDocumentData AdditionalDocumentData { get; set; }

        public List<Pbck7ItemUpload> UploadItemDto { get; set; }

        public Enums.FormType FormType { get; set; }
    }
}
