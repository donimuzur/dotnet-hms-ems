using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.MasterDataApprovalSetting
{
    public class MasterDataApprovalSettingIndexViewModel : BaseModel
    {

        public MasterDataApprovalSettingIndexViewModel()
        {
            Details = new List<MasterDataSetting>();
        }

        public List<MasterDataSetting> Details;
    }


    public class MasterDataSetting
    {
        public int PageId { get; set; }
        public string PageDescription { get; set; }

        public List<MasterDataSettingDetail> MasterDataSettingDetails { get; set; }
    }

    public class MasterDataSettingDetail
    {
        public int PAGE_ID { get; set; }
        public string COLUMN_NAME { get; set; }
        public bool IS_APPROVAL { get; set; }

        public bool IsPrimary { get; set; }

        public string ColumnDescription { get; set; }
    }
}