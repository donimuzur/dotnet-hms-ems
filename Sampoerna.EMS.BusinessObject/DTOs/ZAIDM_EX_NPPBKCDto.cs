using System;
using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class ZAIDM_EX_NPPBKCDto
    {
        public string NPPBKC_ID { get; set; }
        /// <summary>
        /// use for TextField on dropdownlist
        /// </summary>
        public string DROPDOWNTEXT { get; set; }
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
        public System.DateTime CREATED_DATE { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public bool? IS_DELETED { get; set; }

        public LFA1Dto LFA1 { get; set; }
        public T001Dto T001 { get; set; }
        public ICollection<T001WDto> T001W { get; set; }
        public ZAIDM_EX_KPPBC ZAIDM_EX_KPPBC { get; set; }
        public ICollection<POA_MAP> POA_MAP { get; set; }
        public USER USER { get; set; }
    }
}
