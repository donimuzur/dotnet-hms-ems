using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface IHeaderFooterBLL
    {
        HeaderFooterDetails GetDetailsById(int id);
        List<HeaderFooter> GetAll();
        SaveHeaderFooterOutput Save(HeaderFooterDetails headerFooterData);
    }
}