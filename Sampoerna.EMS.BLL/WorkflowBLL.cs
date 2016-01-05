﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Server;
using Sampoerna.EMS.BLL.Services;
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
        private IWasteRoleServices _wasteRoleServices;

        private IPoaDelegationServices _poaDelegationServices;

        public WorkflowBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;

            _userBll = new UserBLL(_uow, _logger);
            _poabll = new POABLL(_uow, _logger);
            _poaMapBll = new ZaidmExPOAMapBLL(_uow, _logger);
            _workflowHistoryBll = new WorkflowHistoryBLL(_uow, _logger);
            _wasteRoleServices = new WasteRoleServices(_uow, _logger);
            _poaDelegationServices = new PoaDelegationServices(_uow, _logger);
        }

        public bool AllowEditDocument(WorkflowAllowEditAndSubmitInput input)
        {
            if (input.DocumentStatus != Enums.DocumentStatus.Draft && input.DocumentStatus != Enums.DocumentStatus.Rejected)
                return false;

            if (input.CreatedUser != input.CurrentUser)
                return false;

            return true;
        }


        public bool AllowEditDocumentPbck1(WorkflowAllowEditAndSubmitInput input)
        {
            bool isEditable = false;

            if ((input.DocumentStatus == Enums.DocumentStatus.Rejected || input.DocumentStatus == Enums.DocumentStatus.Draft  
                || input.DocumentStatus == Enums.DocumentStatus.WaitingGovApproval || input.DocumentStatus == Enums.DocumentStatus.GovRejected) && input.CreatedUser == input.CurrentUser)
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
            //var poaApprovalUserData = _poaMapBll.GetByUserLogin(approvalUser);
            //var data = poaApprovalUserData.Where(c => c.NPPBKC_ID == nppbkcId).ToList();

            //return data.Count > 0;

            var poaApprovalUserData = _poabll.GetPoaActiveByNppbkcId(nppbkcId);

            //add delegate poa too
            List<string> listUser = poaApprovalUserData.Select(c => c.POA_ID).Distinct().ToList();
            var listPoaDelegate =
                       _poaDelegationServices.GetListPoaDelegateByDate(listUser, DateTime.Now);
            listUser.AddRange(listPoaDelegate);

            return listUser.Contains(approvalUser);

            
           
        }

        private bool IsOnePlant(string plantId, string approvalUser)
        {
            var listApprovalUser = _poabll.GetPoaActiveByPlantId(plantId);

            //add delegate poa too
            List<string> listUser = listApprovalUser.Select(c => c.POA_ID).Distinct().ToList();
            var listPoaDelegate =
                       _poaDelegationServices.GetListPoaDelegateByDate(listUser, DateTime.Now);
            listUser.AddRange(listPoaDelegate);
            return  listUser.Contains(approvalUser);

            //var data = listApprovalUser.FirstOrDefault(c => c.POA_ID == approvalUser);

            //return data != null;
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
                //if (_poabll.GetUserRole(input.CreatedUser) != Enums.UserRole.User)
                //    return false;

                //if document was rejected then must approve by poa that rejected
                var rejectedPoa = _workflowHistoryBll.GetApprovedRejectedPoaByDocumentNumber(input.DocumentNumber);
                if (rejectedPoa != "")
                {
                    if (input.CurrentUser != rejectedPoa)
                        return false;
                }

                if (input.FormType == Enums.FormType.PBCK3)
                {
                    var rejectedSourcePoa = _workflowHistoryBll.GetApprovedRejectedPoaByDocumentNumber(input.DocumentNumberSource);
                    if (rejectedSourcePoa != "")
                    {
                        if (input.CurrentUser != rejectedSourcePoa)
                            return false;
                    }
                }

                //poa must be active
                var poa = _poabll.GetActivePoaById(input.CurrentUser);
                if (poa == null)
                    return false;

                var isPoaCreatedUser = _poabll.GetActivePoaById(input.CreatedUser);
                if (isPoaCreatedUser != null)
                {
                    //created user is poa, let's check isOneNppbkc with current user or not
                    return IsOneNppbkc(input.NppbkcId, input.CurrentUser);
                   
                }
                
                return input.PlantId != null ? IsOnePlant(input.PlantId, input.CurrentUser) : IsOneNppbkc(input.NppbkcId, input.CurrentUser);
            }
            
            //if (input.DocumentStatus == Enums.DocumentStatus.WaitingForApprovalManager)
            //{
            //    if (input.UserRole != Enums.UserRole.Manager)
            //        return false;

            //    //get poa id by document number in workflow history

            //    var poaId = _workflowHistoryBll.GetPoaByDocumentNumber(input.DocumentNumber);

            //    if (string.IsNullOrEmpty(poaId))
            //        return false;

            //    var managerId = _poabll.GetManagerIdByPoaId(poaId);

            //    return managerId == input.CurrentUser;

            //}

            return false;
          
        }

        public bool AllowGovApproveAndReject(WorkflowAllowApproveAndRejectInput input)
        {
            //if (input.CreatedUser == input.CurrentUser)
            //    return false;
            var completedEdit = false;
            if(input.FormType == Enums.FormType.PBCK1 && input.DocumentStatus == Enums.DocumentStatus.Completed){
                completedEdit = true;
            }

            if (input.DocumentStatus != Enums.DocumentStatus.WaitingGovApproval && !completedEdit)
                return false;

            if (input.DocumentStatus == Enums.DocumentStatus.WaitingGovApproval || completedEdit)
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
                if (input.UserRole == Enums.UserRole.Manager && input.ManagerApprove == input.CurrentUser)
                    return true;
            }

            return false;
        }

        public bool AllowGiCreated(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser != input.CurrentUser)
                return false;

            return input.DocumentStatus == Enums.DocumentStatus.GICreated ||
                   input.DocumentStatus == Enums.DocumentStatus.GICompleted ||
                    input.DocumentStatus == Enums.DocumentStatus.WaitingForSealing;
        }

        public bool AllowGrCreated(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser != input.CurrentUser)
                return false;
            
            return input.DocumentStatus == Enums.DocumentStatus.GRCreated ||
                    input.DocumentStatus == Enums.DocumentStatus.GRCompleted ||
                    input.DocumentStatus == Enums.DocumentStatus.WaitingForUnSealing;
        }

        public bool AllowTfPostedPortToImporter(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser != input.CurrentUser)
                return false;

            return input.DocumentStatus == Enums.DocumentStatus.TFPosted;
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

        public bool AllowAttachmentCompleted(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.DocumentStatus != Enums.DocumentStatus.Completed) return false;
            if (input.CreatedUser == input.CurrentUser) return true;
            if (input.UserRole == Enums.UserRole.POA)
            {
                return input.CurrentUser == input.PoaApprove;
            }
            return false;
        }

        public bool AllowAttachment(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.DocumentStatus <= Enums.DocumentStatus.WaitingGovApproval) return false;
            if (input.CreatedUser == input.CurrentUser) return true;
            if (input.UserRole == Enums.UserRole.POA)
            {
                return input.CurrentUser == input.PoaApprove;
            }
            return false;
        }

        public bool AllowStoGiCompleted(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser != input.CurrentUser)
                return false;

            return input.DocumentStatus == Enums.DocumentStatus.StoRecGICompleted;
        }

        public bool AllowStoGrCreated(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser != input.CurrentUser)
                return false;

            return input.DocumentStatus == Enums.DocumentStatus.StoRecGRCompleted;
        }

        public bool AllowGoodIssue(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser != input.CurrentUser)
                return false;

            if (input.Ck5ManualType != Enums.Ck5ManualType.Trial)
                return false;

            return input.DocumentStatus == Enums.DocumentStatus.GoodIssue;
        }

        public bool AllowDomesticAlcoholPurchaseOrder(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser != input.CurrentUser)
                return false;

            return input.DocumentStatus == Enums.DocumentStatus.PurchaseOrder;
        }

        public bool AllowDomesticAlcoholGoodIssue(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser != input.CurrentUser)
                return false;

            return input.DocumentStatus == Enums.DocumentStatus.GoodIssue;
        }

        public bool AllowDomesticAlcoholGoodReceive(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser != input.CurrentUser)
                return false;

            return input.DocumentStatus == Enums.DocumentStatus.GoodReceive;
        }

        public bool AllowGoodReceive(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser != input.CurrentUser)
                return false;

            if (input.Ck5ManualType != Enums.Ck5ManualType.Trial)
                return false;

            return input.DocumentStatus == Enums.DocumentStatus.GoodReceive;
        }

        public bool AllowWasteGoodIssue(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser != input.CurrentUser)
                return false;

            return input.DocumentStatus == Enums.DocumentStatus.GoodIssue;
        }

        public bool AllowWasteGoodReceive(WorkflowAllowApproveAndRejectInput input)
        {
            //if (input.CreatedUser != input.CurrentUser)
            //    return false;

            if (input.DocumentStatus != Enums.DocumentStatus.GoodReceive)
                return false;

            if (input.CreatedUser == input.CurrentUser)
                return true;

            return IsOnePlant(input.DestPlant, input.CurrentUser);

        }

        public bool AllowWasteDisposal(WorkflowAllowApproveAndRejectInput input)
        {
            //if (input.CreatedUser != input.CurrentUser)
            //    return false;

            if (!_wasteRoleServices.IsUserDisposalTeamByPlant(input.CurrentUser, input.DestPlant))
                return false;

            return input.DocumentStatus == Enums.DocumentStatus.WasteDisposal;
        }

        public bool AllowWasteApproval(WorkflowAllowApproveAndRejectInput input)
        {
            //if (input.CreatedUser != input.CurrentUser)
            //    return false;

            if (!_wasteRoleServices.IsUserWasteApproverByPlant(input.CurrentUser, input.DestPlant))
                return false;

            return input.DocumentStatus == Enums.DocumentStatus.WasteApproval;
        }
    }
}
