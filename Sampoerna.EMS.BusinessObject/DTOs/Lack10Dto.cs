using Sampoerna.EMS.Core;
using System;
using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack10Dto
    {
        public int Lack10Id { get; set; }
        public string Lack10Number { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyNpwp { get; set; }
        public int? PeriodMonth { get; set; }
        public int? PeriodYears { get; set; }
        public string PeriodNameInd { get; set; }
        public string PerionNameEng { get; set; }
        public string NppbkcId { get; set; }
        public string PlantId { get; set; }
        public string PlantName { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public Enums.Lack10ReportType ReportType { get; set; }
        public Enums.DocumentStatus Status { get; set; }
        public Enums.DocumentStatusGovType2? GovStatus { get; set; }

        public string StatusName { get; set; }
        public DateTime? DecreeDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public string ApprovedByManager { get; set; }
        public DateTime? ApprovedDateManager { get; set; }

        public string RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }

        public string Reason { get; set; }
        public string Remark { get; set; }

        //Month
        public int MonthId { get; set; }
        public string MonthNameIndo { get; set; }
        public string MonthNameEng { get; set; }

        public List<Lack10Item> Lack10Item { get; set; }
        public List<Lack10DecreeDocDto> Lack10DecreeDoc { get; set; }
    }

    public class Lack10Item
    {
        public long Lack10ItemId { get; set; }
        public int Lack10Id { get; set; }
        public string FaCode { get; set; }
        public string BrandDescription { get; set; }
        public string Werks { get; set; }
        public string PlantName { get; set; }
        public string Type { get; set; }
        public string Uom { get; set; }
        public Decimal WasteValue { get; set; }
    }
}
