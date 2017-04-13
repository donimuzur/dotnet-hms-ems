using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class MasterDataApprovalSettingBLL : IMasterDataApprovalSettingBLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<MASTER_DATA_APPROVE_SETTING> _repository;
        
        private IPageBLL _pageBLL;

        private string includeTables = "PAGE";


        public MasterDataApprovalSettingBLL(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<MASTER_DATA_APPROVE_SETTING>();
            
            _pageBLL = new PageBLL(_uow,_logger);

        }

        public MasterDataApprovalSettingDto GetAllEditableColumn(int pageId)
        {
            var page = _pageBLL.GetPageByID(pageId);
            var tableDetails = _repository.GetTableDetail(page.MAIN_TABLE);
            
            Expression<Func<MASTER_DATA_APPROVE_SETTING, bool>> queryFilter = PredicateHelper.True<MASTER_DATA_APPROVE_SETTING>();
            
            queryFilter = queryFilter.And(c => c.PAGE_ID == page.PAGE_ID);

            var masterDataSettings = _repository.Get(queryFilter, null, includeTables).ToList();

            var rc = Mapper.Map<MasterDataApprovalSettingDto>(page);
            
            foreach (var table in tableDetails)
            {
                var detail = Mapper.Map<MasterDataApprovalSettingDetail>(table);
                var isApproval = masterDataSettings.Where(x => x.COLUMN_NAME == detail.COLUMN_NAME).Select(x=> x.IS_APPROVAL).FirstOrDefault();
                detail.IS_APPROVAL = isApproval;
                detail.PAGE_ID = page.PAGE_ID;
                if (detail.ColumnDescription != null)
                {
                    rc.Details.Add(detail);
                }
                
            }

            return rc;

        }


        public List<MasterDataApprovalSettingDto> GetAllMasterSettingsPage()
        {
            var pages = _pageBLL.GetPages().Where(x=> !string.IsNullOrEmpty(x.MAIN_TABLE));

            return Mapper.Map<List<MasterDataApprovalSettingDto>>(pages);
            
        }

        public void SaveSetting(MasterDataApprovalSettingDto data)
        {
            var datatoSave = Mapper.Map<List<MASTER_DATA_APPROVE_SETTING>>(data.Details);
            foreach (var detail in datatoSave)
            {
                _repository.InsertOrUpdate(detail);
            }

            _uow.SaveChanges();
        }
    }
}
