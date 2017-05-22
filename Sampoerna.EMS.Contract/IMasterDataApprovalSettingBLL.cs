using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject.DTOs;


namespace Sampoerna.EMS.Contract
{
    public interface IMasterDataApprovalSettingBLL
    {
        MasterDataApprovalSettingDto GetAllEditableColumn(int pageId);

        void SaveSetting(MasterDataApprovalSettingDto data);

        List<MasterDataApprovalSettingDto> GetAllMasterSettingsPage();

        
    }
}
