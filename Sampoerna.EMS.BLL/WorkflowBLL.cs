﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Server;
using Sampoerna.EMS.BLL.Services;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Core;
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
        private IUserPlantMapService _userPlantMapService;

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
            _userPlantMapService = new UserPlantMapService(_uow, _logger);
        }

        public bool AllowEditDocument(WorkflowAllowEditAndSubmitInput input)
        {
            if (input.DocumentStatus != Enums.DocumentStatus.Draft && input.DocumentStatus != Enums.DocumentStatus.Rejected)
                return false;

            if (input.CreatedUser != input.CurrentUser)
            {
                return _poaDelegationServices.IsDelegatedUserByUserAndDate(input.CreatedUser, input.CurrentUser,
                    DateTime.Now);
            }

            return true;
        }


        public bool AllowEditDocumentPbck1(WorkflowAllowEditAndSubmitInput input)
        {
            bool allowUser = true;
            if (input.CreatedUser != input.CurrentUser)
            {
                allowUser = _poaDelegationServices.IsDelegatedUserByUserAndDate(input.CreatedUser, input.CurrentUser,
                    DateTime.Now);
            }
            
            if ((input.DocumentStatus == Enums.DocumentStatus.Rejected || input.DocumentStatus == Enums.DocumentStatus.Draft
                || input.DocumentStatus == Enums.DocumentStatus.WaitingGovApproval || input.DocumentStatus == Enums.DocumentStatus.GovRejected) 
                && allowUser)
                return true;

            return false;
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
            if (input.DocumentStatus == Enums.DocumentStatus.WaitingForApproval || input.DocumentStatus == Enums.DocumentStatus.WaitingForApproval2)
            {
                if (input.UserRole != Enums.UserRole.POA)
                    return false;
                
                //created user need to as user
                //if (_poabll.GetUserRole(input.CreatedUser) != Enums.UserRole.User)
                //    return false;

                //if document was rejected then must approve by poa that rejected
                //var rejectedPoa = _workflowHistoryBll.GetApprovedRejectedPoaByDocumentNumber(input.DocumentNumber);
                var rejectedPoa = _workflowHistoryBll.GetDtoApprovedRejectedPoaByDocumentNumber(input.DocumentNumber);
                
                if (rejectedPoa != null)
                {
                 
                    //delegate
                    if (!IsPoaAllowedDelegate(input.DocumentNumber, input.CurrentUser))
                        return false;

                    //end delegate

                    //if (input.CurrentUser != rejectedPoa)
                    //    return false;
                }

                if (input.FormType == Enums.FormType.PBCK3)
                {
                    var rejectedSourcePoa = _workflowHistoryBll.GetApprovedRejectedPoaByDocumentNumber(input.DocumentNumberSource);
                    if (rejectedSourcePoa != "" && rejectedSourcePoa != input.CreatedUser)
                    {
                        //if (input.CurrentUser != rejectedSourcePoa)
                        //    return false;
                        //delegate
                        if (!IsPoaAllowedDelegate(input.DocumentNumberSource, input.CurrentUser))
                            return false;
                        //end delegate
                    }
                }
                else if (input.FormType == Enums.FormType.PBCK1)
                {
                    var lisPoa = _poabll.GetPoaByNppbkcIdAndMainPlant(input.NppbkcId);
                    //add delegate poa too
                    List<string> listUser = lisPoa.Select(c => c.POA_ID).Distinct().ToList();
                    var listPoaDelegate =
                               _poaDelegationServices.GetListPoaDelegateByDate(listUser, DateTime.Now);
                    listUser.AddRange(listPoaDelegate);

                    return listUser.Contains(input.CurrentUser);
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
            
            if (input.DocumentStatus == Enums.DocumentStatus.WaitingForApprovalController)
            {
                if (input.UserRole != Enums.UserRole.Controller)
                    return false;

                //get poa id by document number in workflow history

                //var poaId = _workflowHistoryBll.GetPoaByDocumentNumber(input.DocumentNumber);

                //if (string.IsNullOrEmpty(poaId))
                //    return false;

                //var managerId = _poabll.GetManagerIdByPoaId(poaId);

                //return managerId == input.CurrentUser;

                return true;

            }

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
                string originalPoa;

                if (input.UserRole == Enums.UserRole.Controller)
                    return false;

                if (input.CreatedUser == input.CurrentUser)
                    return true;

                originalPoa = input.CreatedUser;

                ////get delegate if exist
                //var listDelegatedUser = _poaDelegationServices.GetPoaDelegationToByPoaFromAndDate(
                //    input.CreatedUser, DateTime.Now);
                //if (listDelegatedUser.Contains(input.CurrentUser))
                //    return true;

                if (input.UserRole == Enums.UserRole.POA)
                {
                    //get poa Original that already approve or reject
                    var workflowHistoryDto =
                        _workflowHistoryBll.GetDtoApprovedRejectedPoaByDocumentNumber(input.DocumentNumber);

                    if (workflowHistoryDto != null)
                    {

                        if (!string.IsNullOrEmpty(workflowHistoryDto.COMMENT) &&
                            workflowHistoryDto.COMMENT.Contains(Constans.LabelDelegatedBy)) //approve by delegated
                        {
                            //find the original
                            originalPoa =
                                workflowHistoryDto.COMMENT.Substring(
                                    workflowHistoryDto.COMMENT.IndexOf(Constans.LabelDelegatedBy,
                                        System.StringComparison.Ordinal));
                            originalPoa = originalPoa.Replace(Constans.LabelDelegatedBy, "");
                            originalPoa = originalPoa.Replace("]", "");
                        }
                        else
                        {
                            originalPoa = workflowHistoryDto.ACTION_BY;
                        }
                    }


                    ////get poa that already approve or reject
                    //var poaId = _workflowHistoryBll.GetApprovedRejectedPoaByDocumentNumber(input.DocumentNumber);
                    //if (string.IsNullOrEmpty(poaId))
                    //    return false;

                    //if (poaId == input.CurrentUser)
                    //    return true;
                }

                //get delegated user
                var listUser = new List<string>();
                listUser.Add(originalPoa);
                var poaDelegate = _poaDelegationServices.GetPoaDelegationToByPoaFromAndDate(originalPoa,
                    DateTime.Now);

                listUser.AddRange(poaDelegate);

                if (originalPoa != input.CreatedUser)
                {
                    //get delegate for created user too
                    poaDelegate = _poaDelegationServices.GetPoaDelegationToByPoaFromAndDate(input.CreatedUser,
                    DateTime.Now);

                    listUser.AddRange(poaDelegate);
                }

                if (listUser.Contains(input.CurrentUser))
                    return true;


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
                if (input.UserRole == Enums.UserRole.Controller && input.ManagerApprove == input.CurrentUser)
                    return true;
            }

            return false;
        }

        public bool AllowGiCreated(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser != input.CurrentUser)
            {
                if (
                    !_poaDelegationServices.IsDelegatedUserByUserAndDate(input.CreatedUser, input.CurrentUser,
                        DateTime.Now))
                    return false;
            }

            return input.DocumentStatus == Enums.DocumentStatus.GICreated ||
                   input.DocumentStatus == Enums.DocumentStatus.GICompleted ||
                    input.DocumentStatus == Enums.DocumentStatus.WaitingForSealing;
        }

        public bool AllowGrCreated(WorkflowAllowApproveAndRejectInput input)
        {
            //if (input.CreatedUser != input.CurrentUser)
            //{
            //    if (
            //        !_poaDelegationServices.IsDelegatedUserByUserAndDate(input.CreatedUser, input.CurrentUser,
            //            DateTime.Now))
            //        return false;
            //}
            if (!IsUserUnsealing(input))
                return false;

            return input.DocumentStatus == Enums.DocumentStatus.GRCreated ||
                    input.DocumentStatus == Enums.DocumentStatus.GRCompleted ||
                    input.DocumentStatus == Enums.DocumentStatus.WaitingForUnSealing;
        }

        public bool AllowTfPostedPortToImporter(WorkflowAllowApproveAndRejectInput input)
        {
            //if (input.CreatedUser != input.CurrentUser)
            //    if (
            //        !_poaDelegationServices.IsDelegatedUserByUserAndDate(input.CreatedUser, input.CurrentUser,
            //            DateTime.Now))
            //        return false;
            if (!IsUserUnsealing(input))
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
                if (
                   !_poaDelegationServices.IsDelegatedUserByUserAndDate(input.CreatedUser, input.CurrentUser,
                       DateTime.Now))
                    return false;

            return true;
        }
     
        public bool AllowAttachmentCompleted(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.DocumentStatus != Enums.DocumentStatus.Completed) return false;
            if (input.CreatedUser == input.CurrentUser) return true;
            if (input.UserRole == Enums.UserRole.User)
            {
                if (!_poaDelegationServices.IsDelegatedUserByUserAndDate(input.CreatedUser, input.CurrentUser, DateTime.Now))
                    return false;    
            }
            else if (input.UserRole == Enums.UserRole.POA)
            {
                return IsPoaAllowedDelegate(input.DocumentNumber, input.CurrentUser);
                //return input.CurrentUser == input.PoaApprove;
            }
            return false;
        }

        public bool AllowAttachment(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.DocumentStatus <= Enums.DocumentStatus.WaitingGovApproval) return false;
            if (input.CreatedUser == input.CurrentUser) return true;
            if (input.UserRole == Enums.UserRole.User)
            {
                if (!_poaDelegationServices.IsDelegatedUserByUserAndDate(input.CreatedUser, input.CurrentUser, DateTime.Now))
                    return false;
            }
            else if (input.UserRole == Enums.UserRole.POA)
            {
                return IsPoaAllowedDelegate(input.DocumentNumber, input.CurrentUser);
                //return input.CurrentUser == input.PoaApprove;
            }
            return false;
        }

        public bool AllowStoGiCompleted(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser != input.CurrentUser)
                if (
                    !_poaDelegationServices.IsDelegatedUserByUserAndDate(input.CreatedUser, input.CurrentUser,
                        DateTime.Now))
                    return false;

            return input.DocumentStatus == Enums.DocumentStatus.StoRecGICompleted;
        }

        public bool AllowStoGrCreated(WorkflowAllowApproveAndRejectInput input)
        {
            //if (input.CreatedUser != input.CurrentUser)
            //    if (
            //        !_poaDelegationServices.IsDelegatedUserByUserAndDate(input.CreatedUser, input.CurrentUser,
            //            DateTime.Now))
            //        return false;

            if (!IsUserUnsealing(input))
                return false;

            return input.DocumentStatus == Enums.DocumentStatus.StoRecGRCompleted;
        }

        public bool AllowGoodIssue(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser != input.CurrentUser)
                if (
                    !_poaDelegationServices.IsDelegatedUserByUserAndDate(input.CreatedUser, input.CurrentUser,
                        DateTime.Now))
                    return false;

            if (input.Ck5ManualType != Enums.Ck5ManualType.Trial)
                return false;

            return input.DocumentStatus == Enums.DocumentStatus.GoodIssue;
        }

        public bool AllowDomesticAlcoholPurchaseOrder(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser != input.CurrentUser)
                if (
                    !_poaDelegationServices.IsDelegatedUserByUserAndDate(input.CreatedUser, input.CurrentUser,
                        DateTime.Now))
                    return false;

            return input.DocumentStatus == Enums.DocumentStatus.PurchaseOrder;
        }

        public bool AllowDomesticAlcoholGoodIssue(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser != input.CurrentUser)
                if (
                   !_poaDelegationServices.IsDelegatedUserByUserAndDate(input.CreatedUser, input.CurrentUser,
                       DateTime.Now))
                    return false;

            return input.DocumentStatus == Enums.DocumentStatus.GoodIssue;
        }

        public bool AllowDomesticAlcoholGoodReceive(WorkflowAllowApproveAndRejectInput input)
        {
            //if (input.CreatedUser != input.CurrentUser)
            //    if (
            //        !_poaDelegationServices.IsDelegatedUserByUserAndDate(input.CreatedUser, input.CurrentUser,
            //            DateTime.Now))
            //        return false;
            if (!IsUserUnsealing(input))
                return false;

            return input.DocumentStatus == Enums.DocumentStatus.GoodReceive;
        }

        public bool AllowGoodReceive(WorkflowAllowApproveAndRejectInput input)
        {
            //if (input.CreatedUser != input.CurrentUser)
            //    if (
            //        !_poaDelegationServices.IsDelegatedUserByUserAndDate(input.CreatedUser, input.CurrentUser,
            //            DateTime.Now))
            //        return false;
            if (!IsUserUnsealing(input))
                return false;

            if (input.Ck5ManualType != Enums.Ck5ManualType.Trial)
                return false;

            return input.DocumentStatus == Enums.DocumentStatus.GoodReceive;
        }

        public bool AllowWasteGoodIssue(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser != input.CurrentUser)
                if (
                    !_poaDelegationServices.IsDelegatedUserByUserAndDate(input.CreatedUser, input.CurrentUser,
                        DateTime.Now))
                    return false;

            return input.DocumentStatus == Enums.DocumentStatus.GoodIssue;
        }

        public bool AllowWasteGoodReceive(WorkflowAllowApproveAndRejectInput input)
        {
          
            if (input.DocumentStatus != Enums.DocumentStatus.GoodReceive)
                return false;
          
            //if (!IsUserUnsealing(input))
            //    return false;

            //if (input.CreatedUser == input.CurrentUser) return true;

            //return IsOnePlant(input.DestPlant, input.CurrentUser);

            return IsUserUnsealing(input);
        }

        public bool AllowWasteDisposal(WorkflowAllowApproveAndRejectInput input)
        {
            //if (input.CreatedUser != input.CurrentUser)
            //    return false;

            if (!_wasteRoleServices.IsUserDisposalTeamByPlant(input.CurrentUser, input.DestPlant))

            {
                //get delegated disposal poa
                var listDisposal = _wasteRoleServices.GetUserDisposalTeamByPlant(input.DestPlant);
                var poaDelegate = _poaDelegationServices.GetListPoaDelegateByDate(listDisposal, DateTime.Now);
                if (!poaDelegate.Contains(input.CurrentUser))
                    return false;
            }

            return input.DocumentStatus == Enums.DocumentStatus.WasteDisposal;
        }

        //public bool AllowWasteApproval(WorkflowAllowApproveAndRejectInput input)
        //{
        //    //if (input.CreatedUser != input.CurrentUser)
        //    //    return false;

        //    if (!_wasteRoleServices.IsUserWasteApproverByPlant(input.CurrentUser, input.DestPlant))
        //        return false;

        //    return input.DocumentStatus == Enums.DocumentStatus.WasteApproval;
        //}

        private bool IsPoaAllowedDelegate(string documentNumber, string currentUser)
        {
            //get history is poa delegated or poa original
            var listUser = new List<string>();
            var approvedRejectedPoa = _workflowHistoryBll.GetDtoApprovedRejectedPoaByDocumentNumber(documentNumber);
            
            if (approvedRejectedPoa == null) return false;

            string originalPoa;
            if (!string.IsNullOrEmpty(approvedRejectedPoa.COMMENT) &&
                approvedRejectedPoa.COMMENT.Contains(Constans.LabelDelegatedBy))
            {
                //rejected by delegated
                //find the original
                originalPoa =
                    approvedRejectedPoa.COMMENT.Substring(approvedRejectedPoa.COMMENT.IndexOf(Constans.LabelDelegatedBy,
                        System.StringComparison.Ordinal));
                originalPoa = originalPoa.Replace(Constans.LabelDelegatedBy, "");
                originalPoa = originalPoa.Replace("]", "");
            }
            else
            {
                originalPoa = approvedRejectedPoa.ACTION_BY;
            }

            listUser.Add(originalPoa);

            var poaDelegate = _poaDelegationServices.GetPoaDelegationToByPoaFromAndDate(originalPoa, DateTime.Now);
            listUser.AddRange(poaDelegate);

            if (listUser.Contains(currentUser))
                return true;

            return false;
        }

        public bool IsAllowEditLack1(string createdUser, string currentUserId,Enums.DocumentStatus status, Enums.UserRole role, string documentNumber)
        {
            if (_poabll.GetUserRole(currentUserId) == Enums.UserRole.Administrator)
            {
                return true;
            }

            if (status == Enums.DocumentStatus.WaitingGovApproval)
            {
                string originalPoa;

                if (createdUser == currentUserId)
                    return true;

                originalPoa = createdUser;

                if (role == Enums.UserRole.POA)
                {
                    //get poa Original that already approve or reject
                    var workflowHistoryDto =
                        _workflowHistoryBll.GetDtoApprovedRejectedPoaByDocumentNumber(documentNumber);

                    if (workflowHistoryDto != null)
                    {

                        if (!string.IsNullOrEmpty(workflowHistoryDto.COMMENT) &&
                            workflowHistoryDto.COMMENT.Contains(Constans.LabelDelegatedBy)) //approve by delegated
                        {
                            //find the original
                            originalPoa =
                                workflowHistoryDto.COMMENT.Substring(
                                    workflowHistoryDto.COMMENT.IndexOf(Constans.LabelDelegatedBy,
                                        System.StringComparison.Ordinal));
                            originalPoa = originalPoa.Replace(Constans.LabelDelegatedBy, "");
                            originalPoa = originalPoa.Replace("]", "");
                        }
                        else
                        {
                            originalPoa = workflowHistoryDto.ACTION_BY;
                        }
                    }
                }

                //get delegated user
                var listUser = new List<string>();
                listUser.Add(originalPoa);
                var poaDelegate = _poaDelegationServices.GetPoaDelegationToByPoaFromAndDate(originalPoa,
                    DateTime.Now);

                listUser.AddRange(poaDelegate);

                if (originalPoa != createdUser)
                {
                    //get delegate for created user too
                    poaDelegate = _poaDelegationServices.GetPoaDelegationToByPoaFromAndDate(createdUser,
                    DateTime.Now);

                    listUser.AddRange(poaDelegate);
                }

                if (listUser.Contains(currentUserId))
                    return true;


            }

            if (createdUser != currentUserId)
                if (
                    !_poaDelegationServices.IsDelegatedUserByUserAndDate(createdUser, currentUserId,
                        DateTime.Now))
                    return false;

            if (!(status == Enums.DocumentStatus.Draft || status == Enums.DocumentStatus.Rejected
              || status == Enums.DocumentStatus.WaitingGovApproval || status == Enums.DocumentStatus.Completed))
            {
                return false;
            }

            return true;
        }

        public bool AllowApproveAndRejectPbck1(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser == input.CurrentUser)
                return false;

            if (input.DocumentStatus != Enums.DocumentStatus.WaitingForApproval)
                return false;

            if (input.UserRole != Enums.UserRole.POA)
                return false;

            var lisPoa = _poabll.GetPoaByNppbkcIdAndMainPlant(input.NppbkcId);

            //add delegate poa too
            List<string> listUser = lisPoa.Select(c => c.POA_ID).Distinct().ToList();
            var listPoaDelegate =
                       _poaDelegationServices.GetListPoaDelegateByDate(listUser, DateTime.Now);
            listUser.AddRange(listPoaDelegate);

            return listUser.Contains(input.CurrentUser);
        }

        private bool IsUserUnsealing(WorkflowAllowApproveAndRejectInput input)
        {
            if (input.CreatedUser == input.CurrentUser) return true;
            if (_poaDelegationServices.IsDelegatedUserByUserAndDate(input.CreatedUser, input.CurrentUser,DateTime.Now)) 
                return true;
            //return false;
            ////get user by plant 
            //var listUserPlantMap = _userPlantMapService.GetByPlantId(input.PlantId);
            //var listUser = listUserPlantMap.Select(c => c.USER_ID).ToList();
            //if (listUser.Contains(input.CurrentUser))
            //    return true;
            ////and get user by plant delegate
            //var listUserDelegate = _poaDelegationServices.GetListPoaDelegateByDate(listUser, DateTime.Now);
            //return listUserDelegate.Contains(input.CurrentUser);

            //get user by plant 
            var listUser = new List<string>();

            var listUserPlantMap = _userPlantMapService.GetUserBRoleMapByPlantIdAndUserRole(input.DestPlant,
                Enums.UserRole.User);
            listUser.AddRange(listUserPlantMap);

            //list poa
            var listPoa = _poabll.GetPoaActiveByNppbkcId(input.DestNppbkcId);
            
            listUser.AddRange(listPoa.Select(c => c.POA_ID));

            if (listUser.Contains(input.CurrentUser))
                return true;
           

            var listUserDelegate = _poaDelegationServices.GetListPoaDelegateByDate(listUser, DateTime.Now);
            return listUserDelegate.Contains(input.CurrentUser);

        }

        public bool AllowAccessData(WorkflowAllowAccessDataInput input)
        {
            if (input.UserRole == Enums.UserRole.Administrator) return true;
            if (input.UserPlant.Contains(input.DataPlant))
                return true;
            
            //only for edit document
            if (_poaDelegationServices.IsDelegatedUserByUserAndDate(input.DataUser, input.UserId, DateTime.Now))
            {
                if (input.UserPlant.Contains(input.DataPlant))
                    return true;
            }


            return false;
        }
    }
}
