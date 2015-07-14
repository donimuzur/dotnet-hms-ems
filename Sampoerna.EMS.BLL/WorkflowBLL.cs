using System;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class WorkflowBLL : IWorkflowBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;

        public WorkflowBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
        }

        public bool CanEditDocument(Enums.DocumentStatus status)
        {
            return status == Enums.DocumentStatus.Draft;
        }

        public bool IsAllowed(WorkflowIsAllowedInput input)
        {
            if (input.DocumentStatus == Enums.DocumentStatus.Draft)
                return false;

            if (input.DocumentStatus == Enums.DocumentStatus.WaitingForApproval)
            {
                if (input.UserRole == Enums.UserRole.Manager)
                    return true;

                if (input.UserRole == Enums.UserRole.POA)
                {
                    //if document is created by user then true
                    //else false

                }

                return false;
            }

            return false;
        }
    }
}
