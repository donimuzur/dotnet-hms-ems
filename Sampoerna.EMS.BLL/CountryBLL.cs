
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class CountryBLL : ICountryBLL
    {
        private IGenericRepository<COUNTRY> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

         public CountryBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<COUNTRY>();
        }

         public CountryDto GetMonth(int id)
        {
            return AutoMapper.Mapper.Map<CountryDto>(_repository.GetByID(id));
        }

         public List<CountryDto> GetAll()
         {
             return AutoMapper.Mapper.Map<List<CountryDto>>(_repository.Get().ToList());
         }

         public CountryDto GetCountryByCode(string code)
         {
             return AutoMapper.Mapper.Map<CountryDto>(_repository.Get(c => c.COUNTRY_CODE == code).FirstOrDefault());
         }

    }

    

}
