using System.Collections.Generic;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.LACK1
{
    public class Lack1IndexCompletedDocumentViewModel : BaseModel
    {
        public Lack1IndexCompletedDocumentViewModel()
        {
            Details = new List<Lack1CompletedDocumentData>();
        }
        public string NppbkcId { get; set; }
        public string Poa { get; set; }
        public string PlantId { get; set; }
        public string ReportedOn { get; set; }
        public string Creator { get; set; }

        public SelectList NppbkcIdList { get; set; }
        public SelectList PoaList { get; set; }
        public SelectList PlantIdList { get; set; }
        public SelectList ReportedOnList { get; set; }
        public SelectList CreatorList { get; set; }

        public List<Lack1CompletedDocumentData> Details { get; set; }
    }
}