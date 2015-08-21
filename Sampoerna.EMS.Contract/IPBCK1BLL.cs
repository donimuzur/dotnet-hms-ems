using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface IPBCK1BLL
    {
        List<Pbck1Dto> GetCompletedDocumentByParam(Pbck1GetCompletedDocumentByParamInput input);

        List<Pbck1Dto> GetOpenDocumentByParam(Pbck1GetOpenDocumentByParamInput input);
        
        List<Pbck1Dto> GetAllByParam(Pbck1GetByParamInput input);

        Pbck1Dto GetById(long id);

        SavePbck1Output Save(Pbck1SaveInput pbck1);

        void Delete(long id);

        string GetPbckNumberById(long id);

        List<Pbck1ProdConverterOutput> ValidatePbck1ProdConverterUpload(List<Pbck1ProdConverterInput> inputs);

        List<Pbck1ProdPlanOutput> ValidatePbck1ProdPlanUpload(IEnumerable<Pbck1ProdPlanInput> inputs);

        void Pbck1Workflow(Pbck1WorkflowDocumentInput input);

        List<Pbck1SummaryReportDto> GetSummaryReportByParam(Pbck1GetSummaryReportByParamInput input);

        List<Pbck1MonitoringUsageDto> GetMonitoringUsageByParam(Pbck1GetMonitoringUsageByParamInput input);

        Pbck1ReportDto GetPrintOutDataById(int id);

        Pbck1Dto GetByDocumentNumber(string documentNumber);

        List<ZAIDM_EX_NPPBKCCompositeDto> GetNppbkByCompanyCode(string companyCode);
        void UpdateReportedOn(Pbck1UpdateReportedOn input);

    }
}
