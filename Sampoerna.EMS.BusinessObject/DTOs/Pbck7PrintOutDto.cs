using System;
using System.Collections.Generic;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Pbck73PrintOutDto
    {
        public Pbck73PrintOutDto()
        {
            Items = new List<Pbck73ItemPrintOutDto>();
            HeaderFooter = new HEADER_FOOTER_MAPDto();
        }
        public int PbckId { get; set; }
        public string PbckNumber { get; set; }
        public string Lampiran { get; set; }
        public string PoaId { get; set; }
        public string PoaName { get; set; }
        public string PoaTitle { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyAddressParagraf { get; set; }
        public string NppbkcId { get; set; }
        public string NppbkcTextTo { get; set; }
        public string NppbkcCity { get; set; }
        public string NppbkcStartDate { get; set; }
        public string VendorCity { get; set; }
        public string PrintedDate { get; set; }
        public Enums.DocumentTypePbck7AndPbck3 DocumentType { get; set; }
        public DateTime? ExecDateFrom { get; set; }
        public DateTime? ExecDateTo { get; set; }
        public string ExecDateDisplayString { get; set; }
        public HEADER_FOOTER_MAPDto HeaderFooter { get; set; }

        public List<Pbck73ItemPrintOutDto> Items { get; set; }

    }

    public class Pbck73ItemPrintOutDto
    {
        public string FaCode { get; set; }
        public string ProdTypeAlias { get; set; }
        public string Brand { get; set; }
        public decimal? Content { get; set; }
        public decimal? Qty { get; set; }
        public string SeriesValue { get; set; }
        public decimal? Hje { get; set; }
        public decimal? Tariff { get; set; }
        public decimal? ExciseValue { get; set; }

    }
}
