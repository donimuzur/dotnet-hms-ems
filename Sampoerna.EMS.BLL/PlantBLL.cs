using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class PlantBLL : IPlantBLL
    {

        private IGenericRepository<T001W> _repository;
        private IGenericRepository<PLANT_RECEIVE_MATERIAL> _plantReceiveMaterialRepository;
        private IGenericRepository<T001W> _t001WRepository; 
        private IChangesHistoryBLL _changesHistoryBll;
        private ILogger _logger;
        private IUnitOfWork _uow;
        //private string includeTables = "ZAIDM_EX_NPPBKC, PLANT_RECEIVE_MATERIAL, PLANT_RECEIVE_MATERIAL.ZAIDM_EX_GOODTYP";
        private string includeTables = "ZAIDM_EX_NPPBKC, ZAIDM_EX_NPPBKC.T001";
       
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
        }

        public T001W GetT001W(string NppbkcId, bool IsPlant)
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
            return Mapper.Map<Plant>(_repository.Get(c => c.WERKS == id, null, includeTables).FirstOrDefault());
        }

        public List<Plant> GetAll()
        {

            return Mapper.Map<List<Plant>>(_repository.Get(null, null, includeTables).ToList());
            //return Mapper.Map<List<Plant>>(_repository.Get().ToList());

        }

        public void save(Plant plantT1001W, string userId)
        {
            if (!string.IsNullOrEmpty(plantT1001W.WERKS))
            {
                //update
                var origin =
                    _repository.Get(c => c.WERKS == plantT1001W.WERKS, null, includeTables).FirstOrDefault();

               // plantT1001W.NPPBKC_ID = _nppbkcbll.GetById(plantT1001W.WERKS).NPPBKC_ID;

                SetChanges(origin, plantT1001W, userId);

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

        private void SetChanges(T001W origin, Plant data, string userId)
        {
            var changesData = new Dictionary<string, bool>();
            changesData.Add("NPPBKC_ID", string.IsNullOrEmpty(origin.NPPBKC_ID)  && ! string.IsNullOrEmpty(data.NPPBKC_ID) ? origin.NPPBKC_ID.Equals(data.NPPBKC_ID) : true);
            changesData.Add("CITY", !string.IsNullOrEmpty(origin.ORT01) && !string.IsNullOrEmpty(data.ORT01) ? origin.ORT01.Equals(data.ORT01) : true);
            changesData.Add("ADDRESS", !string.IsNullOrEmpty(origin.ADDRESS) && !string.IsNullOrEmpty(data.ADDRESS) ? origin.ADDRESS.Equals(data.ADDRESS) : true);
            changesData.Add("SKEPTIS", !string.IsNullOrEmpty(origin.SKEPTIS) && !string.IsNullOrEmpty(data.SKEPTIS) ? origin.SKEPTIS.Equals(data.SKEPTIS) : true);
            changesData.Add("IS_MAIN_PLANT", origin.IS_MAIN_PLANT.Equals(data.IS_MAIN_PLANT));

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
                        case "NPPBKC_NO":
                            changes.OLD_VALUE = origin.ZAIDM_EX_NPPBKC != null ? origin.ZAIDM_EX_NPPBKC.NPPBKC_ID : "NULL";
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
    }
}
