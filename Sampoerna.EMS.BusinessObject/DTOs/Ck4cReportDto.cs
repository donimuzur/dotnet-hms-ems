using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Ck4cReportDto
    {
        public Ck4cReportDto()
        {
            Ck4cItemList = new List<Ck4cReportItemDto>();
            Detail = new Ck4cReportInformationDto();
            HeaderFooter = new HEADER_FOOTER_MAPDto();
        }

        public Ck4cReportInformationDto Detail { get; set; }
        public List<Ck4cReportItemDto> Ck4cItemList { get; set; }
        public HEADER_FOOTER_MAPDto HeaderFooter { get; set; }
    }

    public class Ck4cReportInformationDto
    {
        public int Ck4cId { get; set; }

        public string Number { get; set; }

        public string ReportedOn { get; set; }

        public string ReportedPeriodStart { get; set; }

        public string ReportedPeriodEnd { get; set; }

        public string ReportedMonth { get; set; }

        public string ReportedYear { get; set; }

        public string CompanyName { get; set; }

        public string CompanyAddress { get; set; }

        public string Nppbkc { get; set; }

        public string Poa { get; set; }

        public string Header { get; set; }

        public string Footer { get; set; }

        public string Preview { get; set; }

        public string ReportedOnDay { get; set; }

        public string ReportedOnMonth { get; set; }

        public string ReportedOnYear { get; set; }

        public string NBatang { get; set; }

        public string NGram { get; set; }

        public string ProdTotal { get; set; }

        public string City { get; set; }
    }

    public class Ck4cReportItemDto
    {
        public int Ck4cItemId { get; set; }
    }
}
