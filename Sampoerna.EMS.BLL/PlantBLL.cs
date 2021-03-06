﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class PlantBLL : IPlantBLL
    {

        private IGenericRepository<T001W> _repository;
        private IGenericRepository<PLANT_RECEIVE_MATERIAL> _plantReceiveMaterialRepository;
        private IGenericRepository<T001W> _t001WRepository;
        private IGenericRepository<T001K> _repositoryT001K;
        private IGenericRepository<ZAIDM_EX_NPPBKC> _nppbkcRepository;

        private IChangesHistoryBLL _changesHistoryBll;
        private ILogger _logger;
        private IUnitOfWork _uow;
        //private string includeTables = "ZAIDM_EX_NPPBKC, PLANT_RECEIVE_MATERIAL, PLANT_RECEIVE_MATERIAL.ZAIDM_EX_GOODTYP";
        private string includeTables = "ZAIDM_EX_NPPBKC, ZAIDM_EX_NPPBKC.T001,T001K.T001";

        private IZaidmExNPPBKCBLL _nppbkcbll;

        public PlantBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<T001W>();
            _plantReceiveMaterialRepository = _uow.GetGenericRepository<PLANT_RECEIVE_MATERIAL>();
            _t001WRepository = _uow.GetGenericRepository<T001W>();
            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);
            _nppbkcbll = new ZaidmExNPPBKCBLL(_uow, _logger);
            _repositoryT001K = _uow.GetGenericRepository<T001K>();
            _nppbkcRepository = _uow.GetGenericRepository<ZAIDM_EX_NPPBKC>();
        }

        public T001W GetT001W(string NppbkcId, bool? IsPlant)
        {
            var query = PredicateHelper.True<T001W>();

            query = query.And(p => p.NPPBKC_ID == NppbkcId);

            if (IsPlant == false)
            {
                query = query.And(p => p.IS_MAIN_PLANT == IsPlant || p.IS_MAIN_PLANT == null);
            }
            else
            {
                query = query.And(p => p.IS_MAIN_PLANT == IsPlant);
            }

            return _t001WRepository.Get(query).FirstOrDefault();
        }

        public Plant GetId(string id)
        {
            var data = _repository.Get(c => c.WERKS == id, null, includeTables).FirstOrDefault();

            return Mapper.Map<Plant>(data);
        }

        public List<Plant> GetAll()
        {

            // return Mapper.Map<List<Plant>>(_repository.Get(null, null, includeTables).ToList());
            return Mapper.Map<List<Plant>>(_repository.Get().ToList());

        }

        public void save(Plant plantT1001W, string userId)
        {
            if (!string.IsNullOrEmpty(plantT1001W.WERKS))
            {
                //update
                var origin =
                    _repository.Get(c => c.WERKS == plantT1001W.WERKS, null, includeTables).FirstOrDefault();
                var originMaterialReceive = _plantReceiveMaterialRepository.Get(x => x.PLANT_ID == origin.WERKS, null, "ZAIDM_EX_GOODTYP").ToList();
                // plantT1001W.NPPBKC_ID = _nppbkcbll.GetById(plantT1001W.WERKS).NPPBKC_ID;

                SetChanges(origin, plantT1001W, userId, originMaterialReceive);

                //hapus dulu aja ya ? //todo ask the cleanist way
                var dataToDelete =
                    _plantReceiveMaterialRepository.Get(c => c.PLANT_ID == plantT1001W.WERKS)
                        .ToList();

                foreach (var item in dataToDelete)
                {
                    _plantReceiveMaterialRepository.Delete(item);
                }

                //todo automapper for update data ???
                Mapper.Map<Plant, T001W>(plantT1001W, origin);

                //origin.PLANT_RECEIVE_MATERIAL = plantT1001W.PLANT_RECEIVE_MATERIAL;
            }
            else
            {
                //Insert
                var origin = Mapper.Map<T001W>(plantT1001W);
                origin.CREATED_DATE = DateTime.Now;
                _repository.Insert(origin);

            }

            try
            {
                foreach (var plantReceiveMaterial in plantT1001W.PLANT_RECEIVE_MATERIAL)
                {
                    _plantReceiveMaterialRepository.Insert(plantReceiveMaterial);
                }
                _uow.SaveChanges();
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
            }
        }

        private void SetChanges(T001W origin, Plant data, string userId, List<PLANT_RECEIVE_MATERIAL> originReceive)
        {
            var changesData = new Dictionary<string, bool>();

            changesData.Add("NPPBKC_ID", origin.NPPBKC_ID == data.NPPBKC_ID);
            changesData.Add("NPPBKC_IMPORT_ID", origin.NPPBKC_IMPORT_ID == data.NPPBKC_IMPORT_ID);
            changesData.Add("CITY", origin.ORT01 == data.ORT01);
            changesData.Add("ADDRESS", origin.ADDRESS == data.ADDRESS);
            changesData.Add("SKEPTIS", origin.SKEPTIS == data.SKEPTIS);
            changesData.Add("IS_MAIN_PLANT", origin.IS_MAIN_PLANT == data.IS_MAIN_PLANT);
            changesData.Add("PHONE", origin.PHONE == data.PHONE);

            var originMaterialDesc = string.Empty;
            if (originReceive != null)
            {
                var orLength = originReceive.Count;
                var currOr = 0;
                foreach (var or in originReceive)
                {
                    currOr++;
                    originMaterialDesc += or.ZAIDM_EX_GOODTYP.EXT_TYP_DESC;
                    if (currOr < orLength)
                    {
                        originMaterialDesc += ", ";
                    }
                }

            }
            var editMaterialDesc = string.Empty;
            if (data.PLANT_RECEIVE_MATERIAL != null)
            {
                var orLength = data.PLANT_RECEIVE_MATERIAL.Count;
                var currOr = 0;
                foreach (var or in data.PLANT_RECEIVE_MATERIAL)
                {
                    currOr++;
                    editMaterialDesc += or.ZAIDM_EX_GOODTYP.EXT_TYP_DESC;
                    if (currOr < orLength)
                    {
                        editMaterialDesc += ", ";
                    }
                }

            }
            changesData.Add("RECEIVE_MATERIAL", originMaterialDesc == editMaterialDesc);
            foreach (var listChange in changesData)
            {
                if (!listChange.Value)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Core.Enums.MenuList.MasterPlant,
                        FORM_ID = data.WERKS,
                        FIELD_NAME = listChange.Key,
                        MODIFIED_BY = userId,
                        MODIFIED_DATE = DateTime.Now
                    };
                    switch (listChange.Key)
                    {
                        case "NPPBKC_ID":
                            changes.OLD_VALUE = origin.ZAIDM_EX_NPPBKC != null ? origin.NPPBKC_ID : "NULL";
                            changes.NEW_VALUE = data.NPPBKC_ID;
                            break;
                        case "CITY":
                            changes.OLD_VALUE = origin.ORT01;
                            changes.NEW_VALUE = data.ORT01;
                            break;
                        case "ADDRESS":
                            changes.OLD_VALUE = origin.ADDRESS;
                            changes.NEW_VALUE = data.ADDRESS;
                            break;
                        case "SKEPTIS":
                            changes.OLD_VALUE = origin.SKEPTIS;
                            changes.NEW_VALUE = data.SKEPTIS;
                            break;
                        case "IS_MAIN_PLANT":
                            changes.OLD_VALUE = origin.IS_MAIN_PLANT.HasValue ? origin.IS_MAIN_PLANT.Value.ToString() : "NULL";
                            changes.NEW_VALUE = data.IS_MAIN_PLANT.HasValue ? data.IS_MAIN_PLANT.Value.ToString() : "NULL";
                            break;
                        case "PHONE":
                            changes.OLD_VALUE = origin.PHONE;
                            changes.NEW_VALUE = data.PHONE;
                            break;
                        case "RECEIVE_MATERIAL":
                            changes.OLD_VALUE = originMaterialDesc;
                            changes.NEW_VALUE = editMaterialDesc;
                            break;
                        case "NPPBKC_IMPORT_ID":
                            changes.OLD_VALUE = origin.NPPBKC_IMPORT_ID;
                            changes.NEW_VALUE = data.NPPBKC_IMPORT_ID;
                            break;
                    }
                    _changesHistoryBll.AddHistory(changes);
                }
            }
        }

        public string GetPlantWerksById(string id)
        {
            var dbPlant = _repository.GetByID(id);
            return dbPlant == null ? string.Empty : dbPlant.WERKS;
        }

        public string GetPlantNameById(long id)
        {
            var dbPlant = _repository.GetByID(id);
            return dbPlant == null ? string.Empty : dbPlant.NAME1;
        }

        public List<PLANT_RECEIVE_MATERIAL> GetReceiveMaterials(string plantId)
        {
            return _plantReceiveMaterialRepository.Get(p => p.PLANT_ID == plantId).ToList();

        }

        public List<T001W> GetAllPlant()
        {
            return _repository.Get(null, null, includeTables).ToList();
        }

        public List<Plant> GetPlantByNppbkc(string nppbkcId)
        {
            Expression<Func<T001W, bool>> queryFilter = PredicateHelper.True<T001W>();
            if (!string.IsNullOrEmpty(nppbkcId))
            {
                
                queryFilter = queryFilter.And(c => !string.IsNullOrEmpty(c.NPPBKC_ID) && (c.NPPBKC_ID.Contains(nppbkcId) || c.NPPBKC_IMPORT_ID.Contains(nppbkcId)));
            }

            var dbData = _repository.Get(queryFilter, null, includeTables);
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            var toReturn = Mapper.Map<List<Plant>>(dbData);
            for (var i = 0; i < toReturn.Count; i++)
            {
                toReturn[i].IS_IMPORT_ID = toReturn[i].NPPBKC_IMPORT_ID == nppbkcId;
            }

            return toReturn;
        }
        
        public T001WDto GetT001WById(string id)
        {
            var dbData = _repository.Get(c => c.WERKS == id, null, includeTables).FirstOrDefault();

            return Mapper.Map<T001WDto>(dbData);
        }

        public T001WDto GetT001WByIdImport(string id)
        {
            var dbData = _repository.Get(c => c.WERKS == id && c.NPPBKC_IMPORT_ID != null, null, includeTables).FirstOrDefault();
            T001WDto tempDto = Mapper.Map<T001WDto>(dbData);

            tempDto.NPPBKC_ID = tempDto.NPPBKC_IMPORT_ID;
            var nppbkcData = _nppbkcbll.GetById(tempDto.NPPBKC_ID);
            if (nppbkcData != null)
            {
                tempDto.KppbcCity = nppbkcData.CITY;
                tempDto.KppbcNo = nppbkcData.KPPBC_ID;    
            }
            
            return tempDto;
        }

        List<T001W> IPlantBLL.Get(string nppbkcId)
        {
            return _repository.Get(c => c.NPPBKC_ID == nppbkcId).ToList();
        }

        public T001WDto GetMainPlantByNppbkcId(string nppbkcId)
        {
            includeTables = "T001K, T001K.T001, ZAIDM_EX_NPPBKC";
            Expression<Func<T001W, bool>> queryFilter =
                c => (c.NPPBKC_ID == nppbkcId || c.NPPBKC_IMPORT_ID == nppbkcId) && c.IS_MAIN_PLANT.HasValue && c.IS_MAIN_PLANT.Value;
            var dbData = _repository.Get(queryFilter, null, includeTables).FirstOrDefault();
            return Mapper.Map<T001WDto>(dbData);


        }

        public List<T001WCompositeDto> GetCompositeListByNppbkcId(string nppbkcId)
        {
            Expression<Func<T001W, bool>> queryFilter = PredicateHelper.True<T001W>();
            //if (!string.IsNullOrEmpty(nppbkcId))
            //{
                
            //}

            queryFilter = queryFilter.And(c => !string.IsNullOrEmpty(c.NPPBKC_ID) && (c.NPPBKC_ID.Contains(nppbkcId) || c.NPPBKC_IMPORT_ID.Contains(nppbkcId)));

            var dbData = _repository.Get(queryFilter, null, includeTables);
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            return Mapper.Map<List<T001WCompositeDto>>(dbData);
        }

        public List<T001WCompositeDto> GetCompositeListByNppbkcId(string nppbkcId,string companyId)
        {
            Expression<Func<T001W, bool>> queryFilter = PredicateHelper.True<T001W>();
            //if (!string.IsNullOrEmpty(nppbkcId))
            //{

            //}

            queryFilter = queryFilter.And(c => !string.IsNullOrEmpty(c.NPPBKC_ID) && (c.NPPBKC_ID.Contains(nppbkcId) || c.NPPBKC_IMPORT_ID.Contains(nppbkcId)));

            queryFilter = queryFilter.And(c => c.T001K.BUKRS == companyId);

            var dbData = _repository.Get(queryFilter, null, includeTables);
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            return Mapper.Map<List<T001WCompositeDto>>(dbData);
        }
        
        public List<Plant> GetActivePlant()
        {
            Expression<Func<T001W, bool>> queryFilter =
                c => c.IS_DELETED != true && c.ZAIDM_EX_NPPBKC.IS_DELETED != true;
            return Mapper.Map<List<Plant>>(_repository.Get(queryFilter, null, includeTables).ToList());
        }

        public List<T001WCompositeDto> GetCompositeListByNppbkcIdWithFlag(string nppbkcId)
        {
            //check if NPPBKC flagged
            var nppbkcData = _nppbkcRepository.Get(c => c.NPPBKC_ID == nppbkcId).FirstOrDefault();
            if (nppbkcData == null || !nppbkcData.FLAG_FOR_LACK1.HasValue || !nppbkcData.FLAG_FOR_LACK1.Value)
            {
                return new List<T001WCompositeDto>();
            }
            
            Expression<Func<T001W, bool>> queryFilter = PredicateHelper.True<T001W>();
            //if (!string.IsNullOrEmpty(nppbkcId))
            //{

            //}

            queryFilter =
                queryFilter.And(
                    c =>
                        !string.IsNullOrEmpty(c.NPPBKC_ID) &&
                        (c.NPPBKC_ID.Contains(nppbkcId) || c.NPPBKC_IMPORT_ID.Contains(nppbkcId)));

            var dbData = _repository.Get(queryFilter, null, includeTables);
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            return Mapper.Map<List<T001WCompositeDto>>(dbData);
        }
    }
}
