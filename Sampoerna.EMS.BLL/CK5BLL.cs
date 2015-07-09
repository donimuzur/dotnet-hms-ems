using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
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
        private string includeTables = "";
        private IDocumentSequenceNumberBLL _docSeqNumBll;
        private IMasterDataBLL _masterDataBll;
        private IBrandRegistrationBLL _brandRegistrationBll;
        private IUnitOfMeasurementBLL _uomBll;

        public CK5BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;

            _repository = _uow.GetGenericRepository<CK5>();

            _docSeqNumBll = new DocumentSequenceNumberBLL(_uow, _logger);
            _masterDataBll = new MasterDataBLL(_uow);
            _brandRegistrationBll = new BrandRegistrationBLL(_uow, _logger);
            _uomBll = new UnitOfMeasurementBLL(_uow,_logger);
        }

        public CK5 GetById(long id)
        {
            var dtData = _repository.GetByID(id);
            if (dtData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return dtData;
        }

        public CK5 GetByIdIncludeTables(long id)
        {
            includeTables = "ZAIDM_EX_GOODTYP,EX_SETTLEMENT,EX_STATUS,REQUEST_TYPE,PBCK1,CARRIAGE_METHOD,COUNTRY, UOM";
            var dtData = _repository.Get(c=>c.CK5_ID == id, null,includeTables).FirstOrDefault();
            if (dtData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return dtData;
        }
        public List<CK5> GetAll()
        {
            includeTables = "T1001W.ZAIDM_EX_NPPBKC, T1001W1.ZAIDM_EX_NPPBKC";
            return _repository.Get(null, null, includeTables).ToList();
        }


        public List<CK5> GetCK5ByType(Enums.CK5Type ck5Type)
        {
            includeTables = "T1001W.ZAIDM_EX_NPPBKC, T1001W1.ZAIDM_EX_NPPBKC, T1001W, T1001W1";
            return _repository.Get(c => c.CK5_TYPE == ck5Type, null, includeTables).ToList();
        }

        public List<CK5> GetCK5ByParam(CK5Input input)
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

            return rc.ToList();
        }

        public void SaveCk5(CK5 ck5)
        {
            //get generate number


            if (ck5.CK5_ID <= 0)
            {
                //insert
                long plantId = 0;
                if (ck5.SOURCE_PLANT_ID.HasValue)
                    plantId = ck5.SOURCE_PLANT_ID.Value;

                var plant = _masterDataBll.GetPlantById(plantId);
                long nppbkc = 0;
                if (plant.NPPBCK_ID.HasValue)
                    nppbkc = plant.NPPBCK_ID.Value;

                var input = new GenerateDocNumberInput()
                {
                    Year = DateTime.Now.Year,
                    Month = DateTime.Now.Month,
                    NppbkcId = nppbkc
                };
                ck5.SUBMISSION_NUMBER = _docSeqNumBll.GenerateNumber(input);
                //ck5.CREATED_DATE = DateTime.Now;
            }
          
            _repository.InsertOrUpdate(ck5);
            _uow.SaveChanges();
        }

        private List<CK5MaterialOutput> ValidateCk5Material(List<CK5MaterialInput> inputs)
        {
            var messageList = new List<string>();
            var outputList = new List<CK5MaterialOutput>();

            foreach (var ck5MaterialInput in inputs)
            {
                messageList.Clear();

                //var output = new CK5MaterialOutput();
                var output = AutoMapper.Mapper.Map<CK5MaterialOutput>(ck5MaterialInput);

                //validate
                var dbBrand = _brandRegistrationBll.GetByFaCode(ck5MaterialInput.Brand);
                if (dbBrand == null)
                    messageList.Add("Brand Not Exist");

                if (!Utils.ConvertHelper.IsNumeric(ck5MaterialInput.Qty))
                    messageList.Add("Qty not valid");

                if (!_uomBll.IsUomNameExist(ck5MaterialInput.Uom))
                    messageList.Add("UOM not exist");

                if (!Utils.ConvertHelper.IsNumeric(ck5MaterialInput.Convertion))
                    messageList.Add("Convertion not valid");

                if (!Utils.ConvertHelper.IsNumeric(ck5MaterialInput.ConvertedUom))
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
    }
}
