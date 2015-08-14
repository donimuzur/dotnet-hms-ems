using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.BusinessObject.DTOs;
﻿using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class Lack1GetByParamInput
    {
        public string NppbKcId { get; set; }
        public string Poa { get; set; }
        public string PlantId { get; set; }
        //public int? PeriodMonth { get; set; }
        //public int? PeriodYear { get; set; }
        public string Creator { get; set; }
        public string SubmissionDate { get; set; }

        /// <summary>
        /// optional if want to sorting from query
        /// </summary>
        public string SortOrderColumn { get; set; }
        public Enums.LACK1Type Lack1Type { get; set; }

      
    }

    public class Lack1SaveInput
    {
        public Lack1Dto Lack1 { get; set; }
        public string UserId { get; set; }
        public Enums.ActionType WorkflowActionType { get; set; }
    }

    public class Lack1WorkflowDocumentInput
    {
        public long DocumentId { get; set; }
        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public string Comment { get; set; }
        public Enums.ActionType ActionType { get; set; }
        public string DocumentNumber { get; set; }

        public Lack1WorkflowDocumentData AdditionalDocumentData { get; set; }

    }

    public class Lack1WorkflowDocumentData
    {
        public DateTime DecreeDate { get; set; }
        public Lack1DocumentDto Lack1Document { get; set; }
    }
    public class Lack1GetLatestSaldoPerPeriodInput
    {
        public int MonthTo { get; set; }
        public int YearTo { get; set; }
        public string NppbkcId { get; set; }
    }

}
