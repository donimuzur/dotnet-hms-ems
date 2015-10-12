namespace Sampoerna.EMS.Website.Models.LACK2
{
    public class Lack2DetailViewModel : BaseModel
    {
        public Lack2DetailViewModel()
        {
            Detail = new Lack2ItemModel();
        }
        public Lack2ItemModel Detail { get; set; }

        #region View Purpose
        
        public bool AllowGovApproveAndReject { get; set; }
        public bool AllowApproveAndReject { get; set; }
        public bool AllowManagerReject { get; set; }
        public string Comment { get; set; }

        public string MenuLack2OpenDocument { get; set; }
        public string MenuLack2CompletedDocument { get; set; }

        #endregion

    }
}