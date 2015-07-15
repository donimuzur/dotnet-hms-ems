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
        private IMasterDataBLL _masterDataBll;
        private IBrandRegistrationBLL _brandRegistrationBll;
        private IUnitOfMeasurementBLL _uomBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IZaidmExNPPBKCBLL _nppbkcBll;
        private IZaidmExGoodTypeBLL _goodTypeBll;
        private IPlantBLL _plantBll;
        private IPBCK1BLL _pbck1Bll;

        private string includeTables = "T1001W.ZAIDM_EX_NPPBKC.T1001,CK5_MATERIAL, ZAIDM_EX_NPPBKC.T1001,ZAIDM_EX_NPPBKC.ZAIDM_EX_KPPBC, ZAIDM_EX_NPPBKC, ZAIDM_EX_GOODTYP,EX_SETTLEMENT,EX_STATUS,REQUEST_TYPE,T1001W, T1001W1, PBCK1,CARRIAGE_METHOD,COUNTRY, UOM, USER, CK5_MATERIAL";

        public CK5BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;

            _repository = _uow.GetGenericRepository<CK5>();
            _repositoryCK5Material = _uow.GetGenericRepository<CK5_MATERIAL>();

            _docSeqNumBll = new DocumentSequenceNumberBLL(_uow, _logger);
            _masterDataBll = new MasterDataBLL(_uow);
            _brandRegistrationBll = new BrandRegistrationBLL(_uow, _logger);
            _uomBll = new UnitOfMeasurementBLL(_uow,_logger);
            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);
            _workflowHistoryBll = new WorkflowHistoryBLL(_uow, _logger);
            _nppbkcBll = new ZaidmExNPPBKCBLL(_uow, _logger);
            _goodTypeBll = new ZaidmExGoodTypeBLL(_uow,_logger);
            _plantBll = new PlantBLL(_uow, _logger);
            _pbck1Bll = new PBCK1BLL(_uow,_logger);
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
            includeTables = "ZAIDM_EX_GOODTYP,EX_SETTLEMENT,EX_STATUS,REQUEST_TYPE,PBCK1,CARRIAGE_METHOD,COUNTRY, UOM";
            var dtData = _repository.Get(c=>c.CK5_ID == id, null,includeTables).FirstOrDefault();
            if (dtData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return dtData;
        }

      
        public List<CK5Dto> GetAll()
        {
            //includeTables = "T1001W.ZAIDM_EX_NPPBKC, T1001W1.ZAIDM_EX_NPPBKC";
            //return _repository.Get(null, null, includeTables).ToList();
            includeTables = "T1001W.ZAIDM_EX_NPPBKC, T1001W1.ZAIDM_EX_NPPBKC";

            var dtData = _repository.Get(null, null, includeTables).ToList();

            return Mapper.Map<List<CK5Dto>>(dtData);
        }


        public List<CK5> GetCK5ByType(Enums.CK5Type ck5Type)
        {
            includeTables = "T1001W.ZAIDM_EX_NPPBKC, T1001W1.ZAIDM_EX_NPPBKC, T1001W, T1001W1";
            return _repository.Get(c => c.CK5_TYPE == ck5Type, null, includeTables).ToList();
        }

        public List<CK5Dto> GetInitDataListIndex(Enums.CK5Type ck5Type)
        {
            includeTables = "T1001W.ZAIDM_EX_NPPBKC, T1001W1.ZAIDM_EX_NPPBKC";
            
            var dtData = _repository.Get(null, null, includeTables).ToList();

            return Mapper.Map<List<CK5Dto>>(dtData);
        }

        public List<CK5Dto> GetCK5ByParam(CK5GetByParamInput input)
        {
            includeTables = "T1001W.ZAIDM_EX_NPPBKC, T1001W1.ZAIDM_EX_NPPBKC, T1001W, T1001W1,UOM";

            Expression<Func<CK5, bool>> queryFilter = PredicateHelper.True<CK5>();

            if (!string.IsNullOrEmpty(input.DocumentNumber))
            {
                queryFilter = queryFilter.And(c => c.SUBMISSION_NUMBER.Contains(input.DocumentNumber));
            }

            if (input.POA.HasValue)
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY.HasValue && c.APPROVED_BY.Value == input.POA.Value);
            }

            if (input.Creator.HasValue)
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY.HasValue && c.CREATED_BY.Value == input.Creator.Value);
            }

            if (input.NPPBKCOrigin.HasValue)
            {
                //queryFilter = queryFilter.And(c => c.SOURCE_PLANT_ID.HasValue && c.SOURCE_PLANT_ID.Value == input.NPPBKCOrigin.Value);
                queryFilter = queryFilter.And(c => c.T1001W.NPPBCK_ID == input.NPPBKCOrigin.Value);
                
            }

            if (input.NPPBKCDestination.HasValue)
            {
                //queryFilter = queryFilter.And(c => c.DEST_PLANT_ID.HasValue && c.DEST_PLANT_ID.Value == input.NPPBKCDestination.Value);
                queryFilter = queryFilter.And(c => c.T1001W1.NPPBCK_ID == input.NPPBKCDestination.Value);
            }


            queryFilter = queryFilter.And(c => c.CK5_TYPE == input.Ck5Type);
            

            Func<IQueryable<CK5>, IOrderedQueryable<CK5>> orderBy = null;
            if (!string.IsNullOrEmpty(input.SortOrderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<CK5>(input.SortOrderColumn));
            }

            var rc = _repository.Get(queryFilter, orderBy, includeTables);
            if (rc == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            var mapResult = Mapper.Map<List<CK5Dto>>(rc.ToList());

            return mapResult;

            
        }

        public CK5Dto SaveCk5(CK5SaveInput input)
        {
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

                //todo mapper
                //Mapper.Map<CK5Dto, CK5>(input.Ck5Dto, dbData);
                dbData.REQUEST_TYPE_ID = input.Ck5Dto.REQUEST_TYPE_ID;
                dbData.STATUS_ID = Enums.DocumentStatus.Draft;
                dbData.CK5_TYPE = input.Ck5Dto.CK5_TYPE;
                dbData.KPPBC_CITY = input.Ck5Dto.KPPBC_CITY;

                dbData.REGISTRATION_NUMBER = input.Ck5Dto.REGISTRATION_NUMBER;
                dbData.EX_GOODS_TYPE_ID = input.Ck5Dto.EX_GOODS_TYPE_ID;
                dbData.EX_SETTLEMENT_ID = input.Ck5Dto.EX_SETTLEMENT_ID;
                dbData.EX_STATUS_ID = input.Ck5Dto.EX_STATUS_ID;
                dbData.REQUEST_TYPE_ID = input.Ck5Dto.REQUEST_TYPE_ID;

                dbData.SOURCE_PLANT_ID = input.Ck5Dto.SOURCE_PLANT_ID;
                dbData.DEST_PLANT_ID = input.Ck5Dto.DEST_PLANT_ID;
                dbData.INVOICE_NUMBER = input.Ck5Dto.INVOICE_NUMBER;
                dbData.PBCK1_DECREE_ID = input.Ck5Dto.PBCK1_DECREE_ID;
                dbData.CARRIAGE_METHOD_ID = input.Ck5Dto.CARRIAGE_METHOD_ID;
                dbData.GRAND_TOTAL_EX = input.Ck5Dto.GRAND_TOTAL_EX;
                dbData.INVOICE_DATE = input.Ck5Dto.INVOICE_DATE;
                dbData.PACKAGE_UOM_ID = input.Ck5Dto.PACKAGE_UOM_ID;

                //delete child first
                foreach (var ck5Material in dbData.CK5_MATERIAL.ToList())
                {
                    _repositoryCK5Material.Delete(ck5Material);
                }

              

            }
            else
            {
                long plantId = 0;
                if (input.Ck5Dto.SOURCE_PLANT_ID.HasValue)
                    plantId = input.Ck5Dto.SOURCE_PLANT_ID.Value;

                var plant = _masterDataBll.GetPlantById(plantId);
                long nppbkc = 0;
                if (plant.NPPBCK_ID.HasValue)
                    nppbkc = plant.NPPBCK_ID.Value;

                var generateNumberInput = new GenerateDocNumberInput()
                {
                    Year = DateTime.Now.Year,
                    Month = DateTime.Now.Month,
                    NppbkcId = nppbkc
                };
                input.Ck5Dto.SUBMISSION_NUMBER = _docSeqNumBll.GenerateNumber(generateNumberInput);
                input.Ck5Dto.STATUS_ID = Enums.DocumentStatus.Draft;
                input.Ck5Dto.CREATED_DATE = DateTime.Now;
                input.Ck5Dto.CREATED_BY = input.UserId;
             
                dbData = new CK5();

                //Mapper.Map<CK5Dto, CK5>(input.Ck5Dto, dbData);
                dbData.CK5_TYPE = input.Ck5Dto.CK5_TYPE;
                dbData.KPPBC_CITY = input.Ck5Dto.KPPBC_CITY;
                dbData.SUBMISSION_NUMBER = input.Ck5Dto.SUBMISSION_NUMBER;
                dbData.REGISTRATION_NUMBER = input.Ck5Dto.REGISTRATION_NUMBER;
                dbData.EX_GOODS_TYPE_ID = input.Ck5Dto.EX_GOODS_TYPE_ID;
                dbData.EX_SETTLEMENT_ID = input.Ck5Dto.EX_SETTLEMENT_ID;
                dbData.EX_STATUS_ID = input.Ck5Dto.EX_STATUS_ID;
                dbData.REQUEST_TYPE_ID = input.Ck5Dto.REQUEST_TYPE_ID;

                dbData.SOURCE_PLANT_ID = input.Ck5Dto.SOURCE_PLANT_ID;
                dbData.DEST_PLANT_ID = input.Ck5Dto.DEST_PLANT_ID;
                dbData.INVOICE_NUMBER = input.Ck5Dto.INVOICE_NUMBER;
                dbData.PBCK1_DECREE_ID = input.Ck5Dto.PBCK1_DECREE_ID;
                dbData.CARRIAGE_METHOD_ID = input.Ck5Dto.CARRIAGE_METHOD_ID;
                dbData.GRAND_TOTAL_EX = input.Ck5Dto.GRAND_TOTAL_EX;
                dbData.INVOICE_DATE = input.Ck5Dto.INVOICE_DATE;
                dbData.PACKAGE_UOM_ID = input.Ck5Dto.PACKAGE_UOM_ID;

                dbData.STATUS_ID = Enums.DocumentStatus.Draft;
               
            }

            dbData.CREATED_DATE = DateTime.Now;
            dbData.CREATED_BY = input.UserId;

            //insert child
            //insert the data
            foreach (var ck5Item in input.Ck5Material)
            {
                var ck5Material = Mapper.Map<CK5_MATERIAL>(ck5Item);
                ck5Material.PLANT_ID = dbData.SOURCE_PLANT_ID;
                dbData.CK5_MATERIAL.Add(ck5Material);
            }


            _repository.InsertOrUpdate(dbData);


            //workflowhistory
            var inputWorkflowHistory = new CK5WorkflowHistoryInput();
            inputWorkflowHistory.DocumentId = dbData.CK5_ID;
            inputWorkflowHistory.DocumentNumber = dbData.SUBMISSION_NUMBER;
            inputWorkflowHistory.UserId = input.UserId;
            inputWorkflowHistory.UserRole = input.UserRole;
            inputWorkflowHistory.ActionType = Enums.ActionType.Save;

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

                if (!_uomBll.IsUomNameExist(ck5MaterialInput.Uom))
                    messageList.Add("UOM not exist");

                if (!Utils.ConvertHelper.IsNumeric(ck5MaterialInput.Convertion))
                    messageList.Add("Convertion not valid");

                if (!_uomBll.IsUomNameExist(ck5MaterialInput.ConvertedUom))
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
                
                output.ConvertedQty = Convert.ToInt32(output.Qty)*Convert.ToInt32(output.Convertion);

                var dbBrand = _brandRegistrationBll.GetByFaCode(output.Brand);

                output.Hje = dbBrand.HJE_IDR.HasValue ? dbBrand.HJE_IDR.Value : 0;
                output.Tariff = dbBrand.TARIFF.HasValue ? dbBrand.TARIFF.Value : 0;

                output.ExciseValue = output.ConvertedQty*output.Tariff;

            }

            return outputList;
        }

        private void SetChangesHistory(CK5Dto origin, CK5Dto data, int userId)
        {
            var changesData = new Dictionary<string, bool>();

            changesData.Add("KPPBC_CITY", origin.KPPBC_CITY.Equals(data.KPPBC_CITY));
            changesData.Add("REGISTRATION_NUMBER", origin.REGISTRATION_NUMBER == data.REGISTRATION_NUMBER);

            changesData.Add("EX_GOODS_TYPE_ID", origin.EX_GOODS_TYPE_ID.Equals(data.EX_GOODS_TYPE_ID));
            changesData.Add("EX_SETTLEMENT_ID", origin.EX_SETTLEMENT_ID.Equals(data.EX_SETTLEMENT_ID));
            changesData.Add("EX_STATUS_ID", origin.EX_STATUS_ID.Equals(data.EX_STATUS_ID));
            changesData.Add("REQUEST_TYPE_ID", origin.REQUEST_TYPE_ID.Equals(data.REQUEST_TYPE_ID));
            changesData.Add("SOURCE_PLANT_ID", origin.SOURCE_PLANT_ID.Equals(data.SOURCE_PLANT_ID));
            changesData.Add("DEST_PLANT_ID", origin.DEST_PLANT_ID.Equals(data.DEST_PLANT_ID));

            changesData.Add("INVOICE_NUMBER", origin.INVOICE_NUMBER == data.INVOICE_NUMBER);
            changesData.Add("INVOICE_DATE", origin.INVOICE_DATE.Equals(data.INVOICE_DATE));

            changesData.Add("PBCK1_DECREE_ID", origin.PBCK1_DECREE_ID.Equals(data.PBCK1_DECREE_ID));
            changesData.Add("CARRIAGE_METHOD_ID", origin.CARRIAGE_METHOD_ID.Equals(data.CARRIAGE_METHOD_ID));

            changesData.Add("GRAND_TOTAL_EX", origin.GRAND_TOTAL_EX.Equals(data.GRAND_TOTAL_EX));
            changesData.Add("PACKAGE_UOM_ID", origin.PACKAGE_UOM_ID.Equals(data.PACKAGE_UOM_ID));

            foreach (var listChange in changesData)
            {
                if (listChange.Value) continue;
                var changes = new CHANGES_HISTORY();
                changes.FORM_TYPE_ID = Enums.MenuList.CK5;
                changes.FORM_ID = origin.CK5_ID;
                changes.FIELD_NAME = listChange.Key;
                changes.MODIFIED_BY = userId;
                changes.MODIFIED_DATE = DateTime.Now;
                switch (listChange.Key)
                {
                    case "KPPBC_CITY":
                        long city = 0;
                        if (data.KPPBC_CITY.HasValue)
                            city = data.KPPBC_CITY.Value;

                        changes.OLD_VALUE = origin.KppbcCityName;
                        changes.NEW_VALUE = _nppbkcBll.GetCityByNppbkcId(city);
                        break;
                    case "REGISTRATION_NUMBER":
                        changes.OLD_VALUE = origin.REGISTRATION_NUMBER;
                        changes.NEW_VALUE = data.REGISTRATION_NUMBER;
                        break;
                    case "EX_GOODS_TYPE_ID":
                        changes.OLD_VALUE = origin.GoodTypeDesc;
                        changes.NEW_VALUE = _goodTypeBll.GetGoodTypeDescById(data.EX_GOODS_TYPE_ID);
                        break;
                    case "EX_SETTLEMENT_ID":
                        changes.OLD_VALUE = origin.ExSettlementName;
                        changes.NEW_VALUE = _masterDataBll.GetExSettlementsNameById(data.EX_SETTLEMENT_ID);
                        break;
                    case "EX_STATUS_ID":
                        changes.OLD_VALUE = origin.ExStatusName;
                        changes.NEW_VALUE = _masterDataBll.GetExSettlementsNameById(data.EX_STATUS_ID);
                        break;
                    case "REQUEST_TYPE_ID":
                        changes.OLD_VALUE = origin.RequestTypeName;
                        changes.NEW_VALUE = _masterDataBll.GetRequestTypeNameById(data.REQUEST_TYPE_ID);
                        break;
                    case "SOURCE_PLANT_ID":
                        long sourcePlant = 0;
                        if (data.SOURCE_PLANT_ID.HasValue)
                            sourcePlant = data.SOURCE_PLANT_ID.Value;

                        changes.OLD_VALUE = origin.SourcePlantName;
                        changes.NEW_VALUE = _plantBll.GetPlantNameById(sourcePlant);
                        break;
                    case "DEST_PLANT_ID":
                        long destPlant = 0;
                        if (data.DEST_PLANT_ID.HasValue)
                            destPlant = data.DEST_PLANT_ID.Value;

                        changes.OLD_VALUE = origin.DestPlantName;
                        changes.NEW_VALUE = _plantBll.GetPlantNameById(destPlant);
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
                        long pbck1 = 0;
                        if (data.PBCK1_DECREE_ID.HasValue)
                            pbck1 = data.PBCK1_DECREE_ID.Value;

                        changes.OLD_VALUE = origin.PbckNumber;
                        changes.NEW_VALUE = _pbck1Bll.GetPbckNumberById(pbck1);
                        break;

                    case "CARRIAGE_METHOD_ID":
                        changes.OLD_VALUE = origin.CarriageMethodName;
                        changes.NEW_VALUE = _masterDataBll.GetCarriageMethodeNameById(data.CARRIAGE_METHOD_ID);
                        break;

                    case "GRAND_TOTAL_EX":
                        changes.OLD_VALUE = origin.GRAND_TOTAL_EX.ToString();
                        changes.NEW_VALUE = data.GRAND_TOTAL_EX.ToString();
                        break;

                    case "PACKAGE_UOM_ID":
                        changes.OLD_VALUE = origin.PackageUomName;
                        changes.NEW_VALUE = _uomBll.GetUomNameById(data.PACKAGE_UOM_ID);
                        break;


                }
                _changesHistoryBll.AddHistory(changes);
            }
        }

        private void AddWorkflowHistory(CK5WorkflowHistoryInput input)
        {
            var inputWorkflowHistory = new GetByActionAndFormNumberInput();
            inputWorkflowHistory.ActionType = input.ActionType;
            inputWorkflowHistory.FormNumber = input.DocumentNumber;

            WorkflowHistoryDto dbData = null;


            //only save can be update, else insert new one
            if (input.ActionType == Enums.ActionType.Save)
                dbData = _workflowHistoryBll.GetByActionAndFormNumber(inputWorkflowHistory);


            if (dbData == null)
            {
                dbData = new WorkflowHistoryDto()
                {
                    ACTION = input.ActionType,
                    FORM_NUMBER = input.DocumentNumber,
                    FORM_TYPE_ID = Core.Enums.FormType.CK5
                };
            }
            dbData.FORM_ID = input.DocumentId;
            if (!string.IsNullOrEmpty(input.Comment))
                dbData.COMMENT = input.Comment;


            dbData.ACTION_BY = input.UserId;
            dbData.ROLE = input.UserRole;
            dbData.ACTION_DATE = DateTime.Now;

            _workflowHistoryBll.Save(dbData);
        }

        public CK5DetailsOutput GetDetailsCK5(long id)
        {
            var output = new CK5DetailsOutput();

            var dtData = _repository.Get(c => c.CK5_ID == id, null, includeTables).FirstOrDefault();
            if (dtData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

           output.Ck5Dto =  Mapper.Map<CK5Dto>(dtData);

            //details
            output.Ck5MaterialDto = Mapper.Map<List<CK5MaterialDto>>(dtData.CK5_MATERIAL);

            //change history data
            output.ListChangesHistorys = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.CK5,output.Ck5Dto.CK5_ID);

            //workflow history
           
            output.ListWorkflowHistorys = _workflowHistoryBll.GetByFormNumber(dtData.SUBMISSION_NUMBER);

            return output;
        }

        public List<CK5MaterialDto> GetCK5MaterialByCK5Id(long id)
        {
            var result = _repositoryCK5Material.Get(c => c.CK5_ID == id);

            if (result == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return Mapper.Map<List<CK5MaterialDto>>(result);
        }

        public void SubmitDocument(CK5WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw  new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS_ID != Enums.DocumentStatus.Draft)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            dbData.STATUS_ID = Enums.DocumentStatus.WaitingForApproval;

            var inputWorkflowHistory = new CK5WorkflowHistoryInput();
            inputWorkflowHistory.DocumentId = input.DocumentId;
            inputWorkflowHistory.DocumentNumber = dbData.SUBMISSION_NUMBER;
            inputWorkflowHistory.UserId = input.UserId;
            inputWorkflowHistory.UserRole = input.UserRole;
            inputWorkflowHistory.ActionType = Enums.ActionType.Submit;

            AddWorkflowHistory(inputWorkflowHistory);

            _uow.SaveChanges();
        }

        public void ApproveDocument(CK5WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS_ID != Enums.DocumentStatus.WaitingForApproval)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            dbData.STATUS_ID = Enums.DocumentStatus.Approved;
            dbData.APPROVED_BY = input.UserId;
            dbData.APPROVED_DATE = DateTime.Now;

            var inputWorkflowHistory = new CK5WorkflowHistoryInput();
            inputWorkflowHistory.DocumentId = input.DocumentId;
            inputWorkflowHistory.DocumentNumber = dbData.SUBMISSION_NUMBER;
            inputWorkflowHistory.UserId = input.UserId;
            inputWorkflowHistory.UserRole = input.UserRole;
            inputWorkflowHistory.ActionType = Enums.ActionType.Approve;

            AddWorkflowHistory(inputWorkflowHistory);

            _uow.SaveChanges();
        }

        public void RejectDocument(CK5WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS_ID != Enums.DocumentStatus.WaitingForApproval)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            //change back to draft
            dbData.STATUS_ID = Enums.DocumentStatus.Draft;
            

            //todo ask
            dbData.APPROVED_BY = null;
            dbData.APPROVED_DATE = null;

            var inputWorkflowHistory = new CK5WorkflowHistoryInput();
            inputWorkflowHistory.DocumentId = input.DocumentId;
            inputWorkflowHistory.DocumentNumber = dbData.SUBMISSION_NUMBER;
            inputWorkflowHistory.UserId = input.UserId;
            inputWorkflowHistory.UserRole = input.UserRole;
            inputWorkflowHistory.ActionType = Enums.ActionType.Reject;
            inputWorkflowHistory.Comment = input.Comment;

            AddWorkflowHistory(inputWorkflowHistory);

            _uow.SaveChanges();
        }
    }
}
