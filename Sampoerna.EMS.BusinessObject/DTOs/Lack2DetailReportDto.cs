using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack2DetailReportDto
    {
        public string Lack2Number { get; set; }
        public string Ck5GiDate { get; set; }
        public string Ck5RegistrationNumber { get; set; }
        public string Ck5RegistrationDate { get; set; }
        public string Ck5Total { get; set; }
        public string ReceivingCompanyCode { get; set; }
        public string ReceivingCompanyName { get; set; }
        public string ReceivingNppbkc { get; set; }
        public string ReceivingAddress { get; set; }

        public string Ck5SendingPlant { get; set; }
        public string SendingPlantAddress { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string NppbkcId { get; set; }
        public string TypeExcisableGoods { get; set; }
        public string TypeExcisableGoodsDesc { get; set; }

        public string PeriodYear { get; set; }
        public DateTime? GiDate { get; set; }
    }
}
