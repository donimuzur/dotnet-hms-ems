namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack1ItemViewModel : BaseModel
    {
        public Lack1ItemModel Detail { get; set; }
        public string ControllerAction { get; set; }
        public bool AllowGovApproveAndReject { get; set; }
        public bool AllowApproveAndReject { get; set; }
        public bool AllowManagerReject { get; set; }
        public string Comment { get; set; }
    }
}