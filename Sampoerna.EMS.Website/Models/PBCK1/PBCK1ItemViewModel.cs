using System.Collections.Generic;
using System.Web.Mvc;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.PBCK1
{
    public class Pbck1ItemViewModel : BaseModel
    {
        public Pbck1ItemViewModel()
        {
            ProductConversions = new List<Pbck1ProdConvModel>();
            ProductPlans = new List<Pbck1ProdPlanModel>();
        }

        public string SubmitType { get; set; }

        public Pbck1Item Detail { get; set; }

        public Enums.PBCK1Type Pbck1Types { get; set; }

        public SelectList PbckReferenceList { get; set; }

        public SelectList NppbkcList { get; set; }

        public SelectList SupplierPortList { get; set; }

        public SelectList SupplierPlantList { get; set; }

        public SelectList PoaList { get; set; }

        public SelectList GoodTypeList { get; set; }

        public SelectList MonthList { get; set; }

        public SelectList YearList { get; set; }

        public SelectList UomList { get; set; }

        public Enums.DocumentStatusGov StatusGovList { get; set; }

        public List<Pbck1ProdConvModel> ProductConversions { get; set; }
        public List<Pbck1ProdPlanModel> ProductPlans { get; set; }

    }
}