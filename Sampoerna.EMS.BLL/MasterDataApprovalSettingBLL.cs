using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    class MasterDataApprovalSettingBLL : IMasterDataApprovalSettingBLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<MASTER_DATA_APPROVE_SETTING> _repository;

        private string includeTables = "PAGE";

        public List<string> GetAllEditableColumn(int pageId)
        {
            throw new NotImplementedException();
            
        }

        public void SaveSetting(MasterDataApprovalSettingDto data)
        {
            
            throw new NotImplementedException();
        }
    }
}
