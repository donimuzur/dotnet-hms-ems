using System;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class NlogDto
    {
        public long Nlog_Id { get; set; }
        public DateTime? Timestamp { get; set; }
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
