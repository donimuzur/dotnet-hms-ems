using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Contract
{
    public interface IMasterDataAprovalBLL
    {
        T MasterDataApprovalValidation<T>(int pageId, string userId, T oldObject, T newObject, out bool isExist,
            out MASTER_DATA_APPROVAL newApproval, bool isCommit = false);
        void Approve(string userId, int masterApprovalId);

        List<MASTER_DATA_APPROVAL> GetList(Enums.DocumentStatus status = Enums.DocumentStatus.WaitingForMasterApprover);

        List<MASTER_DATA_APPROVAL> GetByPageId(int pageId,
            Enums.DocumentStatus status = Enums.DocumentStatus.WaitingForMasterApprover);

        MASTER_DATA_APPROVAL GetByApprovalId(int approvalId);

        void Reject(string userId, int masterApprovalId);

        bool SendEmailWorkflow(int approvalId);
        bool SendEmailWorkflowByParam(MASTER_DATA_APPROVAL data);
    }
}
