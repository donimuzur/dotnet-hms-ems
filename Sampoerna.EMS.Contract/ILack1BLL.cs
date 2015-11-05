using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface ILACK1BLL
    {
        List<Lack1Dto> GetAllByParam(Lack1GetByParamInput input);
        List<Lack1Dto> GetCompletedDocumentByParam(Lack1GetByParamInput input);
        
        decimal GetLatestSaldoPerPeriod(Lack1GetLatestSaldoPerPeriodInput input);

        List<Lack1Dto> GetByPeriod(Lack1GetByPeriodParamInput input);

        Lack1GeneratedOutput GenerateLack1DataByParam(Lack1GenerateDataParamInput input);

        Lack1CreateOutput Create(Lack1CreateParamInput input);

        Lack1DetailsDto GetDetailsById(int id);

        Lack1PrintOutDto GetPrintOutData(int id);

        void Lack1Workflow(Lack1WorkflowDocumentInput input);

        SaveLack1Output SaveEdit(Lack1SaveEditInput input);

        List<Lack1SummaryReportDto> GetSummaryReportByParam(Lack1GetSummaryReportByParamInput input);

        List<int> GetYearList();

        List<ZAIDM_EX_NPPBKCCompositeDto> GetNppbckListByCompanyCode(string companyCode);

        List<Lack1DetailReportDto> GetDetailReportByParam(Lack1GetDetailReportByParamInput input);

        List<Lack1DetailsDto> GetPbck1RealizationList(Lack1GetPbck1RealizationListParamInput input);
        List<Lack1Dto> GetDashboardDataByParam(Lack1GetDashboardDataByParamInput input);

    }
}
