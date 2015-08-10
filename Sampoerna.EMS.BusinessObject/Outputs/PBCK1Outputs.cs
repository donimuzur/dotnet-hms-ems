
using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class SavePbck1Output : BLLBaseOutput
    {
        public long Id { get; set; }
        public string Pbck1Number { get; set; }
    }

    public class DeletePbck1Output : BLLBaseOutput
    {
        
    }

}
