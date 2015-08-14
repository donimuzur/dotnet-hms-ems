using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.MessagingService;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class CK5BLL : ICK5BLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<CK5> _repository;
        private IGenericRepository<CK5_MATERIAL> _repositoryCK5Material;

        private IDocumentSequenceNumberBLL _docSeqNumBll;
      
        private IBrandRegistrationBLL _brandRegistrationBll;
        private IUnitOfMeasurementBLL _uomBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IMessageService _messageService;
        private IPrintHistoryBLL _printHistoryBll;
        private IMonthBLL _monthBll;
        private IPOABLL _poaBll;


        private string includeTables = "CK5_MATERIAL, PBCK1, UOM, USER, USER1, CK5_FILE_UPLOAD";

        public CK5BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;

            _repository = _uow.GetGenericRepository<CK5>();
            _repositoryCK5Material = _uow.GetGenericRepository<CK5_MATERIAL>();

            _docSeqNumBll = new DocumentSequenceNumberBLL(_uow, _logger);
          
            _brandRegistrationBll = new BrandRegistrationBLL(_uow, _logger);
            _uomBll = new UnitOfMeasurementBLL(_uow, _logger);
            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);
            _workflowHistoryBll = new WorkflowHistoryBLL(_uow, _logger);
            _messageService = new MessageService(_logger);

            _printHistoryBll = new PrintHistoryBLL(_uow, _logger);
            _monthBll = new MonthBLL(_uow, _logger);
            _poaBll = new POABLL(_uow, _logger);
        }
        

        public CK5Dto GetById(long id)
        {
            //var dtData = _repository.GetByID(id);
            //if (dtData == null)
            //    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //return dtData;

            //includeTables = "ZAIDM_EX_GOODTYP,EX_SETTLEMENT,EX_STATUS,REQUEST_TYPE,PBCK1,CARRIAGE_METHOD,COUNTRY, UOM";
            var dtData = _repository.Get(c => c.CK5_ID == id, null, includeTables).FirstOrDefault();
            if (dtData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return Mapper.Map<CK5Dto>(dtData);

        }

        public CK5 GetByIdIncludeTables(long id)
        {
            //includeTables = "ZAIDM_EX_GOODTYP,EX_SETTLEMENT,EX_STATUS,REQUEST_TYPE,PBCK1,CARRIAGE_METHOD,COUNTRY, UOM";
            var dtData = _repository.Get(c => c.CK5_ID == id, null, includeTables).FirstOrDefault();
            if (dtData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return dtData;
        }


        public List<CK5Dto> GetAll()
        {
            var dtData = _repository.Get(null, null, includeTables).ToList();

            return Mapper.Map<List<CK5Dto>>(dtData);
        }


        public List<CK5> GetCK5ByType(Enums.CK5Type ck5Type)
        {
            //includeTables = "T1001W.ZAIDM_EX_NPPBKC, T1001W1.ZAIDM_EX_NPPBKC, T1001W, T1001W1";
            return _repository.Get(c => c.CK5_TYPE == ck5Type, null, includeTables).ToList();
        }

        public List<CK5Dto> GetInitDataListIndex(Enums.CK5Type ck5Type)
        {
           // includeTables = "T1001W.ZAIDM_EX_NPPBKC, T1001W1.ZAIDM_EX_NPPBKC";

            var dtData = _repository.Get(null, null, includeTables).ToList();

            return Mapper.Map<List<CK5Dto>>(dtData);
        }

        public List<CK5Dto> GetCK5ByParam(CK5GetByParamInput input)
        {
            //includeTables = "T1001W.ZAIDM_EX_NPPBKC, T1001W1.ZAIDM_EX_NPPBKC, T1001W, T1001W1,UOM";

            Expression<Func<CK5, bool>> queryFilter = PredicateHelper.True<CK5>();

            if (!string.IsNullOrEmpty(input.DocumentNumber))
            {
                queryFilter = queryFilter.And(c => c.SUBMISSION_NUMBER.Contains(input.DocumentNumber));
            }

            if (!string.IsNullOrEmpty(input.POA))
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY_POA.Contains(input.POA));
            }

            if (!string.IsNullOrEmpty(input.Creator))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY.Contains(input.Creator));
            }

            if (!string.IsNullOrEmpty(input.NPPBKCOrigin))
            {
                queryFilter = queryFilter.And(c => c.SOURCE_PLANT_NPPBKC_ID.Contains(input.NPPBKCOrigin));

            }

            if (!string.IsNullOrEmpty(input.NPPBKCDestination))
            {
                queryFilter = queryFilter.And(c => c.DEST_PLANT_NPPBKC_ID.Contains(input.NPPBKCDestination));

            }

            if (input.Ck5Type == Enums.CK5Type.Completed)
                queryFilter = queryFilter.And(c => c.STATUS_ID == Enums.DocumentStatus.Completed);
            else
                queryFilter = queryFilter.And(c => c.STATUS_ID != Enums.DocumentStatus.Completed 
                                    && c.CK5_TYPE == input.Ck5Type);
                
            
            //Func<IQueryable<CK5>, IOrderedQueryable<CK5>> orderBy = null;
            //if (!string.IsNullOrEmpty(input.SortOrderColumn))
            //{
            //    orderBy = c => c.OrderByDescending(OrderByHelper.GetOrderByFunction<CK5>(input.SortOrderColumn));
            //}
            //default case of ordering
            //Func<IQueryable<CK5>, IOrderedQueryable<CK5>> orderByFilter = n => n.OrderByDescending(z => z.CREATED_DATE);
            Func<IQueryable<CK5>, IOrderedQueryable<CK5>> orderByFilter = n => n.OrderByDescending(z => z.STATUS_ID).ThenBy(z=>z.APPROVED_BY_MANAGER);


            var rc = _repository.Get(queryFilter, orderByFilter, includeTables);
            if (rc == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            var mapResult = Mapper.Map<List<CK5Dto>>(rc.ToList());

            return mapResult;


        }

        public CK5Dto SaveCk5(CK5SaveInput input)
        {
            //workflowhistory
            var inputWorkflowHistory = new CK5WorkflowHistoryInput();

            CK5 dbData = null;
            if (input.Ck5Dto.CK5_ID > 0)
            {
                //update
                dbData = _repository.Get(c => c.CK5_ID == input.Ck5Dto.CK5_ID, null, includeTables).FirstOrDefault();
                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
                
                //set changes history
                var origin = Mapper.Map<CK5Dto>(dbData);

                SetChangesHistory(origin, input.Ck5Dto, input.UserId);

               
                Mapper.Map<CK5Dto, CK5>(input.Ck5Dto, dbData);

                //no change status for edit 2015-07-24
                //dbData.STATUS_ID = Enums.DocumentStatus.Revised;
                dbData.MODIFIED_DATE = DateTime.Now;

                //delete child first
                foreach (var ck5Material in dbData.CK5_MATERIAL.ToList())
                {
                    _repositoryCK5Material.Delete(ck5Material);
                }
                
                inputWorkflowHistory.ActionType = Enums.ActionType.Modified;

                //insert new data
                foreach (var ck5Item in input.Ck5Material)
                {
                    var ck5Material = Mapper.Map<CK5_MATERIAL>(ck5Item);
                    ck5Material.PLANT_ID = dbData.SOURCE_PLANT_ID;
                    dbData.CK5_MATERIAL.Add(ck5Material);
                }

            }
            else
            {
                //create new ck5 documents
                var generateNumberInput = new GenerateDocNumberInput()
                {
                    Year = DateTime.Now.Year,
                    Month = DateTime.Now.Month,
                    NppbkcId = input.Ck5Dto.SOURCE_PLANT_NPPBKC_ID
                };

                input.Ck5Dto.SUBMISSION_NUMBER = _docSeqNumBll.GenerateNumber(generateNumberInput);
                input.Ck5Dto.SUBMISSION_DATE = DateTime.Now;
                input.Ck5Dto.STATUS_ID = Enums.DocumentStatus.Draft;
                input.Ck5Dto.CREATED_DATE = DateTime.Now;
                input.Ck5Dto.CREATED_BY = input.UserId;

                dbData = new CK5();

            
                Mapper.Map<CK5Dto, CK5>(input.Ck5Dto, dbData);
            
                dbData.STATUS_ID = Enums.DocumentStatus.Draft;

                inputWorkflowHistory.ActionType = Enums.ActionType.Created;

                foreach (var ck5Item in input.Ck5Material)
                {
                    var ck5Material = Mapper.Map<CK5_MATERIAL>(ck5Item);
                    ck5Material.PLANT_ID = dbData.SOURCE_PLANT_ID;
                    dbData.CK5_MATERIAL.Add(ck5Material);
                }
                
                _repository.Insert(dbData);

            }

            

            ////insert child
            ////insert the data
            //foreach (var ck5Item in input.Ck5Material)
            //{
            //    var ck5Material = Mapper.Map<CK5_MATERIAL>(ck5Item);
            //    ck5Material.PLANT_ID = dbData.SOURCE_PLANT_ID;
            //    dbData.CK5_MATERIAL.Add(ck5Material);
            //}

           
            //_repository.InsertOrUpdate(dbData);

            inputWorkflowHistory.DocumentId = dbData.CK5_ID;
            inputWorkflowHistory.DocumentNumber = dbData.SUBMISSION_NUMBER;
            inputWorkflowHistory.UserId = input.UserId;
            inputWorkflowHistory.UserRole = input.UserRole;
         

            AddWorkflowHistory(inputWorkflowHistory);


            _uow.SaveChanges();

            return Mapper.Map<CK5Dto>(dbData);


        }

        private List<CK5MaterialOutput> ValidateCk5Material(List<CK5MaterialInput> inputs)
        {
            var messageList = new List<string>();
            var outputList = new List<CK5MaterialOutput>();

            foreach (var ck5MaterialInput in inputs)
            {
                messageList.Clear();

                //var output = new CK5MaterialOutput();
                var output = Mapper.Map<CK5MaterialOutput>(ck5MaterialInput);

                //validate
                var dbBrand = _brandRegistrationBll.GetByPlantIdAndFaCode(ck5MaterialInput.Plant, ck5MaterialInput.Brand);
                if (dbBrand == null)
                    messageList.Add("Brand Not Exist");

                if (!Utils.ConvertHelper.IsNumeric(ck5MaterialInput.Qty))
                    messageList.Add("Qty not valid");

                if (!_uomBll.IsUomIdExist(ck5MaterialInput.Uom))
                    messageList.Add("UOM not exist");

                if (!Utils.ConvertHelper.IsNumeric(ck5MaterialInput.Convertion))
                    messageList.Add("Convertion not valid");

                if (!_uomBll.IsUomIdExist(ck5MaterialInput.ConvertedUom))
                    messageList.Add("ConvertedUom not valid");

                if (!Utils.ConvertHelper.IsNumeric(ck5MaterialInput.UsdValue))
                    messageList.Add("UsdValue not valid");

                if (messageList.Count > 0)
                {
                    output.IsValid = false;
                    output.Message = "";
                    foreach (var message in messageList)
                    {
                        output.Message += message + ";";
                    }
                }
                else
                {
                    output.IsValid = true;
                }

                outputList.Add(output);
            }

            //return outputList.All(ck5MaterialOutput => ck5MaterialOutput.IsValid);
            return outputList;
        }

        public List<CK5MaterialOutput> CK5MaterialProcess(List<CK5MaterialInput> inputs)
        {
            var outputList = ValidateCk5Material(inputs);

            if (!outputList.All(ck5MaterialOutput => ck5MaterialOutput.IsValid))
                return outputList;

            foreach (var output in outputList)
            {

                output.ConvertedQty = Convert.ToInt32(output.Qty) * Convert.ToInt32(output.Convertion);

                var dbBrand = _brandRegistrationBll.GetByPlantIdAndFaCode(output.Plant, output.Brand);
                if (dbBrand == null)
                {
                    output.Hje = 0;
                    output.Tariff = 0;
                }
                else
                {
                    output.Hje = dbBrand.HJE_IDR.HasValue ? dbBrand.HJE_IDR.Value : 0;
                    output.Tariff = dbBrand.TARIFF.HasValue ? dbBrand.TARIFF.Value : 0;
                }
                
                output.ExciseValue = output.ConvertedQty * output.Tariff;

            }

            return outputList;
        }

        private void SetChangesHistory(CK5Dto origin, CK5Dto data, string userId)
        {
            //todo check the new value
            var changesData = new Dictionary<string, bool>();

            changesData.Add("KPPBC_CITY", origin.KPPBC_CITY == data.KPPBC_CITY);
            changesData.Add("REGISTRATION_NUMBER", origin.REGISTRATION_NUMBER == data.REGISTRATION_NUMBER);

            changesData.Add("EX_GOODS_TYPE", origin.EX_GOODS_TYPE == data.EX_GOODS_TYPE);

            changesData.Add("EX_SETTLEMENT_ID", origin.EX_SETTLEMENT_ID == data.EX_SETTLEMENT_ID);
            changesData.Add("EX_STATUS_ID", origin.EX_STATUS_ID == data.EX_STATUS_ID);
            changesData.Add("REQUEST_TYPE_ID", origin.REQUEST_TYPE_ID == data.REQUEST_TYPE_ID);
            changesData.Add("SOURCE_PLANT_ID", origin.SOURCE_PLANT_ID ==(data.SOURCE_PLANT_ID));
            changesData.Add("DEST_PLANT_ID", origin.DEST_PLANT_ID == (data.DEST_PLANT_ID));

            changesData.Add("INVOICE_NUMBER", origin.INVOICE_NUMBER == data.INVOICE_NUMBER);
            changesData.Add("INVOICE_DATE", origin.INVOICE_DATE == (data.INVOICE_DATE));

            changesData.Add("PBCK1_DECREE_ID", origin.PBCK1_DECREE_ID == (data.PBCK1_DECREE_ID));
            changesData.Add("CARRIAGE_METHOD_ID", origin.CARRIAGE_METHOD_ID == (data.CARRIAGE_METHOD_ID));

            changesData.Add("GRAND_TOTAL_EX", origin.GRAND_TOTAL_EX == (data.GRAND_TOTAL_EX));
           
            changesData.Add("PACKAGE_UOM_ID", origin.PACKAGE_UOM_ID == data.PACKAGE_UOM_ID);

            changesData.Add("DESTINATION_COUNTRY", origin.DEST_COUNTRY_NAME == data.DEST_COUNTRY_NAME);

            foreach (var listChange in changesData)
            {
                if (listChange.Value) continue;
                var changes = new CHANGES_HISTORY();
                changes.FORM_TYPE_ID = Enums.MenuList.CK5;
                changes.FORM_ID = origin.CK5_ID.ToString();
                changes.FIELD_NAME = listChange.Key;
                changes.MODIFIED_BY = userId;
                changes.MODIFIED_DATE = DateTime.Now;
                switch (listChange.Key)
                {
                    case "KPPBC_CITY":
                        changes.OLD_VALUE = origin.KPPBC_CITY;
                        changes.NEW_VALUE = data.KPPBC_CITY;
                        break;
                    case "REGISTRATION_NUMBER":
                        changes.OLD_VALUE = origin.REGISTRATION_NUMBER;
                        changes.NEW_VALUE = data.REGISTRATION_NUMBER;
                        break;
                    case "EX_GOODS_TYPE":
                        changes.OLD_VALUE = EnumHelper.GetDescription(origin.EX_GOODS_TYPE);
                        changes.NEW_VALUE = EnumHelper.GetDescription(data.EX_GOODS_TYPE);
                        break;
                    case "EX_SETTLEMENT_ID":
                        changes.OLD_VALUE = EnumHelper.GetDescription(origin.EX_SETTLEMENT_ID);
                        changes.NEW_VALUE = EnumHelper.GetDescription(data.EX_SETTLEMENT_ID);
                        break;
                    case "EX_STATUS_ID":
                        changes.OLD_VALUE = EnumHelper.GetDescription(origin.EX_STATUS_ID);
                        changes.NEW_VALUE = EnumHelper.GetDescription(data.EX_STATUS_ID);
                        break;
                    case "REQUEST_TYPE_ID":
                        changes.OLD_VALUE = EnumHelper.GetDescription(origin.REQUEST_TYPE_ID);
                        changes.NEW_VALUE = EnumHelper.GetDescription(data.REQUEST_TYPE_ID);
                        break;
                    case "SOURCE_PLANT_ID":
                        changes.OLD_VALUE = origin.SOURCE_PLANT_ID;
                        changes.NEW_VALUE = data.SOURCE_PLANT_ID;
                        break;
                    case "DEST_PLANT_ID":
                        changes.OLD_VALUE = origin.DEST_PLANT_ID;
                        changes.NEW_VALUE = data.DEST_PLANT_ID;
                        break;
                    case "INVOICE_NUMBER":
                        changes.OLD_VALUE = origin.INVOICE_NUMBER;
                        changes.NEW_VALUE = data.INVOICE_NUMBER;
                        break;
                    case "INVOICE_DATE":
                        changes.OLD_VALUE = origin.INVOICE_DATE != null ? origin.INVOICE_DATE.Value.ToString("dd MMM yyyy") : string.Empty;
                        changes.NEW_VALUE = data.INVOICE_DATE != null ? data.INVOICE_DATE.Value.ToString("dd MMM yyyy") : string.Empty;
                        break;
                    case "PBCK1_DECREE_ID":
                      
                        changes.OLD_VALUE = origin.PbckNumber;
                        changes.NEW_VALUE = data.PbckNumber;
                        break;

                    case "CARRIAGE_METHOD_ID":
                        changes.OLD_VALUE = origin.CARRIAGE_METHOD_ID.HasValue ? EnumHelper.GetDescription(origin.CARRIAGE_METHOD_ID) : "NULL";
                        changes.NEW_VALUE = data.CARRIAGE_METHOD_ID.HasValue ? EnumHelper.GetDescription(data.CARRIAGE_METHOD_ID) : "NULL";
                        break;

                    case "GRAND_TOTAL_EX":
                        changes.OLD_VALUE = origin.GRAND_TOTAL_EX.ToString();
                        changes.NEW_VALUE = data.GRAND_TOTAL_EX.ToString();
                        break;

                    case "PACKAGE_UOM_ID":
                        changes.OLD_VALUE = origin.PackageUomName;
                        changes.NEW_VALUE = data.PackageUomName;
                        break;
                    case "DESTINATION_COUNTRY":
                        changes.OLD_VALUE = origin.DEST_COUNTRY_NAME;
                        changes.NEW_VALUE = data.DEST_COUNTRY_NAME;
                        break;

                }
                _changesHistoryBll.AddHistory(changes);
            }
        }

       
        public CK5DetailsOutput GetDetailsCK5(long id)
        {
            var output = new CK5DetailsOutput();

            var dtData = _repository.Get(c => c.CK5_ID == id, null, includeTables).FirstOrDefault();
            if (dtData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            output.Ck5Dto = Mapper.Map<CK5Dto>(dtData);

            //details
            output.Ck5MaterialDto = Mapper.Map<List<CK5MaterialDto>>(dtData.CK5_MATERIAL);

            //change history data
            output.ListChangesHistorys = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.CK5, output.Ck5Dto.CK5_ID.ToString());

            //workflow history
            var input = new GetByFormNumberInput();
            input.FormNumber = dtData.SUBMISSION_NUMBER;
            input.DocumentStatus = dtData.STATUS_ID;
            input.NPPBKC_Id = dtData.SOURCE_PLANT_NPPBKC_ID;

            //output.ListWorkflowHistorys = _workflowHistoryBll.GetByFormNumber(dtData.SUBMISSION_NUMBER);
            output.ListWorkflowHistorys = _workflowHistoryBll.GetByFormNumber(input);


            output.ListPrintHistorys = _printHistoryBll.GetByFormTypeAndFormId(Enums.FormType.CK5, dtData.CK5_ID);
            return output;
        }

        public List<CK5MaterialDto> GetCK5MaterialByCK5Id(long id)
        {
            var result = _repositoryCK5Material.Get(c => c.CK5_ID == id);

            if (result == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return Mapper.Map<List<CK5MaterialDto>>(result);
        }


        #region workflow

        private void AddWorkflowHistory(CK5WorkflowHistoryInput input)
        {
            var inputWorkflowHistory = new GetByActionAndFormNumberInput();
            inputWorkflowHistory.ActionType = input.ActionType;
            inputWorkflowHistory.FormNumber = input.DocumentNumber;

            //WorkflowHistoryDto dbData = null;

            //remark 2015-07-24
            // ... for save should be same like others
            //if yes then remove this function only use one function
            //only save can be update, else insert new one
            //if (input.ActionType == Enums.ActionType.Modified)
            //    dbData = _workflowHistoryBll.GetByActionAndFormNumber(inputWorkflowHistory);


            //if (dbData == null)
            //{
            //    dbData = new WorkflowHistoryDto()
            //    {
            //        ACTION = input.ActionType,
            //        FORM_NUMBER = input.DocumentNumber,
            //        FORM_TYPE_ID = Core.Enums.FormType.CK5
            //    };
            //}
            var dbData = new WorkflowHistoryDto();
            dbData.ACTION = input.ActionType;
            dbData.FORM_NUMBER = input.DocumentNumber;
            dbData.FORM_TYPE_ID = Enums.FormType.CK5;

            dbData.FORM_ID = input.DocumentId;
            if (!string.IsNullOrEmpty(input.Comment))
                dbData.COMMENT = input.Comment;


            dbData.ACTION_BY = input.UserId;
            dbData.ROLE = input.UserRole;
            dbData.ACTION_DATE = DateTime.Now;

            _workflowHistoryBll.Save(dbData);
        }


        private void AddWorkflowHistory(CK5WorkflowDocumentInput input)
        {
            var inputWorkflowHistory = new CK5WorkflowHistoryInput();

            inputWorkflowHistory.DocumentId = input.DocumentId;
            inputWorkflowHistory.DocumentNumber = input.DocumentNumber;
            inputWorkflowHistory.UserId = input.UserId;
            inputWorkflowHistory.UserRole = input.UserRole;
            inputWorkflowHistory.ActionType = input.ActionType;
            inputWorkflowHistory.Comment = input.Comment;

            AddWorkflowHistory(inputWorkflowHistory);
        }

        public void CK5Workflow(CK5WorkflowDocumentInput input)
        {
            var isNeedSendNotif = false;

            switch (input.ActionType)
            {
                case Enums.ActionType.Submit:
                    SubmitDocument(input);
                    isNeedSendNotif = true;
                    break;
                case Enums.ActionType.Approve:
                    ApproveDocument(input);
                    isNeedSendNotif = true;
                    break;
                case Enums.ActionType.Reject:
                    RejectDocument(input);
                    isNeedSendNotif = true;
                    break;
                case Enums.ActionType.GovApprove:
                    GovApproveDocument(input);
                    break;
                case Enums.ActionType.GovReject:
                    GovRejectedDocument(input);
                    break;
                case Enums.ActionType.GovCancel:
                    GovCancelledDocument(input);
                    break;
                case Enums.ActionType.Cancel:
                    CancelledDocument(input);
                    break;
            }

            //todo sent mail
            if (isNeedSendNotif)
                SendEmailWorkflow(input);

            _uow.SaveChanges();
        }

        private void SendEmailWorkflow(CK5WorkflowDocumentInput input)
        {
            //todo: body message from email template
            //todo: to = ?
            //todo: subject = from email template
            var to = "irmansulaeman41@gmail.com";
            var subject = "this is subject for " + input.DocumentNumber;
            var body = "this is body message for " + input.DocumentNumber;
            //var from = "a@gmail.com";

            _messageService.SendEmail( to, subject, body, true);
        }


        private void SubmitDocument(CK5WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS_ID != Enums.DocumentStatus.Draft)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            dbData.STATUS_ID = Enums.DocumentStatus.WaitingForApproval;
            
            input.DocumentNumber = dbData.SUBMISSION_NUMBER;

            AddWorkflowHistory(input);

            switch (input.UserRole)
            {
                case Enums.UserRole.User:
                    dbData.STATUS_ID = Enums.DocumentStatus.WaitingForApproval;
                    break;
                case Enums.UserRole.POA:
                    dbData.STATUS_ID = Enums.DocumentStatus.WaitingForApprovalManager;
                    break;
                default:
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
            }

        }

        private void ApproveDocument(CK5WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS_ID != Enums.DocumentStatus.WaitingForApproval &&
                dbData.STATUS_ID != Enums.DocumentStatus.WaitingForApprovalManager)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            if (input.UserRole == Enums.UserRole.POA)
            {
                dbData.STATUS_ID = Enums.DocumentStatus.WaitingForApprovalManager;
                dbData.APPROVED_BY_POA = input.UserId;
                dbData.APPROVED_DATE_POA = DateTime.Now;
            }
            else if (input.UserRole == Enums.UserRole.Manager)
            {
                dbData.STATUS_ID = Enums.DocumentStatus.WaitingGovApproval;
                dbData.APPROVED_BY_MANAGER = input.UserId;
                dbData.APPROVED_DATE_MANAGER = DateTime.Now;
            }


            input.DocumentNumber = dbData.SUBMISSION_NUMBER;

            AddWorkflowHistory(input);

        }

        private void RejectDocument(CK5WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS_ID != Enums.DocumentStatus.WaitingForApproval &&
                dbData.STATUS_ID != Enums.DocumentStatus.WaitingForApprovalManager)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

         
            //change back to draft
            dbData.STATUS_ID = Enums.DocumentStatus.Draft;

          
            input.DocumentNumber = dbData.SUBMISSION_NUMBER;

            AddWorkflowHistory(input);

        }

        private void GovApproveDocument(CK5WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS_ID != Enums.DocumentStatus.WaitingGovApproval)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            if (input.AdditionalDocumentData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            if (string.IsNullOrEmpty(input.AdditionalDocumentData.RegistrationNumber))
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            if (input.AdditionalDocumentData.RegistrationDate == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            dbData.STATUS_ID = Enums.DocumentStatus.Completed;
            dbData.REGISTRATION_NUMBER = input.AdditionalDocumentData.RegistrationNumber;

            dbData.REGISTRATION_DATE = input.AdditionalDocumentData.RegistrationDate;
            dbData.CK5_FILE_UPLOAD = Mapper.Map<List<CK5_FILE_UPLOAD>>(input.AdditionalDocumentData.Ck5FileUploadList);

           
            input.DocumentNumber = dbData.SUBMISSION_NUMBER;

            AddWorkflowHistory(input);

          
        }

        private void GovRejectedDocument(CK5WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS_ID != Enums.DocumentStatus.WaitingGovApproval)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);


            dbData.STATUS_ID = Enums.DocumentStatus.Completed;
           // input.ActionType = Enums.ActionType.GovReject;

            //dbData.APPROVED_BY_POA = input.UserId;
            //dbData.APPROVED_DATE_POA = DateTime.Now;

            input.DocumentNumber = dbData.SUBMISSION_NUMBER;

            AddWorkflowHistory(input);

        }

        private void GovCancelledDocument(CK5WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS_ID != Enums.DocumentStatus.WaitingGovApproval)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            dbData.STATUS_ID = Enums.DocumentStatus.Completed;
            //input.ActionType = Enums.ActionType.GovCancel;
            //dbData.APPROVED_BY = input.UserId;
            //dbData.APPROVED_DATE = DateTime.Now;

            input.DocumentNumber = dbData.SUBMISSION_NUMBER;

            AddWorkflowHistory(input);
        }

        private void CancelledDocument(CK5WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS_ID != Enums.DocumentStatus.Draft)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            dbData.STATUS_ID = Enums.DocumentStatus.Cancelled;

          
            input.DocumentNumber = dbData.SUBMISSION_NUMBER;

            AddWorkflowHistory(input);
        }

        #endregion

        public List<CK5Dto> GetSummaryReportsByParam(CK5GetSummaryReportByParamInput input)
        {
          
            Expression<Func<CK5, bool>> queryFilter = PredicateHelper.True<CK5>();

            if (!string.IsNullOrEmpty(input.CompanyCodeSource))
            {
                queryFilter = queryFilter.And(c => c.SOURCE_PLANT_COMPANY_CODE.Contains(input.CompanyCodeSource));
            }

            if (!string.IsNullOrEmpty(input.CompanyCodeDest))
            {
                queryFilter = queryFilter.And(c => c.DEST_PLANT_COMPANY_CODE.Contains(input.CompanyCodeDest));
            }

            if (!string.IsNullOrEmpty(input.NppbkcIdSource))
            {
                queryFilter = queryFilter.And(c => c.SOURCE_PLANT_NPPBKC_ID.Contains(input.NppbkcIdSource));
            }

            if (!string.IsNullOrEmpty(input.NppbkcIdDest))
            {
                queryFilter = queryFilter.And(c => c.DEST_PLANT_NPPBKC_ID.Contains(input.NppbkcIdDest));

            }

            if (!string.IsNullOrEmpty(input.PlantSource))
            {
                queryFilter = queryFilter.And(c => c.SOURCE_PLANT_ID.Contains(input.PlantSource));

            }

            if (!string.IsNullOrEmpty(input.PlantDest))
            {
                queryFilter = queryFilter.And(c => c.DEST_PLANT_ID.Contains(input.PlantDest));

            }

            if (input.DateFrom.HasValue)
            {
                input.DateFrom = new DateTime(input.DateFrom.Value.Year, input.DateFrom.Value.Month, input.DateFrom.Value.Day,0,0,0);
                queryFilter = queryFilter.And(c => c.SUBMISSION_DATE >= input.DateFrom);
            }

            if (input.DateTo.HasValue)
            {
                input.DateFrom = new DateTime(input.DateTo.Value.Year, input.DateTo.Value.Month, input.DateTo.Value.Day, 23, 59, 59);
                queryFilter = queryFilter.And(c => c.SUBMISSION_DATE <= input.DateTo);
            }


            queryFilter = queryFilter.And(c => c.CK5_TYPE == input.Ck5Type);

            queryFilter = queryFilter.And(c => c.STATUS_ID == Enums.DocumentStatus.Completed);
          

            var rc = _repository.Get(queryFilter, null, includeTables);
            if (rc == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            var mapResult = Mapper.Map<List<CK5Dto>>(rc.ToList());

            return mapResult;


        }

        public List<CK5Dto> GetCk5CompletedByCk5Type(Enums.CK5Type ck5Type)
        {
            var dtData = _repository.Get(c=>c.STATUS_ID == Enums.DocumentStatus.Completed && c.CK5_TYPE == ck5Type, null, includeTables).ToList();
            return Mapper.Map<List<CK5Dto>>(dtData);
        }

        #region Reports

        public CK5ReportDto GetCk5ReportDataById(long id)
        {
            var dtData = _repository.Get(c => c.CK5_ID == id, null, includeTables).FirstOrDefault();
            if (dtData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //var ck5Report = new CK5ReportDto();
            //ck5Report.ReportDetails = Mapper.Map<>()
            var result = Mapper.Map<CK5ReportDto>(dtData);

            //request type Enums.RequestType
            if (result.ReportDetails.RequestType.Length >= 2)
            {
                string requestType = result.ReportDetails.RequestType.ToCharArray()
                    .Aggregate("", (current, charRequest) => current + (charRequest.ToString() + "."));
                requestType = requestType.Substring(0, requestType.Length - 1);
                result.ReportDetails.RequestType = requestType;
            }
            result.ReportDetails.ExGoodType = (Convert.ToInt32(dtData.EX_GOODS_TYPE)).ToString();

            if (result.ReportDetails.CarriageMethod == "0")
                result.ReportDetails.CarriageMethod = "";

            //date convertion
            if (dtData.SUBMISSION_DATE.HasValue)
                result.ReportDetails.SubmissionDate = DateReportDisplayString(dtData.SUBMISSION_DATE.Value);
            if (dtData.REGISTRATION_DATE.HasValue)
                result.ReportDetails.RegistrationDate = DateReportDisplayString(dtData.REGISTRATION_DATE.Value);
            if (dtData.PBCK1 != null)
            {
                if (dtData.PBCK1.DECREE_DATE.HasValue)
                    result.ReportDetails.RegistrationDate = DateReportDisplayString(dtData.PBCK1.DECREE_DATE.Value);
            }
            if (dtData.INVOICE_DATE.HasValue)
                result.ReportDetails.InvoiceDate = DateReportDisplayString(dtData.INVOICE_DATE.Value);

            result.ReportDetails.PrintDate = DateReportDisplayString(DateTime.Now);

            //get poa info
            POADto poaInfo;
            poaInfo = _poaBll.GetDetailsById(dtData.APPROVED_BY_POA);
            if (poaInfo == null)
                poaInfo = _poaBll.GetDetailsById(dtData.CREATED_BY);

            if (poaInfo != null)
            {
                result.ReportDetails.PoaName = poaInfo.PRINTED_NAME;
                result.ReportDetails.PoaAddress = poaInfo.POA_ADDRESS;
                result.ReportDetails.PoaIdCard = poaInfo.ID_CARD;
                result.ReportDetails.PoaCity = dtData.KPPBC_CITY;
            }

            //for export type
            if (dtData.CK5_TYPE == Enums.CK5Type.Export)
            {
                result.ReportDetails.DestPlantNpwp = "-";
                result.ReportDetails.DestPlantNppbkc = "-";
                result.ReportDetails.DestPlantName = "-";
                result.ReportDetails.DestPlantAddress = "-";
                result.ReportDetails.DestOfficeName = "-";
                result.ReportDetails.DestOfficeCode = "-";

                result.ReportDetails.DestinationCountry = dtData.DEST_COUNTRY_NAME;
                result.ReportDetails.DestinationCode = dtData.DEST_COUNTRY_CODE;
                result.ReportDetails.DestinationNppbkc = dtData.DEST_PLANT_NPPBKC_ID;
                result.ReportDetails.DestinationName = dtData.DEST_PLANT_NAME;
                result.ReportDetails.DestinationAddress = dtData.DEST_PLANT_ADDRESS;
                result.ReportDetails.DestinationOfficeName = dtData.DEST_PLANT_COMPANY_NAME;
                result.ReportDetails.DestinationOfficeCode = dtData.DEST_PLANT_COMPANY_CODE;

                result.ReportDetails.LoadingPort = dtData.LOADING_PORT;
                result.ReportDetails.LoadingPortName = dtData.LOADING_PORT_NAME;
                result.ReportDetails.LoadingPortId = dtData.LOADING_PORT_ID;
                result.ReportDetails.FinalPort = dtData.FINAL_PORT;
                result.ReportDetails.FinalPortName = dtData.FINAL_PORT_NAME;
                result.ReportDetails.FinalPortId = dtData.FINAL_PORT_ID;
            }
            else
            {
                result.ReportDetails.DestPlantNpwp = dtData.DEST_PLANT_NPWP;
                result.ReportDetails.DestPlantNppbkc = dtData.DEST_PLANT_NPPBKC_ID;
                result.ReportDetails.DestPlantName = dtData.DEST_PLANT_NAME;
                result.ReportDetails.DestPlantAddress = dtData.DEST_PLANT_ADDRESS;
                result.ReportDetails.DestOfficeName = dtData.DEST_PLANT_COMPANY_NAME;
                result.ReportDetails.DestOfficeCode = dtData.DEST_PLANT_COMPANY_CODE;

                result.ReportDetails.DestinationCountry = "-";
                result.ReportDetails.DestinationCode = "-";
                result.ReportDetails.DestinationNppbkc = "-";
                result.ReportDetails.DestinationName = "-";
                result.ReportDetails.DestinationAddress = "-";
                result.ReportDetails.DestinationOfficeName = "-";
                result.ReportDetails.DestinationOfficeCode = "-";

                result.ReportDetails.LoadingPort = "-";
                result.ReportDetails.LoadingPortName = "-";
                result.ReportDetails.LoadingPortId = "-";
                result.ReportDetails.FinalPort = "-";
                result.ReportDetails.FinalPortName = "-";
                result.ReportDetails.FinalPortId = "-";
            }
            return result;
            //return Mapper.Map<CK5ReportDto>(dtData);
        }

        #endregion

        private string DateReportDisplayString(DateTime dt)
        {
            var monthPeriodFrom = _monthBll.GetMonth(dt.Month);
            return dt.ToString("dd") + " " + monthPeriodFrom.MONTH_NAME_IND +
                                   " " + dt.ToString("yyyy");
        }

        //public void AddPrintHistory(long id, string userId)
        //{
        //    var dtData = _repository.GetByID(id);
        //     if (dtData == null)
        //        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

        //    var printHistory = new PrintHistoryDto();
        //    printHistory.FORM_ID = dtData.CK5_ID;
        //    printHistory.FORM_NUMBER = dtData.SUBMISSION_NUMBER;
        //    printHistory.FORM_TYPE_ID = Enums.FormType.CK5;
        //    printHistory.PRINT_BY = userId;
        //    printHistory.PRINT_DATE = DateTime.Now;


        //    _printHistoryBll.AddPrintHistory(printHistory);

        //    _uow.SaveChanges();
        //}
    }
}
