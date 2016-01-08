using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
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
        
        private string _includeTables = "POA, USER";

        public PoaDelegationServices(IUnitOfWork uow, ILogger logger)
        {
           _uow = uow;
           _logger = logger;
           _repository = _uow.GetGenericRepository<POA_DELEGATION>();

           
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
    }
}
