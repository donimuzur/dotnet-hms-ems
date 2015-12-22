using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Website.Models.NPPBKC
{
    public class NPPBKCIViewModels : BaseModel
    {

        public NPPBKCIViewModels()
        {
            Details = new List<VirtualNppbckDetails>();

        }
        public List<VirtualNppbckDetails> Details { get; set; }
        
    }
    public class NppbkcFormModel : BaseModel
    {
        public NppbkcFormModel()
        {
            Detail = new VirtualNppbckDetails();
            Plant = new List<T001W>();
        }
        public VirtualNppbckDetails Detail { get; set; }
        public List<T001W> Plant { get; set; }
    }

    public class VirtualNppbckDetails
    {
        public string VirtualNppbckId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        [Required(ErrorMessage = "please fill this field")]
        public string CityAlias { get; set; }

        [Required(ErrorMessage = "please fill this field")]
        public string RegionOfficeOfDGCE { get; set; }
        public string Region { get; set; }
        [Required(ErrorMessage = "please fill this field")]
        public string TextTo { get; set; }

        [Required(ErrorMessage = "Enter the Issued date.")]

        public DateTime? CreateDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        public string KppbcId { get; set; }
        public ZAIDM_EX_KPPBC KPPBC { get; set; }
        public string AcountNumber { get; set; }
        public LFA1 VENDOR { get; set; }
        public string Is_Deleted { get; set; }
        public bool FlagForLack1 { get; set; }
    }

    public class NppbkcItemModel
    {
        public string NPPBKC_ID { get; set; }
        public string ADDR1 { get; set; }
        public string ADDR2 { get; set; }
        public string CITY { get; set; }
        public string CITY_ALIAS { get; set; }
        public string KPPBC_ID { get; set; }
        public string REGION { get; set; }
        public string REGION_DGCE { get; set; }
        public string VENDOR_ID { get; set; }
        public string BUKRS { get; set; }
        public string TEXT_TO { get; set; }
        public DateTime? START_DATE { get; set; }
        public DateTime? END_DATE { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public bool? IS_DELETED { get; set; }
        public bool? FLAG_FOR_LACK1 { get; set; }


        public T001Dto T001 { get; set; }
    }

}