using System;
using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Website.Models.NPPBKC
{
    public class NPPBKCViewModels : BaseModel
    {
       

        public NPPBKCViewModels()
        {
            Details = new List<DetailsNppbck>();
        }
        public List<DetailsNppbck> Details { get; set; }
       
        public DetailsNppbck Detail { get; set; }
       
        
    }

    public class DetailsNppbck : BaseModel
    {
        public long NppbckId { get; set; }
        public long NppbckNo { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string RegionOfficeIdNppbkc { get; set; }
        public string TextTo { get; set; }
        public long KppbcId { get; set; }
        public string CityAlias { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public ZAIDM_EX_KPPBC Kppbc { get; set; }
        public C1LFA1 Vendor { get; set; }

        
    }

}