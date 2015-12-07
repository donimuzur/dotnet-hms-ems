using System;
using System.ComponentModel.DataAnnotations;

namespace Sampoerna.EMS.Website.Models.XmlLog
{
    public class XmlLogFormViewModel : BaseModel
    {
        public long XmlLogId { get; set; }

         [UIHint("DateTime")]
        public DateTime? TimeStamp { get; set; }
         public string TimeStampDisplay { get; set; }
        public string Level { get; set; }
        public string Type { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string Stacktrace { get; set; }
        public string Source { get; set; }
        public string Data { get; set; }
        public string FileName { get; set; }
    }
}