using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Website.Models.POA
{
    public class POAViewModel : BaseModel
    {
        public POAViewModel()
        {
            Details = new List<ZaidmExPOAOutput>();
        }
        public List<ZaidmExPOAOutput> Details { get; set; }

       
    }

    public class POAViewModelDetails
    {
        public string PoaIdCard { get; set; }
        public string UserName { get; set; }
        public string PoaPrintedName { get; set; }
        public string PoaAddress { get; set; }
        public string PoaPhone { get; set; }
        public string Title { get; set; }
       
    }


}