using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class Lack1GeneratedOutput : BLLBaseOutput
    {
        public Lack1GeneratedDto Data { get; set; }
        public bool IsWithTisToTisReport { get; set; }
        public bool IsEtilAlcohol { get; set; }
        public bool HasWasteData { get; set; }
    }
}
