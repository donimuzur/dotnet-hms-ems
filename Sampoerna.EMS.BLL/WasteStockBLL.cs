using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Voxteneo.WebComponents.Logger;

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

        public GetListMaterialUomByMaterialAndPlantOutput GetListMaterialUomByMaterialAndPlant(string materialNumber, string plantId)
        {
            var output = new GetListMaterialUomByMaterialAndPlantOutput();

            var dbMaterial = _materialBll.GetByPlantIdAndStickerCode(plantId, materialNumber);

            if (dbMaterial != null)
            {
                output.Uom = dbMaterial.BASE_UOM_ID;
            }
            return output;

        }

        public WasteStockDto SaveWasteStock(WasteStockSaveInput input)
        {
            //ValidateWasteRole(input);

            WASTE_STOCK dbData = null;
            if (input.WasteStockDto.WASTE_STOCK_ID > 0)
            {
                //update
                dbData = _repository.GetByID(input.WasteStockDto.WASTE_STOCK_ID);
                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                //set changes history
                //var origin = Mapper.Map<WasteRoleDto>(dbData);

                //SetChangesHistory(origin, input.WasteRoleDto, input.UserId);

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
    }
}
