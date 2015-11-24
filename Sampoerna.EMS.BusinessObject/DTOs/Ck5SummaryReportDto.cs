using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
   public class Ck5SummaryReportDto
    {
        public long Ck5Id { get; set; }

        public string Ck5TypeDescription { get; set; }
        public string KppbcCityName { get; set; }
        public string SubmissionNumber { get; set; }
        public string SubmissionDate { get; set; }
        public string ExGoodTypeDesc { get; set; }
        public string ExciseSettlement { get; set; }
        public string ExciseStatus { get; set; }
        public string RequestType { get; set; }
        public string SourcePlant { get; set; }
        public string DestinationPlant { get; set; }

        public string UnpaidExciseFacilityNumber { get; set; }
        public string UnpaidExciseFacilityDate { get; set; }
        public string SealingNotificationDate { get; set; }
        public string SealingNotificationNumber { get; set; }
        public string UnSealingNotificationDate { get; set; }
        public string UnSealingNotificationNumber { get; set; }
        public string Lack1 { get; set; }
        public string Lack2 { get; set; }

        public string TanggalAju { get; set; }
        public string NomerAju { get; set; }
        public string TanggalPendaftaran { get; set; }
        public string NomerPendaftaran { get; set; }
        public string OriginCeOffice { get; set; }
        public string OriginCompany { get; set; }
        public string OriginCompanyNppbkc { get; set; }
        public string OriginCompanyAddress { get; set; }
        public string DestinationCountry { get; set; }
        public string NumberBox { get; set; }
        public string ContainPerBox { get; set; }
        public string TotalOfExcisableGoods { get; set; }
        public string BanderolPrice { get; set; }
        public string ExciseTariff { get; set; }
        public string ExciseValue { get; set; }
        public string DestinationCeOffice { get; set; }
        public string DestCompanyAddress { get; set; }
        public string DestCompanyNppbkc { get; set; }
        public string DestCompanyName { get; set; }
        public string LoadingPort { get; set; }
        public string LoadingPortName { get; set; }
        public string StoNumberSender { get; set; }
        public string StoNumberReciever { get; set; }
        public string StoBNumber { get; set; }
        public string DnNumber { get; set; }
        public string GrDate { get; set; }
        public string GiDate { get; set; }
        public string Status { get; set; }
    }
}
