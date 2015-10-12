using System.Linq;
using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;
using AutoMapper;

namespace Sampoerna.EMS.BLL
{
    public class CK4CItemBLL : ICK4CItemBLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<CK4C_ITEM> _repository;

        public CK4CItemBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<CK4C_ITEM>();
        }
        public void DeleteByCk4cId(long ck4cId)
        {
            var dataToDelete = _repository.Get(c => c.CK4C_ID == ck4cId);
            if (dataToDelete != null)
            {
                foreach (var ck4cItem in dataToDelete.ToList())
                {
                    _repository.Delete(ck4cItem);
                }
            }
        }

        public List<Ck4cItem> GetDataByPlantAndFacode(string plant, string facode)
        {
            var data = _repository.Get(c => c.WERKS == plant && c.FA_CODE == facode).ToList();

            return Mapper.Map<List<Ck4cItem>>(data);
        }
    }
}