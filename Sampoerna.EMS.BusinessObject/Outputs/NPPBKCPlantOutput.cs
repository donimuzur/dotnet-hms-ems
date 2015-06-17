using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Outputs
{
    public class NPPBKCPlantOutput
    {
        //NPPBKC_PLANT
        public int NppbkcPlantId { get; set; }
        public long? NppbkcId { get; set; }
        public long? PlantId { get; set; }
        public string Skeptis { get; set; }
        public int? ExGoodsTypeId { get; set; }
        public decimal? Conversion { get; set; }

        //T1001
        public long CompanyIdT1001 { get; set; }
        public string Bukrs { get; set; }
        public string BukrsTxt { get; set; }
        public string Npwp { get; set; }
        public DateTime? CreateDateT1001 { get; set; }

        //ZAIDM_EX_GOODTYP
        public int GoodTypeId { get; set; }
        public int? ExcGoodType { get; set; }
        public string ExtTypDesc { get; set; }
        public DateTime? CreateTime { get; set; }

        //ZAIDM_EX_NPPBKC
        public long NppckId { get; set; }
        public string NppckNo { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string City { get; set; }
        public long? KppbcId { get; set; }
        public int? RegionOfficeId { get; set; }
        public long? CompanyIdZaidmExNPPBKC { get; set; }
        public long? VendorIdZaidmExNPPBKC { get; set; }
        public string TextTo { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
