namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class PBCK1Input
    {
        public string NPBCKID { get; set; }
        public int? POA { get; set;      }
        public string Pbck1Type { get; set; }

        public int? GoodType_ID { get; set; }
        public int? Creator { get; set; }
        public int? Year { get; set; }
        /// <summary>
        /// optional if want to sorting from query
        /// </summary>
        public string SortOrderColumn { get; set; }

    }
}
