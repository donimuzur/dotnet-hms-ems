using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack2GeneratedDto
    {
        public Lack2GeneratedDto()
        {
            Ck5Items = new List<Lack2GeneratedItemDto>();
        }
        public List<Lack2GeneratedItemDto> Ck5Items { get; set; }
    }

    public class Lack2GeneratedItemDto
    {
        public long CK5_ID { get; set; }
        public string GIDateStr { get; set; }
        public string NoDateWithFormat { get; set; }
        public decimal? GRAND_TOTAL_EX { get; set; }
        public string DEST_PLANT_COMPANY_NAME { get; set; }
        public string DEST_PLANT_NPPBKC_ID { get; set; }
        public string DEST_PLANT_ADDRESS { get; set; }
    }
}
