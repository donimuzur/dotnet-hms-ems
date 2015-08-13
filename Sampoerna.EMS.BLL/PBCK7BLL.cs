using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
   
    public class PBCK7BLL : IPBCK7BLL
    {
        private ILogger _looger;
        //private IGenericRepository<PBCK3_PBCK7> _repository;
        private IUnitOfWork _uow;
        private IBACK1BLL _back1Bll;

        private string includeTable = "BACK1";

        public PBCK7BLL(IUnitOfWork uow, ILogger logger)
        {
            _looger = logger;
            _uow = uow;
            _back1Bll = new BACK1BLL(_uow, _looger);
           //_repository = new _uow.GetGenericRepository<PBCK3_PBCK7>();
           
        }

        public List<Pbck7Dto> GetAllByParam(BusinessObject.Inputs.Pbck7Input input)
        {
            throw new NotImplementedException();
        }
    }


}
