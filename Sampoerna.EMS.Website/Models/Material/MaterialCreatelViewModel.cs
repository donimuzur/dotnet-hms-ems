﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.Material
{
    public class MaterialCreateViewModel : BaseModel
    {
        public long MaterialId { get; set; }

        [Required, Display(Name = "Material Number")]
        public string MaterialNumber { get; set; }

        [Required, Display(Name = "Material Description")]
        public string MaterialDesc { get; set; }

        [Required, Display(Name = "Material Group")]
        public string MaterialGroup { get; set; }

        [Required, Display(Name = "Purchasing Group")]
        public string PurchasingGroup { get; set; }



        [Required, Display(Name = "Plant")]
        public Nullable<int> PlantId { get; set; }
        public string PlantName { get; set; }

        [Required, Display(Name = "Excisable Good Type")]
        public Nullable<int> GoodTypeId { get; set; }
        public string GoodTypeName { get; set; }

        [Required, Display(Name = "Issue Storace Loc")]
        public string IssueStorageLoc { get; set; }

        [Required, Display(Name = "Base UOM")]
        public Nullable<int> UomId { get; set; }
        public string UomName { get; set; }

        [Display(Name = "Created On")]
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedById { get; set; }

        [Display(Name = "Created By"), Editable(false)]
        public string CreatedBy { get; set; }
        public Nullable<bool> IsFromSap { get; set; }

        [Display(Name = "Changed On"), Editable(false)]
        public Nullable<System.DateTime> ChangedDate { get; set; }


        public Nullable<int> ChangedById { get; set; }

        [Display(Name = "Changed By"), Editable(false)]
        public string ChangedBy { get; set; }


        private Nullable<bool> _isPlantDelete;
        [Required, Display(Name = "Plant Deletion")]
        public bool IsPlantDelete
        {
            get
            {
                if (this._isPlantDelete.HasValue)
                {
                    return this._isPlantDelete.Value;
                }
                else {
                    return false;
                }
            }
            set{
                this._isPlantDelete = value;
            }
        }


        private Nullable<bool> _isClientDelete;

        [Required, Display(Name = "Client Deletion")]
        public bool IsClientDelete
        {
            get
            {
                if (this._isClientDelete.HasValue)
                {
                    return this._isClientDelete.Value;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                this._isClientDelete = value;
            }
        }

        [Required, Display(Name = "Convertion")]
        public object Convertion { get; set; }

        // list for dropdown in the form
        public SelectList PlantList { get; set; }
        public SelectList GoodTypeList { get; set; }
        public SelectList BaseUOM { get; set; }

        
    }
}