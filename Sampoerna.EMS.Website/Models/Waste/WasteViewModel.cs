using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.Waste
{
    public class WasteViewModel : BaseModel
    {
        public WasteViewModel()
        {
            Details = new List<WasteDetail>();
        }
        public string CompanyCode { get; set; }
        public string PlantWerks { get; set; }
        public string FaCode { get; set; }
        public string BrandDescription { get; set; }
        
        //waste Reject
        public string RejectCigaretteStick{get;set;}
        public decimal? MarkerRejectStickQty { get; set; }
        public decimal? PackerRejectStickQty { get; set; }

        public DateTime WasteProductionDate { get; set; }
        public string CompanyName { get; set; }
        public string PlantName { get; set; }

        //WasteGram
        public decimal WasteQtyGram { get; set; }
        public decimal? DustWasteGramQty { get; set; }
        public decimal? FloorWasteGramQty { get; set; }

        //WasteStick
        public decimal WasteQtyStick{get;set;}
        public decimal? DustWasteStickQty { get; set; }
        public decimal? FloorWasteStickQty { get; set; }

        public Enums.CK4CType Ck4CType { get; set; }

        public List<WasteDetail> Details { get; set; }

        //SelectList
        public SelectList CompanyCodeList { get; set; }
        public SelectList PlantWerksList { get; set; }

    }

    public class WasteDetail : BaseModel
    {
        public string CompanyCode { get; set; }
        public string PlantWerks { get; set; }
        public string FaCode { get; set; }
        public string BrandDescription { get; set; }

        //waste Reject
        public string RejectCigaretteStick { get; set; }

        public string MarkerStr
        {
            get; set;
        }
        public decimal? MarkerRejectStickQty { get; set; }
        public string PackerStr
        {
            get;
            set;
        }
        public decimal? PackerRejectStickQty { get; set; }

        public DateTime WasteProductionDate { get; set; }
        public string CompanyName { get; set; }
        public string PlantName { get; set; }

        //WasteGram
        public decimal WasteQtyGram { get; set; }
        public string DustGramStr
        {
            get;
            set;
        }
        public decimal? DustWasteGramQty { get; set; }
        public string FloorGramStr
        {
            get;
            set;
        }
        public decimal? FloorWasteGramQty { get; set; }

        //WasteStick
        public decimal WasteQtyStick { get; set; }

        public string DustStickStr
        {
            get;
            set;
        }
        public decimal? DustWasteStickQty { get; set; }
        public string FloorStickStr
        {
            get;
            set;
        }
        public decimal? FloorWasteStickQty { get; set; }

        public Enums.CK4CType Ck4CType { get; set; }


        //SelectList
        public SelectList CompanyCodeList { get; set; }
        public SelectList PlantWerksList { get; set; }
        public SelectList FacodeList { get; set; }
        
    }
}