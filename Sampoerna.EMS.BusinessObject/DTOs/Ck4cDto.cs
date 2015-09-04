using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Ck4CDto
    {
        public int Ck4CId { get; set; }
        public string Number { get; set; }
        public string CompnayId { get; set; }
        public string CompanyName { get; set; }
        public string PlantId { get; set; }
        public string PlantName { get; set; }
        public string NppbkcId { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? ReportedOn { get; set; }
        public int? ReportedPeriod { get; set; }
        public int? ReportedMonth { get; set; }
        public int? ReportedYears { get; set; }
        public int Status { get; set; }
        public int  StatusGoverment { get; set; }

        //Month
        public int MonthId { get; set; }
        public string MonthNameIndo { get; set; }
        public string MonthNameEng { get; set; }

        //CK4CItem
        public long Ck4CItemId { get; set; }
        public string FaCode { get; set; }
        public string Werks { get; set; }
        public Decimal ProdQtyPacked { get; set; }
        public Decimal ProdQtyUnpacked { get; set; }
        public string UomProudQty { get; set; }
        public string ProdDate { get; set; }

    }
}
