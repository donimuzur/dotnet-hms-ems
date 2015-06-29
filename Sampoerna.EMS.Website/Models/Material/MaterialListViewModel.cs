using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Website.Models.Material
{
    public class MaterialListViewModel : BaseModel
    {
        public MaterialListViewModel()
        {
            Details = new List<ZAIDM_EX_MATERIAL>();
        }
        public List<ZAIDM_EX_MATERIAL> Details { get; set; }


    }
}