
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using iTextSharp.text;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.XmlFileManagement
{
    public class XmlFileManagementFormViewModel : BaseModel
    {
        public XmlFileManagementFormViewModel()
        {
            DetailListLogs = new List<XmlFileManagementDetailsViewModel>();
        }

        public long XmlLogId { get; set; }

        public string FileName { get; set; }
        [UIHint("DateTime")]
        public DateTime? TimeStamp { get; set; }
        public string TimeStampDisplay { get; set; }
        public Enums.XmlLogStatus XmlLogStatus { get; set; }
        public string XmlLogStatusDescription { get; set; }
        public bool IsError { get; set; }
        
        public List<XmlFileManagementDetailsViewModel> DetailListLogs { get; set; }

    }

    public class XmlFileManagementDetailsViewModel
    {
        public string Description { get; set; }
        public string TimeStampDisplay { get; set; }
    }
}