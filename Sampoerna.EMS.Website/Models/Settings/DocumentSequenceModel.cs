using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.Settings
{
    public class DocumentSequenceModel 
    {
        public int LastSequence;
        public int MonthInt;

        public string MonthName_Ind;
        public string MonthName_Eng;

        public int Year;
    }

    public class DocumentSequenceListModel : BaseModel {

        public DocumentSequenceListModel() {
            Details = new List<DocumentSequenceModel>();
        }
        public List<DocumentSequenceModel> Details { get; set; }
    }
}