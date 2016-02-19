using System;
using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class Lack1SaveEditInput
    {
        public Lack1DetailsDto Detail { get; set; }
        public string UserId { get; set; }
        public Enums.ActionType WorkflowActionType { get; set; }
        public bool IsTisToTis { get; set; }
    }

    public class Lack1GetDashboardDataByParamInput
    {
        public int? PeriodMonth { get; set; }
        public int? PeriodYear { get; set; }
        public string Creator { get; set; }
        public string Poa { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public List<string> NppbkcList { get; set; }
        public List<string> DocumentNumberList { get; set; }
        public List<string> ListNppbkc { get; set; }
        public List<string> ListUserPlant { get; set; }

    }

    public class Lack1UpdateSomeField
    {
        public long Id { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public decimal? WasteQty { get; set; }
        public string WasteUom { get; set; }
        public decimal? ReturnQty { get; set; }
        public string ReturnUom { get; set; }
        public string Noted { get; set; }
    }
}
