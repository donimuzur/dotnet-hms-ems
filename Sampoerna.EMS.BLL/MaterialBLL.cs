﻿using System.Linq.Expressions;
using Sampoerna.EMS.BLL.Services;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Voxteneo.WebComponents.Logger;
using Sampoerna.EMS.BusinessObject.DTOs;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class MaterialBLL : IMaterialBLL
    {
        private IGenericRepository<ZAIDM_EX_MATERIAL> _repository;
        
        private ILogger _logger;
        private IUnitOfWork _uow;
        private string includeTables = "T001W, MATERIAL_UOM, UOM,ZAIDM_EX_GOODTYP";
        private ChangesHistoryBLL _changesHistoryBll;
        private IMasterDataAprovalBLL _masterDataAprovalBLL;
        private IZaidmExMaterialService _zaidmExMaterialService;
        private IGenericRepository<MATERIAL_UOM> _repositoryUoM;
        private IExGroupTypeBLL _goodTypeGroupBLL;
        
        public MaterialBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_MATERIAL>();
            _repositoryUoM = _uow.GetGenericRepository<MATERIAL_UOM>();
            _changesHistoryBll = new ChangesHistoryBLL(_uow,_logger);
            _goodTypeGroupBLL = new ExGroupTypeBLL(_uow, logger);
            _masterDataAprovalBLL = new MasterDataApprovalBLL(_uow,_logger);
            _zaidmExMaterialService = new ZaidmExMaterialService(_uow,_logger);
            
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

        

        


        public MaterialOutput Save(MaterialDto data, string userId)
        {
            bool isNew = false;
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

                var tempNewData = AutoMapper.Mapper.Map<ZAIDM_EX_MATERIAL>(data);
                tempNewData = _masterDataAprovalBLL.MasterDataApprovalValidation((int)Enums.MenuList.MaterialMaster, userId, origin,
                    tempNewData);
                data = AutoMapper.Mapper.Map<MaterialDto>(tempNewData);

                if (data.CLIENT_DELETION != (originDto.CLIENT_DELETION.HasValue ? originDto.CLIENT_DELETION : false))
                {
                     _zaidmExMaterialService.ClientDeletion(data, userId);

                }

                if (data.PLANT_DELETION != (originDto.PLANT_DELETION.HasValue ? originDto.PLANT_DELETION.Value : false))
                {
                    _zaidmExMaterialService.PlantDeletion(data, userId);
                }
                
                SetChanges(originDto, data, userId);

                data.MATERIAL_UOM = origin.MATERIAL_UOM;
            }
            else {
                data.CREATED_BY = userId;
                data.CREATED_DATE = DateTime.Now;

                isNew = true;
            }

            var dataToSave = AutoMapper.Mapper.Map<ZAIDM_EX_MATERIAL>(data);

            var output = new MaterialOutput();

            try
            {
                if (!isNew)
                {
                    _repository.InsertOrUpdate(dataToSave);

                    _uow.SaveChanges();
                }
                else
                {
                    _masterDataAprovalBLL.MasterDataApprovalValidation((int)Enums.MenuList.MaterialMaster, userId, new ZAIDM_EX_MATERIAL(), dataToSave,true);
                }
                
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

        public List<MaterialDto> GetMaterialByPlantIdAndGoodType(string plantId,int goodTypeGroup)
        {
            //var goodtypegroupidval = goodTypeGroup.HasValue ? goodTypeGroup.Value : 0;
            
            var dbGoodTypeList = _goodTypeGroupBLL.GetById(goodTypeGroup);
            List<string> goodtypelist = new List<string>();
            if (dbGoodTypeList != null)
            {
                goodtypelist = dbGoodTypeList.EX_GROUP_TYPE_DETAILS.Select(x => x.GOODTYPE_ID).ToList();
            }

            var data =
                _repository.Get(p => p.WERKS == plantId && p.EXC_GOOD_TYP != null && goodtypelist.Contains(p.EXC_GOOD_TYP), null, includeTables);

            return AutoMapper.Mapper.Map<List<MaterialDto>>(data);
        }

        public MaterialDto GetMaterialByPlantIdAndMaterialNumber(string plantId, string materialNumber)
        {
            var data =
                _repository.Get(p => p.WERKS == plantId && p.STICKER_CODE == materialNumber, null, includeTables).FirstOrDefault();

            return AutoMapper.Mapper.Map<MaterialDto>(data);
        }


        public List<MATERIAL_UOM> GetMaterialUomByPlant(string plant)
        {
            return _repositoryUoM.Get(c => c.WERKS == plant).ToList();
        }

        public List<ZAIDM_EX_MATERIAL> getAllMaterial(string goodType)
        {
            Expression<Func<ZAIDM_EX_MATERIAL, bool>> queryFilter = PredicateHelper.True<ZAIDM_EX_MATERIAL>();

            if (!string.IsNullOrEmpty(goodType)) queryFilter = queryFilter.And(x => x.EXC_GOOD_TYP == goodType);

            var data = _repository.Get(queryFilter, null, includeTables).ToList();

            return data.ToList();
        }
    }
}
