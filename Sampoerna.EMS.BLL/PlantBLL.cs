using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class PlantBLL : IPlantBLL
    {

        private IGenericRepository<T1001W> _repository;
        private IGenericRepository<PLANT_RECEIVE_MATERIAL> _plantReceiveMaterialRepository;
        private IChangesHistoryBLL _changesHistoryBll;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private string includeTables = "ZAIDM_EX_NPPBKC, PLANT_RECEIVE_MATERIAL, PLANT_RECEIVE_MATERIAL.ZAIDM_EX_GOODTYP";
        private IZaidmExNPPBKCBLL _nppbkcbll;
        
        public PlantBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<T1001W>();
            _plantReceiveMaterialRepository = _uow.GetGenericRepository<PLANT_RECEIVE_MATERIAL>();
            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);
            _nppbkcbll = new ZaidmExNPPBKCBLL(_uow, _logger, _changesHistoryBll);
        }

        public Plant GetId(long id)
        {
            return Mapper.Map<Plant>(_repository.Get(c => c.PLANT_ID == id,null, includeTables).FirstOrDefault());
        }

        public List<Plant> GetAll()
        {

           return Mapper.Map<List<Plant>>(_repository.Get(null, null, includeTables).ToList());
            
        }

        public void save(Plant plantT1001W, int userId)
        {
            if (plantT1001W.PLANT_ID != 0)
            {
                //update
                var origin =
                    _repository.Get(c => c.PLANT_ID == plantT1001W.PLANT_ID, null, includeTables).FirstOrDefault();

                plantT1001W.NPPBKC_NO = _nppbkcbll.GetById(plantT1001W.NPPBCK_ID.Value).NPPBKC_NO;

                SetChanges(origin, plantT1001W, userId);

                //hapus dulu aja ya ? //todo ask the cleanist way
                var dataToDelete =
                    _plantReceiveMaterialRepository.Get(c => c.PLANT_ID == plantT1001W.PLANT_ID)
                        .ToList();

                foreach (var item in dataToDelete)
                {
                    _plantReceiveMaterialRepository.Delete(item);
                }

                Mapper.Map<Plant, T1001W>(plantT1001W, origin);
                origin.PLANT_RECEIVE_MATERIAL = null;
                origin.PLANT_RECEIVE_MATERIAL = plantT1001W.PLANT_RECEIVE_MATERIAL;
            }
            else
            {
                //Insert
                var origin = Mapper.Map<T1001W>(plantT1001W);
                origin.CREATED_DATE = DateTime.Now;
                _repository.Insert(origin);
            }

            try
            {
                _uow.SaveChanges();
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
            }
        }

        private void SetChanges(T1001W origin, Plant data, int userId)
        {
            var changesData = new Dictionary<string, bool>();
            changesData.Add("NPPBKC_NO", origin.NPPBCK_ID.HasValue && origin.NPPBCK_ID.Equals(data.NPPBCK_ID));
            changesData.Add("CITY", !string.IsNullOrEmpty(origin.CITY) && !string.IsNullOrEmpty(data.CITY) ? origin.CITY.Equals(data.CITY) : true);
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
                        FORM_ID = data.PLANT_ID,
                        FIELD_NAME = listChange.Key,
                        MODIFIED_BY = userId,
                        MODIFIED_DATE = DateTime.Now
                    };
                    switch (listChange.Key)
                    {
                        case "NPPBKC_NO":
                            changes.OLD_VALUE = origin.ZAIDM_EX_NPPBKC != null ? origin.ZAIDM_EX_NPPBKC.NPPBKC_NO : "NULL";
                            changes.NEW_VALUE = data.NPPBKC_NO;
                            break;
                        case "CITY":
                            changes.OLD_VALUE = origin.CITY;
                            changes.NEW_VALUE = data.CITY;
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

        public string GetPlantWerksById(long id)
        {
            var dbPlant = _repository.GetByID(id);
            return dbPlant == null ? string.Empty : dbPlant.WERKS;
        }
    }
}
