namespace Sampoerna.EMS.Website.Models.LACK2
{
    public class Lack2EditViewModel : BaseModel
    {
        public Lack2EditViewModel()
        {
            Detail = new Lack2ItemModel();
        }
        public Lack2ItemModel Detail { get; set; }
    }
}