using Sampoerna.EMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.Contract
{
    public interface IWorkflowSettingBLL
    {
        void SaveWorkflowState(WORKFLOW_STATE data);

        List<WORKFLOW_STATE> GetAllByFormId(long id);

        WORKFLOW_STATE GetAllById(long id);
    }
}
