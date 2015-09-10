using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class WasteBLL : IWasteBLL
    {
        private IGenericRepository<WASTE> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAIDM_EX_BRAND> _repositoryBrand;
        private IGenericRepository<T001W> _repositoryPlant;
        private IGenericRepository<ZAIDM_EX_GOODTYP> _repositoryGood;

        public WasteBLL(ILogger logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<WASTE>();
            _repositoryBrand = _uow.GetGenericRepository<ZAIDM_EX_BRAND>();
            _repositoryGood = _uow.GetGenericRepository<ZAIDM_EX_GOODTYP>();
           _repositoryPlant = _uow.GetGenericRepository<T001W>();   
        }
        public List<WasteDto> GetAllByParam(WasteGetByParamInput input)
        {
            throw new NotImplementedException();
        }

        public List<WasteDto> GetAllWaste()
        {
            var dbData = _repository.Get().ToList();
            return Mapper.Map<List<WasteDto>>(dbData);

        }

        public void Save(WasteDto wastenDto)
        {
            WASTE dbWaste = new WASTE();
            dbWaste = Mapper.Map<WASTE>(dbWaste);
        }

        public WasteDto GetById(string companyCode, string plantWerk, string faCode, DateTime productionDate)
        {
            throw new NotImplementedException();
        }

        public List<BusinessObject.DTOs.WasteDto> GetByCompPlant(string comp, string plant, string nppbkc)
        {
            throw new NotImplementedException();
        }

        public WASTE GetExistDto(string companyCode, string plantWerk, string faCode, DateTime productionDate)
        {
            throw new NotImplementedException();
        }
    }
}
