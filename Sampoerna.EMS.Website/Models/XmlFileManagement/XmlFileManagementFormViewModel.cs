
using System;
using System.ComponentModel.DataAnnotations;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.XmlFileManagement
{
    public class XmlFileManagementFormViewModel : BaseModel
    {
        public long XmlLogId { get; set; }

        public string FileName { get; set; }
        [UIHint("DateTime")]
        public DateTime? TimeStamp { get; set; }
        public string TimeStampDisplay { get; set; }
        public Enums.XmlLogStatus XmlLogStatus { get; set; }
        public string XmlLogStatusDescription { get; set; }
        
    }
}