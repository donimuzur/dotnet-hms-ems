using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Ck4CDto
    {
        public int Ck4CId { get; set; }
        public string Number { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string PlantId { get; set; }
        public string PlantName { get; set; }
        public string NppbkcId { get; set; }
        public string ApprovedByPoa { get; set; }
        public DateTime? ApprovedDatePoa { get; set; }
        public string ApprovedByManager { get; set; }
        public DateTime? ApprovedDateManager { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? ReportedOn { get; set; }
        public int? ReportedPeriod { get; set; }
        public int ReportedMonth { get; set; }
        public int ReportedYears { get; set; }
        public Enums.DocumentStatus Status { get; set; }
        public Enums.StatusGovCk4c? StatusGoverment { get; set; }
        public string Comment { get; set; }
        public DateTime? DecreeDate { get; set; }

        //Month
        public int MonthId { get; set; }
        public string MonthNameIndo { get; set; }
        public string MonthNameEng { get; set; }

        public List<Ck4cItem> Ck4cItem { get; set; }
    }

    public class Ck4cItem
    {
        public long Ck4CItemId { get; set; }
        public int Ck4CId { get; set; }
        public string FaCode { get; set; }
        public string Werks { get; set; }
        public Decimal ProdQty { get; set; }
        public string ProdQtyUom { get; set; }
        public DateTime ProdDate { get; set; }
        public Decimal HjeIdr { get; set; }
        public Decimal Tarif { get; set; }
        public string ProdCode { get; set; }
        public Decimal PackedQty { get; set; }
        public Decimal UnpackedQty { get; set; }
        public int ContentPerPack { get; set; }
        public int PackedInPack { get; set; }
    }
}
