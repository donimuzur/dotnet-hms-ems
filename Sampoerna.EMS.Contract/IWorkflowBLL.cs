using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Contract
{
    public interface IWorkflowBLL
    {
        //List<UserTree> GetUserTree();
        //UserTree GetUserTreeByUserID(int userID);
        bool AllowEditDocument(WorkflowAllowEditAndSubmitInput input);

        bool AllowEditDocumentPbck1(WorkflowAllowEditAndSubmitInput input);

        bool AllowApproveAndReject(WorkflowAllowApproveAndRejectInput input);

        bool AllowGovApproveAndReject(WorkflowAllowApproveAndRejectInput input);

        bool AllowPrint(Enums.DocumentStatus documentStatus);

        bool AllowManagerReject(WorkflowAllowApproveAndRejectInput input);

        bool AllowGiCreated(WorkflowAllowApproveAndRejectInput input);

        bool AllowGrCreated(WorkflowAllowApproveAndRejectInput input);

        bool AllowCancelSAP(WorkflowAllowApproveAndRejectInput input);
    }
}
