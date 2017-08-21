using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.BrandRegistration
{
    public class BrandRegistrationReqDetailViewModel : BaseModel
    {
        public BrandRegistrationReqDetailViewModel()
        {
            this.ViewModel = new BrandRegistrationReqDetailModel();
            this.ListBrandRegistrationReqDetail = new List<BrandRegistrationReqDetailModel>();
        }
        public BrandRegistrationReqDetailModel ViewModel { get; set; }
        public List<BrandRegistrationReqDetailModel> ListBrandRegistrationReqDetail { get; set; }
    }
}