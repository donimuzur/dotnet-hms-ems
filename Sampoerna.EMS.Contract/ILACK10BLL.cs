using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface ILACK10BLL
    {
        List<Lack10Dto> GetByParam(Lack10GetByParamInput input);
        List<Lack10Item> GenerateWasteData(Lack10GetWasteDataInput input);
        Lack10Dto GetByItem(Lack10Dto item);
        Lack10Dto Save(Lack10Dto item, string userId);
        void Lack10Workflow(Lack10WorkflowDocumentInput input);
        Lack10Dto GetById(long id);
        bool AllowEditCompletedDocument(Lack10Dto item, string userId);
        void UpdateSubmissionDate(Lack10UpdateSubmissionDate input);
        Lack10ExportDto GetLack10ExportById(int id);
        List<Lack10SummaryReportDto> GetSummaryReportsByParam(Lack10GetSummaryReportByParamInput input);
    }
}
