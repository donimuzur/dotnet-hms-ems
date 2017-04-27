using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Utils;

namespace Sampoerna.EMS.Website.Models.PBCK1
{
    public class QuotaMonitoringViewModel : BaseModel
    {
        public QuotaMonitoringViewModel()
        {
            Detail = new QuotaMonitoringModel();
        }

        public QuotaMonitoringModel Detail { get; set; }
    }

    public class QuotaMonitoringListViewModel : BaseModel
    {
        public QuotaMonitoringListViewModel()
        {
            Details = new List<QuotaMonitoringModel>();
        }

        public List<QuotaMonitoringModel> Details { get; set; }
    }

    public class QuotaMonitoringModel
    {
        public QuotaMonitoringModel()
        {
            Details = new List<QuotaMonitoringDetail>();
        }

        public int MONITORING_ID { get; set; }
        public string NPPBKC_ID { get; set; }
        public string SUPPLIER_NPPBKC_ID { get; set; }
        public string SUPPLIER_WERKS { get; set; }
        public DateTime? PERIOD_FROM { get; set; }
        public DateTime? PERIOD_TO { get; set; }

        public int? EX_GROUP_TYPE { get; set; }

        public string ExGoodTypeDescription {
            get
            {
                if (EX_GROUP_TYPE != null)
                {
                    return EnumHelper.GetDescription((Enums.ExGoodsType) EX_GROUP_TYPE);
                }

                return string.Empty;
            }
        }

        public decimal TotalApprovedQuota { get; set; }
        public decimal TotalUsedQuota { get; set; }
        public decimal TotalRemainingQuota { get; set; }

        public List<QuotaMonitoringDetail> Details { get; set; } 
    }

    public class QuotaMonitoringDetail
    {
        public int MONITORING_DETAIL_ID { get; set; }
        public int? MONITORING_ID { get; set; }
        public string USER_ID { get; set; }
        public int? ROLE_ID { get; set; }

        public string RoleDescription { get; set; }
        public Enums.EmailStatus? EMAIL_STATUS { get; set; }
    }
}