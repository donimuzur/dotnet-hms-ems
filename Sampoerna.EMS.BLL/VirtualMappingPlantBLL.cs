﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class VirtualMappingPlantBLL : IVirtualMappingPlantBLL
    {
        private IGenericRepository<VIRTUAL_PLANT_MAP> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<T001> _repositoryT1001;
        private IChangesHistoryBLL _changesHistoryBll;

        public VirtualMappingPlantBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<VIRTUAL_PLANT_MAP>();
            _repositoryT1001 = _uow.GetGenericRepository<T001>();
            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);

        }

        public VIRTUAL_PLANT_MAP GetById(long id)
        {
            return _repository.GetByID(id);
        }

        public VIRTUAL_PLANT_MAP GetByCompany(string companyid) {
            try
            {
                return _repository.Get(x => x.COMPANY_ID == companyid && x.IS_DELETED != true).SingleOrDefault();
            }
            catch (Exception)
            {
                
                throw new Exception("More than one virtual plant mapping found");
            }
            
        }

        public VIRTUAL_PLANT_MAP GetByIdIncludeChild(int id)
        {
            return _repository.Get(a => a.VIRTUAL_PLANT_MAP_ID == id, null, "T001,T001W,T001W1").FirstOrDefault();
        }

        public List<VIRTUAL_PLANT_MAP> GetAll()
        {
            return _repository.Get(null, null, "T001,T001W,T001W1").ToList();
        }
        
        public bool Save(VIRTUAL_PLANT_MAP virtualPlant)
        {
            bool success = false;
            var isexist = _repository.Get(x => x.COMPANY_ID == virtualPlant.COMPANY_ID && x.EXPORT_PLANT_ID == virtualPlant.EXPORT_PLANT_ID && x.IMPORT_PLANT_ID == virtualPlant.IMPORT_PLANT_ID).Count() > 0;
            if (!isexist && virtualPlant.VIRTUAL_PLANT_MAP_ID == 0)
            {
                _repository.Insert(virtualPlant);
                success = true;
            }
            else if(!isexist){
                _repository.Update(virtualPlant);
                success = true;
            }

            //_repository.InsertOrUpdate(virtualPlant);
            _uow.SaveChanges();
            return success;
        }

        public bool Delete(int id, string userId)
        {
            var newStatus = false; 
            var data = _repository.GetByID(id);

            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Core.Enums.MenuList.VirtualMappingPlant,
                FORM_ID = data.VIRTUAL_PLANT_MAP_ID.ToString(),
                FIELD_NAME = "IS_ACTIVE",
                MODIFIED_BY = userId,
                MODIFIED_DATE = DateTime.Now,
                OLD_VALUE = data.IS_DELETED.HasValue ? data.IS_DELETED.Value.ToString() : "NULL",
                NEW_VALUE = data.IS_DELETED.HasValue && data.IS_DELETED.Value ? "FALSE" : "TRUE"
            };

            if (data.IS_DELETED.HasValue && data.IS_DELETED.Value)
            {
                //Activate
                data.IS_DELETED = false;
            }
            else
            {
                //DeActivate
                data.IS_DELETED = true;
                newStatus = true;
            }
            _repository.Update(data);

            _changesHistoryBll.AddHistory(changes);

            _uow.SaveChanges();

            return newStatus;
        }

        private void SetChanges(VIRTUAL_PLANT_MAP origin, VIRTUAL_PLANT_MAP data, string userId)
        {
            var changesData = new Dictionary<string, bool>();
            changesData.Add("COMPANY_ID", origin.COMPANY_ID.Equals(data.COMPANY_ID));
            changesData.Add("IMPORT_PLANT_ID", origin.IMPORT_PLANT_ID.Equals(data.IMPORT_PLANT_ID));
            changesData.Add("EXPORT_PLANT_ID", origin.EXPORT_PLANT_ID.Equals(data.EXPORT_PLANT_ID));
            changesData.Add("IS_DELETED", origin.IS_DELETED.Equals(data.IS_DELETED));


            foreach (var listChange in changesData)
            {
                if (!listChange.Value)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Core.Enums.MenuList.HeaderFooter,
                        FORM_ID = data.VIRTUAL_PLANT_MAP_ID.ToString(),
                        FIELD_NAME = listChange.Key,
                        MODIFIED_BY = userId,
                        MODIFIED_DATE = DateTime.Now
                    };
                    switch (listChange.Key)
                    {
                        case "COMPANY_ID":
                            changes.OLD_VALUE = origin.COMPANY_ID;
                            changes.NEW_VALUE = data.COMPANY_ID;
                            break;
                        case "EXPORT_PLANT_ID":
                            changes.OLD_VALUE = origin.EXPORT_PLANT_ID;
                            changes.NEW_VALUE = data.EXPORT_PLANT_ID;
                            break;
                        case "IMPORT_PLANT_ID":
                            changes.OLD_VALUE = origin.IMPORT_PLANT_ID;
                            changes.NEW_VALUE = data.IMPORT_PLANT_ID;
                            break;

                        case "IS_DELETED":
                            changes.OLD_VALUE = origin.IS_DELETED.HasValue ? origin.IS_DELETED.Value.ToString() : "NULL";
                            changes.NEW_VALUE = data.IS_DELETED.HasValue ? data.IS_DELETED.Value.ToString() : "NULL";
                            break;
                    }
                    _changesHistoryBll.AddHistory(changes);
                }
            }
        }

      
    }
}
