using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class Lack2GetSummaryReportByParamInput
    {
        public string CompanyCode { get; set; }
        public string NppbkcId { get; set; }
        public string SendingPlantId { get; set; }
        public string GoodType { get; set; }
        public int? PeriodMonth { get; set; }
        public int? PeriodYear { get; set; }
        public Enums.DocumentStatus? DocumentStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }
        public string Creator { get; set; }
        public string Approver { get; set; }

    }

    public class Lack2GetDetailReportByParamInput
    {
        public string CompanyCode { get; set; }
        public string NppbkcId { get; set; }
        public string SendingPlantId { get; set; }
        public string GoodType { get; set; }

        public int? PeriodMonth { get; set; }
        public int? PeriodYear { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
     
    }
}
