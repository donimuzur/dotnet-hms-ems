﻿using Sampoerna.EMS.BusinessObject.Inputs;
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

        public WorkflowBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;

            _userBll = new UserBLL(_uow,_logger);
            _poabll = new POABLL(_uow,_logger);
            _poaMapBll = new ZaidmExPOAMapBLL(_uow, _logger);
        }

        public bool AllowEditDocument(WorkflowAllowEditAndSubmitInput input)
        {
            if (input.DocumentStatus != Enums.DocumentStatus.Draft)
                return false;

            if (input.CreatedUser != input.CurrentUser)
                return false;

            return true;
        }
        
        private bool IsOneGroup(string createdUser, string currentUserGroup)
        {
            var dbCreatedUser = _userBll.GetUserById(createdUser);
            if (dbCreatedUser == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return dbCreatedUser.USER_GROUP_ID == currentUserGroup;
        }

        /// <summary>
        /// Is in NPPBKC
        /// </summary>
        /// <param name="createdUser"></param>
        /// <param name="approvalUser"></param>
        /// <returns></returns>
        private bool IsOneNPPBKC(string createdUser, string approvalUser)
        {
            var poaCreatedUserData = _poaMapBll.GetByUserLogin(createdUser);
            var poaApprovalUserData = _poaMapBll.GetByUserLogin(approvalUser);
            return poaCreatedUserData != null && poaApprovalUserData != null &&
                   poaApprovalUserData.NPPBKC_ID == poaCreatedUserData.NPPBKC_ID;
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
            
            if (input.DocumentStatus != Enums.DocumentStatus.WaitingForApproval)
                return false;

            if (input.DocumentStatus == Enums.DocumentStatus.WaitingForApproval)
            {
                if (input.UserRole == Enums.UserRole.Manager) //manager need one group
                    //return IsOneGroup(input.CreatedUser, input.CurrentUserGroup);
                    return IsOneNPPBKC(input.CreatedUser, input.CurrentUser);
                
                //if user = poa , should only approve that created by user
                if (input.UserRole == Enums.UserRole.POA)
                {
                    //if created user = poa , false
                    if (_poabll.GetUserRole(input.CreatedUser) == Enums.UserRole.POA)
                        return false;

                    //if document is created by user in one group then true
                    //else false
                    //return IsOneGroup(input.CreatedUser, input.CurrentUserGroup);
                    return IsOneNPPBKC(input.CreatedUser, input.CurrentUser);
                }

                return false;
            }

            return false;
        }

        public bool AllowGovApproveAndReject(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser == input.CurrentUser)
                return false;

            if (input.DocumentStatus != Enums.DocumentStatus.WaitingGovApproval)
                return false;

            if (input.DocumentStatus == Enums.DocumentStatus.WaitingGovApproval)
            {
                if (input.UserRole == Enums.UserRole.Manager)
                    return true;
            }

            return false;

        }


    }
}
