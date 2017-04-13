using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class MasterDataApprovalBLL : IMasterDataAprovalBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<MASTER_DATA_APPROVAL> _repository;

        private IPageBLL _pageBLL;
        private IMasterDataApprovalSettingBLL _approvalSettingBLL;
        private string includeTables = "MASTER_DATA_APPROVAL_DETAIL";
        public MasterDataApprovalBLL(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;

            _repository = _uow.GetGenericRepository<MASTER_DATA_APPROVAL>();
            _pageBLL = new PageBLL(_uow,_logger);
            _approvalSettingBLL = new MasterDataApprovalSettingBLL(_uow,_logger);
        }
        public T MasterDataApprovalValidation<T>(int pageId,string userId, T oldObject, T newObject)
        {

            var approvalSettings = _approvalSettingBLL.GetAllEditableColumn(pageId);
            //var fieldNames = typeof(T).GetFields()
            //                .Select(field => field.Name)
            //                .ToList();


            var needApprovalFields = approvalSettings.Details.Where(x => x.IS_APPROVAL.HasValue && x.IS_APPROVAL.Value);
            var needApprovalList = new List<MASTER_DATA_APPROVAL_DETAIL>();
            foreach (var isneedApprove in needApprovalFields)
            {
                //var isneedApprove = fieldName;

                if (isneedApprove != null)
                {
                    var masterDataApprovalDetail = new MASTER_DATA_APPROVAL_DETAIL();
                    var oldValue = oldObject.GetType().GetProperty(isneedApprove.COLUMN_NAME).GetValue(oldObject, null);
                    var newValue = newObject.GetType().GetProperty(isneedApprove.COLUMN_NAME).GetValue(newObject, null);

                    if (oldValue != newValue)
                    {
                        masterDataApprovalDetail.OLD_VALUE = oldValue.ToString();
                        masterDataApprovalDetail.NEW_VALUE = newValue.ToString();
                        masterDataApprovalDetail.COLUMN_DESCRIPTION = isneedApprove.ColumnDescription;
                        masterDataApprovalDetail.COLUMN_NAME = isneedApprove.COLUMN_NAME;

                        needApprovalList.Add(masterDataApprovalDetail);

                        newObject.GetType().GetProperty(isneedApprove.COLUMN_NAME).SetValue(newObject, oldValue);
                    }
                    


                }
                

            }

            var newApproval = new MASTER_DATA_APPROVAL();
            if (needApprovalList.Count > 0)
            {
                newApproval.CREATED_BY = userId;
                newApproval.CREATED_DATE = DateTime.Now;
                newApproval.PAGE_ID = approvalSettings.PageId;
                newApproval.STATUS_ID = Enums.DocumentStatus.Draft;

                newApproval.MASTER_DATA_APPROVAL_DETAIL = needApprovalList;

                _repository.Insert(newApproval);
            }

            return newObject;
        }
    }
}
