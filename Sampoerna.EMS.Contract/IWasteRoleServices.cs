using System.Collections.Generic;

namespace Sampoerna.EMS.Contract
{
    public interface IWasteRoleServices
    {
        bool IsUserWasteApproverByPlant(string userId, string plantId);

        bool IsUserDisposalTeamByPlant(string userId, string plantId);

        List<string> GetListEmailDisposalTeamByPlant(string plantId);

        List<string> GetListEmailWasteApprovalByPlant(string plantId);

        List<string> GetListEmailTransportationAndFactoryLogisticTeamByPlant(string sourcePlant, string destPlant);

        List<string> GetUserDisposalTeamByPlant(string plantId);

        string GetPlantIdByUserId(string userId);
    }
}
