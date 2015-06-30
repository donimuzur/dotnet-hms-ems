﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Website.Models.NPPBKC
{
    public class NPPBKCIViewModels : BaseModel
    {

        public NPPBKCIViewModels()
        {
            Details = new List<ZAIDM_EX_NPPBKC>();
            
        }
        public List<ZAIDM_EX_NPPBKC> Details { get; set; }
        
     
    }
    public class NppbkcFormModel : BaseModel
    {
        public NppbkcFormModel()
        {
            Detail = new VirtualNppbckDetails();
            Plant = new List<T1001W>();
        }
        public VirtualNppbckDetails Detail { get; set; }
        public List<T1001W> Plant { get; set; }
    }

    public class VirtualNppbckDetails 
    {
        public long VirtualNppbckId { get; set; }
        public string NppbckNo { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string CityAlias { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        public string RegionOfficeOfDGCE{ get; set; }
        public string Region { get; set; }
        public string TextTo { get; set; }
        public DateTime? CreateDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)] 
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)] 
        public DateTime? EndDate { get; set; }
        public string KppbcId { get; set; }
        public ZAIDM_EX_KPPBC KPPBC { get; set; }
        public string AcountNumber { get; set; }
        public C1LFA1 VENDOR { get; set; }
    }

}