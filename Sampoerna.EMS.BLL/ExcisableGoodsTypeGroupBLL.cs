using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class ExcisableGoodsTypeGroupBLL : IExcisableGoodsTypeGroupBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAIDM_EX_GOODTYP> _repositoryGoodsType;

        public ExcisableGoodsTypeGroupBLL(IUnitOfWork uow, ILogger logger)  
        {
            _logger = logger;
            _uow = uow;
            _repositoryGoodsType = _uow.GetGenericRepository<ZAIDM_EX_GOODTYP>();    
        }


        public List<ZAIDM_EX_GOODTYP> GetAll()
        {
            return _repositoryGoodsType.Get().ToList();
        }
    }
}
