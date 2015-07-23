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
    }

    public class Pbck1GetOpenDocumentByParamInput : Pbck1GetByParamInput
    {
        
    }

    public class Pbck1GetCompletedDocumentByParamInput : Pbck1GetByParamInput
    {
        
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
    }

}