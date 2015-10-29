using System;
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
        SavePbck1Output Save(Pbck1WorkflowDocumentInput input);
        void Delete(long id);

        string GetPbckNumberById(long id);

        List<Pbck1ProdConverterOutput> ValidatePbck1ProdConverterUpload(List<Pbck1ProdConverterInput> inputs, string nppbkc, bool isCheckedPbck1Import);

        ValidatePbck1ProdPlanUploadOutput ValidatePbck1ProdPlanUpload(ValidatePbck1ProdPlanUploadParamInput input);

        void Pbck1Workflow(Pbck1WorkflowDocumentInput input);

        List<Pbck1SummaryReportDto> GetSummaryReportByParam(Pbck1GetSummaryReportByParamInput input);

        List<Pbck1MonitoringUsageDto> GetMonitoringUsageByParam(Pbck1GetMonitoringUsageByParamInput input);

        Pbck1ReportDto GetPrintOutDataById(int id);

        Pbck1Dto GetByDocumentNumber(string documentNumber);

        List<ZAIDM_EX_NPPBKCCompositeDto> GetNppbkByCompanyCode(string companyCode);
        void UpdateReportedOn(Pbck1UpdateReportedOn input);

        List<Pbck1Dto> GetAllPbck1ByPbck1Ref(int pbckRef);

        List<Pbck1Dto> GetPbck1CompletedDocumentByPlant(string plant);

        List<Pbck1Dto> GetPbck1CompletedDocumentByPlantAndSubmissionDate(string plantId, string plantNppbkcId, DateTime? submissionDate, string destPlantNppbkcId, List<string> goodtypes);

        List<ZAIDM_EX_GOODTYPCompositeDto> GetGoodsTypeByNppbkcId(string nppbkcId);

        List<T001WCompositeDto> GetSupplierPlantByParam(Pbck1GetSupplierPlantByParamInput input);

        string checkUniquePBCK1(Pbck1SaveInput pbck1);
        Pbck1Dto GetPBCK1Reference(Pbck1ReferenceSearchInput input);

        List<CK5ExternalSupplierDto> GetExternalSupplierList(List<string> goodTypeList = null);

        List<Pbck1Dto> GetPbck1CompletedDocumentByExternalAndSubmissionDate(string exSupplierId, string exSupplierNppbkcId,
            DateTime? submissionDate, string destPlantNppbkcId, List<string> goodtypes);

        List<Pbck1Dto> GetByRef(int pbckId);
    }
}
