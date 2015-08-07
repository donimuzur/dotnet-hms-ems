using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class MaterialBLL : IMaterialBLL
    {
        private IGenericRepository<ZAIDM_EX_MATERIAL> _repository;
        
        private ILogger _logger;
        private IUnitOfWork _uow;
        private string includeTables = "T001W, MATERIAL_UOM, UOM,ZAIDM_EX_GOODTYP, USER";
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
                _repository.Get(p => p.STICKER_CODE == materialnumber, null, includeTables)
                    .Select(p => p.T001W)
                    .ToList();
            return data;
        }


        public MaterialOutput Save(ZAIDM_EX_MATERIAL data,string userId)
        {


            
             
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
            existingData.IS_DELETED = true;
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
                OLD_VALUE = existingData.IS_DELETED.HasValue ? existingData.IS_DELETED.Value.ToString() : "NULL",
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
    }
}
