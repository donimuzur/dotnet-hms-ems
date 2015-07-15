using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Contract
{
    public interface IWorkflowBLL
    {
        //List<UserTree> GetUserTree();
        //UserTree GetUserTreeByUserID(int userID);
        bool IsAllowEditDocument(Enums.DocumentStatus status);

        bool AllowApproveAndReject(WorkflowIsAllowedInput input);
    }
}
