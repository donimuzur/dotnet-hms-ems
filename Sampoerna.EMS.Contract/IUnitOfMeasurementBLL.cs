using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface IUnitOfMeasurementBLL
    {
        UOM GetById(string  id);
        List<UOM> GetAll();
 void Save(UOM uom,string userid,bool isEdit);

        string GetUomNameById(int? id);

        bool IsUomNameExist(string name);
    }
}