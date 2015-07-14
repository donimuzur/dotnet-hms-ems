using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Contract
{
    public interface IWorkflowBLL
    {
        //List<UserTree> GetUserTree();
        //UserTree GetUserTreeByUserID(int userID);
        bool CanEditDocument(Enums.DocumentStatus status);
    }
}
