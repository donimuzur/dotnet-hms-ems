using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL.Services
{
    public class WasteRoleServices : IWasteRoleServices
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<WASTE_ROLE> _repository;

        private string _includeTables = "T001W, USER";
        
        public WasteRoleServices(IUnitOfWork uow, ILogger logger)
        {
           _uow = uow;
           _logger = logger;
           _repository = _uow.GetGenericRepository<WASTE_ROLE>();

        }


        public bool IsUserWasteApproverByPlant(string userId, string plantId)
        {
            var dbWaste =
                _repository.Get(
                    c => c.USER_ID == userId && c.WERKS == plantId && c.GROUP_ROLE == Enums.WasteGroup.WasteApprover)
                    .FirstOrDefault();

            return dbWaste != null;
        }

        public bool IsUserDisposalTeamByPlant(string userId, string plantId)
        {
            var dbWaste =
                _repository.Get(
                    c => c.USER_ID == userId && c.WERKS == plantId && c.GROUP_ROLE == Enums.WasteGroup.DisposalTeam)
                    .FirstOrDefault();

            return dbWaste != null;
        }

        public List<string> GetListEmailDisposalTeamByPlant(string plantId)
        {
            var result = new List<string>();
            var dbWaste =
                _repository.Get(c => c.WERKS == plantId && c.GROUP_ROLE == Enums.WasteGroup.DisposalTeam, null, "USER").ToList();

            foreach (var wasteRole in dbWaste)
            {
                if (wasteRole.USER != null)
                {
                    result.Add(wasteRole.USER.EMAIL);
                }
            }

            return result;
        }

        public List<string> GetListEmailWasteApprovalByPlant(string plantId)
        {
            var result = new List<string>();
            var dbWaste =
                _repository.Get(c => c.WERKS == plantId && c.GROUP_ROLE == Enums.WasteGroup.WasteApprover, null, "USER").ToList();

            foreach (var wasteRole in dbWaste)
            {
                if (wasteRole.USER != null)
                {
                    result.Add(wasteRole.USER.EMAIL);
                }
            }

            return result;
        }

        public List<string> GetListEmailTransportationAndFactoryLogisticTeamByPlant(string sourcePlant, string destPlant)
        {
            var listPlant = new List<string> {sourcePlant, destPlant};

            var result = new List<string>();
            var dbWaste =
                _repository.Get(c => listPlant.Contains(c.WERKS) 
                    && (c.GROUP_ROLE == Enums.WasteGroup.Transportation || c.GROUP_ROLE == Enums.WasteGroup.FactoryLogistic), 
                    null, "USER").ToList();

            foreach (var wasteRole in dbWaste)
            {
                if (wasteRole.USER != null)
                {
                    result.Add(wasteRole.USER.EMAIL);
                }
            }

            return result.Distinct().ToList();
        }

        public List<string> GetUserDisposalTeamByPlant(string plantId)
        {
            var result = new List<string>();
            var dbWaste =
                _repository.Get(c => c.WERKS == plantId && c.GROUP_ROLE == Enums.WasteGroup.DisposalTeam).ToList();

            foreach (var wasteRole in dbWaste)
            {
                if (wasteRole.USER != null)
                {
                    result.Add(wasteRole.USER_ID);
                }
            }

            return result;
        }
    }
}
