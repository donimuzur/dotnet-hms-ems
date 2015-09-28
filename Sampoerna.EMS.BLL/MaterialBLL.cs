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
using Sampoerna.EMS.BusinessObject.DTOs;

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

        public MaterialDto getByID(string materialId, string plant)
        {
            var data = _repository.Get(q => q.STICKER_CODE == materialId && q.WERKS == plant, null, includeTables).FirstOrDefault();
            return AutoMapper.Mapper.Map<MaterialDto>(data);
            //return _repository.GetByID(materialId);
        }

        public List<MaterialDto> getAll()
        {
            var data = _repository.Get(null, null, includeTables).ToList();

            return AutoMapper.Mapper.Map<List<MaterialDto>>(data);
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

        private void PlantDeletion(MaterialDto data, string userId) {

            var original = _repository.Get(x => x.STICKER_CODE == data.STICKER_CODE && x.WERKS == data.WERKS, null, "").FirstOrDefault();
            if (original.PLANT_DELETION != data.PLANT_DELETION)
            {
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

            original.PLANT_DELETION = data.PLANT_DELETION;
        }

        private void ClientDeletion(MaterialDto data, string userId)
        {
            var original = _repository.Get(x => x.STICKER_CODE == data.STICKER_CODE && x.WERKS == data.WERKS, null, "").FirstOrDefault();

            
            var datatobeclientdeleted = _repository.Get(x => x.STICKER_CODE == data.STICKER_CODE, null, "").ToList();

            foreach (var detail in datatobeclientdeleted) {

                
                if (original.CLIENT_DELETION != data.CLIENT_DELETION)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Core.Enums.MenuList.MaterialMaster,
                        FORM_ID = detail.STICKER_CODE + detail.WERKS,
                        FIELD_NAME = "CLIENT_DELETION",
                        MODIFIED_BY = userId,
                        MODIFIED_DATE = DateTime.Now,
                        OLD_VALUE = detail.CLIENT_DELETION.HasValue ? detail.CLIENT_DELETION.Value.ToString() : "NULL",
                        NEW_VALUE = data.CLIENT_DELETION.HasValue ? data.CLIENT_DELETION.ToString() : "NULL"
                    };

                    _changesHistoryBll.AddHistory(changes);
                }
                detail.CLIENT_DELETION = data.CLIENT_DELETION;
                
            }

            
            //_uow.SaveChanges();
        }


        public MaterialOutput Save(MaterialDto data, string userId)
        {
            var origin = _repository.Get(x=>x.STICKER_CODE == data.STICKER_CODE && x.WERKS == data.WERKS,null,includeTables).SingleOrDefault();
            var originDto = AutoMapper.Mapper.Map<MaterialDto>(origin);
            //var edited = AutoMapper.Mapper.Map<ZAIDM_EX_MATERIAL>(model);
            //AutoMapper.Mapper.Map(model, data);
            if (originDto != null)
            {
                data.MODIFIED_BY = userId;
                data.MODIFIED_DATE = DateTime.Now;
                data.CREATED_DATE = origin.CREATED_DATE;
                data.CREATED_BY = origin.CREATED_BY;

                if (data.CLIENT_DELETION != (originDto.CLIENT_DELETION.HasValue ? originDto.CLIENT_DELETION : false))
                {
                    ClientDeletion(data, userId);

                }

                if (data.PLANT_DELETION != (originDto.PLANT_DELETION.HasValue ? originDto.PLANT_DELETION.Value : false))
                {
                    PlantDeletion(data, userId);
                }
                SetChanges(originDto, data, userId);
           
            }
            else {
                data.CREATED_BY = userId;
                data.CREATED_DATE = DateTime.Now;

                
            }
            data.MATERIAL_UOM = origin.MATERIAL_UOM;
            var dataToSave = AutoMapper.Mapper.Map<ZAIDM_EX_MATERIAL>(data);
           
            _repository.InsertOrUpdate(dataToSave);
            

            
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

        public void SaveUoM(MATERIAL_UOM data,string userid)
        {
            try
            {
                string newdata = string.Format("{0} - {1}", data.MEINH, data.UMREN);
                
                var changes = new CHANGES_HISTORY
                {
                    FORM_TYPE_ID = Core.Enums.MenuList.MaterialMaster,
                    FORM_ID = data.STICKER_CODE + data.WERKS,
                    FIELD_NAME = "CONVERTION_ADDED",
                    MODIFIED_BY = userid,
                    MODIFIED_DATE = DateTime.Now,
                    OLD_VALUE = string.Empty,
                    NEW_VALUE = newdata
                };
                _repositoryUoM.InsertOrUpdate(data);
                _changesHistoryBll.AddHistory(changes);
                _uow.SaveChanges();
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

        private void SetChanges(MaterialDto origin, MaterialDto data,string userid)
        {
            var changesData = new Dictionary<string, bool>();
            changesData.Add("MATERIAL_DESC", origin.MATERIAL_DESC==data.MATERIAL_DESC);
            changesData.Add("PURCHASING_GROUP", origin.PURCHASING_GROUP==data.PURCHASING_GROUP);
            changesData.Add("MATERIAL_GROUP", origin.MATERIAL_GROUP==data.MATERIAL_GROUP);
            changesData.Add("BASE_UOM", origin.BASE_UOM_ID==data.BASE_UOM_ID);
            changesData.Add("ISSUE_STORANGE_LOC", origin.ISSUE_STORANGE_LOC==data.ISSUE_STORANGE_LOC);
            changesData.Add("EX_GOODTYP", origin.EXC_GOOD_TYP==data.EXC_GOOD_TYP);
            changesData.Add("TARIFF", origin.TARIFF == data.TARIFF);
            changesData.Add("TARIFF_CURR", origin.TARIFF_CURR == data.TARIFF_CURR);
            changesData.Add("HJE", origin.HJE == data.HJE);
            changesData.Add("HJE_CURR", origin.HJE_CURR == data.HJE_CURR);
            //changesData.Add("PLANT_DELETION", origin.PLANT_DELETION.Equals(data.PLANT_DELETION));
            //changesData.Add("CLIENT_DELETION", origin.CLIENT_DELETION.Equals(data.CLIENT_DELETION));

            foreach (var listChange in changesData)
            {
                if (!listChange.Value)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Core.Enums.MenuList.MaterialMaster,
                        FORM_ID = data.STICKER_CODE + data.WERKS,
                        FIELD_NAME = listChange.Key,
                        MODIFIED_BY = userid,
                        MODIFIED_DATE = DateTime.Now
                    };
                    switch (listChange.Key)
                    {

                        case "MATERIAL_DESC":
                            changes.OLD_VALUE = origin.MATERIAL_DESC;
                            changes.NEW_VALUE = data.MATERIAL_DESC;
                            break;
                        case "PURCHASING_GROUP":
                            changes.OLD_VALUE = origin.PURCHASING_GROUP;
                            changes.NEW_VALUE = data.PURCHASING_GROUP;
                            break;
                        case "MATERIAL_GROUP":
                            changes.OLD_VALUE = origin.MATERIAL_GROUP;
                            changes.NEW_VALUE = data.MATERIAL_GROUP;
                            break;

                        case "BASE_UOM":
                            changes.OLD_VALUE = origin.BASE_UOM_ID;
                            changes.NEW_VALUE = data.BASE_UOM_ID;
                            break;
                        case "ISSUE_STORANGE_LOC":
                            changes.OLD_VALUE = origin.ISSUE_STORANGE_LOC;
                            changes.NEW_VALUE = data.ISSUE_STORANGE_LOC;
                            break;
                        case "TARIFF":
                            changes.OLD_VALUE = origin.TARIFF.HasValue? origin.TARIFF.Value.ToString() : "0";
                            changes.NEW_VALUE = data.TARIFF.HasValue? data.TARIFF.Value.ToString() : "0";
                            break;
                        case "TARIFF_CURR":
                            changes.OLD_VALUE = origin.TARIFF_CURR;
                            changes.NEW_VALUE = data.TARIFF_CURR;
                            break;
                        case "HJE":
                            changes.OLD_VALUE = origin.HJE.HasValue ? origin.HJE.Value.ToString() : "0";
                            changes.NEW_VALUE = data.HJE.HasValue ? data.HJE.Value.ToString() : "0";
                            break;
                        case "HJE_CURR":
                            changes.OLD_VALUE = origin.HJE_CURR;
                            changes.NEW_VALUE = data.HJE_CURR;
                            break;
                        //case "PLANT_DELETION":
                        //    changes.OLD_VALUE = origin.IsPlantDelete.ToString();
                        //    changes.NEW_VALUE = data.PLANT_DELETION.ToString();
                        //    break;
                        //case "CLIENT_DELETION":
                        //    changes.OLD_VALUE = origin.IsClientDelete.ToString();
                        //    changes.NEW_VALUE = data.CLIENT_DELETION.ToString();
                        //    break;

                    }
                    _changesHistoryBll.AddHistory(changes);
                }
            }
        }

        public List<ZAIDM_EX_MATERIAL> GetByFlagDeletion(bool? isDelete,string plant = "")
        {
            
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

            if (plant == "")
            {
                return filteredData.GroupBy(x => x.STICKER_CODE)
                .Select(x =>
                    x.Select(y => new ZAIDM_EX_MATERIAL()
                    {
                        STICKER_CODE = y.STICKER_CODE
                    }).First()

                ).OrderBy(x => x.STICKER_CODE).ToList();
            }
            else {
                return filteredData.Where(x => x.WERKS == plant)
                    .GroupBy(x => x.STICKER_CODE)
                    .Select(x =>
                        x.Select(y => new ZAIDM_EX_MATERIAL()
                        {
                            STICKER_CODE = y.STICKER_CODE
                        }).First()

                    ).OrderBy(x => x.STICKER_CODE).ToList();
                
            }
            
        }


        public ZAIDM_EX_MATERIAL GetByPlantIdAndStickerCode(string plantId, string stickerCode)
        {
            var dbData = _repository.Get(b => b.WERKS == plantId && b.STICKER_CODE == stickerCode, null, includeTables).FirstOrDefault();
            return dbData;
        }

        public List<MaterialDto> GetMaterialByPlantId(string plantId)
        {
            var data =
                _repository.Get(p => p.WERKS == plantId, null, includeTables );

            return AutoMapper.Mapper.Map<List<MaterialDto>>(data);
        }

        public List<MaterialDto> GetMaterialByPlantIdAndGoodTypeNotNull(string plantId)
        {
            var data =
                _repository.Get(p => p.WERKS == plantId && p.EXC_GOOD_TYP != null, null, includeTables);

            return AutoMapper.Mapper.Map<List<MaterialDto>>(data);
        }
        public MaterialDto GetMaterialByPlantIdAndMaterialNumber(string plantId, string materialNumber)
        {
            var data =
                _repository.Get(p => p.WERKS == plantId && p.STICKER_CODE == materialNumber, null, includeTables).FirstOrDefault();

            return AutoMapper.Mapper.Map<MaterialDto>(data);
        }
    }
}
