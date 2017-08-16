using Sampoerna.EMS.Website.Models.Shared;
using Sampoerna.EMS.Website.Models.SupportDoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.ProductDevelopment
{
    public class ProductDevelopmentModel : BaseModel
    {
        public string PD_NO { get; set; }
        public long PD_ID { get; set; }
        public string Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public string Modified_By { get; set; }
        public DateTime Modified_Date { get; set; }
        public UserModel Creator { set; get; }
        public UserModel LastEditor { set; get; }
        public int Next_Action { get; set; }             
        public SupportDocModel SupportDoc { get; set; }
        public List<SupportDocModel> ListSupportDoc { get; set; }            
        public Shared.WorkflowHistory RevisionData { set; get; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public bool IsCreator { set; get; }
        public IEnumerable<SelectListItem> ListAction { get; set; }

    }
}