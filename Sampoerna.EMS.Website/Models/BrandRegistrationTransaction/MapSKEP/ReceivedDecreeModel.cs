using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.MapSKEP
{
    public class ReceivedDecreeModel : BaseModel
    {
        public long Received_ID { get; set; }
        public string Received_No { get; set; }
        public string Nppbkc_ID { get; set; }
        public string Kppbc { get; set; }
        public string Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public string LastModified_By { get; set; }
        public DateTime LastModified_Date { get; set; }
        public string LastApproved_By { get; set; }
        public DateTime? LastApproved_Date { get; set; }
        public long LastApproved_Status { get; set; }
        public string StrLastApproved_Status { get; set; }
        public long PD_ID { get; set; }
        public string Decree_No { get; set; }
        public DateTime? Decree_Date { get; set; }
        public string strDecree_Date { get; set; }
        public DateTime? Decree_StartDate { get; set; }
        public string strDecree_StartDate { get; set; }
        public CompanyModel Company { set; get; }
        public string CompanyName { set; get; }
        public string AddressPlant { set; get; }
        public UserModel Creator { set; get; }
        public String CreatorName { set; get; }
        public UserModel Approver { set; get; }
        public UserModel LastEditor { set; get; }
        public ReferenceModel ApprovalStatusDescription { set; get; }
        public bool IsCreator { set; get; }
        public bool IsSubmitted { set; get; }
        public bool IsApproved { set; get; }
        public Shared.WorkflowHistory RevisionData { set; get; }
        public ReceivedDecreeDetailModel Detail { set; get; }
    }
}