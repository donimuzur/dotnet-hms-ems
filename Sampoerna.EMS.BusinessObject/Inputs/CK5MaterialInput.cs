using System.Collections.Generic;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class CK5MaterialInput
    {
        public string Plant { get; set; }
        public string Brand { get; set; }
        public string Qty { get; set; }
        public string Uom { get; set; }
        public string Convertion { get; set; }
        public string ConvertedUom { get; set; }
        public string UsdValue { get; set; }
        public string Note { get; set; }

        public Enums.ExGoodsType ExGoodsType { get; set; }
       
    }

    public class Ck5MaterialGetForLackByParamInput
    {
        public string CompanyCode { get; set; }
        public string NppbkcId { get; set; }
        public string ReceivedPlantId { get; set; }
        public Enums.Lack1Level Lack1Level { get; set; }
        public int ExGroupTypeId { get; set; }
        public string SupplierPlantId { get; set; }
        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }

        public bool IsExcludeSameNppbkcId { get; set; }

        public List<string> StoNumberList { get; set; }
    }
}
