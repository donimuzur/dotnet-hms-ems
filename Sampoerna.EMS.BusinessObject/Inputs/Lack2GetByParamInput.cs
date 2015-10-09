using System.Collections.Generic;
using Sampoerna.EMS.Core;
using System;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class Lack2GetByParamInput
    {
        public string NppbKcId { get; set; }
        public string Poa { get; set; }
        public string PlantId { get; set; }
        //public int? PeriodMonth { get; set; }
        //public int? PeriodYear { get; set; }
        public string Creator { get; set; }
        public DateTime? SubmissionDate { get; set; }
        
        /// <summary>
        /// optional if want to sorting from query
        /// </summary>
        public string SortOrderColumn { get; set; }

        public bool IsOpenDocList { get; set; }

        public string UserId { get; set; }
        public Enums.UserRole UserRole { get; set; }

        public List<string> NppbkcList { get; set; }

        public List<string> DocumentNumberList { get; set; }

    }
}
