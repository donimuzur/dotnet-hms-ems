﻿using System;
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

       List<CK5Dto> GetCK5MarketReturnCompletedByParam(CK5GetByParamInput input);

       CK5Dto SaveCk5(CK5SaveInput input);

       List<CK5Dto> GetCk5ByType(Enums.CK5Type ck5Type);
       List<CK5Dto> GetCk5ByPBCK1(int pbck1Id);
       List<CK5MaterialOutput> CK5MaterialProcess(List<CK5MaterialInput> inputs, Enums.ExGoodsType groupType);

       CK5DetailsOutput GetDetailsCK5(long id);

       List<CK5MaterialDto> GetCK5MaterialByCK5Id(long id);

       void CK5Workflow(CK5WorkflowDocumentInput input);

      // List<CK5Dto> GetSummaryReportsByParam(CK5GetSummaryReportByParamInput input);

       List<Ck5SummaryReportDto> GetSummaryReportsViewByParam(CK5GetSummaryReportByParamInput input);

       List<CK5Dto> GetCk5CompletedByCk5Type(Enums.CK5Type ck5Type);

       CK5ReportDto GetCk5ReportDataById(long id);

       //void AddPrintHistory(long id, string userId);

       List<CK5FileUploadDocumentsOutput> CK5UploadFileDocumentsProcess(List<CK5UploadFileDocumentsInput> inputs);

       void InsertListCk5(CK5SaveListInput input);

       CK5XmlDto GetCk5ForXmlById(long id);

       void GovApproveDocumentRollback(CK5WorkflowDocumentInput input);

       void CancelSTOCreatedRollback(CK5WorkflowDocumentInput input);

       GetQuotaAndRemainOutput GetQuotaRemainAndDatePbck1(int pbckId, int exgrouptype, Enums.CK5Type ck5type);

       GetQuotaAndRemainOutput GetQuotaRemainAndDatePbck1ByCk5Id(long ck5Id);

       //GetQuotaAndRemainOutput GetQuotaRemainAndDatePbck1ByNewCk5(string plantId, DateTime submissionDate,string destPlantNppbkc,int goodtypeid);

       GetQuotaAndRemainOutput GetQuotaRemainAndDatePbck1Item(string plantId,string plantNppbkcId, DateTime submissionDate, string destPlantNppbkcId, int? goodtypegroupid);

       List<int> GetAllYearsByGiDate();
       

       Back1DataOutput GetBack1ByCk5Id(long ck5Id);

       void CK5CompletedAttachment(CK5WorkflowDocumentInput input);

       List<MaterialDto> GetValidateMaterial(string plantId, int goodTypeGroup);

       List<CK5ExternalSupplierDto> GetExternalSupplierList(Enums.CK5Type ck5Type);

       CK5ExternalSupplierDto GetExternalSupplierItem(string plantId, Enums.CK5Type ck5Type);

       GetQuotaAndRemainOutput GetQuotaRemainAndDatePbck1ItemExternal(string plantId, string plantNppbkcId,
           DateTime submissionDate, string destPlantNppbkcId, int? goodtypegroupid);

       List<CK5> GetAllCompletedPortToImporter(long currentCk5ref = 0);

       CK5MaterialOutput ValidateMaterial(CK5MaterialInput input, Enums.ExGoodsType groupType);

       List<CK5MaterialOutput> Ck5MarketReturnMaterialProcess(List<CK5MaterialInput> inputs);

       List<GetListMaterialMarketReturnOutput> GetListMaterialMarketReturn(string plantId);

       CK5MaterialOutput ValidateCk5MarketReturnMaterial(CK5MaterialInput input);

       GetBrandByPlantAndMaterialNumberOutput GetBrandByPlantAndMaterialNumber(string plantId, string materialNumber);

       List<GetListMaterialMarketReturnOutput> GetListMaterialWaste(string plantId);

       WasteStockQuotaOutput GetWasteStockQuota(string plantId, string materialNumber);

       CK5MaterialOutput ValidateCk5WasteMaterial(CK5MaterialInput input);

       List<CK5MaterialOutput> Ck5WasteMaterialProcess(List<CK5MaterialInput> inputs);
       
        void AddAttachmentDocument(CK5WorkflowDocumentInput input);

        List<Ck5MarketReturnSummaryReportDto> GetSummaryReportsMarketReturnByParam(CK5MarketReturnGetSummaryReportByParamInput input);

       List<Ck5MatdocDto> GetMatdocList(long ck5Id = 0);

       void EditCompletedDocument(EditCompletedDocumentCk5Input input);
   }
}
