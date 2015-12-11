using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class GenerateDocNumberInput
    {
        public string NppbkcId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public Enums.FormType FormType { get; set; }
    }
}
