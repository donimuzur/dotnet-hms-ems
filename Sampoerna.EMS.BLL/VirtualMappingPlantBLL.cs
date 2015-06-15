using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class VirtualMappingPlantBLL : IVirtualMappingPlanBLL
    {
        private IGenericRepository<VIRTUAL_PLANT_MAP> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<T1001> _repositoryT1001; 

        public VirtualMappingPlantBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<VIRTUAL_PLANT_MAP>();
            _repositoryT1001 = _uow.GetGenericRepository<T1001>();

        }

        public List<SaveVirtualMappingPlantOutput> GetAll()
        {
            var repoVirtualPlantMap = _repository.Get().ToList();
            var repoT1001 = _repositoryT1001.Get().ToList();

            //var repoPlantMapTest = _repository.GetQuery();
            //var repoT1001Test = _repositoryT1001.GetQuery();

            var result = repoVirtualPlantMap.Join(repoT1001,
                virtualMap => virtualMap.COMPANY_ID,
                T1001 => T1001.COMPANY_ID,
                (virtualMap, T1001) => new SaveVirtualMappingPlantOutput
                {
                    Company = T1001.BUKRSTXT,
                    ImportVitualPlant = virtualMap.IMPORT_PLANT_ID,
                    ExportVirtualPlant = virtualMap.EXPORT_PLANT_ID
                }).ToList();

            //var test = from p in _repository.GetQuery()
            //           join k in _repositoryT1001.GetQuery()
            //        on p.COMPANY_ID equals k.COMPANY_ID
            //    select new
            //    {
            //        company = k.BUKRSTXT,
            //        export = p.EXPORT_PLANT_ID,
            //        import = p.IMPORT_PLANT_ID
            //    };


            return result;


            //return _repository.Get().ToList();
        }



        public SaveVirtualMappingPlantOutput Save(VIRTUAL_PLANT_MAP virtualPlantMap)
        {
            if (virtualPlantMap.VIRTUAL_PLANT_MAP_ID > 0)
            {
                //update
                _repository.Update(virtualPlantMap);
            }
            else
            {
                //Insert
                _repository.Insert(virtualPlantMap);
            }

            var output = new SaveVirtualMappingPlantOutput();

            try
            {
                _uow.SaveChanges();
                output.Success = true;
                output.Id = virtualPlantMap.VIRTUAL_PLANT_MAP_ID;
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
                output.Success = false;
                output.ErrorCode = ExceptionCodes.BaseExceptions.unhandled_exception.ToString();
                output.ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
            }
            return output ;
        }
    }
}
