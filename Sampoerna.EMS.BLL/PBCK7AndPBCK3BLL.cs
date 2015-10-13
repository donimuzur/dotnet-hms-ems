using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{

    public class PBCK7AndPBCK3BLL : IPBCK7And3BLL
    {
        private ILogger _logger;
        private IGenericRepository<PBCK3_PBCK7> _repository;
        private IGenericRepository<PBCK7> _repositoryPbck7;
        private IGenericRepository<PBCK3> _repositoryPbck3;
        private IGenericRepository<BACK1> _repositoryBack1;
        private IGenericRepository<BACK3> _repositoryBack3;
        private IGenericRepository<CK2> _repositoryCk2;
        private IGenericRepository<PBCK7_ITEM> _repositoryPbck7Item; 
        private IZaidmExNPPBKCBLL _nppbkcbll;
        private IUnitOfWork _uow;
        private IBACK1BLL _back1Bll;
        private IPOABLL _poabll;
        private string includeTable = "PBCK7_ITEM";
        private WorkflowHistoryBLL _workflowHistoryBll;

        private IDocumentSequenceNumberBLL _docSeqNumBll;

        public PBCK7AndPBCK3BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _back1Bll = new BACK1BLL(_uow, _logger);
            _poabll = new POABLL(_uow, _logger);
            _repositoryPbck7 = _uow.GetGenericRepository<PBCK7>();
            _repositoryPbck3 = _uow.GetGenericRepository<PBCK3>();
            _workflowHistoryBll = new WorkflowHistoryBLL(_uow, logger);
            _repository = _uow.GetGenericRepository<PBCK3_PBCK7>();
            _repositoryBack1 = _uow.GetGenericRepository<BACK1>();
            _repositoryBack3 = _uow.GetGenericRepository<BACK3>();
            _repositoryCk2 = _uow.GetGenericRepository<CK2>();
            _nppbkcbll = new ZaidmExNPPBKCBLL(_uow, logger);
            _repositoryPbck7Item = _uow.GetGenericRepository<PBCK7_ITEM>();

            _docSeqNumBll = new DocumentSequenceNumberBLL(_uow,_logger);
        }

        public List<Pbck7AndPbck3Dto> GetAllPbck7()
        {
            return Mapper.Map<List<Pbck7AndPbck3Dto>>(_repositoryPbck7.Get().ToList());
        }

        public List<Pbck3Dto> GetAllPbck3()
        {
            return Mapper.Map<List<Pbck3Dto>>(_repositoryPbck3.Get().ToList()); ;
        }

       
        public List<Pbck7AndPbck3Dto> GetPbck7SummaryReportsByParam(Pbck7SummaryInput input)
        {
            Expression<Func<PBCK7, bool>> queryFilter = PredicateHelper.True<PBCK7>();
            if (!string.IsNullOrEmpty(input.NppbkcId))
            {
                queryFilter = queryFilter.And(c => c.NPPBKC == input.NppbkcId);
            }
            if (!string.IsNullOrEmpty(input.PlantId))
            {
                queryFilter = queryFilter.And(c => c.PLANT_ID == input.PlantId);
            }
            if (!string.IsNullOrEmpty(input.Pbck7Number))
            {
                queryFilter = queryFilter.And(c => c.PBCK7_NUMBER == input.Pbck7Number);
            }

            if (input.From != null)
            {
                
                queryFilter = queryFilter.And(c => c.PBCK7_DATE.Year >= input.From);
            }
            if (input.To != null)
            {

                queryFilter = queryFilter.And(c => c.PBCK7_DATE.Year <= input.To);
            }


            Func<IQueryable<PBCK7>, IOrderedQueryable<PBCK7>> orderBy = null;
            if (!string.IsNullOrEmpty(input.ShortOrderColum))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<PBCK7>(input.ShortOrderColum));
            }

            var dbData = _repositoryPbck7.Get(queryFilter, orderBy);
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            var mapResult = Mapper.Map<List<Pbck7AndPbck3Dto>>(dbData.ToList());
            foreach (var pbck7AndPbck3Dto in mapResult)
            {
                pbck7AndPbck3Dto.Back1Dto = GetBack1ByPbck7(pbck7AndPbck3Dto.Pbck7Id);
            }
            return mapResult;
       
        }

        public List<Pbck3Dto> GetPbck3SummaryReportsByParam(Pbck3SummaryInput input)
        {
            Expression<Func<PBCK3, bool>> queryFilter = PredicateHelper.True<PBCK3>();
            if (!string.IsNullOrEmpty(input.NppbkcId))
            {
                queryFilter = queryFilter.And(c => c.PBCK7.NPPBKC == input.NppbkcId);
            }
            if (!string.IsNullOrEmpty(input.PlantId))
            {
                queryFilter = queryFilter.And(c => c.PBCK7.PLANT_ID == input.PlantId);
            }
            if (!string.IsNullOrEmpty(input.Pbck3Number))
            {
                queryFilter = queryFilter.And(c => c.PBCK3_NUMBER == input.Pbck3Number);
            }

            if (input.From != null)
            {

                queryFilter = queryFilter.And(c => c.PBCK3_DATE.Year >= input.From);
            }
            if (input.To != null)
            {

                queryFilter = queryFilter.And(c => c.PBCK3_DATE.Year <= input.To);
            }


            Func<IQueryable<PBCK3>, IOrderedQueryable<PBCK3>> orderBy = null;
            if (!string.IsNullOrEmpty(input.ShortOrderColum))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<PBCK3>(input.ShortOrderColum));
            }

            var dbData = _repositoryPbck3.Get(queryFilter, orderBy, "PBCK7");
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            var mapResult = Mapper.Map<List<Pbck3Dto>>(dbData.ToList());
            foreach (var pbck3Dto in mapResult)
            {
                pbck3Dto.Back3Dto = GetBack3ByPbck3Id(pbck3Dto.Pbck3Id);
                pbck3Dto.Ck2Dto = GetCk2ByPbck3Id(pbck3Dto.Pbck3Id);
            }
            return mapResult;
        }

        public List<Pbck7AndPbck3Dto> GetPbck7ByParam(Pbck7AndPbck3Input input, Login user, bool IsComplete=false)
        {
            Expression<Func<PBCK7, bool>> queryFilter = PredicateHelper.True<PBCK7>();

            if (user.UserRole == Enums.UserRole.POA)
            {
                var nppbkc = _nppbkcbll.GetNppbkcsByPOA(user.USER_ID).Select(d => d.NPPBKC_ID).ToList();

                queryFilter = queryFilter.And(c => (c.CREATED_BY == user.USER_ID || (c.STATUS != Enums.DocumentStatus.Draft && nppbkc.Contains(c.NPPBKC))));


            }
            else if (user.UserRole == Enums.UserRole.Manager)
            {
                var poaList = _poabll.GetPOAIdByManagerId(user.USER_ID);
                var document = _workflowHistoryBll.GetDocumentByListPOAId(poaList);

                queryFilter = queryFilter.And(c => c.STATUS != Enums.DocumentStatus.Draft && c.STATUS != Enums.DocumentStatus.WaitingForApproval && document.Contains(c.PBCK7_NUMBER));
            }
            else
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == user.USER_ID);
            }
            if (IsComplete)
            {
                queryFilter = queryFilter.And(c => c.STATUS == Enums.DocumentStatus.Completed);
            }
            else
            {
                queryFilter = queryFilter.And(c => c.STATUS != Enums.DocumentStatus.Completed);
            }


            if (!string.IsNullOrEmpty(input.NppbkcId))
            {
                queryFilter = queryFilter.And(c => c.NPPBKC == input.NppbkcId);
            }
            if (!string.IsNullOrEmpty(input.PlantId))
            {
                queryFilter = queryFilter.And(c => c.PLANT_ID == input.PlantId);
            }
            if (!string.IsNullOrEmpty(input.Poa))
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY == input.Poa);
            }
            if (!string.IsNullOrEmpty(input.Creator))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.Creator);
            }
            if (!string.IsNullOrEmpty((input.Pbck7Date)))
            {
                var dt = Convert.ToDateTime(input.Pbck7Date);
                queryFilter = queryFilter.And(c => c.PBCK7_DATE == dt);
            }
           

            Func<IQueryable<PBCK7>, IOrderedQueryable<PBCK7>> orderBy = null;
            if (!string.IsNullOrEmpty(input.ShortOrderColum))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<PBCK7>(input.ShortOrderColum));
            }

            var dbData = _repositoryPbck7.Get(queryFilter, orderBy, includeTable);
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            var mapResult = Mapper.Map<List<Pbck7AndPbck3Dto>>(dbData.ToList());

            return mapResult;
        }

        public List<Pbck3Dto> GetPbck3ByParam(Pbck7AndPbck3Input input, Login user, bool IsComplete=false)
        {
            Expression<Func<PBCK3, bool>> queryFilter = PredicateHelper.True<PBCK3>();

            if (user.UserRole == Enums.UserRole.POA)
            {
                var nppbkc = _nppbkcbll.GetNppbkcsByPOA(user.USER_ID).Select(d => d.NPPBKC_ID).ToList();

                queryFilter = queryFilter.And(c => (c.CREATED_BY == user.USER_ID || (c.STATUS != Enums.DocumentStatus.Draft && nppbkc.Contains(c.PBCK7.NPPBKC))));


            }
            else if (user.UserRole == Enums.UserRole.Manager)
            {
                var poaList = _poabll.GetPOAIdByManagerId(user.USER_ID);
                var document = _workflowHistoryBll.GetDocumentByListPOAId(poaList);

                queryFilter = queryFilter.And(c => c.STATUS != Enums.DocumentStatus.Draft && c.STATUS != Enums.DocumentStatus.WaitingForApproval && document.Contains(c.PBCK3_NUMBER));
            }
            else
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == user.USER_ID);
            }
            if (!string.IsNullOrEmpty(input.NppbkcId))
            {
                queryFilter = queryFilter.And(c => c.PBCK7.NPPBKC == input.NppbkcId);
            }
            if (!string.IsNullOrEmpty(input.PlantId))
            {
                queryFilter = queryFilter.And(c => c.PBCK7.PLANT_ID == input.PlantId);
            }
            if (!string.IsNullOrEmpty(input.Poa))
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY == input.Poa);
            }
            if (!string.IsNullOrEmpty(input.Creator))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.Creator);
            }
            if (!string.IsNullOrEmpty((input.Pbck3Date)))
            {
                var dt = Convert.ToDateTime(input.Pbck3Date);
                queryFilter = queryFilter.And(c => c.PBCK3_DATE == dt);
            }

            if (IsComplete)
            {
                queryFilter = queryFilter.And(c => c.STATUS == Enums.DocumentStatus.Completed);
            }
            else
            {
                queryFilter = queryFilter.And(c => c.STATUS != Enums.DocumentStatus.Completed);
            }
            Func<IQueryable<PBCK3>, IOrderedQueryable<PBCK3>> orderBy = null;
            if (!string.IsNullOrEmpty(input.ShortOrderColum))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<PBCK3>(input.ShortOrderColum));
            }

            var dbData = _repositoryPbck3.Get(queryFilter, orderBy, "PBCK7");
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            var mapResult = Mapper.Map<List<Pbck3Dto>>(dbData.ToList());
            return mapResult;
        }

        public Pbck7AndPbck3Dto GetPbck7ById(int? id)
        {
            var result = _repositoryPbck7.Get(x => x.PBCK7_ID == id, null, includeTable).FirstOrDefault();
            if(result == null)
                return  new Pbck7AndPbck3Dto();
            var pbck7 = Mapper.Map<Pbck7AndPbck3Dto>(result);
            pbck7.Back1Dto = Mapper.Map<Back1Dto>(result.BACK1.LastOrDefault());
            return pbck7;
        }

        public void Insert(Pbck7AndPbck3Dto pbck7AndPbck3Dto)
        {
            var dataToAdd = Mapper.Map<PBCK3_PBCK7>(pbck7AndPbck3Dto);
            _repository.InsertOrUpdate(dataToAdd);
            _uow.SaveChanges();

            var history = new WorkflowHistoryDto();
            history.FORM_ID = dataToAdd.PBCK3_PBCK7_ID;
            if (pbck7AndPbck3Dto.Pbck3Dto.Pbck3Status == Enums.DocumentStatus.Draft)
            {
                history.ACTION = GetActionTypePbck7(pbck7AndPbck3Dto, pbck7AndPbck3Dto.ModifiedBy);
                history.ACTION_BY = GetActionByPbck7(pbck7AndPbck3Dto);
                history.ACTION_DATE = DateTime.Now;
                history.FORM_NUMBER = pbck7AndPbck3Dto.Pbck7Number;
            }
            history.FORM_TYPE_ID = Enums.FormType.PBCK7;
            history.COMMENT = pbck7AndPbck3Dto.Comment;
            //set workflow history
            var getUserRole = _poabll.GetUserRole(history.ACTION_BY);
            history.ROLE = getUserRole;
            _workflowHistoryBll.AddHistory(history);
            _uow.SaveChanges();
        }

        public int? InsertPbck7(Pbck7AndPbck3Dto pbck7AndPbck3Dto)
        {
            var dataToAdd = Mapper.Map<PBCK7>(pbck7AndPbck3Dto);
            _repositoryPbck7.InsertOrUpdate(dataToAdd);
            _uow.SaveChanges();

            var history = new WorkflowHistoryDto();
            history.FORM_ID = dataToAdd.PBCK7_ID;
            history.ACTION = GetActionTypePbck7(pbck7AndPbck3Dto, pbck7AndPbck3Dto.ModifiedBy);
            history.ACTION_BY = GetActionByPbck7(pbck7AndPbck3Dto);
            history.ACTION_DATE = DateTime.Now;
            history.FORM_NUMBER = pbck7AndPbck3Dto.Pbck7Number;
            history.FORM_TYPE_ID = Enums.FormType.PBCK7;
            history.COMMENT = pbck7AndPbck3Dto.Comment;
            //set workflow history
            var getUserRole = _poabll.GetUserRole(history.ACTION_BY);
            history.ROLE = getUserRole;
            _workflowHistoryBll.AddHistory(history);
            _uow.SaveChanges();
            return dataToAdd.PBCK7_ID;
        }

        public void InsertPbck7Item(Pbck7ItemUpload item)
        {
            var uploadItemToAdd = Mapper.Map<PBCK7_ITEM>(item);
            _repositoryPbck7Item.InsertOrUpdate(uploadItemToAdd);
            _uow.SaveChanges();
            
        }

        public void InsertBack1(Back1Dto back1)
        {
            var back1ToAdd = Mapper.Map<BACK1>(back1);
            _repositoryBack1.InsertOrUpdate(back1ToAdd);
            _uow.SaveChanges();
        }

        public void InsertBack3(Back3Dto back3)
        {
            var back3ToAdd = Mapper.Map<BACK3>(back3);
            _repositoryBack3.InsertOrUpdate(back3ToAdd);
            _uow.SaveChanges();
        }

        public void InsertCk2(Ck2Dto ck2)
        {
            var ck2ToAdd = Mapper.Map<CK2>(ck2);
            _repositoryCk2.InsertOrUpdate(ck2ToAdd);
            _uow.SaveChanges(); 
        }

        public Back1Dto GetBack1ByPbck7(int pbck7Id)
        {
            var data = _repositoryBack1.Get(x => x.PBCK7_ID == pbck7Id, null, "BACK1_DOCUMENT");
            return Mapper.Map<Back1Dto>(data.LastOrDefault());
        }

        public Pbck3Dto GetPbck3ByPbck7Id(int? id)
        {
            var data = _repositoryPbck3.Get(p=>p.PBCK7_ID == id);
            return Mapper.Map<Pbck3Dto>(data.LastOrDefault());
        }

        public Back3Dto GetBack3ByPbck3Id(int? id)
        {
            var data = _repositoryBack3.Get(p => p.PBCK3_ID == id, null, "BACK3_DOCUMENT");
            return Mapper.Map<Back3Dto>(data.LastOrDefault());
        }

        public Ck2Dto GetCk2ByPbck3Id(int? id)
        {
            var data = _repositoryCk2.Get(p => p.PBCK3_ID == id, null, "CK2_DOCUMENT");
            return Mapper.Map<Ck2Dto>(data.LastOrDefault());
        }

        public void InsertPbck3(Pbck3Dto pbck3Dto)
        {
            var dataToAdd = Mapper.Map<PBCK3>(pbck3Dto);
            _repositoryPbck3.InsertOrUpdate(dataToAdd);
            _uow.SaveChanges();

            var history = new WorkflowHistoryDto();
            history.FORM_ID = dataToAdd.PBCK3_ID;
            history.ACTION = GetActionTypePbck3(pbck3Dto, pbck3Dto.ModifiedBy);
            history.ACTION_BY = GetActionByPbck3(pbck3Dto);
            history.ACTION_DATE = DateTime.Now;
            history.FORM_NUMBER = pbck3Dto.Pbck3Number;
            history.FORM_TYPE_ID = Enums.FormType.PBCK3;
            history.COMMENT = pbck3Dto.Comment;
            //set workflow history
            var getUserRole = _poabll.GetUserRole(history.ACTION_BY);
            history.ROLE = getUserRole;
            _workflowHistoryBll.AddHistory(history);
            _uow.SaveChanges();
        }
        private Core.Enums.ActionType GetActionTypePbck3(Pbck3Dto pbck3, string modifiedBy)
        {
            var docStatus = pbck3.Pbck3Status;
            if (docStatus == Core.Enums.DocumentStatus.Draft)
            {
                if (pbck3.IsRejected)
                {
                    return Core.Enums.ActionType.Reject;
                }
                if (modifiedBy != null)
                {
                    return Core.Enums.ActionType.Modified;
                }
                return Core.Enums.ActionType.Created;
            }
            if (docStatus == Core.Enums.DocumentStatus.WaitingForApproval)
            {
                return Core.Enums.ActionType.Submit;
            }

            if (docStatus == Core.Enums.DocumentStatus.WaitingForApprovalManager)
            {
                return Core.Enums.ActionType.Approve;
            }

            if (docStatus == Core.Enums.DocumentStatus.WaitingGovApproval)
            {
                return Core.Enums.ActionType.Approve;
            }
            if (docStatus == Core.Enums.DocumentStatus.GovApproved)
            {
                return Core.Enums.ActionType.GovApprove;
            }
            if (docStatus == Core.Enums.DocumentStatus.Completed)
            {
                return Core.Enums.ActionType.Completed;
            }
            return Core.Enums.ActionType.Reject;
        }

        private Core.Enums.ActionType GetActionTypePbck7(Pbck7AndPbck3Dto pbck7pbck3, string modifiedBy)
        {
            var docStatus = pbck7pbck3.Pbck7Status;
            if (docStatus == Core.Enums.DocumentStatus.Draft)
            {
                if (pbck7pbck3.IsRejected)
                {
                    return Core.Enums.ActionType.Reject;
                }
                if (modifiedBy != null)
                {
                    return Core.Enums.ActionType.Modified;
                }
                return Core.Enums.ActionType.Created;
            }
            if (docStatus == Core.Enums.DocumentStatus.WaitingForApproval)
            {
                return Core.Enums.ActionType.Submit;
            }

            if (docStatus == Core.Enums.DocumentStatus.WaitingForApprovalManager)
            {
                return Core.Enums.ActionType.Approve;
            }

            if (docStatus == Core.Enums.DocumentStatus.WaitingGovApproval)
            {
                return Core.Enums.ActionType.Approve;
            }
            if (docStatus == Core.Enums.DocumentStatus.GovApproved)
            {
                return Core.Enums.ActionType.GovApprove;
            }
            if (docStatus == Core.Enums.DocumentStatus.Completed)
            {
                return Core.Enums.ActionType.Completed;
            }
            return Core.Enums.ActionType.Reject;
        }
        
        private string GetActionByPbck7(Pbck7AndPbck3Dto pbck3pbkc7)
        {
            if (pbck3pbkc7.Pbck7Status == Core.Enums.DocumentStatus.Draft)
            {
                if (pbck3pbkc7.IsRejected)
                {
                    return pbck3pbkc7.RejectedBy;
                }
                if (pbck3pbkc7.ModifiedBy != null)
                {
                    return pbck3pbkc7.ModifiedBy;
                }
                return pbck3pbkc7.CreatedBy;
            }
            if (pbck3pbkc7.Pbck7Status == Core.Enums.DocumentStatus.WaitingForApproval)
            {
                return pbck3pbkc7.CreatedBy;
            }
            if (pbck3pbkc7.Pbck7Status == Core.Enums.DocumentStatus.WaitingForApprovalManager)
            {
                if (pbck3pbkc7.ApprovedBy != null)
                {
                    return pbck3pbkc7.ApprovedBy;
                }
                else
                {
                    return pbck3pbkc7.CreatedBy;
                }
            }
            if (pbck3pbkc7.Pbck7Status == Core.Enums.DocumentStatus.WaitingGovApproval)
            {
                return pbck3pbkc7.ApprovedByManager;
            }
            if (pbck3pbkc7.Pbck7Status == Core.Enums.DocumentStatus.Rejected)
            {
                return pbck3pbkc7.RejectedBy;
            }



            return pbck3pbkc7.CreatedBy;
        }

        private string GetActionByPbck3(Pbck3Dto pbck3)
        {
            if (pbck3.Pbck3Status == Core.Enums.DocumentStatus.Draft)
            {
                if (pbck3.IsRejected)
                {
                    return pbck3.RejectedBy;
                }
                if (pbck3.ModifiedBy != null)
                {
                    return pbck3.ModifiedBy;
                }
                return pbck3.CreatedBy;
            }
            if (pbck3.Pbck3Status == Core.Enums.DocumentStatus.WaitingForApproval)
            {
                return pbck3.CreatedBy;
            }
            if (pbck3.Pbck3Status == Core.Enums.DocumentStatus.WaitingForApprovalManager)
            {
                if (pbck3.ApprovedBy != null)
                {
                    return pbck3.ApprovedBy;
                }
                else
                {
                    return pbck3.CreatedBy;
                };
            }
            if (pbck3.Pbck3Status == Core.Enums.DocumentStatus.WaitingGovApproval)
            {
                return pbck3.ApprovedByManager;
            }
            if (pbck3.Pbck3Status == Core.Enums.DocumentStatus.Rejected)
            {
                return pbck3.RejectedBy;
            }



            return pbck3.CreatedBy;
        }

        private void AddWorkflowHistory(Pbck7Pbck3WorkflowHistoryInput input)
        {
            var inputWorkflowHistory = new GetByActionAndFormNumberInput();
            inputWorkflowHistory.ActionType = input.ActionType;
            inputWorkflowHistory.FormNumber = input.DocumentNumber;

            var dbData = new WorkflowHistoryDto();
            dbData.ACTION = input.ActionType;
            dbData.FORM_NUMBER = input.DocumentNumber;
            dbData.FORM_TYPE_ID = input.FormType;

            dbData.FORM_ID = input.DocumentId;
            if (!string.IsNullOrEmpty(input.Comment))
                dbData.COMMENT = input.Comment;


            dbData.ACTION_BY = input.UserId;
            dbData.ROLE = input.UserRole;
            dbData.ACTION_DATE = DateTime.Now;

            _workflowHistoryBll.Save(dbData);
        }

        //private void AddWorkflowHistory(Pbck7Pbck3WorkflowDocumentInput input)
        //{
        //    var inputWorkflowHistory = new Pbck7Pbck3WorkflowHistoryInput();

        //    inputWorkflowHistory.DocumentId = input.DocumentId;
        //    inputWorkflowHistory.DocumentNumber = input.DocumentNumber;
        //    inputWorkflowHistory.UserId = input.UserId;
        //    inputWorkflowHistory.UserRole = input.UserRole;
        //    inputWorkflowHistory.ActionType = input.ActionType;
        //    inputWorkflowHistory.FormType = input.FormType
        //    inputWorkflowHistory.Comment = input.Comment;

        //    AddWorkflowHistory(inputWorkflowHistory);
        //}


        public Pbck7AndPbck3Dto SavePbck7(Pbck7Pbck3SaveInput input)
        {
            //workflowhistory
            var inputWorkflowHistory = new Pbck7Pbck3WorkflowHistoryInput();

            PBCK7 dbData = null;

            if (input.Pbck7Pbck3Dto.Pbck7Id > 0)
            {
                //update
                dbData = _repositoryPbck7.Get(c => c.PBCK7_ID == input.Pbck7Pbck3Dto.Pbck7Id, null, null).FirstOrDefault();
                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                //set changes history
                //var origin = Mapper.Map<Pbck7AndPbck3Dto>(dbData);

                //SetChangesHistory(origin, input.Pbck4Dto, input.UserId);

                Mapper.Map<Pbck7AndPbck3Dto, PBCK7>(input.Pbck7Pbck3Dto, dbData);

                if (dbData.STATUS == Enums.DocumentStatus.Rejected)
                {
                    dbData.STATUS = Enums.DocumentStatus.Draft;
                }

                dbData.MODIFIED_DATE = DateTime.Now;
                dbData.MODIFIED_BY = input.UserId;

                //delete child first
                foreach (var pbck4Item in dbData.PBCK7_ITEM.ToList())
                {
                    _repositoryPbck7Item.Delete(pbck4Item);
                }

                inputWorkflowHistory.ActionType = Enums.ActionType.Modified;

                //insert new data
                foreach (var pbck7Items in input.Pbck7Pbck3Items)
                {
                    var pbck7Item = Mapper.Map<PBCK7_ITEM>(pbck7Items);
                    //pbck4Item.PLANT_ID = dbData.PLANT_ID;
                    dbData.PBCK7_ITEM.Add(pbck7Item);
                }

            }
            else
            {

                var generateNumberInput = new GenerateDocNumberInput()
                {
                    Year = input.Pbck7Pbck3Dto.Pbck7Date.Year,
                    Month = input.Pbck7Pbck3Dto.Pbck7Date.Month,
                    NppbkcId = input.Pbck7Pbck3Dto.NppbkcId,
                    FormType = Enums.FormType.PBCK7
                };

                input.Pbck7Pbck3Dto.Pbck7Number = _docSeqNumBll.GenerateNumber(generateNumberInput);

                input.Pbck7Pbck3Dto.Pbck7Status = Enums.DocumentStatus.Draft;
                input.Pbck7Pbck3Dto.CreateDate = DateTime.Now;
                input.Pbck7Pbck3Dto.CreatedBy = input.UserId;

                dbData = new PBCK7();

                Mapper.Map<Pbck7AndPbck3Dto, PBCK7>(input.Pbck7Pbck3Dto, dbData);

                inputWorkflowHistory.ActionType = Enums.ActionType.Created;

                //insert new data
                foreach (var pbck7Material in input.Pbck7Pbck3Items)
                {

                    var pbck7Item = Mapper.Map<PBCK7_ITEM>(pbck7Material);
                    
                    dbData.PBCK7_ITEM.Add(pbck7Item);
                }

                _repositoryPbck7.Insert(dbData);


            }

            inputWorkflowHistory.DocumentId = dbData.PBCK7_ID;
            inputWorkflowHistory.DocumentNumber = dbData.PBCK7_NUMBER;
            inputWorkflowHistory.UserId = input.UserId;
            inputWorkflowHistory.UserRole = input.UserRole;
            inputWorkflowHistory.FormType = Enums.FormType.PBCK7;

            AddWorkflowHistory(inputWorkflowHistory);

            try
            {
                _uow.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }


            return Mapper.Map<Pbck7AndPbck3Dto>(dbData);
        }

        public Pbck7DetailsOutput GetDetailsPbck7ById(int id)
        {
            var output = new Pbck7DetailsOutput();

            var result = _repositoryPbck7.Get(x => x.PBCK7_ID == id, null, includeTable).FirstOrDefault();
            if (result == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);//return new Pbck7AndPbck3Dto();

            output.Pbck7Dto = Mapper.Map<Pbck7AndPbck3Dto>(result);

            output.Back1Dto = GetBack1ByPbck7(id);
            output.Pbck3Dto = GetPbck3ByPbck7Id(id);

            if (output.Pbck3Dto != null)
            {
                output.Back3Dto = GetBack3ByPbck3Id(output.Pbck3Dto.Pbck3Id);
                output.Ck2Dto = GetCk2ByPbck3Id(output.Pbck3Dto.Pbck3Id);
            }
            if (output.Back1Dto == null)
                output.Back1Dto = new Back1Dto();
            if (output.Pbck3Dto == null)
                output.Pbck3Dto = new Pbck3Dto();
            if (output.Back3Dto == null)
                output.Back3Dto = new Back3Dto();
            if (output.Ck2Dto == null)
                output.Ck2Dto = new Ck2Dto();

            //workflow history
            var workflowInput = new GetByFormNumberInput();
            workflowInput.FormId = result.PBCK7_ID;
            workflowInput.FormNumber = result.PBCK7_NUMBER;
            workflowInput.DocumentStatus = result.STATUS;
            workflowInput.NPPBKC_Id = result.NPPBKC;
            workflowInput.FormType = Enums.FormType.PBCK7;

            output.WorkflowHistoryPbck7 = _workflowHistoryBll.GetByFormNumber(workflowInput);

            workflowInput.FormId = output.Pbck3Dto.Pbck3Id;
            workflowInput.FormNumber = output.Pbck3Dto.Pbck3Number;
            workflowInput.DocumentStatus = output.Pbck3Dto.Pbck3Status;
            workflowInput.NPPBKC_Id = result.NPPBKC;
            workflowInput.FormType = Enums.FormType.PBCK3;

            output.WorkflowHistoryPbck3 = _workflowHistoryBll.GetByFormNumber(workflowInput);

            return output;

            //var pbck7 = Mapper.Map<Pbck7AndPbck3Dto>(result);
            //pbck7.Back1Dto = Mapper.Map<Back1Dto>(result.BACK1.LastOrDefault());
            //return pbck7;
        }

        private List<Pbck7ItemsOutput> ValidatePbck7Items(List<Pbck7ItemsInput> inputs)
        {
            var messageList = new List<string>();
            var outputList = new List<Pbck7ItemsOutput>();

            foreach (var pbck7ItemInput in inputs)
            {
                messageList.Clear();

                var output = Mapper.Map<Pbck7ItemsOutput>(pbck7ItemInput);

                var dbBrand = _brandRegistrationServices.GetByPlantIdAndFaCode(pbck4ItemInput.Plant, pbck4ItemInput.FaCode);
                if (dbBrand == null)
                    messageList.Add("FA Code Not Exist");

                var dbCk1 = _ck1Services.GetCk1ByCk1Number(pbck4ItemInput.Ck1No);
                if (dbCk1 == null)
                    messageList.Add("CK-1 Number Not Exist");

                if (!ConvertHelper.IsNumeric(pbck4ItemInput.ReqQty))
                    messageList.Add("Req Qty not valid");

                if (!ConvertHelper.IsNumeric(pbck4ItemInput.ApprovedQty))
                    messageList.Add("Approved Qty not valid");

                if (!string.IsNullOrEmpty(pbck4ItemInput.NoPengawas))
                {
                    if (pbck4ItemInput.NoPengawas.Length > 10)
                        messageList.Add("No Pengawas Max Length 10");
                }

                //validate ReqQty to block stock

                var blockStockData = _blockStockBll.GetBlockStockByPlantAndMaterialId(pbck4ItemInput.Plant,
                    pbck4ItemInput.FaCode);
                if (blockStockData.Count == 0)
                {
                    messageList.Add("Block Stock not available");
                }
                else
                {
                    var blockDecimal = blockStockData.Sum(blockStockDto => blockStockDto.BLOCKED.HasValue ? blockStockDto.BLOCKED.Value : 0);
                    if (ConvertHelper.ConvertToDecimalOrZero(pbck4ItemInput.ReqQty) > blockDecimal)
                        messageList.Add("Req Qty more than Block Stock");
                }

                if (messageList.Count > 0)
                {
                    output.IsValid = false;
                    output.Message = "";
                    foreach (var message in messageList)
                    {
                        output.Message += message + ";";
                    }
                }
                else
                {
                    output.IsValid = true;
                }

                outputList.Add(output);
            }


            return outputList;
        }

        public List<Pbck7ItemsOutput> Pbck7ItemProcess(List<Pbck7ItemsInput> inputs)
        {
            var outputList = ValidatePbck4Items(inputs);

            if (!outputList.All(c => c.IsValid))
                return outputList;

            foreach (var output in outputList)
            {
                var resultValue = GetAdditionalValuePbck4Items(output);


                output.StickerCode = resultValue.StickerCode;
                output.SeriesCode = resultValue.SeriesCode;
                output.BrandName = resultValue.BrandName;
                output.ProductAlias = resultValue.ProductAlias;

                output.Content = resultValue.Content;

                output.Hje = resultValue.Hje;
                output.Tariff = resultValue.Tariff;
                output.Colour = resultValue.Colour;

                output.TotalHje = resultValue.TotalHje;

                output.TotalStamps = resultValue.TotalStamps;
                output.CK1_ID = resultValue.CK1_ID;
                output.BlockedStock = resultValue.BlockedStock;
            }

            return outputList;
        }
    }


}
