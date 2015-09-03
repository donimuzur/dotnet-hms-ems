using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class InvMovementGetForLack1ByParamInput
    {
        public string PlantId { get; set; }
        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }
        public Enums.Lack1Level Lack1Level { get; set; }
        public string NppbkcId { get; set; }

    }
}
