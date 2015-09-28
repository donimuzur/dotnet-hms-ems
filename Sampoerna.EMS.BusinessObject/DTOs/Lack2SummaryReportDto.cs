using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack2SummaryReportDto
    {
        public string Lack2Number { get; set; }
        public string DocumentType { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string NppbkcId { get; set; }
        public string Ck5SendingPlant { get; set; }
        public string SendingPlantAddress { get; set; }
        public string Lack2Period { get; set; }
        public string Lack2Date { get; set; }
        public string TypeExcisableGoods { get; set; }

        public string TotalDeliveryExcisable { get; set; } //ask 
        public string Uom { get; set; } //ask 

        public string Poa { get; set; } //ask 
        public string PoaManager { get; set; } //ask 

        public string CreatedDate { get; set; }
        public string CreatedTime { get; set; }
        public string CreatedBy { get; set; }

        public string ApprovedDate { get; set; }
        public string ApprovedTime { get; set; }
        public string ApprovedBy { get; set; }

        public string LastChangedDate { get; set; }
        public string LastChangedTime { get; set; }

        public string Status { get; set; }

        public string LegalizeData { get; set; } //ask

        //search
        public string PeriodYear { get; set; }
    }
}
