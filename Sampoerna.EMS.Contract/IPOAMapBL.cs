using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface IPOAMapBLL
    {
        List<POA_MAP> GetByNppbckId(string  NppbckdId);
        POA_MAP GetById(int Id);
        List<POA_MAP> GetAll();
        void Save(POA_MAP poaMap);
        void Delete(int id);

        POA_MAP GetByNppbckId(string nppbkc, string plant, string poa);

        List<string> GetPlantByPoaId(string id);
        List<string> GetNppbkcByPoaId(string id);
        List<string> GetCompanyByPoaId(string id);
    }
}