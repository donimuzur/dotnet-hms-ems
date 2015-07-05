using System;
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
        public string NAME1 { get; set; }
        public string ORT01 { get; set; }
        public string CITY { get; set; }
        public string PHONE { get; set; }
        public string ADDRESS { get; set; }
        public string SKEPTIS { get; set; }
        public string NPPBKC_ID { get; set; }
        public string KPPBC_NO { get; set; }
        public bool? IS_MAIN_PLANT { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public List<PLANT_RECEIVE_MATERIAL> PLANT_RECEIVE_MATERIAL { get; set; }
    }
}
