namespace Sampoerna.EMS.BusinessObject.DTOs
{
    /// <summary>
    /// use for Dropdownlist source or grid
    /// make it light, mean no need lot of field from T001W table
    /// </summary>
    public class T001WCompositeDto
    {
        public string WERKS { get; set; }
        public string NAME1 { get; set; }
        public string ORT01 { get; set; }
        public string PHONE { get; set; }
        public string ADDRESS { get; set; }
        public string SKEPTIS { get; set; }
        public string NPPBKC_ID { get; set; }

        public string DROPDOWNTEXTFIELD { get; set; }
    }
}
