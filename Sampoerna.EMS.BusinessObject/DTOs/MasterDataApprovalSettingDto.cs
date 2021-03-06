﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class MasterDataApprovalSettingDetail
    {
        public int PAGE_ID { get; set; }
        public string COLUMN_NAME { get; set; }
        public bool? IS_APPROVAL { get; set; }

        public bool IsPrimary { get; set; }

        public string ColumnDescription { get; set; }
    }

    public class MasterDataApprovalSettingDto
    {
        public MasterDataApprovalSettingDto()
        {
            Details = new List<MasterDataApprovalSettingDetail>();
        }

        public int PageId { get; set; }
        public string PageDescription { get; set; }

        public List<MasterDataApprovalSettingDetail> Details { get; set; }
    }
}
