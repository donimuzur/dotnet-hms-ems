using System.Collections.Generic;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.XmlLog
{
    public class XmlLogIndexViewModel : BaseModel
    {
        public XmlLogIndexViewModel()
        {
            ListXmlLogs = new List<XmlLogFormViewModel>();
        }

        public string FileName { get; set; }
        public int? Month { get; set; }
        public string LogDate { get; set; }

        public SelectList FileNameList { get; set; }
        public SelectList MonthList { get; set; }

        public List<XmlLogFormViewModel> ListXmlLogs { get; set; }
    }

 
}