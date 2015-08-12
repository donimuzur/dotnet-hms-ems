using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class MaterialBLL : IMaterialBLL
    {
        private IGenericRepository<ZAIDM_EX_MATERIAL> _repository;
        
        private ILogger _logger;
        private IUnitOfWork _uow;
        private string includeTables = "T001W, MATERIAL_UOM, UOM,ZAIDM_EX_GOODTYP";
        private ChangesHistoryBLL _changesHistoryBll;
        private IGenericRepository<MATERIAL_UOM> _repositoryUoM;
        public MaterialBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_MATERIAL>();
            _repositoryUoM = _uow.GetGenericRepository<MATERIAL_UOM>();
            _changesHistoryBll = new ChangesHistoryBLL(_uow,_logger);
        }

        public ZAIDM_EX_MATERIAL getByID(string materialId, string plant)
        {
            return _repository.Get(q => q.STICKER_CODE == materialId && q.WERKS == plant, null, includeTables).FirstOrDefault();
            //return _repository.GetByID(materialId);
        }

        public List<ZAIDM_EX_MATERIAL> getAll()
        {
            return _repository.Get(null, null, includeTables).ToList();
        }

        public List<string> getStickerCode()
        {
            return _repository.Get(null, null, includeTables).Select(p=>p.STICKER_CODE).Distinct().ToList();
        }

        public List<T001W> getAllPlant(string materialnumber)
        {
            var data =
                _repository.Get(p => p.STICKER_CODE == materialnumber && p.PLANT_DELETION != true, null, includeTables)
                    .Select(p => p.T001W)
                    .ToList();
            return data;
        }

        private void PlantDeletion(ZAIDM_EX_MATERIAL data, string userId) {
            data.PLANT_DELETION = true;

            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Core.Enums.MenuList.MaterialMaster,
                FORM_ID = data.STICKER_CODE + data.WERKS,
                FIELD_NAME = "PLANT_DELETION",
                MODIFIED_BY = userId,
                MODIFIED_DATE = DateTime.Now,
                OLD_VALUE = data.PLANT_DELETION.HasValue ? data.PLANT_DELETION.Value.ToString() : "NULL",
                NEW_VALUE = true.ToString()
            };

            _changesHistoryBll.AddHistory(changes);
        }

        private void CLientDeletion(ZAIDM_EX_MATERIAL data, string userId) {
            var datatobeclientdeleted = _repository.Get(x => x.STICKER_CODE == data.STICKER_CODE,null,"").ToList();

            foreach (var detail in datatobeclientdeleted) {
                detail.CLIENT_DELETION = data.CLIENT_DELETION;
                var changes = new CHANGES_HISTORY
                {
                    FORM_TYPE_ID = Core.Enums.MenuList.MaterialMaster,
                    FORM_ID = detail.STICKER_CODE+detail.WERKS,
                    FIELD_NAME = "CLIENT_DELETION",
                    MODIFIED_BY = userId,
                    MODIFIED_DATE = DateTime.Now,
                    OLD_VALUE = detail.CLIENT_DELETION.HasValue ? detail.CLIENT_DELETION.Value.ToString() : "NULL",
                    NEW_VALUE = true.ToString()
                };

                _changesHistoryBll.AddHistory(changes);
            }

            
            //_uow.SaveChanges();
        }


        public MaterialOutput Save(ZAIDM_EX_MATERIAL data,string userId)
        {


            if (data.CLIENT_DELETION == true) { 
                CLientDeletion(data, userId);
                
            }
            else if (data.PLANT_DELETION == true) {
                PlantDeletion(data, userId);
            }
             
                _repository.InsertOrUpdate(data);
            

            
            var output = new MaterialOutput();

            try
            {
                _uow.SaveChanges();
                output.Success = true;
                output.materialId = data.STICKER_CODE;
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
                output.Success = false;
                output.ErrorCode = ExceptionCodes.BaseExceptions.unhandled_exception.ToString();
                output.ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
            }
            return output;
        }

        public void SaveUoM(MATERIAL_UOM data)
        {
            try
            {
                _repositoryUoM.InsertOrUpdate(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete(string mn, string p, string userId)
        {
            var existingData = _repository.GetByID(mn, p);
           //existingData.CHANGED_BY = userId;
            //existingData.CHANGED_DATE = DateTime.Now;
            _repository.Update(existingData);

            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Core.Enums.MenuList.MaterialMaster,
                FORM_ID = existingData.STICKER_CODE,
                FIELD_NAME = "IS_DELETED",
                MODIFIED_BY = userId,
                MODIFIED_DATE = DateTime.Now,
                NEW_VALUE = true.ToString()
            };

            _changesHistoryBll.AddHistory(changes);

            var output = new MaterialOutput();
            try
            {
                _uow.SaveChanges();
                output.Success = true;
                output.materialId = existingData.STICKER_CODE;
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
                output.Success = false;
                output.ErrorCode = ExceptionCodes.BaseExceptions.unhandled_exception.ToString();
                output.ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
            }
        }

        public int DeleteMaterialUom(int id, string userId, string mn, string p)
        {
            try
            {
                var existingData = _repositoryUoM.GetByID(id);
                string oldData = string.Format("{0} - {1}", existingData.MEINH, existingData.UMREN);

                _repositoryUoM.Delete(id);

                var changes = new CHANGES_HISTORY
                {
                    FORM_TYPE_ID = Core.Enums.MenuList.MaterialMaster,
                    FORM_ID = mn + p,
                    FIELD_NAME = "CONVERTION_DELETED",
                    MODIFIED_BY = userId,
                    MODIFIED_DATE = DateTime.Now,
                    OLD_VALUE = oldData,
                    NEW_VALUE = string.Empty
                };

                _changesHistoryBll.AddHistory(changes);

                _uow.SaveChanges();
            }
            catch (Exception)
            {
                
                return -1;
                
            }
            return 0;

        }

        public List<ZAIDM_EX_MATERIAL> GetByFlagDeletion(bool? isDelete)
        {
            //Expression<Func<ZAIDM_EX_MATERIAL, bool>> queryFilter = PredicateHelper.True<ZAIDM_EX_MATERIAL>();
            //Expression<Func<ZAIDM_EX_MATERIAL, bool>> queryFilterPlant = PredicateHelper.True<ZAIDM_EX_MATERIAL>();
            //Expression<Func<ZAIDM_EX_MATERIAL, bool>> queryFilterClient = PredicateHelper.True<ZAIDM_EX_MATERIAL>();
            //if (isDelete.HasValue)
            //{
            //    queryFilterPlant = isDelete.Value ?
            //        queryFilterPlant.And(c => c.PLANT_DELETION.HasValue &&( c.CLIENT_DELETION.Value == isDelete.Value))
            //        : queryFilterPlant.And(c => !c.IS_DELETED.HasValue || c.IS_DELETED.Value == isDelete.Value);
            //}
            var datalistFromDb = _repository.Get(null, null, includeTables);
            List<ZAIDM_EX_MATERIAL> filteredData = new List<ZAIDM_EX_MATERIAL>();
            foreach (var data in datalistFromDb) {
                if (!(data.CLIENT_DELETION.HasValue ? data.CLIENT_DELETION.Value : false)) {
                    filteredData.Add(data);
                }

                if (data.PLANT_DELETION.HasValue ? data.PLANT_DELETION.Value : false) {
                    filteredData.Remove(data);
                }
            }

            return filteredData.GroupBy(x => x.STICKER_CODE)
                .Select(x =>
                    x.Select(y => new ZAIDM_EX_MATERIAL() { 
                        STICKER_CODE = y.STICKER_CODE
                    }).First()

                ).ToList();
        }


        
    }
}
