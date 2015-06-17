using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
     public  class ZaidmExNPPBKCOutput : BLLBaseOutput
    {
        //ZaidmExNPPBKC
        public long NppckId { get; set; }
        public string NppckNo { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string City { get; set; }
        public long? KppbcId { get; set; }
        public int?  RegionOfficeId { get; set; }
        public long? CompanyIdZaidmExNPPBKC { get; set; }
        public long? VendorIdZaidmExNPPBKC { get; set; }
        public string TextTo { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreateDate { get; set; }

       //T1001
        public long CompanyIdT1001 { get; set; }
        public string Bukrs { get; set; }
        public string BukrsTxt { get; set; }
        public string Npwp { get; set; }
        public DateTime? CreateDateT1001 { get; set; }
        
        //1LFA1
        public long VendorId1LFA1 { get; set; }
        public string Lifnr { get; set; }

        public string Name1  { get; set; }
        public string Name2 { get; set; }
         

    }
}
