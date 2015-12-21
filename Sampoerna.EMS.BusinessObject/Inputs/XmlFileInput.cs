using System;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class GetXmlLogByParamInput
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

    }

    public class UpdateXmlFileInput
    {
        public long XmlId { get; set; }

        public string UserId { get; set; }

        public string SourcePath { get; set; }

        public string DestPath { get; set; }
    }
}
