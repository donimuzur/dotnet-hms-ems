﻿using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class WorkflowHistoryBLL : IWorkflowHistoryBLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;

        private IGenericRepository<WORKFLOW_HISTORY> _repository;
        private IZaidmExPOAMapBLL _poaMapBll;
        private IPOABLL _poaBll;
        private string includeTables = "USER";

        public WorkflowHistoryBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<WORKFLOW_HISTORY>();
            _poaMapBll = new ZaidmExPOAMapBLL(_uow,_logger);
            _poaBll = new POABLL(_uow,_logger);
        }

        public WorkflowHistoryDto GetById(long id)
        {
            return Mapper.Map<WorkflowHistoryDto>(_repository.Get(c => c.WORKFLOW_HISTORY_ID == id, null, includeTables).FirstOrDefault());
        }

        public List<WorkflowHistoryDto> GetByFormTypeAndFormId(GetByFormTypeAndFormIdInput input)
        {
            var dbData =
                _repository.Get(c => c.FORM_TYPE_ID == input.FormType && c.FORM_ID == input.FormId, null, includeTables)
                    .ToList();
            return Mapper.Map<List<WorkflowHistoryDto>>(dbData);
        }

        public WorkflowHistoryDto GetByActionAndFormNumber(GetByActionAndFormNumberInput input)
        {
            var dbData =
                _repository.Get(c => c.ACTION == input.ActionType && c.FORM_NUMBER == input.FormNumber, null,
                    includeTables).FirstOrDefault();
            return Mapper.Map<WorkflowHistoryDto>(dbData);
        }


        public List<WorkflowHistoryDto> GetByFormNumber(string formNumber)
        {
            var dbData =
                _repository.Get(c => c.FORM_NUMBER == formNumber, null, includeTables)
                    .ToList();
            return Mapper.Map<List<WorkflowHistoryDto>>(dbData);
        }


        public void AddHistory(WorkflowHistoryDto history)
        {
            var dbData = Mapper.Map<WORKFLOW_HISTORY>(history);
            _repository.Insert(dbData);
        }

        public void Save(WorkflowHistoryDto history)
        {
            WORKFLOW_HISTORY dbData = null;
            if (history.WORKFLOW_HISTORY_ID > 0)
            {
                dbData = _repository.GetByID(history.WORKFLOW_HISTORY_ID);
                Mapper.Map<WorkflowHistoryDto, WORKFLOW_HISTORY>(history, dbData);
            }
            else
            {
                dbData = Mapper.Map<WORKFLOW_HISTORY>(history);
                _repository.Insert(dbData);
            }
        }

        public List<WorkflowHistoryDto> GetByFormNumber(GetByFormNumberInput input)
        {
            var dbData =
                _repository.Get(c => c.FORM_NUMBER == input.FormNumber, null, includeTables)
                    .ToList();
            var result = Mapper.Map<List<WorkflowHistoryDto>>(dbData);

            if (input.DocumentStatus == Enums.DocumentStatus.WaitingForApproval)
            {
                var rejected = dbData.FirstOrDefault(c => c.ACTION == Enums.ActionType.Reject);
                
                if (rejected != null)
                {
                    //was rejected
                    input.IsRejected = true;
                    input.RejectedBy = rejected.ACTION_BY;
                }

                 result.Add(CreateWaitingApprovalRecord(input));
            }
            else if (input.DocumentStatus == Enums.DocumentStatus.WaitingForApprovalManager)
            {
                result.Add(CreateWaitingApprovalRecord(input));
            }

            return result;
        }

        private WorkflowHistoryDto CreateWaitingApprovalRecord(GetByFormNumberInput input)
        {
            var newRecord = new WorkflowHistoryDto();
            newRecord.FORM_NUMBER = input.FormNumber;
            newRecord.ACTION = Enums.ActionType.WaitingForApproval;


            string displayUserId = "";
            if (input.IsRejected)
            {
                displayUserId = input.RejectedBy;
            }
            else
            {
                if (input.DocumentStatus == Enums.DocumentStatus.WaitingForApproval)
                {
                    var listPoa = _poaMapBll.GetPOAByNPPBKCID(input.NPPBKC_Id);
                    displayUserId = listPoa.Aggregate("", (current, poaMapDto) => current + (poaMapDto.POA_ID + ","));
                    if (displayUserId.Length > 0)
                        displayUserId = displayUserId.Substring(0, displayUserId.Length - 1);

                    newRecord.ROLE = Enums.UserRole.POA;
                }
                else if (input.DocumentStatus == Enums.DocumentStatus.WaitingForApprovalManager)
                {
                    //get action by poa
                    var poaId = GetPoaByDocumentNumber(input.FormNumber);
                    displayUserId = _poaBll.GetManagerIdByPoaId(poaId);
                    //var historyWorkflow =
                    //    _repository.Get(
                    //        c =>
                    //            c.FORM_NUMBER == input.FormNumber && c.ACTION == Enums.ActionType.Approve &&
                    //            c.ROLE == Enums.UserRole.POA).FirstOrDefault();

                    //if (historyWorkflow != null)
                    //{
                    //    displayUserId = _poaBll.GetManagerIdByPoaId(historyWorkflow.ACTION_BY);
                    //}
                    //else
                    //{
                    //    historyWorkflow =
                    //        _repository.Get(
                    //            c =>
                    //                c.FORM_NUMBER == input.FormNumber && c.ACTION == Enums.ActionType.Submit &&
                    //                c.ROLE == Enums.UserRole.POA).FirstOrDefault();

                    //    if (historyWorkflow != null)
                    //    {
                    //        displayUserId = _poaBll.GetManagerIdByPoaId(historyWorkflow.ACTION_BY);
                    //    }

                    //}
                    newRecord.ROLE = Enums.UserRole.Manager;
                }
            }
            

           
            newRecord.UserId = displayUserId;
           

            return newRecord;
        }

        public string GetRejectedPoaByDocumentNumber(string documentNumber)
        {
            string result = "";

            var dbData =
                _repository.Get(
                    c =>
                        c.FORM_NUMBER == documentNumber && c.ACTION == Enums.ActionType.Reject &&
                        c.ROLE == Enums.UserRole.POA).FirstOrDefault();

            if (dbData != null)
                result = dbData.ACTION_BY;

            return result;
        }

        public string GetPoaByDocumentNumber(string documentNumber)
        {
            
            var dtData =
                _repository.Get(
                    c =>
                        c.FORM_NUMBER == documentNumber && c.ACTION == Enums.ActionType.Submit &&
                        c.ROLE == Enums.UserRole.POA).FirstOrDefault();

            if (dtData != null)
                return dtData.ACTION_BY;

            dtData =
                _repository.Get(
                    c =>
                        c.FORM_NUMBER == documentNumber && c.ACTION == Enums.ActionType.Approve &&
                        c.ROLE == Enums.UserRole.POA).FirstOrDefault();
            if (dtData != null)
                return dtData.ACTION_BY;

            return "";

        }
    }
}
