using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.MapSKEP
{
    public class ReceivedDecreeDetailViewModel : BaseModel
    {
        public ReceivedDecreeDetailViewModel()
        {
            this.ViewModel = new ReceivedDecreeDetailModel();
            this.ListReceivedDecreeDetail = new List<ReceivedDecreeDetailModel>();
        }

        public ReceivedDecreeDetailModel ViewModel { get; set; }
        public List<ReceivedDecreeDetailModel> ListReceivedDecreeDetail { get; set; }
    }
}