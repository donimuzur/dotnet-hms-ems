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
    }

    public class Ck4cReportItemDto
    {
        public int Ck4cItemId { get; set; }
    }
}
