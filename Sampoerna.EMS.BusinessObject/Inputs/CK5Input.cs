﻿
using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class CK5GetByParamInput
    {
        public string DocumentNumber { get; set; }

        public int? POA { get; set; }

        public int? NPPBKCOrigin { get; set; }

        public int? NPPBKCDestination { get; set; }

        public int? Creator { get; set; }

        public string SortOrderColumn { get; set; }

        public Enums.CK5Type Ck5Type { get; set; }
    }

    public class CK5SaveInput
    {
        public CK5Dto Ck5Dto { get; set; }
        public int UserId { get; set; }
        public Enums.ActionType WorkflowActionType { get; set; }
        public List<CK5MaterialDto> Ck5Material { get; set; } 
    }
}
