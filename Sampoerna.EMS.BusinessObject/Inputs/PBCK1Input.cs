namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class PBCK1Input
    {
        public string NPBCKID { get; set; }
        public string POA { get; set;      }
        public Core.Enums.PBCK1Type? Pbck1Type { get; set; }

        public string GoodType_ID { get; set; }
        public string  Creator { get; set; }
        public int? Year { get; set; }
        /// <summary>
        /// optional if want to sorting from query
        /// </summary>
        public string SortOrderColumn { get; set; }

    }
}
