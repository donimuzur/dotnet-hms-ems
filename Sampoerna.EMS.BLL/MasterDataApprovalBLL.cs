using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CrystalDecisions.Shared;
using Sampoerna.EMS.BLL.Services;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.MessagingService;
using Sampoerna.EMS.Utils;
//using Sampoerna.EMS.XMLReader;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class MasterDataApprovalBLL : IMasterDataAprovalBLL
    {
        //private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<MASTER_DATA_APPROVAL> _repository;


        private IPageBLL _pageBLL;
        //private IUserBLL _userBLL;
        private IBrandRegistrationService _brandRegistrationBLL;
        private IPoaService _poaBll;
        private IPOAMapBLL _poaMapBLL;
        private IZaidmExMaterialService _materialBLL;
        private IMasterDataApprovalSettingBLL _approvalSettingBLL;
        private IMessageService _messageService;
        private IChangesHistoryBLL _changesHistoryBLL;
        //private XmlBrandRegistrationWriter _xmlWriter;
        private string includeTables = "MASTER_DATA_APPROVAL_DETAIL,PAGE";
        public MasterDataApprovalBLL(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            //_logger = logger;

            _repository = _uow.GetGenericRepository<MASTER_DATA_APPROVAL>();
            _pageBLL = new PageBLL(_uow, logger);
            //_userBLL = new UserBLL(_uow, logger);
            _approvalSettingBLL = new MasterDataApprovalSettingBLL(_uow, logger);
            _changesHistoryBLL = new ChangesHistoryBLL(_uow, logger);
            _brandRegistrationBLL = new BrandRegistrationService(_uow, logger);
            _messageService = new MessageService(logger);
            _poaBll = new POAService(_uow, logger);
            _poaMapBLL = new POAMapBLL(_uow, logger);
            _materialBLL = new ZaidmExMaterialService(_uow, logger);
            //_xmlWriter = new XmlBrandRegistrationWriter(_uow,_logger);
        }
        public T MasterDataApprovalValidation<T>(int pageId, string userId, T oldObject, T newObject,out bool isExist, bool isCommit = false)
        {

            var approvalSettings = _approvalSettingBLL.GetAllEditableColumn(pageId);
            //var fieldNames = typeof(T).GetFields()
            //                .Select(field => field.Name)
            //                .ToList();
            isExist = false;

            var needApprovalFields = approvalSettings.Details.Where(x => x.IS_APPROVAL.HasValue && x.IS_APPROVAL.Value);
            var needApprovalList = new List<MASTER_DATA_APPROVAL_DETAIL>();
            foreach (var isneedApprove in needApprovalFields)
            {
                //var isneedApprove = fieldName;
                var masterDataApprovalDetail = new MASTER_DATA_APPROVAL_DETAIL();
                if (newObject == null) // delete data from database logic (not flagging)
                {
                    var oldValue = oldObject.GetType().GetProperty(isneedApprove.COLUMN_NAME).GetValue(oldObject, null);
                    masterDataApprovalDetail.OLD_VALUE = oldValue.ToString();
                    masterDataApprovalDetail.NEW_VALUE = null;
                    masterDataApprovalDetail.COLUMN_DESCRIPTION = isneedApprove.ColumnDescription;
                    masterDataApprovalDetail.COLUMN_NAME = isneedApprove.COLUMN_NAME;

                    needApprovalList.Add(masterDataApprovalDetail);

                   //newObject.GetType().GetProperty(isneedApprove.COLUMN_NAME).SetValue(newObject, oldValue);
                    continue;
                }

                if (isneedApprove != null)
                {
                    
                    var oldValue = oldObject.GetType().GetProperty(isneedApprove.COLUMN_NAME).GetValue(oldObject, null);
                    var newValue = newObject.GetType().GetProperty(isneedApprove.COLUMN_NAME).GetValue(newObject, null);

                    

                    if (oldValue == null)
                    {
                        masterDataApprovalDetail.OLD_VALUE = string.Empty;
                        masterDataApprovalDetail.NEW_VALUE = string.Empty;
                        if (newValue != null) masterDataApprovalDetail.NEW_VALUE = newValue.ToString();
                        masterDataApprovalDetail.COLUMN_DESCRIPTION = isneedApprove.ColumnDescription;
                        masterDataApprovalDetail.COLUMN_NAME = isneedApprove.COLUMN_NAME;

                        if (masterDataApprovalDetail.OLD_VALUE != masterDataApprovalDetail.NEW_VALUE)
                        {
                            needApprovalList.Add(masterDataApprovalDetail);
                            var checkOldObject = GenerateFormId(pageId, oldObject) != null;
                            if (checkOldObject)
                            {
                                newObject.GetType().GetProperty(isneedApprove.COLUMN_NAME).SetValue(newObject, oldValue);
                            }
                            
                        }
                        
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
                if (newObject == null) // delete data without flagging
                {
                    newApproval.FORM_ID = GenerateFormId(approvalSettings.PageId, oldObject);
                }
                newApproval.MASTER_DATA_APPROVAL_DETAIL = needApprovalList;

                isExist = CheckExitingOngoingApproval(newApproval.FORM_ID);
                if (!isExist)
                {
                    _repository.Insert(newApproval);
                    SendEmailWorkflowByParam(newApproval);
                }
                else
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.ApprovalMasterExist);
                }
                
            }

            if (isCommit)
            {
                if (!isExist)
                {
                    _uow.SaveChanges();
                    //SendEmailWorkflow(newApproval.APPROVAL_ID);
                }
                
            }

            return newObject;
        }


        private bool CheckExitingOngoingApproval(string formId)
        {
            var data = _repository.Get(x => x.FORM_ID == formId && x.STATUS_ID == Enums.DocumentStatus.WaitingForMasterApprover).FirstOrDefault();

            return data != null;
        }

        public void CreateNewDataForApproval<T>(int pageId,string userId,T data)
        {
            var page = _pageBLL.GetPageByID(pageId);
            var tabledetails = _repository.GetTableDetail(page.MAIN_TABLE);

            foreach (var tabledetail in tabledetails)
            {
                
            }
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
                bool isDelete;
                var newData = UpdateObjectByFormId(data,out isDelete);
                if (newData != null)
                {
                    if (newData.GetType() == typeof (ZAIDM_EX_BRAND))
                    {
                        var dataBrand = (ZAIDM_EX_BRAND) newData;
                        dataBrand.CREATED_BY = data.CREATED_BY;
                        dataBrand.CREATED_DATE = DateTime.Now;
                        dataBrand.MODIFIED_BY = data.APPROVED_BY;
                        dataBrand.MODIFIED_DATE = DateTime.Now;
                        _brandRegistrationBLL.Save(dataBrand);

                        

                    }
                    else if (newData.GetType() == typeof (POA))
                    {
                        var dataPoa = (POA) newData;
                        dataPoa.CREATED_BY = data.CREATED_BY;
                        dataPoa.CREATED_DATE = DateTime.Now;
                        dataPoa.MODIFIED_BY = data.APPROVED_BY;
                        dataPoa.MODIFIED_DATE = DateTime.Now;
                        _poaBll.Save(dataPoa);
                    }
                    else if (newData.GetType() == typeof (ZAIDM_EX_MATERIAL))
                    {
                        var dataMaterial = (ZAIDM_EX_MATERIAL) newData;
                        dataMaterial.CREATED_BY = data.CREATED_BY;
                        dataMaterial.CREATED_DATE = DateTime.Now;
                        dataMaterial.MODIFIED_BY = data.APPROVED_BY;
                        dataMaterial.MODIFIED_DATE = DateTime.Now;
                        _materialBLL.Save(dataMaterial);
                    }
                    else if (newData.GetType() == typeof (POA_MAP))
                    {
                        var dataPoaMap = (POA_MAP) newData;
                        dataPoaMap.CREATED_BY = data.CREATED_BY;
                        dataPoaMap.CREATED_DATE = DateTime.Now;
                        dataPoaMap.MODIFIED_BY = data.APPROVED_BY;
                        dataPoaMap.MODIFIED_DATE = DateTime.Now;
                        _poaMapBLL.Save(dataPoaMap);
                    }
                }

                if (!isDelete)
                {
                    UpdateChangesHistory(data);
                    _uow.SaveChanges();
                }
                else //delete non flagging
                {
                    if (data.PAGE_ID == (int) Enums.MenuList.POAMap)
                    {
                        _poaMapBLL.Delete(int.Parse(data.FORM_ID));
                    }
                }
                

                //create xml for brand registration

                SendEmailWorkflow(data.APPROVAL_ID);





            }
        }

        

        public void Reject(string userId, int masterApprovalId)
        {
            var data = _repository.Get(x => x.APPROVAL_ID == masterApprovalId, null, includeTables).FirstOrDefault();
            if (data != null)
            {
                data.STATUS_ID = Enums.DocumentStatus.Rejected;
                data.APPROVED_BY = userId;
                data.APPROVED_DATE = DateTime.Now;

                
                _uow.SaveChanges();
                SendEmailWorkflow(data.APPROVAL_ID);
            }
        }

        private string GenerateFormId(int pageId,object obj)
        {
            try
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
                    case (int) Enums.MenuList.POAMap:
                        return obj.GetType().GetProperty("POA_MAP_ID").GetValue(obj).ToString();
                    case (int) Enums.MenuList.MaterialMaster:
                        var werksM = obj.GetType().GetProperty("WERKS").GetValue(obj).ToString();

                        var stickerCodeM = obj.GetType().GetProperty("STICKER_CODE").GetValue(obj).ToString();
                        return werksM + "-" + stickerCodeM;
                }
            }
            catch 
            {
                return null;
                
            }
            
            

            return null;
        }

        private object UpdateObjectByFormId(MASTER_DATA_APPROVAL approvalData,out bool isDelete)
        {


            PropertyInfo propInfo;
            isDelete = false;
            if (approvalData.PAGE_ID == (int) Enums.MenuList.BrandRegistration)
            {
                var tempId = approvalData.FORM_ID.Split('-');
                var werks = tempId[0];
                var facode = tempId[1];
                var stickerCode = tempId[2];

                var dataBrand = _brandRegistrationBLL.GetByPlantIdAndFaCodeStickerCode(werks, facode, stickerCode);
                if (dataBrand != null)
                {
                    foreach (var detail in approvalData.MASTER_DATA_APPROVAL_DETAIL)
                    {
                        propInfo = typeof (ZAIDM_EX_BRAND).GetProperty(detail.COLUMN_NAME);
                        dataBrand.GetType()
                            .GetProperty(detail.COLUMN_NAME)
                            .SetValue(dataBrand, CastPropertyValue(propInfo, detail.NEW_VALUE));
                    }

                    return null;
                }
                else
                {
                    ZAIDM_EX_BRAND data = new ZAIDM_EX_BRAND();

                    foreach (var detail in approvalData.MASTER_DATA_APPROVAL_DETAIL)
                    {
                        propInfo = typeof(ZAIDM_EX_BRAND).GetProperty(detail.COLUMN_NAME);
                        data.GetType()
                            .GetProperty(detail.COLUMN_NAME)
                            .SetValue(data, CastPropertyValue(propInfo, detail.NEW_VALUE));
                    }

                    return data;
                }
                
                
                
            }
            else if (approvalData.PAGE_ID == (int) Enums.MenuList.POA)
            {
                var dataPoa = _poaBll.GetById(approvalData.FORM_ID);
                if (dataPoa != null)
                {
                    foreach (var detail in approvalData.MASTER_DATA_APPROVAL_DETAIL)
                    {
                        propInfo = typeof (POA).GetProperty(detail.COLUMN_NAME);
                        dataPoa.GetType()
                            .GetProperty(detail.COLUMN_NAME)
                            .SetValue(dataPoa, CastPropertyValue(propInfo, detail.NEW_VALUE));
                    }

                    return null;
                }
                else
                {
                    POA data = new POA();

                    foreach (var detail in approvalData.MASTER_DATA_APPROVAL_DETAIL)
                    {
                        propInfo = typeof(POA).GetProperty(detail.COLUMN_NAME);
                        data.GetType()
                            .GetProperty(detail.COLUMN_NAME)
                            .SetValue(data, CastPropertyValue(propInfo, detail.NEW_VALUE));
                    }

                    return data;
                }

            }
            else if (approvalData.PAGE_ID == (int)Enums.MenuList.POAMap)
            {
                var dataPoaMap = _poaMapBLL.GetById(int.Parse(approvalData.FORM_ID));
                if (dataPoaMap != null)
                {
                    isDelete = approvalData.MASTER_DATA_APPROVAL_DETAIL.Where(x => x.NEW_VALUE == null).Any();
                    if (!isDelete)
                    {
                        foreach (var detail in approvalData.MASTER_DATA_APPROVAL_DETAIL)
                        {
                            propInfo = typeof (POA_MAP).GetProperty(detail.COLUMN_NAME);
                            dataPoaMap.GetType()
                                .GetProperty(detail.COLUMN_NAME)
                                .SetValue(dataPoaMap, CastPropertyValue(propInfo, detail.NEW_VALUE));
                        }
                    }
                    
                    

                    return null;
                }
                else
                {
                    POA_MAP data = new POA_MAP();

                    foreach (var detail in approvalData.MASTER_DATA_APPROVAL_DETAIL)
                    {
                        propInfo = typeof(POA_MAP).GetProperty(detail.COLUMN_NAME);
                        data.GetType()
                            .GetProperty(detail.COLUMN_NAME)
                            .SetValue(data, CastPropertyValue(propInfo, detail.NEW_VALUE));
                    }

                    return data;
                }


            }
            else if (approvalData.PAGE_ID == (int)Enums.MenuList.MaterialMaster)
            {
                var tempId = approvalData.FORM_ID.Split('-');
                var werks = tempId[0];
                var materialnumber = tempId[1];
                var dataMaterial = _materialBLL.GetByMaterialAndPlantId(materialnumber, werks);
                if (dataMaterial != null)
                {
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
                    return null;
                }
                else
                {
                    ZAIDM_EX_MATERIAL data = new ZAIDM_EX_MATERIAL();

                    foreach (var detail in approvalData.MASTER_DATA_APPROVAL_DETAIL)
                    {
                        propInfo = typeof(ZAIDM_EX_MATERIAL).GetProperty(detail.COLUMN_NAME);
                        data.GetType()
                            .GetProperty(detail.COLUMN_NAME)
                            .SetValue(data, CastPropertyValue(propInfo, detail.NEW_VALUE));
                    }

                    return data;
                }
                

                
            }

            return null;

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


        public bool SendEmailWorkflowByParam(MASTER_DATA_APPROVAL data)
        {

            var mailNotif = ProsesMailNotificationBody(data);
            var success = false;

            if (mailNotif != null)
            {
                if (mailNotif.IsCCExist)
                {
                    success = _messageService.SendEmailToListWithCC(mailNotif.To, mailNotif.CC, mailNotif.Subject, mailNotif.Body, false);
                }
                else
                {
                    success = _messageService.SendEmailToList(mailNotif.To, mailNotif.Subject, mailNotif.Body, false);
                }


            }


            return success;
            

            
        }

        public bool SendEmailWorkflow(int approvalId)
        {
            var data = GetByApprovalId(approvalId);
            var mailNotif = ProsesMailNotificationBody(data);
            var success = false;

            if (mailNotif != null)
            {
                if (mailNotif.IsCCExist)
                {
                    success = _messageService.SendEmailToListWithCC(mailNotif.To, mailNotif.CC, mailNotif.Subject, mailNotif.Body, false);
                }
                else
                {
                    success = _messageService.SendEmailToList(mailNotif.To, mailNotif.Subject, mailNotif.Body, false);
                }


            }


            return success;
        }

        private MailNotification ProsesMailNotificationBody(MASTER_DATA_APPROVAL data)
        {
            var bodyMail = new StringBuilder();
            var rc = new MailNotification();





            var userCreatorInfo = _poaBll.GetUserById(data.CREATED_BY);




            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            var page = _pageBLL.GetPageByID(data.PAGE_ID);
            rc.Subject = "Master Data " + page.MENU_NAME + " Approval Status is " + EnumHelper.GetDescription(data.STATUS_ID);
            bodyMail.Append("Dear Team,<br />");

            bodyMail.Append("Kindly be informed, " + rc.Subject + ". <br />");

            bodyMail.Append(BuildBodyMail(data, webRootUrl));

            var controllerList = _poaBll.GetMasterApprovers();

            rc.Body = bodyMail.ToString();

            if (data.STATUS_ID == Enums.DocumentStatus.WaitingForMasterApprover)
            {
                foreach (var user in controllerList)
                {
                    
                    rc.To.Add(user.EMAIL);
                }



                rc.IsCCExist = true;
                rc.CC.Add(userCreatorInfo.EMAIL); 
            }
            else
            {
                foreach (var user in controllerList)
                {
                    rc.IsCCExist = true;
                    rc.CC.Add(user.EMAIL);
                }
                


                
                rc.To.Add(userCreatorInfo.EMAIL);    
            }
            

            return rc;
        }

        private string BuildBodyMail(MASTER_DATA_APPROVAL data, string webRootUrl)
        {
            var bodyMail = new StringBuilder();
            var page = _pageBLL.GetPageByID(data.PAGE_ID);
            bodyMail.Append("<table><tr><td>Master Data </td><td>: " + page.MENU_NAME + "</td></tr>");
            bodyMail.Append("<tr><td>Form Id </td><td>: " + data.FORM_ID + "</td></tr>");


            //bodyMail.Append("<tr><td>Total Qty Approved </td><td>: " +
            //                ConvertHelper.ConvertDecimalToStringMoneyFormat(pbck1Data.QtyApproved) + " " + pbck1Data.RequestQtyUomName + "</td></tr>");
            //bodyMail.Append("<tr><td>Total Quota Qty Used </td><td>: " +
            //                ConvertHelper.ConvertDecimalToStringMoneyFormat(quotaDetail.QtyCk5) + " " + pbck1Data.RequestQtyUomName + "</td></tr>");
            //bodyMail.Append("<tr><td>Total Quota Qty Remain </td><td>: " +
            //                ConvertHelper.ConvertDecimalToStringMoneyFormat(quotaDetail.RemainQuota) + " " + pbck1Data.RequestQtyUomName + "</td></tr>");

            string userName = "";
            var creator = _poaBll.GetUserById(data.CREATED_BY);
            if (creator != null)
                userName = creator.LAST_NAME + ", " + creator.FIRST_NAME;



            bodyMail.Append("<tr><td>Creator</td><td> : " + userName + "</td></tr>");







            bodyMail.Append("<tr colspan='2'><td><i>To VIEW, Please click this <a href='" + webRootUrl + "/MasterApproval/Detail/" + data.APPROVAL_ID + "'><u>link</u></a> to view detailed information</i></td></tr>");



            bodyMail.Append("</table>");
            bodyMail.AppendLine();
            bodyMail.Append("<br />Regards,<br />");

            return bodyMail.ToString();
        }
    }
}
