using System;

namespace Sampoerna.EMS.Website.Models.PLANT
{
    public class T001WModel
    {
        public string WERKS { get; set; }
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
    }
}