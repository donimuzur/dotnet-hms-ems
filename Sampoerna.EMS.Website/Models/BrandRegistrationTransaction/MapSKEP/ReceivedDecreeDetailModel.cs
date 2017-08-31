using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.MapSKEP
{
    public class ReceivedDecreeDetailModel : BaseModel
    {
        public string Brand_CE { get; set; }
        public string Prod_Code { get; set; }
        public int Company_Tier { get; set; }
        public string Unit { get; set; }
        public string Brand_Content { get; set; }
        public decimal Tariff { get; set; }
        public long Received_ID { get; set; }
        public long PD_Detail_ID { get; set; }
        public long Received_Detail_ID { get; set; }
        public long HJEperPack { get; set; }
        public decimal HJEperPack_dec { get; set; }
        public long HJEperBatang { get; set; }
        public bool IsActive { get; set; }
        public string Request_No { get; set; }
        public string Fa_Code_Old { get; set; }
        public string Fa_Code_Old_Desc { get; set; }
        public string Fa_Code_New { get; set; }
        public string Fa_Code_New_Desc { get; set; }        
        public string CompanyName { get; set; }
        public string HlCode { get; set; }
        public string MarketDesc { get; set; }
        public string ProductionCenter { get; set; }
        public ProductDevelopment.ProductDevDetailModel Item { get; set; }
}
}