using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL.Services
{
    public class PoaDelegationServices : IPoaDelegationServices
    {
         private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<POA_DELEGATION> _repository;
        private IGenericRepository<WORKFLOW_HISTORY> _repositoryWorkflowHistory;
        
        private string _includeTables = "POA, USER";

        //private IPOABLL _poaBll;
        private IUserBLL _userBll;

        public PoaDelegationServices(IUnitOfWork uow, ILogger logger)
        {
           _uow = uow;
           _logger = logger;
           _repository = _uow.GetGenericRepository<POA_DELEGATION>();
           _repositoryWorkflowHistory = _uow.GetGenericRepository<WORKFLOW_HISTORY>();

            //_poabll = new POABLL(_uow, _logger);
            _userBll = new UserBLL(_uow, _logger);
           
        }

        public List<string> GetListPoaDelegateByDate(List<string> listPoa, DateTime date)
        {
            var inputDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
            var result = _repository.Get(c => listPoa.Contains(c.POA_FROM) && c.DATE_FROM <= inputDate && c.DATE_TO >= inputDate);
            //var result = _repository.Get(c => listPoa.Contains(c.POA_FROM));
            return result.Select(c => c.POA_TO).Distinct().ToList();
        }

        public bool IsDelegatedUserByUserAndDate(string userFrom,string userTo, DateTime date)
        {
            var inputDate = new DateTime(date.Year, date.Month, date.Day);
            var result =
                _repository.Get(
                    c =>
                        c.POA_FROM == userFrom && c.POA_TO == userTo && c.DATE_FROM <= inputDate &&
                        c.DATE_TO >= inputDate).FirstOrDefault();

            return result != null;
        }

        public List<string> GetPoaDelegationToByPoaFromAndDate(string userFrom, DateTime date)
        {
            var inputDate = new DateTime(date.Year, date.Month, date.Day);
            var result =
                _repository.Get(
                    c =>
                        c.POA_FROM == userFrom &&  c.DATE_FROM <= inputDate &&
                        c.DATE_TO >= inputDate).Select(c=>c.POA_TO).ToList();

            return result;
        }

        public string GetPoaDelegationToFirstByPoaFromAndDate(string userFrom, DateTime date)
        {
            var inputDate = new DateTime(date.Year, date.Month, date.Day);
            var result =
                _repository.Get(
                    c =>
                        c.POA_FROM == userFrom && c.DATE_FROM <= inputDate &&
                        c.DATE_TO >= inputDate).FirstOrDefault();

            if (result != null)
                return result.POA_TO;

            return string.Empty;
        }

        public POA_DELEGATION GetPoaDelegationByPoaToAndDate(string poaTo, DateTime date)
        {
            var inputDate = new DateTime(date.Year, date.Month, date.Day);
            var result =
                _repository.Get(
                    c => c.POA_TO == poaTo && c.DATE_FROM <= inputDate && c.DATE_TO >= inputDate).FirstOrDefault();

            return result;
        }
        /// <summary>
        /// for workflow history field comment
        /// if delegated user then return string for comment field
        /// </summary>
        /// <param name="userCreated"></param>
        /// <param name="currentUser"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public string CommentDelegatedUserSaveOrSubmit(string userCreated, string currentUser, DateTime date)
        {
            if (userCreated != currentUser)
            {
                if (IsDelegatedUserByUserAndDate(userCreated, currentUser, date))
                    return Constans.LabelDelegatedBy + userCreated;
            }
            return string.Empty;
        }

        public string CommentDelegatedUserApproval(List<string> userApprovedPoa, string currentUser, DateTime date)
        {
            //user delegate already have access
            if (userApprovedPoa.Contains(currentUser))
                return string.Empty;

            var listPoaDelegate = GetListPoaDelegateByDate(userApprovedPoa, DateTime.Now);

            //poa to delegate exist
            if (listPoaDelegate.Contains(currentUser))
            {
                //get poa from
                var poaDelegation = GetPoaDelegationByPoaToAndDate(currentUser, date);
                if (poaDelegation != null)
                {
                    return Constans.LabelDelegatedBy + poaDelegation.POA_FROM;
                }
            }
            
            return string.Empty;
        }

       
        public string CommentDelegatedByHistory(string workflowHistoryComment, string workflowHistoryActionBy,
            string currentUser, Core.Enums.UserRole currentUserRole,string createdUser, DateTime date)
        {
            string originalPoa = "";

            if (currentUserRole == Enums.UserRole.POA)
            {
                //is the rejected original or delegated
                if (!string.IsNullOrEmpty(workflowHistoryComment) &&
                    workflowHistoryComment.Contains(Constans.LabelDelegatedBy))
                {
                    //rejected by delegated
                    //find the original
                    originalPoa =
                        workflowHistoryComment.Substring(workflowHistoryComment.IndexOf(Constans.LabelDelegatedBy,
                            System.StringComparison.Ordinal));
                    originalPoa = originalPoa.Replace(Constans.LabelDelegatedBy, "");
                    originalPoa = originalPoa.Replace("]", "");

                }
                else
                {
                    originalPoa = workflowHistoryActionBy;
                }
            }
            else
            {
                originalPoa = createdUser;
            }

            if (originalPoa == currentUser)
                return string.Empty;

            //confirm current user is delegated from original
            if (IsDelegatedUserByUserAndDate(originalPoa, currentUser, date))
                return Constans.LabelDelegatedBy + originalPoa;

            if (createdUser != currentUser)
            {
                if (IsDelegatedUserByUserAndDate(createdUser, currentUser, date))
                    return Constans.LabelDelegatedBy + createdUser;
            }

            return string.Empty;

        }

        //private string CommentDelegateUser(int formId, Core.Enums.FormType formType, 
        //    WorkflowHistoryDto workflowHistoryDto, string currentUserId, Enums.UserRole currentUserRole,
        //    string createdUser, DateTime date, string nppbkcId, string plantId)
        //{
        //    string comment = "";

        //    var inputHistory = new GetByFormTypeAndFormIdInput();
        //    inputHistory.FormId = formId;
        //    inputHistory.FormType = formType;

        //    //var rejectedPoa = _workflowHistoryBll.GetApprovedOrRejectedPOAStatusByDocumentNumber(inputHistory);
        //    if (workflowHistoryDto != null)
        //    {
        //        comment = CommentDelegatedByHistory(workflowHistoryDto.COMMENT,
        //            workflowHistoryDto.ACTION_BY, currentUserId, currentUserRole, createdUser, date);
        //    }
        //    else
        //    {
        //        var isPoaCreatedUser = _poaBll.GetActivePoaById(createdUser);
        //        List<string> listPoa;
        //        if (isPoaCreatedUser != null) //if creator = poa
        //        {
        //            listPoa = _poaBll.GetPoaActiveByNppbkcId(nppbkcId).Select(c => c.POA_ID).ToList();
        //        }
        //        else
        //        {
        //            listPoa = _poaBll.GetPoaActiveByPlantId(plantId).Select(c => c.POA_ID).ToList();
        //        }

        //        comment = CommentDelegatedUserApproval(listPoa, currentUserId, date);

        //    }

        //    return comment;
        //}

        public string GetEmailDelegateOrOriginalUserByAction(GetEmailDelegateUserInput input)
        {
            string emailResult = "";


            string userId = "";
            switch (input.ActionType)
            {
                case Enums.ActionType.Submit:
                    if (input.CreatedUser != input.CurrentUser)
                    {
                        if (IsDelegatedUserByUserAndDate(input.CreatedUser, input.CurrentUser, input.Date))
                        {
                            userId = input.CurrentUser;
                        }
                    }
                    break;

                case Enums.ActionType.Approve:
                case Enums.ActionType.Reject:
                case Enums.ActionType.GovApprove:
                case Enums.ActionType.GovPartialApprove:
                    //get original email
                    var dbWorkflowHistory =
                        _repositoryWorkflowHistory.Get(
                            c =>
                                c.FORM_TYPE_ID == input.FormType && c.FORM_ID == input.FormId &&
                                c.FORM_NUMBER == input.FormNumber &&
                                (c.ACTION == Enums.ActionType.Reject || c.ACTION == Enums.ActionType.Approve)).OrderByDescending(c=>c.ACTION_DATE).FirstOrDefault();

                    if (dbWorkflowHistory == null)
                    {
                        
                        if (!input.UserApprovedPoa.Contains(input.CurrentUser))
                        {   //delegated user approve
                            //get original
                            var dbPoa = GetPoaDelegationByPoaToAndDate(input.CurrentUser, input.Date);
                            if (dbPoa != null)
                            {
                                userId = dbPoa.POA_FROM;
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dbWorkflowHistory.COMMENT) &&
                            dbWorkflowHistory.COMMENT.Contains(Constans.LabelDelegatedBy))
                        {
                            var originalUser =
                                dbWorkflowHistory.COMMENT.Substring(
                                    dbWorkflowHistory.COMMENT.IndexOf(Constans.LabelDelegatedBy,
                                        System.StringComparison.Ordinal));
                            originalUser = originalUser.Replace(Constans.LabelDelegatedBy, "");
                            originalUser = originalUser.Replace("]", "");
                            if (originalUser != input.CurrentUser)
                                userId = originalUser;
                        }
                        else
                        {
                            if (dbWorkflowHistory.ACTION_BY != input.CurrentUser)
                                userId = dbWorkflowHistory.ACTION_BY;
                        }
                    }

                    ////get email original user
                    ////get from comment
                    //var originalUser =
                    //    dbWorkflowHistory.COMMENT.Substring(
                    //        dbWorkflowHistory.COMMENT.IndexOf(Constans.LabelDelegatedBy,
                    //            System.StringComparison.Ordinal));
                    //originalUser = originalUser.Replace(Constans.LabelDelegatedBy, "");
                    //originalUser = originalUser.Replace("]", "");
                    //userId = originalUser;
                    

                    break;

            }

            if (!string.IsNullOrEmpty(userId))
            {
                var dbUser = _userBll.GetUserById(userId);
                if (dbUser != null)
                {
                    emailResult = dbUser.EMAIL;
                }
            }
            return emailResult;
        }
    }
}
