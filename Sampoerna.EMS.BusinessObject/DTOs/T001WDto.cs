﻿using System;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
   public class T001WDto
    {
        public string WERKS { get; set; }
        public string DROPDOWNTEXTFIELD { get; set; }
        public string NAME1 { get; set; }
        public string ORT01 { get; set; }
        public string PHONE { get; set; }
        public string ADDRESS { get; set; }
        public string SKEPTIS { get; set; }
        public string NPPBKC_ID { get; set; }
        public string NPPBKC_IMPORT_ID { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public bool? IS_MAIN_PLANT { get; set; }

        public string Npwp { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string KppbcCity { get; set; }
        public string KppbcNo { get; set; }
        public string CompanyCode { get; set; }

        public bool IsChecked { get; set; }

        public string ADDRESS_IMPORT { get; set; }
    }

    public class PlantDto
    {
        public string WERKS { get; set; }
        public string NPPBKC_ID { get; set; }
        public string NPPBKC_IMPORT_ID { get; set; }
        public bool IS_IMPORT_ID { get; set; }
        public string NAME1 { get; set; }

        public bool IsChecked { get; set; }
    }
}
