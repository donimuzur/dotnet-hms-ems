using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Contract
{
   public interface ICountryBLL
   {
       CountryDto GetMonth(int id);

       List<CountryDto> GetAll();

       CountryDto GetCountryByCode(string code);
   }
}
