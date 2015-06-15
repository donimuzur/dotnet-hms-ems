using System.Runtime.InteropServices;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class SaveVirtualMappingPlantOutput : BLLBaseOutput
    {
        public long Id { get; set; }
        public string Company { get; set; }

        public long? ImportVitualPlant { set; get; }

        public long ExportVirtualPlant { set; get; }
    }

}
