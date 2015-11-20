using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using AutoMapper;
using CrystalDecisions.Shared.Json;
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
    public class WasteStockBLL : IWasteStockBLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<WASTE_STOCK> _repository;

        private IChangesHistoryBLL _changesHistoryBll;
        private IMaterialBLL _materialBll;

        private string _includeTables = "ZAIDM_EX_MATERIAL, USER, T001W";

        public WasteStockBLL(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<WASTE_STOCK>();

            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);
            _materialBll = new MaterialBLL(_uow, _logger);
        }

        public WasteStockDto GetById(int id)
        {
            var dtData = _repository.GetByID(id);
            if (dtData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return Mapper.Map<WasteStockDto>(dtData);

        }

        public WasteStockDto GetById(int id, bool isIncludeTable)
        {
            string include = "";
            if (isIncludeTable)
            {
                include = _includeTables;
                var dtData = _repository.Get(c => c.WASTE_STOCK_ID == id, null, include).FirstOrDefault();
                if (dtData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                return Mapper.Map<WasteStockDto>(dtData);
            }

            return GetById(id);
        }

        public List<WasteStockDto> GetAllDataOrderByUserAndGroupRole()
        {
            Func<IQueryable<WASTE_STOCK>, IOrderedQueryable<WASTE_STOCK>> orderByFilter =
                n => n.OrderBy(z => z.WERKS).ThenBy(z => z.MATERIAL_NUMBER);

            var listData = _repository.Get(null, orderByFilter, _includeTables).ToList();

            return Mapper.Map<List<WasteStockDto>>(listData);
        }


        public List<GetListMaterialByPlantOutput> GetListMaterialByPlant(string plantId)
        {
            var output = new List<GetListMaterialByPlantOutput>();

            var dbMaterial = _materialBll.GetMaterialByPlantId(plantId);
            foreach (var material in dbMaterial)
            {
                var data = new GetListMaterialByPlantOutput();
                data.MaterialNumber = material.STICKER_CODE;
                data.MaterialDescription = material.MATERIAL_DESC;

                output.Add(data);
            }

            return output;

        }

        public GetListMaterialUomByMaterialAndPlantOutput GetListMaterialUomByMaterialAndPlant(string materialNumber,
            string plantId)
        {
            var output = new GetListMaterialUomByMaterialAndPlantOutput();

            var dbMaterial = _materialBll.GetByPlantIdAndStickerCode(plantId, materialNumber);

            if (dbMaterial != null)
            {
                output.Uom = dbMaterial.BASE_UOM_ID;
            }
            return output;

        }

        private void ValidateWasteStock(WasteStockSaveInput input)
        {
            bool isNeedCheck = false;

            if (input.WasteStockDto.WASTE_STOCK_ID == 0)
            {
                isNeedCheck = true;
            }
            else
            {
                var dbData =
                    _repository.Get(c => c.WASTE_STOCK_ID == input.WasteStockDto.WASTE_STOCK_ID).FirstOrDefault();

                if (dbData != null)
                {
                    if (dbData.WERKS != input.WasteStockDto.WERKS &&
                        dbData.MATERIAL_NUMBER != input.WasteStockDto.MATERIAL_NUMBER)
                    {
                        //check is the data already exist in database
                        isNeedCheck = true;
                    }
                }
            }

            if (isNeedCheck)
            {
                var dbData =
                    _repository.Get(
                        c =>
                            c.WERKS == input.WasteStockDto.WERKS &&
                            c.MATERIAL_NUMBER == input.WasteStockDto.MATERIAL_NUMBER).FirstOrDefault();

                if (dbData != null)
                    throw new Exception(string.Format("Plant : {0}, and Material : {1} already exist.",
                        input.WasteStockDto.WERKS, input.WasteStockDto.MATERIAL_NUMBER));
            }
        }

        public WasteStockDto SaveWasteStock(WasteStockSaveInput input)
        {
            ValidateWasteStock(input);

            WASTE_STOCK dbData = null;
            if (input.WasteStockDto.WASTE_STOCK_ID > 0)
            {
                //update
                dbData = _repository.GetByID(input.WasteStockDto.WASTE_STOCK_ID);
                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                //set changes history
                var origin = Mapper.Map<WasteStockDto>(dbData);

                SetChangesHistory(origin, input.WasteStockDto, input.UserId);

                Mapper.Map<WasteStockDto, WASTE_STOCK>(input.WasteStockDto, dbData);

                dbData.MODIFIED_DATE = DateTime.Now;
                dbData.MODIFIED_BY = input.UserId;

            }
            else
            {
                input.WasteStockDto.CREATED_DATE = DateTime.Now;
                input.WasteStockDto.CREATED_BY = input.UserId;
                dbData = new WASTE_STOCK();
                Mapper.Map<WasteStockDto, WASTE_STOCK>(input.WasteStockDto, dbData);
                _repository.Insert(dbData);
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


            return Mapper.Map<WasteStockDto>(dbData);

        }

        private bool SetChangesHistory(WasteStockDto origin, WasteStockDto data, string userId)
        {
            bool isModified = false;

            var changesData = new Dictionary<string, bool>();
            changesData.Add("PLANT_ID", origin.WERKS == data.WERKS);
            changesData.Add("MATERIAL_NUMBER", origin.MATERIAL_NUMBER == data.MATERIAL_NUMBER);
            changesData.Add("STOCK", origin.STOCK == data.STOCK);

            foreach (var listChange in changesData)
            {
                if (listChange.Value) continue;
                var changes = new CHANGES_HISTORY();
                changes.FORM_TYPE_ID = Enums.MenuList.WasteStock;
                changes.FORM_ID = origin.WASTE_STOCK_ID.ToString();
                changes.FIELD_NAME = listChange.Key;
                changes.MODIFIED_BY = userId;
                changes.MODIFIED_DATE = DateTime.Now;
                switch (listChange.Key)
                {
                    case "PLANT_ID":
                        changes.OLD_VALUE = origin.WERKS;
                        changes.NEW_VALUE = data.WERKS;
                        break;

                    case "MATERIAL_NUMBER":
                        changes.OLD_VALUE = origin.MATERIAL_NUMBER;
                        changes.NEW_VALUE = data.MATERIAL_NUMBER;
                        break;

                    case "STOCK":
                        changes.OLD_VALUE = ConvertHelper.ConvertDecimalToStringMoneyFormat(origin.STOCK);
                        changes.NEW_VALUE = ConvertHelper.ConvertDecimalToStringMoneyFormat(data.STOCK);
                        break;

                }

                _changesHistoryBll.AddHistory(changes);
                isModified = true;
            }
            return isModified;
        }


        public void SaveDataFromWaste(List<WasteStockDto> input, string userId)
        {
            foreach (var item in input)
            {
                var dbdata = Mapper.Map<WASTE_STOCK>(item);
                if (dbdata.ZAIDM_EX_MATERIAL != null)
                {
                    dbdata.CREATED_BY = userId;
                    dbdata.CREATED_DATE = DateTime.Now;

                    _repository.InsertOrUpdate(dbdata);
                }
              
            }
            _uow.SaveChanges();
        }

        //public WasteStockDto GetExisWasteStockByWerksAndMaterial(string werks, string materialNumber)
        //{
        //    var dbdata = _uow.GetGenericRepository<WASTE_STOCK>().Get(p => p.WERKS == werks && p.MATERIAL_NUMBER == materialNumber).FirstOrDefault();
           
        //    return Mapper.Map<WasteStockDto>(dbdata);
        //}

        public void UpdateWasteStockFromWaste(WasteStockDto input)
        {
            var dbWasteStock =
                _repository.Get(c => c.WERKS == input.WERKS && c.MATERIAL_NUMBER == input.MATERIAL_NUMBER)
                    .FirstOrDefault();

            if (dbWasteStock == null)
            {
                dbWasteStock = new WASTE_STOCK();
                Mapper.Map<WasteStockDto, WASTE_STOCK>(input, dbWasteStock);

                dbWasteStock.CREATED_BY = input.CREATED_BY;
                dbWasteStock.CREATED_DATE = DateTime.Now;
            }
            else
            {
                //update
                dbWasteStock.STOCK = input.STOCK;

                dbWasteStock.MODIFIED_BY = input.CREATED_BY;
                dbWasteStock.MODIFIED_DATE = DateTime.Now;
            }
            _repository.InsertOrUpdate(dbWasteStock);
        }
    }
}
