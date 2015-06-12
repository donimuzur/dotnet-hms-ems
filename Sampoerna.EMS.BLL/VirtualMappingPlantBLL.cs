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

        public VirtualMappingPlantBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<VIRTUAL_PLANT_MAP>();
        }

        public List<VIRTUAL_PLANT_MAP> GetAll()
        {
            return _repository.Get().ToList();
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
