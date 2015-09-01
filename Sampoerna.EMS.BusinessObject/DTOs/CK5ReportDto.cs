using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
   public class CK5ReportDto
   {
       public CK5ReportDto()
       {
           //ReportDetails = new List<CK5ReportDetailsDto>();
           ListMaterials = new List<CK5ReportMaterialDto>();
       }

       public CK5ReportDetailsDto ReportDetails { get; set; }
       public List<CK5ReportMaterialDto> ListMaterials { get; set; }
       public Enums.CK5Type Ck5Type { get; set; }
   }

    public class CK5ReportDetailsDto
    {
        public string OfficeName { get; set; }
        public string OfficeCode { get; set; }
        public string SubmissionNumber { get; set; }
        public string SubmissionDate { get; set; }
        public string RegistrationNumber { get; set; }
        public string RegistrationDate { get; set; }
        public string ExGoodType { get; set; }
        public string ExciseSettlement { get; set; }
        public string ExciseStatus { get; set; }
        public string RequestType { get; set; }

        //data pemberitahuan
        public string SourcePlantNpwp { get; set; }
        public string SourcePlantNppbkc { get; set; }
        public string SourcePlantName { get; set; }
        public string SourcePlantAddress { get; set; }
        public string SourceOfficeName { get; set; }
        public string SourceOfficeCode { get; set; }

        public string DestPlantNpwp { get; set; }
        public string DestPlantNppbkc { get; set; }
        public string DestPlantName { get; set; }
        public string DestPlantAddress { get; set; }
        public string DestOfficeName { get; set; }
        public string DestOfficeCode { get; set; }

        public string FacilityNumber { get; set; }
        public string FacilityDate { get; set; }

        public string CarriageMethod { get; set; }
        public string Total { get; set; }
        public string Uom { get; set; }

        public string PrintDate { get; set; }
        public string PoaName { get; set; }
        public string PoaAddress { get; set; }
        public string PoaIdCard { get; set; }
        public string PoaCity { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }

        public string DestinationCountry { get; set; }
        public string DestinationCode { get; set; }
        public string DestinationNppbkc { get; set; }
        public string DestinationName { get; set; }
        public string DestinationAddress { get; set; }
        public string DestinationOfficeName { get; set; }
        public string DestinationOfficeCode { get; set; }

        public string LoadingPort { get; set; }
        public string LoadingPortName { get; set; }
        public string LoadingPortId { get; set; }
        public string FinalPort { get; set; }
        public string FinalPortName { get; set; }
        public string FinalPortId { get; set; }

        public string MonthYear { get; set; }
    }

    public class CK5ReportMaterialDto
    {
        public string Number { get; set; }
        public string Qty { get; set; }
        public string Uom { get; set; }
        public string Convertion { get; set; }
        public string ConvertedQty { get; set; }
        public string ConvertedUom { get; set; }

        //public string ConvertionItem { get; set; }
        //public string ConvertionUomItem { get; set; }

        public string Hje { get; set; }
        public string Tariff { get; set; }
        public string ExciseValue { get; set; }
        public string UsdValue { get; set; }
        public string Note { get; set; }

        public string MaterialDescription { get; set; }

    }
}
