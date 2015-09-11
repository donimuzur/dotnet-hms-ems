using System;
using System.Linq;
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

        public bool AllowEditDocumentPbck1(WorkflowAllowEditAndSubmitInput input)
        {
            bool isEditable = false;

            if (input.DocumentStatus == Enums.DocumentStatus.Draft || input.DocumentStatus == Enums.DocumentStatus.WaitingGovApproval)
                isEditable = true;
            else
                isEditable = false;

            if (input.CreatedUser == input.CurrentUser)
                isEditable = true;
            else
                isEditable = false;

            return isEditable;
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

            //return nppbkcId == poaApprovalUserData.NPPBKC_ID;
            var data = poaApprovalUserData.Where(c => c.NPPBKC_ID == nppbkcId).ToList();

            return data.Count > 0;

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
                var rejectedPoa = _workflowHistoryBll.GetApprovedRejectedPoaByDocumentNumber(input.DocumentNumber);
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

                //if (input.CreatedUser == input.CurrentUser && input.UserRole == Enums.UserRole.User)
                //    return true;

                //allow poa and creator
                if (input.CreatedUser == input.CurrentUser)
                    return true;

                if (input.UserRole == Enums.UserRole.POA)
                {
                   
                    //get poa that already approve or reject
                    var poaId = _workflowHistoryBll.GetApprovedRejectedPoaByDocumentNumber(input.DocumentNumber);
                    if (string.IsNullOrEmpty(poaId))
                        return false;

                    if (poaId == input.CurrentUser)
                        return true;
                }

            }

            return false;

        }

        public bool AllowPrint(Enums.DocumentStatus documentStatus)
        {
            int iStatusAllow = Convert.ToInt32(Enums.DocumentStatus.WaitingGovApproval);

            int currentStatus = Convert.ToInt32(documentStatus);

            return currentStatus >= iStatusAllow;
        }


        public bool AllowManagerReject(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.DocumentStatus == Enums.DocumentStatus.WaitingGovApproval)
            {
                if (input.UserRole == Enums.UserRole.Manager)
                    return true;
            }

            return false;
        }

        public bool AllowGiCreated(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser != input.CurrentUser)
                return false;

            return input.DocumentStatus == Enums.DocumentStatus.GICreated ||
                   input.DocumentStatus == Enums.DocumentStatus.GICompleted;
        }

        public bool AllowGrCreated(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser != input.CurrentUser)
                return false;

            return input.DocumentStatus == Enums.DocumentStatus.GRCreated ||
                    input.DocumentStatus == Enums.DocumentStatus.GRCompleted;
        }

        public bool AllowCancelSAP(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.DocumentStatus < Enums.DocumentStatus.CreateSTO)
                return false;
            if (input.DocumentStatus == Enums.DocumentStatus.Cancelled ||
                input.DocumentStatus == Enums.DocumentStatus.Completed)
                return false;
            if (input.CreatedUser != input.CurrentUser)
                return false;

            return true;
        }

    }
}
