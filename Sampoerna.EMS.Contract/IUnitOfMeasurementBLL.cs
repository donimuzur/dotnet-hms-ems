using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface IUnitOfMeasurementBLL
    {
        UOM GetById(int id);
        List<UOM> GetAll();

        string GetUomNameById(int? id);

        bool IsUomNameExist(string name);

        UOM GetUomByName(string name);

    }
}