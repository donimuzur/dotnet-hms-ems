using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class VirtualMappingPlantBLL : IVirtualMappingPlantBLL
    {
        private IGenericRepository<VIRTUAL_PLANT_MAP> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<T1001> _repositoryT1001;
        private IChangesHistoryBLL _changesHistoryBll;

        public VirtualMappingPlantBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<VIRTUAL_PLANT_MAP>();
            _repositoryT1001 = _uow.GetGenericRepository<T1001>();

        }

        public VIRTUAL_PLANT_MAP GetById(long id)
        {
            return _repository.GetByID(id);
        }

        public VIRTUAL_PLANT_MAP GetByIdIncludeChild(long id)
        {
            return _repository.Get(a => a.VIRTUAL_PLANT_MAP_ID == id, null, "T1001,T1001W,T1001W1").FirstOrDefault();
        }

        public List<VIRTUAL_PLANT_MAP> GetAll()
        {
            return _repository.Get(null, null, "T1001,T1001W,T1001W1").ToList();
        }

        public VIRTUAL_PLANT_MAP Save(VIRTUAL_PLANT_MAP virtualPlant)
        {
            _repository.InsertOrUpdate(virtualPlant);
            _uow.SaveChanges();
            return virtualPlant;
        }

        public void Delete(int id, int userId) {
            var data = _repository.GetByID(id);
            //data.IS_DELETED = true;
            _repository.Update(data);


            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Core.Enums.MenuList.HeaderFooter,
                FORM_ID = data.VIRTUAL_PLANT_MAP_ID,
                //FIELD_NAME = "IS_DELETED",
                MODIFIED_BY = userId,
                MODIFIED_DATE = DateTime.Now,
                //OLD_VALUE = data.IS_DELETED.HasValue ? data.IS_DELETED.Value.ToString() : "NULL",
                NEW_VALUE = true.ToString()
            };

            _changesHistoryBll.AddHistory(changes);
        }

        private void SetChanges(VIRTUAL_PLANT_MAP origin, VIRTUAL_PLANT_MAP data, int userId)
        {
            var changesData = new Dictionary<string, bool>();
            changesData.Add("COMPANY_ID", origin.COMPANY_ID.Equals(data.COMPANY_ID));
            changesData.Add("IMPORT_PLANT_ID", origin.IMPORT_PLANT_ID.Equals(data.IMPORT_PLANT_ID));
            changesData.Add("EXPORT_PLANT_ID", origin.EXPORT_PLANT_ID.Equals(data.EXPORT_PLANT_ID));
            //changesData.Add("IS_DELETED", origin.IS_DELETED.Equals(data.IS_DELETED));
            

            foreach (var listChange in changesData)
            {
                if (!listChange.Value)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Core.Enums.MenuList.HeaderFooter,
                        FORM_ID = data.VIRTUAL_PLANT_MAP_ID,
                        FIELD_NAME = listChange.Key,
                        MODIFIED_BY = userId,
                        MODIFIED_DATE = DateTime.Now
                    };
                    switch (listChange.Key)
                    {
                        case "COMPANY_ID":
                            changes.OLD_VALUE = origin.COMPANY_ID.HasValue ? origin.COMPANY_ID.Value.ToString() : "NULL";
                            changes.NEW_VALUE = data.COMPANY_ID.HasValue ? data.COMPANY_ID.Value.ToString() : "NULL";
                            break;
                        case "EXPORT_PLANT_ID":
                            changes.OLD_VALUE = origin.EXPORT_PLANT_ID.ToString();
                            changes.NEW_VALUE = data.EXPORT_PLANT_ID.ToString();
                            break;
                        case "IMPORT_PLANT_ID":
                            changes.OLD_VALUE = origin.IMPORT_PLANT_ID.ToString();
                            changes.NEW_VALUE = data.IMPORT_PLANT_ID.ToString();
                            break;
                        
                        //case "IS_DELETED":
                        //    changes.OLD_VALUE = origin.IS_DELETED.HasValue ? origin.IS_DELETED.Value.ToString() : "NULL";
                        //    changes.NEW_VALUE = data.IS_DELETED.HasValue ? data.IS_DELETED.Value.ToString() : "NULL";
                        //    break;
                    }
                    _changesHistoryBll.AddHistory(changes);
                }
            }
        }

        //public List<SaveVirtualMappingPlantOutput> GetAll()
        //{
        //    var repoVirtualPlantMap = _repository.Get().ToList();
        //    var repoT1001 = _repositoryT1001.Get().ToList();

        //    //var repoPlantMapTest = _repository.GetQuery();
        //    //var repoT1001Test = _repositoryT1001.GetQuery();

        //    var result = repoVirtualPlantMap.Join(repoT1001,
        //        virtualMap => virtualMap.COMPANY_ID,
        //        T1001 => T1001.COMPANY_ID,
        //        (virtualMap, T1001) => new SaveVirtualMappingPlantOutput
        //        {
        //            Company = T1001.BUKRSTXT,
        //            ImportVitualPlant = virtualMap.IMPORT_PLANT_ID,
        //            ExportVirtualPlant = virtualMap.EXPORT_PLANT_ID
        //        }).ToList();

        //    //var test = from p in _repository.GetQuery()
        //    //           join k in _repositoryT1001.GetQuery() on p.COMPANY_ID equals k.COMPANY_ID
        //    //           select new
        //    //    {
        //    //        company = k.BUKRSTXT,
        //    //        export = p.EXPORT_PLANT_ID,
        //    //        import = p.IMPORT_PLANT_ID
        //    //    };


        //    return result;


        //    //return _repository.Get().ToList();
        //}



        //public SaveVirtualMappingPlantOutput Save(VIRTUAL_PLANT_MAP virtualPlantMap)
        //{
        //    if (virtualPlantMap.VIRTUAL_PLANT_MAP_ID > 0)
        //    {
        //        //update
        //        _repository.Update(virtualPlantMap);
        //    }
        //    else
        //    {
        //        //Insert
        //        _repository.Insert(virtualPlantMap);
        //    }

        //    var output = new SaveVirtualMappingPlantOutput();

        //    try
        //    {
        //        _uow.SaveChanges();
        //        output.Success = true;
        //        output.Id = virtualPlantMap.VIRTUAL_PLANT_MAP_ID;
        //    }
        //    catch (Exception exception)
        //    {
        //        _logger.Error(exception);
        //        output.Success = false;
        //        output.ErrorCode = ExceptionCodes.BaseExceptions.unhandled_exception.ToString();
        //        output.ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
        //    }
        //    return output ;
        //}
    }
}
