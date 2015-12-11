using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface ILACK2BLL
    {
        List<Lack2Dto> GetAll();
        List<Lack2Dto> GetByParam(Lack2GetByParamInput input);
        List<Lack2Dto> GetCompletedByParam(Lack2GetByParamInput input);
        Lack2Dto GetById(int id);
        Lack2DetailsDto GetDetailsById(int id);
        Lack2CreateOutput Create(Lack2CreateParamInput input);
        Lack2GeneratedOutput GenerateLack2DataByParam(Lack2GenerateDataParamInput input);
        Lack2SaveEditOutput SaveEdit(Lack2SaveEditInput input);
        void Lack2Workflow(Lack2WorkflowDocumentInput input);
        List<Lack2SummaryReportDto> GetSummaryReportsByParam(Lack2GetSummaryReportByParamInput input);
        List<Lack2DetailReportDto> GetDetailReportsByParam(Lack2GetDetailReportByParamInput input);
        List<Lack2Dto> GetDashboardDataByParam(Lack2GetDashboardDataByParamInput input);
    }
}
