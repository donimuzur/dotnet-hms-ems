using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface IHeaderFooterBLL
    {
        HeaderFooterDetails GetDetailsById(int id);
        List<HeaderFooter> GetAll();
        HeaderFooter GetById(int id);

        SaveHeaderFooterOutput Save(HeaderFooterDetails headerFooterData, string userId);

        void Delete(int id, string userId);

        HEADER_FOOTER_MAPDto GetByComanyAndFormType(HeaderFooterGetByComanyAndFormTypeInput input);

    }
}