using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Contract.Services
{
    public interface IZaidmExNppbkcService
    {
        List<ZAIDM_EX_NPPBKC> GetNppbkcsByPoa(string poaId);
        List<ZAIDM_EX_NPPBKC> GetNppbkcMainPlantOnlyByPoa(string poaId);
        ZAIDM_EX_NPPBKC GetById(string nppbkcId);

        List<ZAIDM_EX_NPPBKCCompositeDto> GetNppbkcList(List<string> nppbkcList);
    }
}