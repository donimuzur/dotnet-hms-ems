using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
   public interface IPBCK4BLL
   {
       List<Pbck4Dto> GetPbck4ByParam(Pbck4GetByParamInput input);

       Pbck4Dto GetPbck4ById(int id);

       Pbck4DetailsOutput GetDetailsPbck4(int id);

       Pbck4Dto SavePbck4(Pbck4SaveInput input);

       List<Pbck4ItemsOutput> Pbck4ItemProcess(List<Pbck4ItemsInput> inputs);

       void PBCK4Workflow(Pbck4WorkflowDocumentInput input);

       Pbck4ReportDto GetPbck4ReportDataById(int id);

       Pbck4XmlDto GetPbck4ForXmlById(int id);

       List<Pbck4SummaryReportDto> GetSummaryReportsByParam(Pbck4GetSummaryReportByParamInput input);

       void GovApproveDocumentRollback(Pbck4WorkflowDocumentInput input);

       List<GetListBrandByPlantOutput> GetListBrandByPlant(string plantId);

       List<GetListCk1ByNppbkcOutput> GetListCk1ByNppbkc(string nppbkcId);

       GetBrandItemsOutput GetBrandItemsStickerCodeByPlantAndFaCode(string plant, string faCode);

       string GetCk1DateByCk1Id(long ck1Id);

       decimal GetBlockedStockByPlantAndFaCode(string plant, string faCode);

       BlockedStockQuotaOutput GetBlockedStockQuota(string plant, string faCode);

       decimal GetCurrentReqQtyByPbck4IdAndFaCode(int pbck4Id, string faCode);

       List<GetListCk1ByNppbkcOutput> GetListCk1ByPlantAndFaCode(GetListCk1ByPlantAndFaCodeInput input);

       string GetListPoaByNppbkcId(string nppbkcId);

       void SendMailCompletedPbck4Document(Pbck4WorkflowDocumentInput input);

       void UpdateUploadedFileCompleted(List<PBCK4_DOCUMENTDto> input);
   }
}
