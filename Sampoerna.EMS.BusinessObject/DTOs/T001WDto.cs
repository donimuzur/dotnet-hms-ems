using System;

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
        public System.DateTime CREATED_DATE { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<bool> IS_MAIN_PLANT { get; set; }

        public string Npwp { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string KppbcCity { get; set; }
        public string KppbcNo { get; set; }
        public string CompanyCode { get; set; }

        public bool IsChecked { get; set; }
    }

    public class PlantDto
    {
        public string Werks { get; set; }
        public string Name { get; set; }

        public bool IsChecked { get; set; }
    }
}
