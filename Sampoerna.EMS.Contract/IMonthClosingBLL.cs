using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface IMonthClosingBLL
    {
        List<MonthClosingDto> GetList();
        bool Save(MonthClosingDto item);
        MonthClosingDto GetDataByParam(MonthClosingGetByParam param);
        MonthClosingDto GetById(long id);
    }
}
