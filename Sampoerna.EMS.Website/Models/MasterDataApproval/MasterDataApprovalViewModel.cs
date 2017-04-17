using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Website.Models.MasterDataApproval
{
    public class MasterDataApprovalIndexViewModel : BaseModel
    {
        public MasterDataApprovalIndexViewModel()
        {
            Details = new List<MasterDataApprovalDetailViewModel>();
        }

        public List<MasterDataApprovalDetailViewModel> Details { get; set; }
    }

    public class MasterDataApprovalItemViewModel : BaseModel
    {
        public MasterDataApprovalItemViewModel()
        {
            Detail = new MasterDataApprovalDetailViewModel();
        }

        public MasterDataApprovalDetailViewModel Detail { get; set; }

        public bool IsMasterApprover { get; set; }
    }

    public class MasterDataApprovalDetailViewModel
    {
        public int APPROVAL_ID { get; set; }
        public int PAGE_ID { get; set; }

        public string FORM_ID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string APPROVED_BY { get; set; }
        public DateTime APPROVED_DATE { get; set; }
        public Enums.DocumentStatus STATUS_ID { get; set; }

        public string PageDesciption { get; set; }



        public virtual List<MasterDataApprovalDetail> Details { get; set; }
    }


    public class MasterDataApprovalDetail
    {
        public int APPROVAL_DETAIL_ID { get; set; }
        public int APPROVAL_ID { get; set; }
        public string COLUMN_NAME { get; set; }
        public string COLUMN_DESCRIPTION { get; set; }
        public string OLD_VALUE { get; set; }
        public string NEW_VALUE { get; set; }
    }
}