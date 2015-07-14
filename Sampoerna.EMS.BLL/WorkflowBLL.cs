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
    }
}
