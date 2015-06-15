using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Website.Models
{
    public class VirtualMappingPlantViewModel : BaseModel
    {
        public VirtualMappingPlantViewModel()
        {
            Details = new List<SaveVirtualMappingPlantOutput>();
        }

        public List<SaveVirtualMappingPlantOutput> Details { get; set; }

        public SelectList CompanyList { set; get; }

        public SelectList ImportList { set; get; }

        public SelectList ExportList { set; get; }
    }

    public class VirtualMappingPlantDetail
        {
            public long VirtualPlantMapId { get; set; }
            public Nullable<long> CompanyId { get; set; }

            public string CompanyName { get; set; }
            public Nullable<long> ImportPlantId { get; set; }
            public long ExportPlantId { get; set; }

            public virtual T1001 T1001 { get; set; }
            public virtual T1001W T1001W { get; set; }
            public virtual T1001W T1001W1 { get; set; }    
        }
        
    }
