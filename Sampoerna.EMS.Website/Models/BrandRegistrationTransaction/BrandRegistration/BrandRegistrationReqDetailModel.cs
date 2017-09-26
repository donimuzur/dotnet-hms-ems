using Sampoerna.EMS.Website.Models.Market;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.BrandRegistration
{
    public class BrandRegistrationReqDetailModel : BaseModel
    {
        public long Registration_Detail_ID { get; set; }
        public long Registration_ID { get; set; }

        public long PD_Detail_ID { get; set; }
        public string Brand_Ce { get; set; }
        public string Latest_Skep_No { get; set; }
        public string Prod_Code { get; set; }
        public int Company_Tier { get; set; }
        public decimal Hje { get; set; }
        public decimal HJEperPack { get; set; }
        public decimal HJEperBatang { get; set; }
        public string Unit { get; set; }
        public string Brand_Content { get; set; }
        public decimal Tariff { get; set; }
        public string Packaging_Material { get; set; }
        public string Market_ID { get; set; }
        public string Front_Side { get; set; }
        public string Back_Side { get; set; }
        public string Left_Side { get; set; }
        public string Right_Side { get; set; }
        public string Top_Side { get; set; }
        public string Bottom_Side { get; set; }

        public string Request_No { get; set; }
        public string Fa_Code_Old { get; set; }
        public string Fa_Code_Old_Desc { get; set; }
        public string Fa_Code_New { get; set; }
        public string Fa_Code_New_Desc { get; set; }
        public string CompanyName { get; set; }
        public string HlCode { get; set; }
        public string MarketDesc { get; set; }
        public string ProductType { get; set; }
        public string Created_By_Name { get; set; }
        public int ProdDevNextAction { get; set; }
        public BRProductDevDetailModel Item { get; set; }
        public bool Is_Import { get; set; }

        public string ProductionCenter { get; set; }

        
    }
}