using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Website.Models.Material
{
    public class MaterialListViewModel : BaseModel
    {
        public MaterialListViewModel()
        {
            Details = new List<MaterialDetails>();
            SearchView = new MaterialSearchView();
        }

        public MaterialSearchView SearchView { get; set; }
        public List<MaterialDetails> Details { get; set; }
        //public string GoodType { get; set; }
        

        public int TotalData { get; set; }
        public int TotalDataPerPage { get; set; }

        public int CurrentPage { get; set; }
    }


    public class MaterialSearchView
    {
        public string PlantIdSource { get; set; }

        public string PlantNameSource { get; set; }
        public SelectList PlantList { get; set; }
        
        public string MaterialNumberSource { get; set; }

        public string MaterialDescSource { get; set; }



        public string BaseUomSource { get; set; }

        public string UomNameSource { get; set; }
        public SelectList UomList { get; set; }

        public string GoodTypeSource { get; set; }
        public SelectList GoodTypeList { get; set; }
        public string GoodTypeNameSource { get; set; }

        public SelectList DeletionFlag {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem(){ Text = "Yes", Value = "True"},
                    new SelectListItem(){ Text = "No", Value = "False"}
                };
                return new SelectList(items,"Value","Text");
            }
            
        }

        public bool? PlantDeletionSource { get; set; }
        public bool? ClientDeletionSource { get; set; }
    }

    public class MaterialUomDetails
    {
        public MaterialUomDetails()
        {
        }
        public long Id { get; set; }
        public string MaterialNumber { get; set; }
        public string Plant { get; set; }
        public string Meinh { get; set; }
        public Nullable<decimal> Umrez { get; set; }
        public string UmrenStr { get; set; }

        public Nullable<decimal> Umren
        {
            get { return Convert.ToDecimal(UmrenStr); }
            set { value = Umren; }
        }

    }


    public class MaterialDetails {

        public MaterialDetails()
        {
            MaterialUomsList = new List<MaterialUomDetails>();
        }

        private List<MaterialUomDetails> MaterialUomsList { get; set; } 
        public string PlantId { get; set; }

        public string PlantName { get; set; }

        //public string StickerId { get; set; }
        
        //public string StickerCode { get; set; }

        //public string FaCode { get; set; }


        //public string PurchasingGroup { get; set; }


        //public string IssueStorage { get; set; }


        //public Nullable<decimal> Conversion { get; set; }

        public string MaterialNumber { get; set; }

        public string MaterialDesc { get; set; }



        public string BaseUom { get; set; }

        public string UomName { get; set; }


        public Nullable<int> GoodtypId { get; set; }

        public string GoodTypeName { get; set; }

        public string PlantDeletion { get; set; }
        public string ClientDeletion { get; set; }

        




    }
}