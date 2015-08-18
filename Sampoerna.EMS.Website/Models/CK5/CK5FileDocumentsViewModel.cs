using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using DocumentFormat.OpenXml.Math;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.CK5
{
    public class CK5FileDocumentsViewModel : BaseModel
    {
        public CK5FileDocumentsViewModel()
        {
            Ck5FileDocumentItems = new List<CK5FileDocumentItems>();
        }

        public List<CK5FileDocumentItems> Ck5FileDocumentItems { get; set; }

    }

    public class CK5FileDocumentItems
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
        public string InvoiceDateDisplay { get; set; }
        
        public string PbckDecreeNumber { get; set; }
        public string CarriageMethod { get; set; }
        public string GrandTotalEx { get; set; }
        public string Uom { get; set; }
        public string MesssageUploadFileDocuments { get; set; }

        //needed when save to database
        public Enums.CK5Type CK5_TYPE { get; set; }
        public string CE_OFFICE_CODE { get; set; }

        public Enums.ExGoodsType EX_GOODS_TYPE { get; set; }
        public Enums.ExciseSettlement EX_SETTLEMENT_ID { get; set; }
        public Enums.ExciseStatus EX_STATUS_ID { get; set; }
        public Enums.RequestType REQUEST_TYPE_ID { get; set; }

        public DateTime? INVOICE_DATE { get; set; }

        public string SOURCE_PLANT_ID { get; set; }
        public string SOURCE_PLANT_NPWP { get; set; }
        public string SOURCE_PLANT_NPPBKC_ID { get; set; }
        public string SOURCE_PLANT_COMPANY_CODE { get; set; }
        public string SOURCE_PLANT_COMPANY_NAME { get; set; }
        public string SOURCE_PLANT_ADDRESS { get; set; }
        public string SOURCE_PLANT_KPPBC_NAME_OFFICE { get; set; }
        public string SOURCE_PLANT_NAME { get; set; }

        
        public string DEST_PLANT_ID { get; set; }
        public string DEST_PLANT_NPWP { get; set; }
        public string DEST_PLANT_NPPBKC_ID { get; set; }
        public string DEST_PLANT_COMPANY_CODE { get; set; }
        public string DEST_PLANT_COMPANY_NAME { get; set; }
        public string DEST_PLANT_ADDRESS { get; set; }
        public string DEST_PLANT_KPPBC_NAME_OFFICE { get; set; }
        public string DEST_PLANT_NAME { get; set; }

        public Nullable<int> PBCK1_DECREE_ID { get; set; }
        public Enums.CarriageMethod? CARRIAGE_METHOD_ID { get; set; }

        //export type
        public string LOADING_PORT { get; set; }
        public string LOADING_PORT_NAME { get; set; }
        public string LOADING_PORT_ID { get; set; }
        public string FINAL_PORT { get; set; }
        public string FINAL_PORT_NAME { get; set; }
        public string FINAL_PORT_ID { get; set; }
        public string DEST_COUNTRY_CODE { get; set; }
        public string DEST_COUNTRY_NAME { get; set; }

        public string DocSeqNumber { get; set; }
        public string MatNumber { get; set; }
        public string Qty { get; set; }
        public string UomMaterial { get; set; }
        public string Convertion { get; set; }
        public string ConvertedUom { get; set; }
        public string UsdValue { get; set; }
        public string Note { get; set; }

        //additional material
        public int ConvertedQty { get; set; }
        public decimal Hje { get; set; }
        public decimal Tariff { get; set; }
        public decimal ExciseValue { get; set; }
    }
}