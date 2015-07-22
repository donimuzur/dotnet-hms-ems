using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
   public class CK5Dto
    {
        public long CK5_ID { get; set; }
        public Sampoerna.EMS.Core.Enums.CK5Type CK5_TYPE { get; set; }
       
        public Nullable<long> KPPBC_CITY { get; set; }
        public string KppbcCityName { get; set; }
        public string CeOfficeCode { get; set; }

        public Nullable<System.DateTime> SUBMISSION_DATE { get; set; }
        public string REGISTRATION_NUMBER { get; set; }

        public Nullable<int> EX_GOODS_TYPE_ID { get; set; }
        public string GoodTypeDesc { get; set; }

        public Nullable<int> EX_SETTLEMENT_ID { get; set; }
        public string ExSettlementName { get; set; }

        public Nullable<int> EX_STATUS_ID { get; set; }
        public string ExStatusName { get; set; }

        public Nullable<int> REQUEST_TYPE_ID { get; set; }
        public string RequestTypeName { get; set; }

        public string STO_SENDER_NUMBER { get; set; }
        public string STO_RECEIVER_NUMBER { get; set; }
        public string STOB_NUMBER { get; set; }

        public Nullable<long> SOURCE_PLANT_ID { get; set; }
        public string SourcePlantName { get; set; }
        public string SourcePlantWerks { get; set; }
        public string SourceNpwp { get; set; }
        public string SourceNppbkcId { get; set; }
        public string SourceCompanyName { get; set; }
        public string SourceAddress { get; set; }
        public string SourceKppbcName { get; set; }

        public Nullable<long> DEST_PLANT_ID { get; set; }
        public string DestPlantName { get; set; }
        public string DestPlantWerks{ get; set; }
        public string DestNpwp { get; set; }
        public string DestNppbkcId { get; set; }
        public string DestCompanyName { get; set; }
        public string DestAddress { get; set; }
        public string DestKppbcName { get; set; }


        public string INVOICE_NUMBER { get; set; }
        public Nullable<System.DateTime> INVOICE_DATE { get; set; }

        public Nullable<long> PBCK1_DECREE_ID { get; set; }
        public string PbckNumber { get; set; }
        public DateTime? PbckDecreeDate { get; set; }

        public Nullable<int> CARRIAGE_METHOD_ID { get; set; }
        public string CarriageMethodName { get; set; }

        public Nullable<decimal> GRAND_TOTAL_EX { get; set; }
        public Nullable<int> PACKAGE_UOM_ID { get; set; }
        public string PackageUomName { get; set; }

        public Nullable<int> DEST_COUNTRY_ID { get; set; }
        public string HARBOUR { get; set; }
        public string OFFICE_HARBOUR { get; set; }
        public string LAST_SHELTER_HARBOUR { get; set; }
        public string OFFICE_SHELTER { get; set; }
        public string DN_NUMBER { get; set; }
        public Nullable<System.DateTime> GR_DATE { get; set; }
        public Nullable<System.DateTime> GI_DATE { get; set; }
        public string SEALING_NOTIF_NUMBER { get; set; }
        public string UNSEALING_NOTIF_NUMBER { get; set; }
        public Nullable<System.DateTime> SEALING_NOTIF_DATE { get; set; }
        public Nullable<System.DateTime> UNSEALING_NOTIF_DATE { get; set; }
        public string LOADING_PORT { get; set; }
        public string LOADING_PORT_NAME { get; set; }
        public string LOADING_PORT_ID { get; set; }
        public string FINAL_PORT { get; set; }
        public string FINAL_PORT_ID { get; set; }
        public Sampoerna.EMS.Core.Enums.DocumentStatus STATUS_ID { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public Nullable<int> APPROVED_BY { get; set; }
        public Nullable<System.DateTime> APPROVED_DATE { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string SUBMISSION_NUMBER { get; set; }
    }
}
