using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Website.Models.Material
{
    public class MaterialIndexViewModel : BaseModel
    {
        public MaterialIndexViewModel()
        {
            Details = new List<MaterialDetails>();
        }
        public List<MaterialDetails> Details { get; set; }


    }

    public class MaterialDetails {
        public long MaterialId { get; set; }

        public long PlantId { get; set; }

        public string PlantName { get; set; }

        //public string StickerId { get; set; }
        
        //public string StickerCode { get; set; }

        //public string FaCode { get; set; }


        //public string PurchasingGroup { get; set; }


        //public string IssueStorage { get; set; }


        //public Nullable<decimal> Conversion { get; set; }

        public string MaterialNumber { get; set; }

        public string MaterialDesc { get; set; }



        public int BaseUom { get; set; }

        public string UomName { get; set; }


        public Nullable<int> GoodtypId { get; set; }

        public int IsDeleted { get; set; }







    }
}