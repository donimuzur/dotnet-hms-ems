﻿using System;
using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface IPoaDelegationServices
    {
        List<string> GetListPoaDelegateByDate(List<string> listPoa, DateTime date);

        bool IsDelegatedUserByUserAndDate(string userFrom, string userTo, DateTime date);

        string CommentDelegatedUserSaveOrSubmit(string userCreated, string currentUser, DateTime date);

        List<string> GetPoaDelegationToByPoaFromAndDate(string userFrom, DateTime date);

        List<string> GetPoaDelegationFromByPoaToAndDate(string userTo, DateTime date);

        string GetPoaDelegationToFirstByPoaFromAndDate(string userFrom, DateTime date);

        POA_DELEGATION GetPoaDelegationByPoaToAndDate(string poaTo, DateTime date);

        string CommentDelegatedUserApproval(List<string> userApprovedPoa, string currentUser, DateTime date);

        //string CommentDelegatedUserApprovalByNppbkc(string nppbkc, string currentUser, DateTime date);

        //string CommentDelegatedUserApprovalByPlant(string plant, string currentUser, DateTime date);

        string CommentDelegatedByHistory(string workflowHistoryComment, string workflowHistoryActionBy,
            string currentUser, Core.Enums.UserRole currentUserRole, string createdUser, DateTime date);

        string GetEmailDelegateOrOriginalUserByAction(GetEmailDelegateUserInput input);
    }
}
