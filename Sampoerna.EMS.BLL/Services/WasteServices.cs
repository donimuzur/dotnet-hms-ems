using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class WasteServices : IWasteServices
    {
        private IGenericRepository<WASTE> _repository;

        private ILogger _logger;
        private IUnitOfWork _uow;

        public WasteServices(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<WASTE>();
        }

        public List<WASTE> GetWasteDailyProdByParam(GetWasteDailyProdByParamInput input)
        {
            var data = _repository.Get(x => x.WASTE_PROD_DATE >= input.DateFrom && x.WASTE_PROD_DATE <= input.DateTo
                 && string.Compare(x.WERKS, input.PlantFrom) >= 0 &&
                     string.Compare(x.WERKS, input.PlantTo) <= 0).ToList();
            
            return data;
        }

    }
}
