﻿using System;
using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.Business
{
    public class Plant
    {
        public Plant()
        {
            PLANT_RECEIVE_MATERIAL = new List<PLANT_RECEIVE_MATERIAL>();
        }
        public string WERKS { get; set; }
        public string DROPDOWNTEXTFIELD { get; set; }
        public string NAME1 { get; set; }
        public string ORT01 { get; set; }
        public string PHONE { get; set; }
        public string ADDRESS { get; set; }
        public string SKEPTIS { get; set; }
        public string NPPBKC_ID { get; set; }
        public string NPPBKC_IMPORT_ID { get; set; }
        public bool IS_IMPORT_ID { get; set; }
        public string KPPBC_NO { get; set; }
        public bool? IS_MAIN_PLANT { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public List<PLANT_RECEIVE_MATERIAL> PLANT_RECEIVE_MATERIAL { get; set; }
        public string KPPBC_NAME { get; set; }
        public string SUPPLIER_COMPANY { get; set; }

        //FOR CK5
        public string NPWP { get; set; }
        public string COMPANY_NAME { get; set; }
        public string COMPANY_ADDRESS { get; set; }
        public string KPPBC_CITY { get; set; }

        public string COMPANY_CODE { get; set; }

        public string ADDRESS_IMPORT { get; set; }
    }
}
