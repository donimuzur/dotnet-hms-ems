using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class CK5UploadFileDocumentsInput
    {
        public string Ck5Type { get; set; }
        public string KppBcCityName { get; set; }
        public string ExGoodType { get; set; }
        public string ExciseSettlement { get; set; }
        public string ExciseStatus { get; set; }
        public string RequestType { get; set; }
        public string SourcePlantId { get; set; }
        public string DestPlantId { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string PbckDecreeNumber { get; set; }
        public string CarriageMethod { get; set; }
        public string GrandTotalEx { get; set; }
        public string PackageUomName { get; set; }
    }
}
