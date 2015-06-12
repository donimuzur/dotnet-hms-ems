using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Website.Models
{
    public class VirtualMappingPlantViewModel : BaseModel
    {
        public VirtualMappingPlantViewModel()
        {
            Details = new List<VirtualMappingPlantDetail>();
        }

        public List<VirtualMappingPlantDetail> Details { get; set; }
    }

    public class VirtualMappingPlantDetail
        {
            public long VirtualPlantMapId { get; set; }
            public Nullable<long> CompanyId { get; set; }
            public Nullable<long> ImportPlantId { get; set; }
            public long ExportPlantId { get; set; }

            public virtual T1001 T1001 { get; set; }
            public virtual T1001W T1001W { get; set; }
            public virtual T1001W T1001W1 { get; set; }    
        }
        
    }
