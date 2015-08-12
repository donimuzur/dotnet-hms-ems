using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Contract
{
    public interface IZaidmExNPPBKCBLL
    {
        ZAIDM_EX_NPPBKC GetById(string id);
        List<ZAIDM_EX_NPPBKC> GetAll();
        void Save(ZAIDM_EX_NPPBKC nppbkc);

        void Update(ZAIDM_EX_NPPBKC nppbkc);

        void Delete(string id);

        string GetCeOfficeCodeByNppbcId(string nppBkcId);

        ZAIDM_EX_NPPBKCDto GetDetailsById(string id);

        ZAIDM_EX_NPPBKCDto GetDetailsByCityName(string cityName);
    }
}