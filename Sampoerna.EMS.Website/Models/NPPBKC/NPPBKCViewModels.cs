﻿using System.Collections.Generic;

namespace Sampoerna.EMS.Website.Models.NPPBKC
{
    public class NPPBKCViewModels : BaseModel
    {

        public NPPBKCViewModels()
        {
            Details = new List<DetailNppbck>();
        }

        public List<DetailNppbck> Details;
        
    }

    public class DetailNppbck
    {
        public long NppbckId { get; set; }
        public string Addr1 { get; set; }
        public string City { get; set; }
        public string RegionOfficeIdNppbkc { get; set; }
        public string TextTo { get; set; }
    }

}