using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
namespace Sampoerna.EMS.Contract
{
    public interface IMonthClosingDocBLL
    {
        bool Save(List<MonthClosingDocDto> item);
        List<MonthClosingDocDto> GetDocByFlag(string monthFlag);
    }
}
