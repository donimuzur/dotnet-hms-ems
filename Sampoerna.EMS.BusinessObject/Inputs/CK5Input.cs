
namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class CK5Input
    {
        public string DocumentNumber { get; set; }

        public int? POA { get; set; }

        public int? NPPBKCOrigin { get; set; }

        public int? NPPBKCDestination { get; set; }

        public int? Creator { get; set; }

        public string SortOrderColumn { get; set; }

        public int? Ck5Type { get; set; }
    }
}
