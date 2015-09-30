using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Contract
{
   public interface ICK5BLL
   {

       CK5Dto GetById(long id);

       CK5 GetByIdIncludeTables(long id);

       List<CK5Dto> GetAll();

       List<CK5Dto> GetCK5ByParam(CK5GetByParamInput input);

       CK5Dto SaveCk5(CK5SaveInput input);

       List<CK5> GetCK5ByType(Enums.CK5Type ck5Type);

       List<CK5MaterialOutput> CK5MaterialProcess(List<CK5MaterialInput> inputs, Enums.ExGoodsType groupType);

       CK5DetailsOutput GetDetailsCK5(long id);

       List<CK5MaterialDto> GetCK5MaterialByCK5Id(long id);

       void CK5Workflow(CK5WorkflowDocumentInput input);

       List<CK5Dto> GetSummaryReportsByParam(CK5GetSummaryReportByParamInput input);

       List<CK5Dto> GetCk5CompletedByCk5Type(Enums.CK5Type ck5Type);

       CK5ReportDto GetCk5ReportDataById(long id);

       //void AddPrintHistory(long id, string userId);

       List<CK5FileUploadDocumentsOutput> CK5UploadFileDocumentsProcess(List<CK5UploadFileDocumentsInput> inputs);

       void InsertListCk5(CK5SaveListInput input);

       CK5XmlDto GetCk5ForXmlById(long id);

       void GovApproveDocumentRollback(CK5WorkflowDocumentInput input);

       void CancelSTOCreatedRollback(CK5WorkflowDocumentInput input);

       GetQuotaAndRemainOutput GetQuotaRemainAndDatePbck1(int pbckId, int exgrouptype);

       GetQuotaAndRemainOutput GetQuotaRemainAndDatePbck1ByCk5Id(long ck5Id);

       //GetQuotaAndRemainOutput GetQuotaRemainAndDatePbck1ByNewCk5(string plantId, DateTime submissionDate,string destPlantNppbkc,int goodtypeid);

       GetQuotaAndRemainOutput GetQuotaRemainAndDatePbck1Item(string plantId,string plantNppbkcId, DateTime submissionDate, string destPlantNppbkcId, int? goodtypegroupid);

       List<CK5> GetByGIDate(int month, int year,string desPlantId, string goodTypeId);

       List<int> GetAllYearsByGiDate();
       List<CK5> GetAllCompletedPortToImporter();
   }
}
