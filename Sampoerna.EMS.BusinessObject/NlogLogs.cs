//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sampoerna.EMS.BusinessObject
{
    using System;
    using System.Collections.Generic;
    
    public partial class NlogLogs
    {
        public long Nlog_Id { get; set; }
        public Nullable<System.DateTime> Timestamp { get; set; }
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