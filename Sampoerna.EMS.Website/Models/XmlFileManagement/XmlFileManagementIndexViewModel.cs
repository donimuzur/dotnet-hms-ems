
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.XmlFileManagement
{
    public class XmlFileManagementIndexViewModel : BaseModel
    {
        public XmlFileManagementIndexViewModel()
        {
            ListXmlLogs = new List<XmlFileManagementFormViewModel>();
        }

        public DateTime? DateFrom { get; set; }
        public SelectList DateFromList { get; set; }

        public DateTime? DateTo { get; set; }
        public SelectList DateToList { get; set; }


        public List<XmlFileManagementFormViewModel> ListXmlLogs { get; set; }
    }
}