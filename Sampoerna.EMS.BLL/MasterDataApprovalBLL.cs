using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CrystalDecisions.Shared;
using Sampoerna.EMS.BLL.Services;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Utils;
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
        private IBrandRegistrationBLL _brandRegistrationBLL;
        private IPoaService _poaBll;
        private IPOAMapBLL _poaMapBLL;
        private IZaidmExMaterialService _materialBLL;
        private IMasterDataApprovalSettingBLL _approvalSettingBLL;
        private IChangesHistoryBLL _changesHistoryBLL;
        private string includeTables = "MASTER_DATA_APPROVAL_DETAIL,PAGE";
        public MasterDataApprovalBLL(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;

            _repository = _uow.GetGenericRepository<MASTER_DATA_APPROVAL>();
            _pageBLL = new PageBLL(_uow,_logger);
            _approvalSettingBLL = new MasterDataApprovalSettingBLL(_uow,_logger);
            _changesHistoryBLL = new ChangesHistoryBLL(_uow,_logger);
            _brandRegistrationBLL = new BrandRegistrationBLL(_uow,_logger);
            _poaBll = new POAService(_uow,_logger);
            _poaMapBLL = new POAMapBLL(_uow,_logger);
            _materialBLL = new ZaidmExMaterialService(_uow,_logger);
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

                    if (oldValue == null)
                    {
                        masterDataApprovalDetail.OLD_VALUE = string.Empty;
                        masterDataApprovalDetail.NEW_VALUE = string.Empty;
                        if (newValue != null) masterDataApprovalDetail.NEW_VALUE = newValue.ToString();
                        masterDataApprovalDetail.COLUMN_DESCRIPTION = isneedApprove.ColumnDescription;
                        masterDataApprovalDetail.COLUMN_NAME = isneedApprove.COLUMN_NAME;

                        if (masterDataApprovalDetail.OLD_VALUE != masterDataApprovalDetail.NEW_VALUE) needApprovalList.Add(masterDataApprovalDetail);
                    }
                    else
                    {
                        if (!oldValue.Equals(newValue))
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
                

            }

            var newApproval = new MASTER_DATA_APPROVAL();
            if (needApprovalList.Count > 0)
            {
                newApproval.CREATED_BY = userId;
                newApproval.CREATED_DATE = DateTime.Now;
                newApproval.PAGE_ID = approvalSettings.PageId;
                newApproval.STATUS_ID = Enums.DocumentStatus.WaitingForMasterApprover;
                newApproval.FORM_ID = GenerateFormId(approvalSettings.PageId, newObject);
                newApproval.MASTER_DATA_APPROVAL_DETAIL = needApprovalList;

                _repository.Insert(newApproval);
            }

            return newObject;
        }

        public List<MASTER_DATA_APPROVAL> GetList(Enums.DocumentStatus status = Enums.DocumentStatus.WaitingForMasterApprover)
        {
            return _repository.Get(x => x.STATUS_ID == status, null, includeTables).ToList();
        }


        public List<MASTER_DATA_APPROVAL> GetByPageId(int pageId, Enums.DocumentStatus status = Enums.DocumentStatus.WaitingForMasterApprover)
        {
            return _repository.Get(x => x.STATUS_ID == status && x.PAGE_ID == pageId,null,includeTables).ToList();
        }

        public MASTER_DATA_APPROVAL GetByApprovalId(int approvalId)
        {
            return _repository.Get(x=> x.APPROVAL_ID == approvalId,null,includeTables).FirstOrDefault();
        }


        public void Approve(string userId, int masterApprovalId)
        {
            var data = _repository.Get(x=> x.APPROVAL_ID == masterApprovalId,null,includeTables).FirstOrDefault();
            if (data != null)
            {
                data.STATUS_ID = Enums.DocumentStatus.Approved;
                data.APPROVED_BY = userId;
                data.APPROVED_DATE = DateTime.Now;
                
                UpdateObjectByFormId(data);
                UpdateChangesHistory(data);
                _uow.SaveChanges();
            }
        }

        public void Reject(string userId, int masterApprovalId)
        {
            var data = _repository.Get(x => x.APPROVAL_ID == masterApprovalId, null, includeTables).FirstOrDefault();
            if (data != null)
            {
                data.STATUS_ID = Enums.DocumentStatus.Rejected;
                

                
                _uow.SaveChanges();
            }
        }

        private string GenerateFormId(int pageId,object obj)
        {
            
            switch (pageId)
            {
                case (int) Enums.MenuList.BrandRegistration:
                    var werks = obj.GetType().GetProperty("WERKS").GetValue(obj).ToString();
                    var facode = obj.GetType().GetProperty("FA_CODE").GetValue(obj).ToString();
                    var stickerCode = obj.GetType().GetProperty("STICKER_CODE").GetValue(obj).ToString();
                    return werks + "-" + facode + "-" + stickerCode;
                case (int) Enums.MenuList.POA:
                    return obj.GetType().GetProperty("POA_ID").GetValue(obj).ToString();
                case (int)Enums.MenuList.POAMap:
                    return obj.GetType().GetProperty("POA_MAP_ID").GetValue(obj).ToString();
                case (int)Enums.MenuList.MaterialMaster:
                    var werksM = obj.GetType().GetProperty("WERKS").GetValue(obj).ToString();
                    
                    var stickerCodeM = obj.GetType().GetProperty("STICKER_CODE").GetValue(obj).ToString();
                    return werksM + "-" + stickerCodeM;
            }
            

            return null;
        }

        private void UpdateObjectByFormId(MASTER_DATA_APPROVAL approvalData)
        {


            PropertyInfo propInfo;

            if (approvalData.PAGE_ID == (int) Enums.MenuList.BrandRegistration)
            {
                var tempId = approvalData.FORM_ID.Split('-');
                var werks = tempId[0];
                var facode = tempId[1];
                var stickerCode = tempId[2];

                var dataBrand = _brandRegistrationBLL.GetById(werks, facode, stickerCode);
                foreach (var detail in approvalData.MASTER_DATA_APPROVAL_DETAIL)
                {
                    propInfo = typeof(ZAIDM_EX_BRAND).GetProperty(detail.COLUMN_NAME);
                    dataBrand.GetType().GetProperty(detail.COLUMN_NAME).SetValue(dataBrand, CastPropertyValue(propInfo,detail.NEW_VALUE));
                }
                
                
            }
            else if (approvalData.PAGE_ID == (int) Enums.MenuList.POA)
            {
                var dataPoa = _poaBll.GetById(approvalData.FORM_ID);
                foreach (var detail in approvalData.MASTER_DATA_APPROVAL_DETAIL)
                {
                    propInfo = typeof(POA).GetProperty(detail.COLUMN_NAME);
                    dataPoa.GetType().GetProperty(detail.COLUMN_NAME).SetValue(dataPoa, CastPropertyValue(propInfo, detail.NEW_VALUE));
                }
            }
            else if (approvalData.PAGE_ID == (int)Enums.MenuList.POAMap)
            {
                var dataPoaMap = _poaMapBLL.GetById(int.Parse(approvalData.FORM_ID));
                foreach (var detail in approvalData.MASTER_DATA_APPROVAL_DETAIL)
                {
                    propInfo = typeof(POA_MAP).GetProperty(detail.COLUMN_NAME);
                    dataPoaMap.GetType().GetProperty(detail.COLUMN_NAME).SetValue(dataPoaMap, CastPropertyValue(propInfo, detail.NEW_VALUE));
                }

            }
            else if (approvalData.PAGE_ID == (int)Enums.MenuList.MaterialMaster)
            {
                var tempId = approvalData.FORM_ID.Split('-');
                var werks = tempId[0];
                var materialnumber = tempId[1];
                var dataMaterial = _materialBLL.GetByMaterialAndPlantId(materialnumber, werks);
                foreach (var detail in approvalData.MASTER_DATA_APPROVAL_DETAIL)
                {
                    propInfo = typeof(ZAIDM_EX_MATERIAL).GetProperty(detail.COLUMN_NAME);
                    dataMaterial.GetType().GetProperty(detail.COLUMN_NAME).SetValue(dataMaterial, CastPropertyValue(propInfo, detail.NEW_VALUE));
                    
                    if (detail.COLUMN_NAME == "CLIENT_DELETION")
                    {
                        var materialClientDto = Mapper.Map<MaterialDto>(dataMaterial);
                        _materialBLL.ClientDeletion(materialClientDto, approvalData.APPROVED_BY);
                    }
                    else if (detail.COLUMN_NAME == "PLANT_DELETION")
                    {
                        var materialPlantDto = Mapper.Map<MaterialDto>(dataMaterial);
                        _materialBLL.ClientDeletion(materialPlantDto, approvalData.APPROVED_BY);
                    }
                }
            }
            
        }

        private object CastPropertyValue(PropertyInfo property, string value)
        {
            if (property == null || String.IsNullOrEmpty(value))
                return null;
            if (property.PropertyType.IsEnum)
            {
                Type enumType = property.PropertyType;
                if (Enum.IsDefined(enumType, value))
                    return Enum.Parse(enumType, value);
            }
            if (property.PropertyType == typeof(bool))
                return value == "1" || value == "true" || value == "on" || value == "checked";
            else if (property.PropertyType == typeof(Uri))
                return new Uri(Convert.ToString(value));
            else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                //if it's null, just set the value from the reserved word null, and return
                if (value == null)
                {
                    
                    return null;
                }

                //Get the underlying type property instead of the nullable generic
                Type propType = new NullableConverter(property.PropertyType).UnderlyingType;
                return Convert.ChangeType(value, propType);
            }

            else
                return Convert.ChangeType(value, property.PropertyType);
        }


        private void UpdateChangesHistory(MASTER_DATA_APPROVAL data)
        {
            var formId = data.FORM_ID.Split('-');
            foreach (var detail in data.MASTER_DATA_APPROVAL_DETAIL)
            {
                var changesFormId = "";
                switch (data.PAGE_ID)
                {
                    case (int)Enums.MenuList.BrandRegistration:
                        var werks = formId[0];
                        var facode = formId[1];
                        var stickerCode = formId[2];
                        changesFormId = werks+facode+stickerCode;
                        break;
                    case (int)Enums.MenuList.POA:
                        changesFormId = data.FORM_ID;
                        break;
                    case (int)Enums.MenuList.POAMap:
                        changesFormId = data.FORM_ID;
                        break;
                    case (int)Enums.MenuList.MaterialMaster:
                        var werksM = formId[0];

                        var stickerCodeM = formId[1];
                        changesFormId = stickerCodeM + werksM;
                        break;
                }

                CHANGES_HISTORY changes = new CHANGES_HISTORY();
                changes.FIELD_NAME = detail.COLUMN_DESCRIPTION.ToUpper();
                changes.FORM_ID = changesFormId;
                changes.FORM_TYPE_ID = (Enums.MenuList) data.PAGE_ID;
                changes.MODIFIED_BY = data.APPROVED_BY;
                changes.MODIFIED_DATE = data.APPROVED_DATE;
                changes.NEW_VALUE = detail.NEW_VALUE;
                changes.OLD_VALUE = detail.OLD_VALUE;

                _changesHistoryBLL.AddHistory(changes);
            }
            
        }
    }
}
