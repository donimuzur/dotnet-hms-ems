using Sampoerna.EMS.Website.Models.BrandRegistration;
using Sampoerna.EMS.Website.Models.Market;
using Sampoerna.EMS.Website.Models.PLANT;
using Sampoerna.EMS.Website.Models.Shared;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.ProductDevelopment
{
    public class ProductDevDetailModel : BaseModel
    {
        public long PD_DETAIL_ID { get; set; }
        public string Fa_Code_Old { get; set; }
        public string Fa_Code_New { get; set; }
        public string Hl_Code { get; set; }
        public string Market_Id { get; set; }
        public string Fa_Code_Old_Desc { get; set; }
        public string Fa_Code_New_Desc { get; set; }
        public string Werks { get; set; }
        public bool Is_Import { get; set; }
        public long PD_ID { get; set; }
        public string Request_No { get; set; }
        public string Bukrs { get; set; }
        public string Approved_By { get; set; }
        public DateTime? Approved_Date { get; set; }
        public long Approval_Status { get; set; }    
        public string StatusDesc { get; set; }    
        public UserModel Approver { set; get; }       
        public ReferenceModel ApprovalStatusDescription { set; get; }

        public MarketModel Market { get; set; }
        public CompanyModel Company { set; get; }
        public T001WModel Plant { get; set; }

        public bool IsSubmitted { set; get; }
        public bool IsApproved { set; get; }
        public Shared.WorkflowHistory RevisionData { set; get; }
        public string Modified_By { get; set; }
        public DateTime? Modified_Date { get; set; }
       
        public string PD_NO { get; set; }
        public int next_action { get; set; }
        public string createdBy { get; set; }
        public DateTime createdDate { get; set; }

    }
}