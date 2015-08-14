﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.Core;

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

        public Enums.ExGoodsType EX_GOODS_TYPE { get; set; }

        public string DocSeqNumber { get; set; }
        public string MatNumber { get; set; }
        public string Qty { get; set; }
        public string UomMaterial { get; set; }
        public string Convertion { get; set; }
        public string ConvertedUom { get; set; }
        public string UsdValue { get; set; }
        public string Note { get; set; }
    }
}
