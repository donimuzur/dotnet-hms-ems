using System;

namespace Sampoerna.EMS.Website.Models.XmlLog
{
    public class XmlLogFormViewModel : BaseModel
    {
        public long XmlLogId { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}