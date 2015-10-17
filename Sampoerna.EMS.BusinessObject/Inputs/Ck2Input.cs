

using System;
using System.Collections.Generic;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class SaveCk2ByPbck3IdInput
    {
        public int Pbck3Id { get; set; }

        public string Ck2Number { get; set; }
        public DateTime? Ck2Date { get; set; }
        public decimal? Ck2Value { get; set; }

        public List<CK2_DOCUMENT> Ck2Documents { get; set; }
    }
}
