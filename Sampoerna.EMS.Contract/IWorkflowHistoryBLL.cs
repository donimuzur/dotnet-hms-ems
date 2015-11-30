using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface IWorkflowHistoryBLL
    {
        WorkflowHistoryDto GetById(long id);

        List<WorkflowHistoryDto> GetByFormTypeAndFormId(GetByFormTypeAndFormIdInput input);

        List<WorkflowHistoryDto> GetByFormNumber(string formNumber);

        WorkflowHistoryDto GetByActionAndFormNumber(GetByActionAndFormNumberInput input);

        void AddHistory(WorkflowHistoryDto history);

        void Save(WorkflowHistoryDto history);

        List<WorkflowHistoryDto> GetByFormNumber(GetByFormNumberInput input);

        string GetApprovedRejectedPoaByDocumentNumber(string documentNumber);

        string GetPoaByDocumentNumber(string documentNumber);

        void Delete(long id);

        void DeleteByActionAndFormNumber(GetByActionAndFormNumberInput input);

        GetStatusGovHistoryOutput GetStatusGovHistory(string formNumber);

        List<WorkflowHistoryDto> GetByFormId(GetByFormNumberInput input);

        List<string> GetDocumentByListPOAId(List<string> poaID);

        WorkflowHistoryDto RejectedStatusByDocumentNumber(GetByFormTypeAndFormIdInput input);

        WorkflowHistoryDto GetApprovedOrRejectedPOAStatusByDocumentNumber(GetByFormTypeAndFormIdInput input);

        void UpdateHistoryModifiedForSubmit(WorkflowHistoryDto history);

        
    }
}
