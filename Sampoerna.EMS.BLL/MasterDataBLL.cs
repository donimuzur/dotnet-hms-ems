using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.BLL
{
    public class MasterDataBLL : IMasterDataBLL
    {
        private IGenericRepository<T1001> _repositoryT1001;
        private IGenericRepository<T1001W> _repositoryT1001W;

        private IUnitOfWork _uow;

        public MasterDataBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _repositoryT1001 = _uow.GetGenericRepository<T1001>();
            _repositoryT1001W = _uow.GetGenericRepository<T1001W>();
        }

        public List<string> GetDataCompany()
        {
            return _repositoryT1001.Get().Select(p => p.BUKRSTXT).Distinct().ToList();
        }





        public List<AutoCompletePlant> GetAutoCompletePlant()
        {
            
            return AutoMapper.Mapper.Map<List<AutoCompletePlant>>(_repositoryT1001W.Get().ToList());
        

        }
    }
}
