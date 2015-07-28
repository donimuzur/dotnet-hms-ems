using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class WorkflowBLL : IWorkflowBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IUserBLL _userBll;
        private IPOABLL _poabll;
        private IZaidmExPOAMapBLL _poaMapBll;
        private IWorkflowHistoryBLL _workflowHistoryBll;

        public WorkflowBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;

            _userBll = new UserBLL(_uow, _logger);
            _poabll = new POABLL(_uow, _logger);
            _poaMapBll = new ZaidmExPOAMapBLL(_uow, _logger);
            _workflowHistoryBll = new WorkflowHistoryBLL(_uow, _logger);
        }

        public bool AllowEditDocument(WorkflowAllowEditAndSubmitInput input)
        {
            if (input.DocumentStatus != Enums.DocumentStatus.Draft)
                return false;

            if (input.CreatedUser != input.CurrentUser)
                return false;

            return true;
        }

        /// <summary>
        /// Is in NPPBKC
        /// </summary>
        /// <param name="nppbkcId"></param>
        /// <param name="approvalUser"></param>
        /// <returns></returns>
        private bool IsOneNppbkc(string nppbkcId, string approvalUser)
        {
            var poaApprovalUserData = _poaMapBll.GetByUserLogin(approvalUser);
            
            return nppbkcId == poaApprovalUserData.NPPBKC_ID;
            //var poaCreatedUserData = _poaMapBll.GetByUserLogin(createdUser);
            //var poaApprovalUserData = _poaMapBll.GetByUserLogin(approvalUser);
            //return poaCreatedUserData != null && poaApprovalUserData != null &&
            //       poaApprovalUserData.NPPBKC_ID == poaCreatedUserData.NPPBKC_ID;
        }

        /// <summary>
        /// allow to approve and rejected
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool AllowApproveAndReject(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser == input.CurrentUser)
                return false;


            //need approve by POA only
            if (input.DocumentStatus == Enums.DocumentStatus.WaitingForApproval)
            {
                if (input.UserRole != Enums.UserRole.POA)
                    return false;
                
                //created user need to as user
                if (_poabll.GetUserRole(input.CreatedUser) != Enums.UserRole.User)
                    return false;

                //if document was rejected then must approve by poa that rejected
                var rejectedPoa = _workflowHistoryBll.GetRejectedPoaByDocumentNumber(input.DocumentNumber);
                if (rejectedPoa != "")
                {
                    if (input.CurrentUser != rejectedPoa)
                        return false;
                }

                return IsOneNppbkc(input.NppbkcId, input.CurrentUser);
            }
            
            if (input.DocumentStatus == Enums.DocumentStatus.WaitingForApprovalManager)
            {
                if (input.UserRole != Enums.UserRole.Manager)
                    return false;

                //get poa id by document number in workflow history

                var poaId = _workflowHistoryBll.GetPoaByDocumentNumber(input.DocumentNumber);

                if (string.IsNullOrEmpty(poaId))
                    return false;

                var managerId = _poabll.GetManagerIdByPoaId(poaId);

                return managerId == input.CurrentUser;

            }

            return false;
          
        }

        public bool AllowGovApproveAndReject(WorkflowAllowApproveAndRejectInput input)
        {
            //if (input.CreatedUser == input.CurrentUser)
            //    return false;

            if (input.DocumentStatus != Enums.DocumentStatus.WaitingGovApproval)
                return false;

            if (input.DocumentStatus == Enums.DocumentStatus.WaitingGovApproval)
            {
                if (input.UserRole == Enums.UserRole.Manager)
                    return false;

                if (input.CreatedUser == input.CurrentUser)
                    return true;
            }

            return false;

        }


    }
}
