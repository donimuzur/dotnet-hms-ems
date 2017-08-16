using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class MaterialInput
    {
        public string PlantIdSource { get; set; }

        public string PlantNameSource { get; set; }
        

        public string MaterialNumberSource { get; set; }

        public string MaterialDescSource { get; set; }



        

        public string UomNameSource { get; set; }
        

        public string GoodTypeSource { get; set; }
        
        

        public bool? PlantDeletionSource { get; set; }
        public bool? ClientDeletionSource { get; set; }
    }
}
