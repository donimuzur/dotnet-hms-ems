using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface ICK4CBLL
    {
        List<Ck4CDto> GetCompletedDocumentByParam(Ck4cGetCompletedDocumentByParamInput input);

        List<Ck4CDto> GetOpenDocumentByParam(Ck4cGetOpenDocumentByParamInput input);

        List<Ck4CDto> GetAllByParam(Ck4CDashboardParamInput input);

        List<Ck4CDto> GetOpenDocument();

        List<Ck4CDto> GetCompletedDocument();

        Ck4CDto Save(Ck4CDto item, string userId);

        Ck4CDto GetById(long id);

        Ck4CDto GetByItem(Ck4CDto item);

        void Ck4cWorkflow(Ck4cWorkflowDocumentInput input);

        void UpdateReportedOn(Ck4cUpdateReportedOn input);

        Ck4cReportDto GetCk4cReportDataById(int id);

        List<Ck4CSummaryReportDto> GetSummaryReportsByParam(Ck4CGetSummaryReportByParamInput input);

        bool AllowEditCompletedDocument(Ck4CDto item, string userId);

        List<Ck4cItemExportDto> GetCk4cItemById(int id);
    }
}
