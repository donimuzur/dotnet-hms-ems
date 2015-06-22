using System.Collections.Generic;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.PBCK1
{
    public class PBCK1ItemViewModel : BaseModel
    {
        public PBCK1ItemViewModel()
        {
            ProductConversions = new List<PBCK1ProdConvModel>();
            ProductPlans = new List<PBCK1ProdPlanModel>();
        }
        public PBCK1Item Detail { get; set; }

        public Enums.PBCK1Type PBCK1Types { get; set; }

        public SelectList PbckReferenceList { get; set; }

        public SelectList NppbkcList { get; set; }

        public SelectList SupplierPortList { get; set; }

        public SelectList SupplierPlantList { get; set; }

        public SelectList PoaList { get; set; }

        public SelectList GoodTypeList { get; set; }

        public SelectList MonthList { get; set; }

        public SelectList YearList { get; set; }

        public SelectList UOMList { get; set; }

        public Enums.DocumentStatusGov StatusGovList { get; set; }

        public List<PBCK1ProdConvModel> ProductConversions { get; set; }
        public List<PBCK1ProdPlanModel> ProductPlans { get; set; }

    }
}