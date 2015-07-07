using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Contract
{
    public interface IWorkflowHistoryBLL
    {
        WORKFLOW_HISTORY GetById(long id);

        WORKFLOW_HISTORY GetByActionAndFormId(Enums.ActionType actionType, long formId);

        void AddHistory(WORKFLOW_HISTORY history);

        void Save(WORKFLOW_HISTORY history);

        List<WORKFLOW_HISTORY> GetByFormTypeAndFormId(Enums.FormType formTypeId, long id);
    }
}
