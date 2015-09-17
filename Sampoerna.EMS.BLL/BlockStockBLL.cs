using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class BlockStockBLL : IBlockStockBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;

        private IGenericRepository<BLOCK_STOCK> _repository;

        public BlockStockBLL(IUnitOfWork uow, ILogger logger)
       {
           _logger = logger;
           _uow = uow;

           _repository = _uow.GetGenericRepository<BLOCK_STOCK>();
         
       }

        public BLOCK_STOCKDto GetBlockStockById(long id)
        {
            var dtData = _repository.GetByID(id);

            return Mapper.Map<BLOCK_STOCKDto>(dtData);

        }

        public List<BLOCK_STOCKDto> GetBlockStockByPlantAndMaterialId(string plantId , string materialId)
        {
            var dtData = _repository.Get(c=>c.PLANT == plantId && c.MATERIAL_ID == materialId);

            return Mapper.Map<List<BLOCK_STOCKDto>>(dtData);

        }

    }
}
