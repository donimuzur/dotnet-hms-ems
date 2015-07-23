using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;

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

    }
}
