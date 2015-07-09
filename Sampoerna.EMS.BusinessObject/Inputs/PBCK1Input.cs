using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class Pbck1GetByParamInput
    {
        public int? NppbkcId { get; set; }
        public int? Poa { get; set;      }
        public Core.Enums.PBCK1Type? Pbck1Type { get; set; }

        public int? GoodTypeId { get; set; }
        public int? Creator { get; set; }
        public int? Year { get; set; }
        /// <summary>
        /// optional if want to sorting from query
        /// </summary>
        public string SortOrderColumn { get; set; }

    }

    public class Pbck1SaveInput 
    {
        public Pbck1Dto Pbck1 { get; set; }
        public int UserId { get; set; }
        public Enums.ActionType WorkflowActionType { get; set; }
    }

}