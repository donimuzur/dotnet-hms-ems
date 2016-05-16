using System;
using System.Collections.Generic;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack1DetailEaDto
    {
        public string PlantIdReceiver { get; set; }
        public string PlantDescReceiver { get; set; }
        public string PlantIdSupplier { get; set; }
        public string PlantDescSupplier { get; set; }
        public string EaCode { get; set; }
        public string EaDesc { get; set; }
        public string BeginingBalance { get; set; }
        public string BeginingBalanceUom { get; set; }
        public string Ck5EmsNo { get; set; }
        public string Ck5RegNo { get; set; }
        public string Ck5RegDate { get; set; }
        public decimal Ck5Qty { get; set; }
        public string Usage { get; set; }
        public string UsageUom { get; set; }
        public string UsagePostingDate { get; set; }
        public string Level { get; set; }
        public string FlavorCode { get; set; }
        public string FlavorDesc { get; set; }
        public string CfProdCode { get; set; }
        public string CfProdDesc { get; set; }
        public decimal CfProdQty { get; set; }
        public string CfProdUom { get; set; }
        public string ProdPostingDate { get; set; }
        public string ProdDate { get; set; }
        public decimal EndingBalance { get; set; }
        public string EndingBalanceUom { get; set; }

        public ICollection<CK5_MATERIAL> CK5_MATERIAL { get; set; }
        public string StoReceiverNumber { get; set; }
    }
}
