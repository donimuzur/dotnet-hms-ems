using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.LinqExtensions;
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
        private IGenericRepository<CK5_FILE_UPLOAD> _repositoryCK5FileUpload;

        private IDocumentSequenceNumberBLL _docSeqNumBll;
      
        //private IBrandRegistrationBLL _brandRegistrationBll;
        private IMaterialBLL _materialBll;

        private IUnitOfMeasurementBLL _uomBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IMessageService _messageService;
        private IPrintHistoryBLL _printHistoryBll;
        private IMonthBLL _monthBll;
        private IPOABLL _poaBll;

        private IZaidmExNPPBKCBLL _nppbkcBll;
        private IPlantBLL _plantBll;
        private IPBCK1BLL _pbck1Bll;
        private ICountryBLL _countryBll;
        private IExGroupTypeBLL _goodTypeGroupBLL;
        private IVirtualMappingPlantBLL _virtualMappingBLL;

        private IUserBLL _userBll;

        private string includeTables = "CK5_MATERIAL, PBCK1, UOM, USER, USER1, CK5_FILE_UPLOAD";
        private List<string> _allowedCk5Uom =  new List<string>(new string[] { "KG", "G", "L" });

        public CK5BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;

            _repository = _uow.GetGenericRepository<CK5>();
            _repositoryCK5Material = _uow.GetGenericRepository<CK5_MATERIAL>();
            _repositoryCK5FileUpload = _uow.GetGenericRepository<CK5_FILE_UPLOAD>();
            

            _docSeqNumBll = new DocumentSequenceNumberBLL(_uow, _logger);
          
            //_brandRegistrationBll = new BrandRegistrationBLL(_uow, _logger);
            _uomBll = new UnitOfMeasurementBLL(_uow, _logger);
            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);
            _workflowHistoryBll = new WorkflowHistoryBLL(_uow, _logger);
            _messageService = new MessageService(_logger);

            _printHistoryBll = new PrintHistoryBLL(_uow, _logger);
            _monthBll = new MonthBLL(_uow, _logger);
            _poaBll = new POABLL(_uow, _logger);

            _nppbkcBll = new ZaidmExNPPBKCBLL(_uow,_logger);
            _plantBll = new PlantBLL(_uow, _logger);
            _pbck1Bll = new PBCK1BLL(_uow,_logger);
            _countryBll = new CountryBLL(_uow,_logger);
            _materialBll = new MaterialBLL(_uow, _logger);
            _goodTypeGroupBLL = new ExGroupTypeBLL(_uow, logger);
            _virtualMappingBLL = new VirtualMappingPlantBLL(_uow, _logger);
            _userBll = new UserBLL(_uow, _logger);
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
                queryFilter = queryFilter.And(c => c.STATUS_ID == Enums.DocumentStatus.Completed || c.STATUS_ID == Enums.DocumentStatus.Cancelled);
            else
                queryFilter = queryFilter.And(c => c.CK5_TYPE == input.Ck5Type
                                    && (c.STATUS_ID != Enums.DocumentStatus.Completed && c.STATUS_ID != Enums.DocumentStatus.Cancelled));
                
            
            //Func<IQueryable<CK5>, IOrderedQueryable<CK5>> orderBy = null;
            //if (!string.IsNullOrEmpty(input.SortOrderColumn))
            //{
            //    orderBy = c => c.OrderByDescending(OrderByHelper.GetOrderByFunction<CK5>(input.SortOrderColumn));
            //}
            //default case of ordering
            Func<IQueryable<CK5>, IOrderedQueryable<CK5>> orderByFilter = n => n.OrderByDescending(z => z.CREATED_DATE);
            //Func<IQueryable<CK5>, IOrderedQueryable<CK5>> orderByFilter = n => n.OrderByDescending(z => z.STATUS_ID).ThenBy(z=>z.APPROVED_BY_MANAGER);


            var rc = _repository.Get(queryFilter, orderByFilter, includeTables);
            if (rc == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            var mapResult = Mapper.Map<List<CK5Dto>>(rc.ToList());

            return mapResult;


        }

        private void ValidateCk5(CK5SaveInput input)
        {
            if (input.Ck5Dto.CK5_TYPE == Enums.CK5Type.Export ||
                input.Ck5Dto.CK5_TYPE == Enums.CK5Type.PortToImporter ||
                input.Ck5Dto.CK5_TYPE == Enums.CK5Type.Manual)
                return;
            //if domestic not check quota
            if (input.Ck5Dto.CK5_TYPE == Enums.CK5Type.Domestic)
            {
                if (input.Ck5Dto.SOURCE_PLANT_NPPBKC_ID == input.Ck5Dto.DEST_PLANT_NPPBKC_ID)
                    return;
            }
            

            decimal remainQuota = 0;
            if (Utils.ConvertHelper.IsNumeric(input.Ck5Dto.RemainQuota))    
            {
                remainQuota = Convert.ToDecimal(input.Ck5Dto.RemainQuota);
            }

            if (remainQuota < input.Ck5Dto.GRAND_TOTAL_EX)
                throw new BLLException(ExceptionCodes.BLLExceptions.CK5QuotaExceeded);

        
        }
        public CK5Dto SaveCk5(CK5SaveInput input)
        {
            ValidateCk5(input);

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

                if (input.Ck5Dto.CK5_TYPE == Enums.CK5Type.PortToImporter)
                {
                    if (string.IsNullOrEmpty(input.Ck5Dto.SOURCE_PLANT_ID))
                        input.Ck5Dto.SOURCE_PLANT_ID = string.Empty;
                }

                SetChangesHistory(origin, input.Ck5Dto, input.UserId);

                
                Mapper.Map<CK5Dto, CK5>(input.Ck5Dto, dbData);

                //no change status for edit 2015-07-24
                //dbData.STATUS_ID = Enums.DocumentStatus.Revised;
                dbData.MODIFIED_DATE = DateTime.Now;
                if (dbData.STATUS_ID == Enums.DocumentStatus.Rejected)
                {
                    dbData.STATUS_ID = Enums.DocumentStatus.Draft;
                }
              

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

                inputWorkflowHistory.DocumentId = dbData.CK5_ID;
                inputWorkflowHistory.DocumentNumber = dbData.SUBMISSION_NUMBER;
                inputWorkflowHistory.UserId = input.UserId;
                inputWorkflowHistory.UserRole = input.UserRole;


                AddWorkflowHistory(inputWorkflowHistory);


            }
            else
            {
               dbData =  ProcessInsertCk5(input);
            }

            try
            {
                _uow.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }


            return Mapper.Map<CK5Dto>(dbData);


        }

        private List<CK5MaterialOutput> ValidateCk5Material(List<CK5MaterialInput> inputs, Enums.ExGoodsType groupType)
        {
            var messageList = new List<string>();
            var outputList = new List<CK5MaterialOutput>();

            foreach (var ck5MaterialInput in inputs)
            {
                messageList.Clear();

                //var output = new CK5MaterialOutput();
                var output = Mapper.Map<CK5MaterialOutput>(ck5MaterialInput);

                //change to 
                //zaidm_ex_material
                //where werks and sticker_code
                //validate
                var dbMaterial = _materialBll.GetByPlantIdAndStickerCode(ck5MaterialInput.Plant, ck5MaterialInput.Brand);
                if (dbMaterial == null)
                    messageList.Add("Material Number Not Exist");
                else
                {
                    if (string.IsNullOrEmpty(dbMaterial.EXC_GOOD_TYP))
                        messageList.Add("Material is not Excisable goods");
                    else
                    {
                        var exGroupType = _goodTypeGroupBLL.GetGroupByExGroupType(dbMaterial.EXC_GOOD_TYP);
                        if (exGroupType.EX_GROUP_TYPE_ID != (int)groupType)
                        {
                            messageList.Add("This material good type is not matched");
                        }
                    }
                }
                if (!Utils.ConvertHelper.IsNumeric(ck5MaterialInput.Qty))
                    messageList.Add("Qty not valid");

                if (!_uomBll.IsUomIdExist(ck5MaterialInput.Uom))
                    messageList.Add("UOM not exist");

                if (!Utils.ConvertHelper.IsNumeric(ck5MaterialInput.Convertion))
                    messageList.Add("Convertion not valid");

                if (!_uomBll.IsUomIdExist(ck5MaterialInput.ConvertedUom))
                    messageList.Add("ConvertedUom not valid");
                else
                {
                    var uom = _uomBll.GetById(ck5MaterialInput.ConvertedUom);
                    if (!_allowedCk5Uom.Contains(uom.UOM_ID))
                        messageList.Add("Selected UOM must be in KG / G / L");

                }

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

        private List<CK5MaterialOutput> ValidateCk5Material(List<CK5MaterialInput> inputs)
        {
            var messageList = new List<string>();
            var outputList = new List<CK5MaterialOutput>();

            foreach (var ck5MaterialInput in inputs)
            {
                messageList.Clear();

                //var output = new CK5MaterialOutput();
                var output = Mapper.Map<CK5MaterialOutput>(ck5MaterialInput);

                //change to 
                //zaidm_ex_material
                //where werks and sticker_code
                //validate
                var dbMaterial = _materialBll.GetByPlantIdAndStickerCode(ck5MaterialInput.Plant, ck5MaterialInput.Brand);
                if (dbMaterial == null)
                    messageList.Add("Material Number Not Exist");
                else
                {
                    if (string.IsNullOrEmpty(dbMaterial.EXC_GOOD_TYP))
                        messageList.Add("Material is not Excisable goods");
                    else
                    {
                        var exGroupType = _goodTypeGroupBLL.GetGroupByExGroupType(dbMaterial.EXC_GOOD_TYP);
                        if (exGroupType.EX_GROUP_TYPE_ID != (int) ck5MaterialInput.ExGoodsType)
                        {
                            messageList.Add("This material good type is not matched");
                        }
                    }
                }
                if (!Utils.ConvertHelper.IsNumeric(ck5MaterialInput.Qty))
                    messageList.Add("Qty not valid");

                if (!_uomBll.IsUomIdExist(ck5MaterialInput.Uom))
                    messageList.Add("UOM not exist");

                if (!Utils.ConvertHelper.IsNumeric(ck5MaterialInput.Convertion))
                    messageList.Add("Convertion not valid");

                if (!_uomBll.IsUomIdExist(ck5MaterialInput.ConvertedUom))
                    messageList.Add("ConvertedUom not valid");
                else
                {
                    var uom = _uomBll.GetById(ck5MaterialInput.ConvertedUom);
                    if(!_allowedCk5Uom.Contains(uom.UOM_ID))
                        messageList.Add("Selected UOM must be in KG / G / L");
                    
                }

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

        private CK5MaterialOutput GetAdditionalValueCk5Material(CK5MaterialOutput input)
        {
            //input.ConvertedQty = Convert.ToInt32(input.Qty) * Convert.ToInt32(input.Convertion);
            input.ConvertedQty = Utils.ConvertHelper.GetDecimal(input.Qty) * Utils.ConvertHelper.GetDecimal(input.Convertion);

            input.Convertion = Utils.ConvertHelper.GetDecimal(input.Convertion).ToString();

           

            var dbMaterial = _materialBll.GetByPlantIdAndStickerCode(input.Plant, input.Brand);
            if (dbMaterial == null)
            {
                input.Hje = 0;
                input.Tariff = 0;
                
            }
            else
            {
                input.Hje = dbMaterial.HJE.HasValue ? dbMaterial.HJE.Value : 0;
                input.Tariff = dbMaterial.TARIFF.HasValue ? dbMaterial.TARIFF.Value : 0;
                input.MaterialDesc = dbMaterial.ZAIDM_EX_GOODTYP.EXT_TYP_DESC;

                switch (input.ConvertedUom)
                {
                    case "KG":
                        input.ExciseQty = input.ConvertedQty * 1000;
                        input.ExciseUom = "G";
                        break;
                    case "G":
                        input.ExciseQty = input.ConvertedQty;
                        input.ExciseUom = "G";
                        break;
                    case "L":
                        input.ExciseQty = input.ExciseQty;
                        input.ExciseUom = "L";
                        break;
                }
            }

            input.ExciseValue = input.ConvertedQty * input.Tariff;

            return input;
        }

        public List<CK5MaterialOutput> CK5MaterialProcess(List<CK5MaterialInput> inputs, Enums.ExGoodsType groupType)
        {
            var outputList = ValidateCk5Material(inputs,groupType);

            if (!outputList.All(ck5MaterialOutput => ck5MaterialOutput.IsValid))
                return outputList;

            foreach (var output in outputList)
            {
                var resultValue = GetAdditionalValueCk5Material(output);

                output.ExciseQty = resultValue.ExciseQty;
                output.ExciseUom = resultValue.ExciseUom;
                output.ConvertedQty = resultValue.ConvertedQty;
                output.Hje = resultValue.Hje;
                output.Tariff = resultValue.Tariff;
                output.ExciseValue = resultValue.ExciseValue;
              
            }

            return outputList;
        }

        private void SetChangeHistory(string oldValue, string newValue, string fieldName, string userId, string ck5Id)
        {
            var changes = new CHANGES_HISTORY();
            changes.FORM_TYPE_ID = Enums.MenuList.CK5;
            changes.FORM_ID = ck5Id;
            changes.FIELD_NAME = fieldName;
            changes.MODIFIED_BY = userId;
            changes.MODIFIED_DATE = DateTime.Now;

            changes.OLD_VALUE = oldValue;
            changes.NEW_VALUE = newValue;

            _changesHistoryBll.AddHistory(changes);

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

            changesData.Add("SUBMISSION_DATE", origin.SUBMISSION_DATE == data.SUBMISSION_DATE);

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
                    case "SUBMISSION_DATE":
                        changes.OLD_VALUE = origin.SUBMISSION_DATE != null ? origin.SUBMISSION_DATE.Value.ToString("dd MMM yyyy") : string.Empty;
                        changes.NEW_VALUE = data.SUBMISSION_DATE != null ? data.SUBMISSION_DATE.Value.ToString("dd MMM yyyy") : string.Empty;
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
                case Enums.ActionType.GICreated:
                case Enums.ActionType.GICompleted:
                    GiCreatedDocument(input);
                    break;
                case Enums.ActionType.GRCreated:
                case Enums.ActionType.GRCompleted:
                    GrCreatedDocument(input);
                    break;
                case Enums.ActionType.CancelSAP:
                    CancelSAPDocument(input);
                    break;
                case Enums.ActionType.CancelSTOCreated:
                    CancelSTOCreated(input);
                    break;
            }

            //todo sent mail
            if (isNeedSendNotif)
                SendEmailWorkflow(input);

            
            _uow.SaveChanges();
        }

        private void SendEmailWorkflow(CK5WorkflowDocumentInput input)
        {
        //    //todo: body message from email template
        //    //todo: to = ?
        //    //todo: subject = from email template
        //    var to = "irmansulaeman41@gmail.com";
        //    var subject = "this is subject for " + input.DocumentNumber;
        //    var body = "this is body message for " + input.DocumentNumber;
        //    //var from = "a@gmail.com";

            var ck5Dto = Mapper.Map<CK5Dto>(_repository.Get(c => c.CK5_ID == input.DocumentId).FirstOrDefault());

            var mailProcess = ProsesMailNotificationBody(ck5Dto, input.ActionType);

            _messageService.SendEmailToList(mailProcess.To, mailProcess.Subject, mailProcess.Body, true);

        }

        private MailNotification ProsesMailNotificationBody(CK5Dto ck5Dto, Enums.ActionType actionType)
        {
            var bodyMail = new StringBuilder();
            var rc = new MailNotification();

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

            rc.Subject = "CK-5 " + ck5Dto.SUBMISSION_NUMBER + " is " + EnumHelper.GetDescription(ck5Dto.STATUS_ID);
            bodyMail.Append("Dear Team,<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Kindly be informed, " + rc.Subject + ". <br />");
            bodyMail.AppendLine();
            bodyMail.Append("<table><tr><td>Company Code </td><td>: " + ck5Dto.SOURCE_PLANT_COMPANY_CODE + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>NPPBKC </td><td>: " + ck5Dto.SOURCE_PLANT_NPPBKC_ID + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Document Number</td><td> : " + ck5Dto.SUBMISSION_NUMBER + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Document Type</td><td> : CK-5</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr colspan='2'><td><i>Please click this <a href='" + webRootUrl + "/CK5/Details/" + ck5Dto.CK5_ID + "'>link</a> to show detailed information</i></td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("</table>");
            bodyMail.AppendLine();
            bodyMail.Append("<br />Regards,<br />");
            switch (actionType)
            {
                case Enums.ActionType.Submit:
                    if (ck5Dto.STATUS_ID == Enums.DocumentStatus.WaitingForApproval)
                    {
                        List<POADto> poaList;
                        switch (ck5Dto.CK5_TYPE)
                        {
                            case Enums.CK5Type.Export:
                                poaList = _poaBll.GetPoaByNppbkcId(ck5Dto.SOURCE_PLANT_NPPBKC_ID);
                                break;
                            default:
                                poaList = _poaBll.GetPoaByNppbkcId(ck5Dto.DEST_PLANT_NPPBKC_ID);
                                break;
                        }

                        foreach (var poaDto in poaList)
                        {
                            rc.To.Add(poaDto.POA_EMAIL);
                        }    
                        
                    }
                    else if (ck5Dto.STATUS_ID == Enums.DocumentStatus.WaitingForApprovalManager)
                    {
                        var managerId = _poaBll.GetManagerIdByPoaId(ck5Dto.CREATED_BY);
                        var managerDetail = _userBll.GetUserById(managerId);
                        rc.To.Add(managerDetail.EMAIL);
                    }
                    break;
                case Enums.ActionType.Approve:
                    if (ck5Dto.STATUS_ID == Enums.DocumentStatus.WaitingForApprovalManager)
                    {
                        rc.To.Add(GetManagerEmail(ck5Dto.APPROVED_BY_POA));
                    }
                    else if (ck5Dto.STATUS_ID == Enums.DocumentStatus.WaitingGovApproval)
                    {
                        var poaData = _poaBll.GetById(ck5Dto.CREATED_BY);
                        if (poaData != null)
                        {
                            //creator is poa user
                            rc.To.Add(poaData.POA_EMAIL);
                        }
                        else
                        {
                            //creator is excise executive
                            var userData = _userBll.GetUserById(ck5Dto.CREATED_BY);
                            rc.To.Add(userData.EMAIL);
                        }
                    }
                    break;
                case Enums.ActionType.Reject:
                    //send notification to creator
                    var userDetail = _userBll.GetUserById(ck5Dto.CREATED_BY);
                    rc.To.Add(userDetail.EMAIL);
                    break;
            }
            rc.Body = bodyMail.ToString();
            return rc;
        }

        private string GetManagerEmail(string poaId)
        {
            var managerId = _poaBll.GetManagerIdByPoaId(poaId);
            var managerDetail = _userBll.GetUserById(managerId);
            return managerDetail.EMAIL;
        }

        private void SubmitDocument(CK5WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS_ID != Enums.DocumentStatus.Draft)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            string newValue = "";
            string oldValue = EnumHelper.GetDescription(dbData.STATUS_ID);

            dbData.STATUS_ID = Enums.DocumentStatus.WaitingForApproval;
            
            input.DocumentNumber = dbData.SUBMISSION_NUMBER;

            AddWorkflowHistory(input);

            switch (input.UserRole)
            {
                case Enums.UserRole.User:
                    dbData.STATUS_ID = Enums.DocumentStatus.WaitingForApproval;
                    newValue = EnumHelper.GetDescription(Enums.DocumentStatus.WaitingForApproval);
                    break;
                case Enums.UserRole.POA:
                    dbData.STATUS_ID = Enums.DocumentStatus.WaitingForApprovalManager;
                    newValue = EnumHelper.GetDescription(Enums.DocumentStatus.WaitingForApprovalManager);
                    break;
                default:
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
            }

            //set change history
            SetChangeHistory(oldValue, newValue, "STATUS", input.UserId,dbData.CK5_ID.ToString());


        }

        private void ApproveDocument(CK5WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS_ID != Enums.DocumentStatus.WaitingForApproval &&
                dbData.STATUS_ID != Enums.DocumentStatus.WaitingForApprovalManager)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            string oldValue = EnumHelper.GetDescription(dbData.STATUS_ID);
            string newValue = "";

            if (input.UserRole == Enums.UserRole.POA)
            {
                dbData.STATUS_ID = Enums.DocumentStatus.WaitingForApprovalManager;
                dbData.APPROVED_BY_POA = input.UserId;
                dbData.APPROVED_DATE_POA = DateTime.Now;
                newValue = EnumHelper.GetDescription(Enums.DocumentStatus.WaitingForApprovalManager);
            }
            else if (input.UserRole == Enums.UserRole.Manager)
            {
                dbData.STATUS_ID = Enums.DocumentStatus.WaitingGovApproval;
                dbData.APPROVED_BY_MANAGER = input.UserId;
                dbData.APPROVED_DATE_MANAGER = DateTime.Now;
                newValue = EnumHelper.GetDescription(Enums.DocumentStatus.WaitingGovApproval);
            }


            input.DocumentNumber = dbData.SUBMISSION_NUMBER;

            AddWorkflowHistory(input);

            //set change history
            SetChangeHistory(oldValue, newValue, "STATUS", input.UserId,dbData.CK5_ID.ToString());

        }

        private void RejectDocument(CK5WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS_ID != Enums.DocumentStatus.WaitingForApproval &&
                dbData.STATUS_ID != Enums.DocumentStatus.WaitingForApprovalManager &&
                dbData.STATUS_ID != Enums.DocumentStatus.WaitingGovApproval)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            string oldValue = EnumHelper.GetDescription(dbData.STATUS_ID);
            string newValue = "";

            //change back to draft
            dbData.STATUS_ID = Enums.DocumentStatus.Rejected;
            newValue = EnumHelper.GetDescription(Enums.DocumentStatus.Draft);
          
            input.DocumentNumber = dbData.SUBMISSION_NUMBER;
            //input.ActionType = Enums.ActionType.Reject;
            AddWorkflowHistory(input);

            //set change history
            SetChangeHistory(oldValue, newValue, "STATUS", input.UserId, dbData.CK5_ID.ToString());
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

            string oldValue = EnumHelper.GetDescription(dbData.STATUS_ID);

            string newValue = dbData.CK5_TYPE == Enums.CK5Type.PortToImporter ? 
                EnumHelper.GetDescription(Enums.DocumentStatus.TFPosting) : 
                EnumHelper.GetDescription(Enums.DocumentStatus.CreateSTO);
            
            if (dbData.CK5_TYPE == Enums.CK5Type.Manual)
                newValue = EnumHelper.GetDescription(Enums.DocumentStatus.Completed);

            //set change history
            if (oldValue != newValue)
                SetChangeHistory(oldValue, newValue, "STATUS", input.UserId, dbData.CK5_ID.ToString());

            if (dbData.CK5_TYPE == Enums.CK5Type.PortToImporter)
            {
                dbData.STATUS_ID = Enums.DocumentStatus.TFPosting;
            }
            else
            {
                dbData.STATUS_ID = dbData.CK5_TYPE == Enums.CK5Type.Manual
                ? Enums.DocumentStatus.Completed
                : Enums.DocumentStatus.CreateSTO;
            }
            

            
            oldValue = dbData.REGISTRATION_NUMBER;
            newValue = input.AdditionalDocumentData.RegistrationNumber;
            //set change history
            if (oldValue != newValue)
                SetChangeHistory(oldValue, newValue, "REGISTRATION_NUMBER", input.UserId, dbData.CK5_ID.ToString());
            dbData.REGISTRATION_NUMBER = input.AdditionalDocumentData.RegistrationNumber;

            oldValue = dbData.REGISTRATION_DATE.HasValue ? dbData.REGISTRATION_DATE.Value.ToString("dd MMM yyyy") : string.Empty;
            newValue = input.AdditionalDocumentData.RegistrationDate.ToString("dd MMM yyyy");
           
            //set change history
            if (oldValue != newValue)
                SetChangeHistory(oldValue, newValue, "REGISTRATION_DATE", input.UserId, dbData.CK5_ID.ToString());

            dbData.REGISTRATION_DATE = input.AdditionalDocumentData.RegistrationDate;
           
            dbData.CK5_FILE_UPLOAD = Mapper.Map<List<CK5_FILE_UPLOAD>>(input.AdditionalDocumentData.Ck5FileUploadList);

           
            input.DocumentNumber = dbData.SUBMISSION_NUMBER;

            AddWorkflowHistory(input);

           

        }

        private void DeleteCk5FileUploadByCk5Id(long ck5Id)
        {
            var dbData = _repositoryCK5FileUpload.Get(c => c.CK5_ID == ck5Id);
            foreach (var ck5FileUpload in dbData)
            {
                _repositoryCK5FileUpload.Delete(ck5FileUpload);
            }

        }
        public void GovApproveDocumentRollback(CK5WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

         
            dbData.STATUS_ID = Enums.DocumentStatus.WaitingGovApproval;
            dbData.REGISTRATION_NUMBER = string.Empty;

            dbData.REGISTRATION_DATE = null;

            //dbData.CK5_FILE_UPLOAD = Mapper.Map<List<CK5_FILE_UPLOAD>>(input.AdditionalDocumentData.Ck5FileUploadList);
           // dbData.CK5_FILE_UPLOAD = null;
            foreach (var ck5FileUpload in dbData.CK5_FILE_UPLOAD.ToList())
            {
                _repositoryCK5FileUpload.Delete(ck5FileUpload);
            }

            var inputHistory = new GetByActionAndFormNumberInput();
            inputHistory.FormNumber = dbData.SUBMISSION_NUMBER;
            inputHistory.ActionType = Enums.ActionType.GovApprove;

            _workflowHistoryBll.DeleteByActionAndFormNumber(inputHistory);
           
            //todo delete changehistory
            _changesHistoryBll.DeleteByFormIdAndNewValue(dbData.CK5_ID.ToString(), EnumHelper.GetDescription(Enums.ActionType.GovApprove));

            _uow.SaveChanges();

        }

        private void GovRejectedDocument(CK5WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS_ID != Enums.DocumentStatus.WaitingGovApproval)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            string oldValue = EnumHelper.GetDescription(dbData.STATUS_ID);
            string newValue = EnumHelper.GetDescription(Enums.DocumentStatus.Completed); ;
            //set change history
            if (oldValue != newValue)
                SetChangeHistory(oldValue, newValue, "STATUS", input.UserId, dbData.CK5_ID.ToString());

            dbData.STATUS_ID = Enums.DocumentStatus.Completed;
         
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

            string oldValue = EnumHelper.GetDescription(dbData.STATUS_ID);
            string newValue = EnumHelper.GetDescription(Enums.DocumentStatus.Completed); ;
            //set change history
            if (oldValue != newValue)
                SetChangeHistory(oldValue, newValue, "STATUS", input.UserId, dbData.CK5_ID.ToString());
            
            dbData.STATUS_ID = Enums.DocumentStatus.Completed;
            
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

            string oldValue = EnumHelper.GetDescription(dbData.STATUS_ID);
            string newValue = EnumHelper.GetDescription(Enums.DocumentStatus.Cancelled); ;
            //set change history
            if (oldValue != newValue)
                SetChangeHistory(oldValue, newValue, "STATUS", input.UserId, dbData.CK5_ID.ToString());


            dbData.STATUS_ID = Enums.DocumentStatus.Cancelled;


            input.DocumentNumber = dbData.SUBMISSION_NUMBER;

            AddWorkflowHistory(input);
        }

        private void GiCreatedDocument(CK5WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS_ID != Enums.DocumentStatus.GICreated &&
                dbData.STATUS_ID != Enums.DocumentStatus.GICompleted)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            string oldValue = dbData.SEALING_NOTIF_NUMBER;
            string newValue = input.SealingNumber;
            //set change history
            if (oldValue != newValue)
                SetChangeHistory(oldValue, newValue, "SEALING_NOTIF_NUMBER", input.UserId, dbData.CK5_ID.ToString());
            dbData.SEALING_NOTIF_NUMBER = input.SealingNumber;

            oldValue = dbData.SEALING_NOTIF_DATE.HasValue ? dbData.SEALING_NOTIF_DATE.Value.ToString("dd MMM yyyy") : string.Empty;
            newValue = input.SealingDate.HasValue ? input.SealingDate.Value.ToString("dd MMM yyyy") : string.Empty;
            //set change history
            if (oldValue != newValue)
                SetChangeHistory(oldValue, newValue, "SEALING_NOTIF_NUMBER", input.UserId, dbData.CK5_ID.ToString());
            dbData.SEALING_NOTIF_DATE = input.SealingDate;

            input.DocumentNumber = dbData.SUBMISSION_NUMBER;

            AddWorkflowHistory(input);
        }

        private void GrCreatedDocument(CK5WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS_ID != Enums.DocumentStatus.GRCreated &&
                dbData.STATUS_ID != Enums.DocumentStatus.GRCompleted)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            string oldValue = dbData.SEALING_NOTIF_NUMBER;
            string newValue = input.SealingNumber;
            //set change history
            if (oldValue != newValue)
                SetChangeHistory(oldValue, newValue, "SEALING_NOTIF_NUMBER", input.UserId, dbData.CK5_ID.ToString());
            dbData.SEALING_NOTIF_NUMBER = input.SealingNumber;

            oldValue = dbData.SEALING_NOTIF_DATE.HasValue ? dbData.SEALING_NOTIF_DATE.Value.ToString("dd MMM yyyy") : string.Empty;
            newValue = input.SealingDate.HasValue ? input.SealingDate.Value.ToString("dd MMM yyyy") : string.Empty;
            //set change history
            if (oldValue != newValue)
                SetChangeHistory(oldValue, newValue, "SEALING_NOTIF_NUMBER", input.UserId, dbData.CK5_ID.ToString());
            dbData.SEALING_NOTIF_DATE = input.SealingDate;

            oldValue = dbData.UNSEALING_NOTIF_NUMBER;
            newValue = input.UnSealingNumber;

            //string oldValue = dbData.UNSEALING_NOTIF_NUMBER;
            //string newValue = input.UnSealingNumber;
            //set change history
            if (oldValue != newValue)
                SetChangeHistory(oldValue, newValue, "UNSEALING_NOTIF_NUMBER", input.UserId, dbData.CK5_ID.ToString());
            dbData.UNSEALING_NOTIF_NUMBER = input.UnSealingNumber;

            oldValue = dbData.UNSEALING_NOTIF_DATE.HasValue ? dbData.UNSEALING_NOTIF_DATE.Value.ToString("dd MMM yyyy") : string.Empty;
            newValue = input.UnSealingDate.HasValue ? input.UnSealingDate.Value.ToString("dd MMM yyyy") : string.Empty;
            //set change history
            if (oldValue != newValue)
                SetChangeHistory(oldValue, newValue, "UNSEALING_NOTIF_DATE", input.UserId, dbData.CK5_ID.ToString());
            dbData.UNSEALING_NOTIF_DATE = input.UnSealingDate;

            if (!string.IsNullOrEmpty(dbData.DN_NUMBER))
            {
                if (!string.IsNullOrEmpty(dbData.SEALING_NOTIF_NUMBER)
                    && !string.IsNullOrEmpty(dbData.UNSEALING_NOTIF_NUMBER)
                    && dbData.SEALING_NOTIF_DATE.HasValue
                    && dbData.UNSEALING_NOTIF_DATE.HasValue)
                {

                    oldValue = EnumHelper.GetDescription(dbData.STATUS_ID);
                    newValue = EnumHelper.GetDescription(Enums.DocumentStatus.Completed);
                    //set change history
                    SetChangeHistory(oldValue, newValue, "STATUS", input.UserId, dbData.CK5_ID.ToString());

                    dbData.STATUS_ID = Enums.DocumentStatus.Completed;
                }
            }
            input.DocumentNumber = dbData.SUBMISSION_NUMBER;

            AddWorkflowHistory(input);
        }

        public void CancelSTOCreatedRollback(CK5WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);


            dbData.STATUS_ID = Enums.DocumentStatus.CreateSTO;
            
            var inputHistory = new GetByActionAndFormNumberInput();
            inputHistory.FormNumber = dbData.SUBMISSION_NUMBER;
            inputHistory.ActionType = Enums.ActionType.CancelSTOCreated;

            _workflowHistoryBll.DeleteByActionAndFormNumber(inputHistory);

            //todo delete changehistory
            _changesHistoryBll.DeleteByFormIdAndNewValue(dbData.CK5_ID.ToString(), EnumHelper.GetDescription(Enums.ActionType.CancelSTOCreated));

            _uow.SaveChanges();

        }

        private void CancelSTOCreated(CK5WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS_ID < Enums.DocumentStatus.CreateSTO)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
            
            if (!string.IsNullOrEmpty(dbData.DN_NUMBER))
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
            
            string oldValue = EnumHelper.GetDescription(dbData.STATUS_ID);
            string newValue = EnumHelper.GetDescription(Enums.DocumentStatus.Cancelled); ;
            //set change history
            if (oldValue != newValue)
                SetChangeHistory(oldValue, newValue, "STATUS", input.UserId, dbData.CK5_ID.ToString());


            dbData.STATUS_ID = Enums.DocumentStatus.Cancelled;

            input.DocumentNumber = dbData.SUBMISSION_NUMBER;

            AddWorkflowHistory(input);
        }

        private void CancelSAPDocument(CK5WorkflowDocumentInput input)
        {
            var dbData = _repository.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS_ID < Enums.DocumentStatus.CreateSTO)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
          
            if (dbData.GI_DATE.HasValue || dbData.GR_DATE.HasValue)
                throw new BLLException(ExceptionCodes.BLLExceptions.ReversalManualSAP);

            string oldValue = EnumHelper.GetDescription(dbData.STATUS_ID);
            string newValue = EnumHelper.GetDescription(Enums.DocumentStatus.Cancelled); ;
            //set change history
            if (oldValue != newValue)
                SetChangeHistory(oldValue, newValue, "STATUS", input.UserId, dbData.CK5_ID.ToString());


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


            //queryFilter = queryFilter.And(c => c.CK5_TYPE == input.Ck5Type);

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

            

            result.ReportDetails.OfficeCode = dtData.SOURCE_PLANT_NPPBKC_ID;

            if (string.IsNullOrEmpty(dtData.SOURCE_PLANT_NPPBKC_ID))
            {
                result.ReportDetails.OfficeCode = string.Empty;
                result.ReportDetails.SourceOfficeCode = string.Empty;
            }
            else
            {
                if (dtData.SOURCE_PLANT_NPPBKC_ID.Length >= 4)
                {
                    result.ReportDetails.OfficeCode = "00" + dtData.SOURCE_PLANT_NPPBKC_ID.Substring(0, 4);
                    result.ReportDetails.SourceOfficeCode = dtData.SOURCE_PLANT_NPPBKC_ID.Substring(0, 4);
                }
            }
            result.ReportDetails.SourcePlantName = dtData.SOURCE_PLANT_COMPANY_NAME;
          


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
                result.ReportDetails.SubmissionDate = DateReportDisplayString(dtData.SUBMISSION_DATE.Value, false);
            //if (dtData.REGISTRATION_DATE.HasValue)
            //    result.ReportDetails.RegistrationDate = DateReportDisplayString(dtData.REGISTRATION_DATE.Value, false);

            result.ReportDetails.RegistrationNumber = "";
            result.ReportDetails.RegistrationDate = "";

            if (dtData.PBCK1 != null)
            {
                if (dtData.PBCK1.DECREE_DATE.HasValue)
                {
                    if (dtData.CK5_TYPE == Enums.CK5Type.PortToImporter || dtData.CK5_TYPE == Enums.CK5Type.ImporterToPlant)
                        result.ReportDetails.FacilityDate = DateReportDisplayString(dtData.PBCK1.DECREE_DATE.Value,
                            false);
                    else
                        result.ReportDetails.FacilityDate = dtData.PBCK1.DECREE_DATE.Value.ToString("dd/MM/yyyy");
                }
            }
            if (dtData.INVOICE_DATE.HasValue)
                result.ReportDetails.InvoiceDate = DateReportDisplayString(dtData.INVOICE_DATE.Value, false);

            result.ReportDetails.PrintDate = DateReportDisplayString(DateTime.Now, false);
            result.ReportDetails.MonthYear = DateReportDisplayString(DateTime.Now, true);

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
                result.ReportDetails.PoaCity = dtData.KPPBC_CITY.ToUpperInvariant();
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
                result.ReportDetails.DestPlantName = dtData.DEST_PLANT_COMPANY_NAME;
                result.ReportDetails.DestPlantAddress = dtData.DEST_PLANT_ADDRESS;
                result.ReportDetails.DestOfficeName = dtData.DEST_PLANT_COMPANY_NAME;


                result.ReportDetails.DestOfficeCode = dtData.DEST_PLANT_NPPBKC_ID;
                if (dtData.DEST_PLANT_NPPBKC_ID.Length >= 4)
                    result.ReportDetails.DestOfficeCode = "00" + dtData.DEST_PLANT_NPPBKC_ID.Substring(0,4);

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

            result.ReportDetails.CK5Type = dtData.CK5_TYPE.ToString();
            //get material desc

            return result;
            //return Mapper.Map<CK5ReportDto>(dtData);
        }

        #endregion

        private string DateReportDisplayString(DateTime dt, bool isMonthYear)
        {
            var monthPeriodFrom = _monthBll.GetMonth(dt.Month);
            if (isMonthYear) return monthPeriodFrom.MONTH_NAME_IND + " " + dt.ToString("yyyy");
            return dt.ToString("dd") + " " + monthPeriodFrom.MONTH_NAME_IND +
                                   " " + dt.ToString("yyyy");
        }

       
        private List<CK5FileUploadDocumentsOutput> ValidateCk5UploadFileDocuments(List<CK5UploadFileDocumentsInput> inputs)
        {
            var messageList = new List<string>();
            var outputList = new List<CK5FileUploadDocumentsOutput>();

            foreach (var ck5UploadFileDocuments in inputs)
            {
                messageList.Clear();

                //var output = new CK5MaterialOutput();
                var output = Mapper.Map<CK5FileUploadDocumentsOutput>(ck5UploadFileDocuments);

                //ck5type
                if (!ConvertHelper.IsNumeric(ck5UploadFileDocuments.Ck5Type))
                    messageList.Add("Type not valid");
                else
                {
                    if (typeof(Enums.CK5Type).IsEnumDefined(Convert.ToInt32(ck5UploadFileDocuments.Ck5Type)))
                        output.CK5_TYPE =
                            (Enums.CK5Type)
                                Enum.Parse(typeof(Enums.CK5Type), ck5UploadFileDocuments.Ck5Type);
                    else
                        messageList.Add("ExGoodType not valid");
                }

                //kppbc city

                var dbNppbkc = _nppbkcBll.GetDetailsByCityName(ck5UploadFileDocuments.KppBcCityName);
                if (dbNppbkc == null)
                    messageList.Add("City Not Exist");
                else
                    output.CE_OFFICE_CODE = dbNppbkc.ZAIDM_EX_KPPBC.KPPBC_ID;
                
                //excise goods type
                if (!ConvertHelper.IsNumeric(ck5UploadFileDocuments.ExGoodType))
                    messageList.Add("ExGoodType not valid");
                else
                {
                    if (typeof(Enums.ExGoodsType).IsEnumDefined(Convert.ToInt32(ck5UploadFileDocuments.ExGoodType)))
                        output.EX_GOODS_TYPE =
                            (Enums.ExGoodsType)
                                Enum.Parse(typeof(Enums.ExGoodsType), ck5UploadFileDocuments.ExGoodType);
                    else
                        messageList.Add("ExGoodType not valid");
                }

                if (!ConvertHelper.IsNumeric(ck5UploadFileDocuments.ExciseSettlement))
                    messageList.Add("ExciseSettlement not valid");
                else
                {
                    if (typeof (Enums.ExciseSettlement).IsEnumDefined(
                            Convert.ToInt32(ck5UploadFileDocuments.ExciseSettlement)))
                        output.EX_SETTLEMENT_ID =
                            (Enums.ExciseSettlement)
                                Enum.Parse(typeof (Enums.ExciseSettlement), ck5UploadFileDocuments.ExciseSettlement);
                    else
                        messageList.Add("ExciseSettlement not valid");
                }

                if (!ConvertHelper.IsNumeric(ck5UploadFileDocuments.ExciseStatus))
                    messageList.Add("ExciseStatus not valid");
                else
                {
                    if (typeof (Enums.ExciseStatus).IsEnumDefined(Convert.ToInt32(ck5UploadFileDocuments.ExciseStatus)))
                        output.EX_STATUS_ID =
                            (Enums.ExciseStatus)
                                Enum.Parse(typeof (Enums.ExciseStatus), ck5UploadFileDocuments.ExciseStatus);
                    else
                        messageList.Add("ExciseStatus not valid");
                }

                if (!ConvertHelper.IsNumeric(ck5UploadFileDocuments.RequestType))
                    messageList.Add("RequestType not valid");
                else
                {

                    if (typeof (Enums.RequestType).IsEnumDefined(Convert.ToInt32(ck5UploadFileDocuments.RequestType)))
                        output.REQUEST_TYPE_ID =
                            (Enums.RequestType)
                                Enum.Parse(typeof (Enums.RequestType), ck5UploadFileDocuments.RequestType);
                    else
                        messageList.Add("ExciseStatus not valid");
                }

                var sourcePlant = _plantBll.GetT001WById(ck5UploadFileDocuments.SourcePlantId);

                if (sourcePlant == null)
                    messageList.Add("Source Plant Not Exist");
                else
                {
                    output.SOURCE_PLANT_ID = ck5UploadFileDocuments.SourcePlantId;
                    output.SOURCE_PLANT_NPWP = sourcePlant.Npwp;
                    output.SOURCE_PLANT_NPPBKC_ID = sourcePlant.NPPBKC_ID;
                    output.SOURCE_PLANT_COMPANY_CODE = sourcePlant.CompanyCode;
                    output.SOURCE_PLANT_COMPANY_NAME = sourcePlant.CompanyName;
                    output.SOURCE_PLANT_ADDRESS = sourcePlant.CompanyAddress;
                    output.SOURCE_PLANT_KPPBC_NAME_OFFICE = sourcePlant.KppbcCity + "-" + sourcePlant.KppbcNo;
                    output.SOURCE_PLANT_NAME = sourcePlant.NAME1;
                }

                var destPlant = _plantBll.GetT001WById(ck5UploadFileDocuments.DestPlantId);

                if (destPlant == null)
                    messageList.Add("Destination Plant Not Exist");
                else
                {
                    output.DEST_PLANT_ID = ck5UploadFileDocuments.DestPlantId;
                    output.DEST_PLANT_NPWP = destPlant.Npwp;
                    output.DEST_PLANT_NPPBKC_ID = destPlant.NPPBKC_ID;
                    output.DEST_PLANT_COMPANY_CODE = destPlant.CompanyCode;
                    output.DEST_PLANT_COMPANY_NAME = destPlant.CompanyName;
                    output.DEST_PLANT_ADDRESS = destPlant.CompanyAddress;
                    output.DEST_PLANT_KPPBC_NAME_OFFICE = destPlant.KppbcCity + "-" + destPlant.KppbcNo;
                    output.DEST_PLANT_NAME = destPlant.NAME1;
                }

                if (!string.IsNullOrEmpty(ck5UploadFileDocuments.InvoiceDateDisplay))
                {
                    var dateTime = Utils.ConvertHelper.StringToDateTimeCk5FileDocuments(ck5UploadFileDocuments.InvoiceDateDisplay);
                    if (dateTime != null)
                        output.INVOICE_DATE = dateTime;
                    else
                        messageList.Add("Invoice Date not valid");
                }
                if (!string.IsNullOrEmpty(ck5UploadFileDocuments.PbckDecreeNumber))
                {
                    var pbck1 = _pbck1Bll.GetByDocumentNumber(ck5UploadFileDocuments.PbckDecreeNumber);
                    if (pbck1 == null)
                        messageList.Add("PbckDecreeNumber Not Exist");
                    else
                    {
                        output.PBCK1_DECREE_ID = pbck1.Pbck1Id;
                    }
                }

                if (!string.IsNullOrEmpty(ck5UploadFileDocuments.CarriageMethod))
                {
                    if (!ConvertHelper.IsNumeric(ck5UploadFileDocuments.CarriageMethod))
                        messageList.Add("CarriageMethod not valid");
                    else
                    {
                        if (typeof (Enums.CarriageMethod).IsEnumDefined(
                                Convert.ToInt32(ck5UploadFileDocuments.CarriageMethod)))
                            output.CARRIAGE_METHOD_ID =
                                (Enums.CarriageMethod)
                                    Enum.Parse(typeof (Enums.CarriageMethod), ck5UploadFileDocuments.CarriageMethod);
                        else
                            messageList.Add("CarriageMethod not valid");
                    }
                }
                if (!string.IsNullOrEmpty(ck5UploadFileDocuments.PackageUomName))
                {

                    if (!_uomBll.IsUomIdExist(ck5UploadFileDocuments.PackageUomName))
                        messageList.Add("UOM not exist");
                }

                //exsport type
                if (ck5UploadFileDocuments.Ck5Type == Convert.ToInt32(Enums.CK5Type.Export).ToString())
                {
                    if (string.IsNullOrEmpty(ck5UploadFileDocuments.LOADING_PORT))
                        messageList.Add("Loading port empty");
                    if (string.IsNullOrEmpty(ck5UploadFileDocuments.LOADING_PORT_ID))
                        messageList.Add("Loading port Id empty");
                    if (string.IsNullOrEmpty(ck5UploadFileDocuments.LOADING_PORT_NAME))
                        messageList.Add("Loading port Name empty");
                    if (string.IsNullOrEmpty(ck5UploadFileDocuments.FINAL_PORT))
                        messageList.Add("Final port empty");
                    if (string.IsNullOrEmpty(ck5UploadFileDocuments.FINAL_PORT_ID))
                        messageList.Add("Final port Id empty");
                    if (string.IsNullOrEmpty(ck5UploadFileDocuments.FINAL_PORT_NAME))
                        messageList.Add("Final port name empty");

                    //check country code
                    var country = _countryBll.GetCountryByCode(ck5UploadFileDocuments.DEST_COUNTRY_CODE);
                    if (country == null)
                        messageList.Add("Country not exist");
                    else
                    {
                        output.DEST_COUNTRY_NAME = country.COUNTRY_NAME;
                    }
                }

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


        public List<CK5FileUploadDocumentsOutput> CK5UploadFileDocumentsProcess(List<CK5UploadFileDocumentsInput> inputs)
        {
            var lisCk5Material = new List<CK5MaterialInput>();

            
            foreach (var ck5UploadFileDocumentsInput in inputs)
            {
                var inputCk5Material = new CK5MaterialInput();
                inputCk5Material.Plant = ck5UploadFileDocumentsInput.SourcePlantId;
                inputCk5Material.Brand = ck5UploadFileDocumentsInput.MatNumber;
                inputCk5Material.Qty = ck5UploadFileDocumentsInput.Qty;
                inputCk5Material.Uom = ck5UploadFileDocumentsInput.UomMaterial;
                inputCk5Material.Convertion = ck5UploadFileDocumentsInput.Convertion;
                inputCk5Material.ConvertedUom = ck5UploadFileDocumentsInput.ConvertedUom;
                inputCk5Material.UsdValue = ck5UploadFileDocumentsInput.UsdValue;
                inputCk5Material.Note = ck5UploadFileDocumentsInput.Note;
                
                inputCk5Material.ExGoodsType = ck5UploadFileDocumentsInput.EX_GOODS_TYPE;
                lisCk5Material.Add(inputCk5Material);

            }

            var outputListCk5Material = ValidateCk5Material(lisCk5Material);



            var outputList = ValidateCk5UploadFileDocuments(inputs);

            for (int i = 0; i < outputList.Count; i++)
            {
                outputList[i].Message += outputListCk5Material[i].Message;
            }

            if (!outputList.All(c => c.IsValid))
                return outputList;


            if (!outputListCk5Material.All(ck5MaterialOutput => ck5MaterialOutput.IsValid))
                return outputList;

            foreach (var output in outputListCk5Material)
            {
                var resultValue = GetAdditionalValueCk5Material(output);

                output.ConvertedQty = resultValue.ConvertedQty;
                output.Hje = resultValue.Hje;
                output.Tariff = resultValue.Tariff;
                output.ExciseValue = resultValue.ExciseValue;

            }

            foreach (var ck5UploadFileDocumentsInput in outputList)
            {
                var outputCk5Material = new CK5MaterialOutput();
                outputCk5Material.Plant = ck5UploadFileDocumentsInput.SourcePlantId;
                outputCk5Material.Brand = ck5UploadFileDocumentsInput.MatNumber;
                outputCk5Material.Qty = ck5UploadFileDocumentsInput.Qty;
                outputCk5Material.Uom = ck5UploadFileDocumentsInput.UomMaterial;
                outputCk5Material.Convertion = ck5UploadFileDocumentsInput.Convertion;
                outputCk5Material.ConvertedUom = ck5UploadFileDocumentsInput.ConvertedUom;
                outputCk5Material.UsdValue = ck5UploadFileDocumentsInput.UsdValue;
                outputCk5Material.Note = ck5UploadFileDocumentsInput.Note;

                var resultValue = GetAdditionalValueCk5Material(outputCk5Material);

                ck5UploadFileDocumentsInput.ConvertedQty = resultValue.ConvertedQty;
                ck5UploadFileDocumentsInput.Hje = resultValue.Hje;
                ck5UploadFileDocumentsInput.Tariff = resultValue.Tariff;
                ck5UploadFileDocumentsInput.ExciseValue = resultValue.ExciseValue;

            }

            return outputList;
        }

        private CK5 ProcessInsertCk5(CK5SaveInput input)
        {
            //workflowhistory
            var inputWorkflowHistory = new CK5WorkflowHistoryInput();

            CK5 dbData = null;

            ////create new ck5 documents
            //var generateNumberInput = new GenerateDocNumberInput()
            //{
            //    Year = DateTime.Now.Year,
            //    Month = DateTime.Now.Month,
            //    NppbkcId = input.Ck5Dto.SOURCE_PLANT_NPPBKC_ID,
            //    FormType = Enums.FormType.CK5
            //};

            input.Ck5Dto.SUBMISSION_NUMBER = _docSeqNumBll.GenerateNumberByFormType(Enums.FormType.CK5);
            if (!input.Ck5Dto.SUBMISSION_DATE.HasValue) {
                input.Ck5Dto.SUBMISSION_DATE = DateTime.Now;
            }
            
            input.Ck5Dto.STATUS_ID = Enums.DocumentStatus.Draft;
            input.Ck5Dto.CREATED_DATE = DateTime.Now;
            input.Ck5Dto.CREATED_BY = input.UserId;

            if (input.Ck5Dto.CK5_TYPE == Enums.CK5Type.PortToImporter)
            {
                if (string.IsNullOrEmpty(input.Ck5Dto.SOURCE_PLANT_ID))
                    input.Ck5Dto.SOURCE_PLANT_ID = string.Empty;
            }

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
            
            inputWorkflowHistory.DocumentId = dbData.CK5_ID;
            inputWorkflowHistory.DocumentNumber = dbData.SUBMISSION_NUMBER;
            inputWorkflowHistory.UserId = input.UserId;
            inputWorkflowHistory.UserRole = input.UserRole;

            AddWorkflowHistory(inputWorkflowHistory);

            return dbData;
        }

       
        public void InsertListCk5(CK5SaveListInput input)
        {
            List<CK5MaterialDto> listCk5Material = null;
            string docSeqNumber = "-1";
            bool isFirstTime = true;

            CK5Dto ck5Dto = null;
            CK5SaveInput inputSave = null;

            //order first
            input.ListCk5UploadDocumentDto = input.ListCk5UploadDocumentDto.OrderBy(x => x.DocSeqNumber).ToList();

            foreach (var ck5FileDocumentDto in input.ListCk5UploadDocumentDto)
            {
                if (docSeqNumber != ck5FileDocumentDto.DocSeqNumber)
                {
                    docSeqNumber = ck5FileDocumentDto.DocSeqNumber;

                    if (!isFirstTime)
                    {
                        inputSave = new CK5SaveInput();
                        inputSave.Ck5Dto = ck5Dto;
                        inputSave.Ck5Material = listCk5Material;
                        inputSave.UserId = input.UserId;
                        inputSave.UserRole = input.UserRole;

                        ProcessInsertCk5(inputSave);
                    }

                    //new record
                    listCk5Material = new List<CK5MaterialDto>();

                    ck5Dto = Mapper.Map<CK5Dto>(ck5FileDocumentDto);

                    var ck5Material = Mapper.Map<CK5MaterialDto>(ck5FileDocumentDto);

                    listCk5Material.Add(ck5Material);


                }
                else
                {
                    var ck5Material = Mapper.Map<CK5MaterialDto>(ck5FileDocumentDto);

                    if (listCk5Material == null)
                        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                    listCk5Material.Add(ck5Material);
                }
               
                isFirstTime = false;

            }


            //save the lastone
            inputSave = new CK5SaveInput();
            inputSave.Ck5Dto = ck5Dto;
            inputSave.Ck5Material = listCk5Material;
            inputSave.UserId = input.UserId;
            inputSave.UserRole = input.UserRole;
            ProcessInsertCk5(inputSave);

            try
            {
                _uow.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

            //return Mapper.Map<CK5Dto>(dbData);


        }

        public CK5XmlDto GetCk5ForXmlById(long id)
        {
            //includeTables = "ZAIDM_EX_GOODTYP,EX_SETTLEMENT,EX_STATUS,REQUEST_TYPE,PBCK1,CARRIAGE_METHOD,COUNTRY, UOM";
            var dtData = _repository.Get(c => c.CK5_ID == id, null, includeTables).FirstOrDefault();
            if (dtData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            var dataXmlDto = Mapper.Map<CK5XmlDto>(dtData);

            if (dataXmlDto.CK5_TYPE == Enums.CK5Type.ImporterToPlant) {
                var plantMap = _virtualMappingBLL.GetByCompany(dataXmlDto.DEST_PLANT_COMPANY_CODE);
                
                dataXmlDto.SOURCE_PLANT_ID = plantMap.IMPORT_PLANT_ID;
            }

            if (dataXmlDto.CK5_TYPE == Enums.CK5Type.Export) {
                var plantMap = _virtualMappingBLL.GetByCompany(dataXmlDto.SOURCE_PLANT_COMPANY_CODE);

                dataXmlDto.DEST_PLANT_ID = plantMap.EXPORT_PLANT_ID;
            }

            if (dataXmlDto.CK5_TYPE == Enums.CK5Type.PortToImporter) {
                var plantMap = _virtualMappingBLL.GetByCompany(dataXmlDto.DEST_PLANT_COMPANY_CODE);

                dataXmlDto.SOURCE_PLANT_ID = plantMap.IMPORT_PLANT_ID;
                dataXmlDto.DEST_PLANT_ID = plantMap.IMPORT_PLANT_ID;
            }
            
            return dataXmlDto;

        }

        public GetQuotaAndRemainOutput GetQuotaRemainAndDatePbck1(int pbckId, int exgrouptype)
        {
            var output = new GetQuotaAndRemainOutput();

            var pbck1 = _pbck1Bll.GetById(pbckId);
            if (pbck1 == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.Pbck1PeriodNotFound);

            output.Pbck1DecreeDate = "";
            if (pbck1.DecreeDate.HasValue)
                output.Pbck1DecreeDate = pbck1.DecreeDate.Value.ToString("dd/MM/yyyy");

            output.PbckUom = pbck1.RequestQtyUomId;

            if (pbck1.Pbck1Type == Enums.PBCK1Type.New)
            {
                output.QtyApprovedPbck1 = pbck1.QtyApproved.HasValue ? pbck1.QtyApproved.Value : 0;
            }

            else
            {
                decimal remainQuota = 0;
                //additional
                //
                var parentIdPbck1 = pbck1.Pbck1Reference;
                if (!pbck1.Pbck1Reference.HasValue)
                    throw new BLLException(ExceptionCodes.BLLExceptions.Pbck1RefNull);

                var listPbck1 = _pbck1Bll.GetAllPbck1ByPbck1Ref(Convert.ToInt32(parentIdPbck1));

                foreach (var pbck1Dto in listPbck1)
                {
                    if (pbck1Dto.QtyApproved.HasValue)
                        remainQuota += pbck1Dto.QtyApproved.Value;
                }

                output.QtyApprovedPbck1 = remainQuota;
            }

            var periodEnd = pbck1.PeriodTo.Value.AddDays(1);

            ////get ck5 
            //var lisCk5 =
            //    _repository.Get(c => c.STATUS_ID != Enums.DocumentStatus.Cancelled && c.SOURCE_PLANT_ID == pbck1.SupplierPlantWerks 
            //        && c.DEST_PLANT_NPPBKC_ID == pbck1.NppbkcId
            //        && c.SUBMISSION_DATE >= pbck1.PeriodFrom && c.SUBMISSION_DATE <= periodEnd
            //        && c.EX_GOODS_TYPE == (Enums.ExGoodsType) exgrouptype);

            //decimal qtyCk5 = 0;

            //foreach (var ck5 in lisCk5)
            //{
            //    if (ck5.GRAND_TOTAL_EX.HasValue)
            //        qtyCk5 += ck5.GRAND_TOTAL_EX.Value;
            //}

            output.QtyCk5 = GetQuotaCk5(pbck1.SupplierPlantWerks,pbck1.SupplierNppbkcId, pbck1.NppbkcId, pbck1.PeriodFrom, periodEnd, (Enums.ExGoodsType)exgrouptype);

            return output;
        }


        public GetQuotaAndRemainOutput GetQuotaRemainAndDatePbck1ByCk5Id(long ck5Id)
        {
            var output = new GetQuotaAndRemainOutput();

            var ck5DbData = _repository.GetByID(ck5Id);
            var goodTypeGroups = _goodTypeGroupBLL.GetById((int)ck5DbData.EX_GOODS_TYPE).EX_GROUP_TYPE_DETAILS.Select(x=> x.GOODTYPE_ID).ToList();
            if (ck5DbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (ck5DbData.PBCK1_DECREE_ID.HasValue)
            {
                return GetQuotaRemainAndDatePbck1(ck5DbData.PBCK1_DECREE_ID.Value, (int)ck5DbData.EX_GOODS_TYPE);
            }

            else
            {
                var listPbck1 = _pbck1Bll.GetPbck1CompletedDocumentByPlantAndSubmissionDate(ck5DbData.SOURCE_PLANT_ID, ck5DbData.SOURCE_PLANT_NPPBKC_ID,
                    ck5DbData.SUBMISSION_DATE, ck5DbData.DEST_PLANT_NPPBKC_ID, goodTypeGroups);

                output.QtyApprovedPbck1 = 0;

                foreach (var pbck1Dto in listPbck1)
                {
                    if (pbck1Dto.QtyApproved.HasValue)
                        output.QtyApprovedPbck1 += pbck1Dto.QtyApproved.Value;
                }

                if (listPbck1.Count == 0)
                    throw new BLLException(ExceptionCodes.BLLExceptions.Pbck1RefNull);

                var periodStart = listPbck1[0].PeriodFrom;
                var periodEnd = listPbck1[0].PeriodTo.Value.AddDays(1);
                var pbck1npbkc = listPbck1[0].NppbkcId;

                output.PbckUom = listPbck1[0].RequestQtyUomId;

                ////get ck5 
                //var lisCk5 =
                //    _repository.Get(
                //        c =>
                //            c.STATUS_ID != Enums.DocumentStatus.Cancelled &&
                //            c.SOURCE_PLANT_ID == ck5DbData.SOURCE_PLANT_ID
                //             && c.DEST_PLANT_NPPBKC_ID == pbck1npbkc
                //            && c.SUBMISSION_DATE >= periodStart && c.SUBMISSION_DATE <= periodEnd 
                //            && c.EX_GOODS_TYPE == (Enums.ExGoodsType)ck5DbData.EX_GOODS_TYPE);

                //decimal qtyCk5 = 0;

                //foreach (var ck5 in lisCk5)
                //{
                //    if (ck5.GRAND_TOTAL_EX.HasValue)
                //        qtyCk5 += ck5.GRAND_TOTAL_EX.Value;
                //}

                output.QtyCk5 = GetQuotaCk5(ck5DbData.SOURCE_PLANT_ID, ck5DbData.SOURCE_PLANT_NPPBKC_ID, pbck1npbkc, periodStart, periodEnd, ck5DbData.EX_GOODS_TYPE);
            }

            return output;
        }
        
        /// <summary>
        /// called by new document, that don't have a data in database
        /// so return calculate remain here
        /// </summary>
        /// <param name="plantId"></param>
        /// <param name="submissionDate"></param>
        /// <param name="destPlantNppbkcId"></param>
        /// <param name="goodtypegroupid"></param>
        /// <returns></returns>
        public GetQuotaAndRemainOutput GetQuotaRemainAndDatePbck1Item(string plantId,string plantNppbkcId, DateTime submissionDate, string destPlantNppbkcId,int? goodtypegroupid)
        {
            var output = new GetQuotaAndRemainOutput();
          
            var goodtypegroupidval = goodtypegroupid.HasValue ? goodtypegroupid.Value : 0;
            var dbGoodTypeList = _goodTypeGroupBLL.GetById(goodtypegroupidval);
            List<string> goodtypelist = new List<string>();
            if (dbGoodTypeList != null) {
                goodtypelist = _goodTypeGroupBLL.GetById(goodtypegroupidval).EX_GROUP_TYPE_DETAILS.Select(x => x.GOODTYPE_ID).ToList();
            }

            var listPbck1 = _pbck1Bll.GetPbck1CompletedDocumentByPlantAndSubmissionDate(plantId, plantNppbkcId, submissionDate, destPlantNppbkcId, goodtypelist);
            
            
            if (listPbck1.Count == 0)
            {
                //pbck not exist
                output.QtyApprovedPbck1 = 0;
                output.QtyCk5 = 0;
                output.RemainQuota = 0;
                output.PbckUom = "";
            }
            else
            {
                //var pbckgoodtype = listPbck1[0].GoodType;
                //var pbck1GoodTypeExist = grouptype.EX_GROUP_TYPE_DETAILS.Where(x => x.GOODTYPE_ID == pbckgoodtype).Count() > 0;

                
                output.Pbck1Id = listPbck1[0].Pbck1Id;
                output.Pbck1Number = listPbck1[0].Pbck1Number;
                output.Pbck1DecreeDate = listPbck1[0].DecreeDate.HasValue
                    ? listPbck1[0].DecreeDate.Value.ToString("dd/MM/yyyy")
                    : string.Empty;

                output.PbckUom = listPbck1[0].RequestQtyUomId;

                foreach (var pbck1Dto in listPbck1)
                {
                    if (pbck1Dto.QtyApproved.HasValue)
                        output.QtyApprovedPbck1 += pbck1Dto.QtyApproved.Value;
                }

                var periodStart = listPbck1[0].PeriodFrom;
                var periodEnd = listPbck1[0].PeriodTo.Value.AddDays(1);

                var pbck1Npbkc = listPbck1[0].NppbkcId;
                //var suppPlantNppbkcid = listPbck1[0].SupplierNppbkcId;
                ////get ck5 
                //var lisCk5 =
                //    _repository.Get(
                //        c =>
                //            c.STATUS_ID != Enums.DocumentStatus.Cancelled
                //            && c.SOURCE_PLANT_ID == plantId
                //            && c.DEST_PLANT_NPPBKC_ID == pbck1npbkc
                //            && c.SUBMISSION_DATE >= periodStart && c.SUBMISSION_DATE <= periodEnd 
                //          && c.EX_GOODS_TYPE == (Enums.ExGoodsType)goodtypegroupid.Value).ToList();

                //decimal qtyCk5 = 0;

                //foreach (var ck5 in lisCk5)
                //{
                //    if (ck5.GRAND_TOTAL_EX.HasValue)
                //        qtyCk5 += ck5.GRAND_TOTAL_EX.Value;
                //}

                output.QtyCk5 = GetQuotaCk5(plantId, plantNppbkcId, pbck1Npbkc, periodStart, periodEnd, (Enums.ExGoodsType)goodtypegroupidval);

                output.RemainQuota = output.QtyApprovedPbck1 - output.QtyCk5;
                
                

                

            }

            return output;
        }

        

        private decimal GetQuotaCk5(string plantId, string sourceNppbkc, string pbck1Npbkc, DateTime periodStart, DateTime periodEnd, Enums.ExGoodsType goodtypegroupid)
        {
            //get ck5 
            var lisCk5 =
                _repository.Get(
                    c =>
                        c.STATUS_ID != Enums.DocumentStatus.Cancelled
                        && c.SOURCE_PLANT_ID == plantId
                        && c.SOURCE_PLANT_NPPBKC_ID == sourceNppbkc
                        && c.DEST_PLANT_NPPBKC_ID == pbck1Npbkc
                        && c.SUBMISSION_DATE >= periodStart && c.SUBMISSION_DATE <= periodEnd 
                        && c.EX_GOODS_TYPE == goodtypegroupid
                        ).ToList();

            decimal qtyCk5 = 0;

            foreach (var ck5 in lisCk5)
            {
                if (ck5.CK5_TYPE == Enums.CK5Type.Export || ck5.CK5_TYPE == Enums.CK5Type.PortToImporter || ck5.CK5_TYPE == Enums.CK5Type.Manual)
                    continue;
                if (ck5.CK5_TYPE == Enums.CK5Type.Domestic && (ck5.SOURCE_PLANT_ID == ck5.DEST_PLANT_ID))
                    continue;

                if (ck5.GRAND_TOTAL_EX.HasValue)
                    qtyCk5 += ck5.GRAND_TOTAL_EX.Value;
            }

            return qtyCk5;
        }

        public List<CK5> GetByGIDate(int month,  int year, string sourcePlantId, string goodTypeId)
        {
            var goodTypeGroup = _goodTypeGroupBLL.GetGroupByExGroupType(goodTypeId);
            var data =   _repository.Get(
                    p =>
                        p.GI_DATE.HasValue && p.GI_DATE.Value.Month == month && p.GI_DATE.Value.Year == year &&
                        p.SOURCE_PLANT_ID == sourcePlantId && (int) p.EX_GOODS_TYPE == goodTypeGroup.EX_GROUP_TYPE_ID).ToList();

            return data;

        }

        public List<int> GetAllYearsByGiDate()
        {
            var data = _repository.Get(x => x.GI_DATE.HasValue, null, "").Select(x => x.GI_DATE != null ? x.GI_DATE.Value.Year : 0).DistinctBy(x=> x).ToList();

            return data;
        }

        public List<CK5> GetAllCompletedPortToImporter()
        {
            var data =
                _repository.Get(
                    x => x.STATUS_ID == Enums.DocumentStatus.Completed && x.CK5_TYPE == Enums.CK5Type.PortToImporter,null,"").ToList();

            return data;
        }
    }
}
