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
            Ck4cTotal = new List<Ck4cTotalProd>();
        }

        public Ck4cReportInformationDto Detail { get; set; }
        public List<Ck4cReportItemDto> Ck4cItemList { get; set; }
        public List<Ck4cTotalProd> Ck4cTotal { get; set; }
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

        public string ProdQty { get; set; }

        public string ProdType { get; set; }

        public string Merk { get; set; }

        public string Hje { get; set; }

        public string No { get; set; }

        public string NoProd { get; set; }

        public string ProdDate { get; set; }

        public string SumBtg { get; set; }

        public string BtgGr { get; set; }

        public string Isi { get; set; }

        public string Total { get; set; }

        public string ProdWaste { get; set; }

        public string Comment { get; set; }
    }

    public class Ck4cTotalProd
    {
        public string Comment { get; set; }

        public string ProdType { get; set; }

        public string ProdTotal { get; set; }

        public string ProdBtg { get; set; }
    }
}
