using System.Security.Cryptography;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class LACK2BLL : ILACK2BLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<LACK2> _repository;
        private IGenericRepository<LACK2_ITEM> _repositoryItem;
        private IGenericRepository<LACK2_DOCUMENT> _repositoryDocument;
        private IMonthBLL _monthBll;
        private IUserBLL _userBll;
        private IUnitOfMeasurementBLL _uomBll;

        private string includeTables = "MONTH";
        private IWorkflowHistoryBLL _workflowHistoryBll;
        private IPOABLL _poabll;
         
        public LACK2BLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<LACK2>();
            _repositoryItem = _uow.GetGenericRepository<LACK2_ITEM>();
            _uomBll = new UnitOfMeasurementBLL(_uow, _logger);
            _monthBll = new MonthBLL(_uow, _logger);
            _userBll = new UserBLL(_uow, _logger);
            _workflowHistoryBll = new WorkflowHistoryBLL(_uow, _logger);
            _poabll = new POABLL(_uow, _logger);
        }


        public List<Lack2Dto> GetAll()
        {
            return Mapper.Map<List<Lack2Dto>>(_repository.Get());
        }

       

        public List<Lack2Dto> GetOpenDocument()
        {
            return Mapper.Map<List<Lack2Dto>>(_repository.Get(x => x.STATUS != Enums.DocumentStatus.Completed, null, includeTables));
     
        }

        public List<Lack2Dto> GetDocumentByParam(Lack2GetByParamInput input)
        {
            Expression<Func<LACK2, bool>> queryFilter = PredicateHelper.True<LACK2>();

            if (!string.IsNullOrEmpty((input.NppbKcId)))
            {
                queryFilter = queryFilter.And(c => c.NPPBKC_ID == input.NppbKcId);
            }
            if (!string.IsNullOrEmpty((input.PlantId)))
            {
                queryFilter = queryFilter.And(c => c.LEVEL_PLANT_ID == input.PlantId);
            }
            if (!string.IsNullOrEmpty((input.Creator)))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.Creator);
            }
            if (!string.IsNullOrEmpty((input.Poa)))
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY == input.Poa);
            }
            if (input.SubmissionDate.HasValue)
            {
                var date = input.SubmissionDate.Value.Day;
                var month = input.SubmissionDate.Value.Month;
                var year = input.SubmissionDate.Value.Year;
                var dateToCompare = new DateTime(year, month, date);
                queryFilter = queryFilter.And(c => c.SUBMISSION_DATE.Equals(dateToCompare));
            }
            if (input.IsOpenDocList)
            {
                queryFilter = queryFilter.And(c => c.STATUS != Enums.DocumentStatus.Completed);
            }
            else
            {
                queryFilter = queryFilter.And(c => c.STATUS == Enums.DocumentStatus.Completed);
            }

            Func<IQueryable<LACK2>, IOrderedQueryable<LACK2>> orderBy = null;

            if (!string.IsNullOrEmpty(input.SortOrderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<LACK2>(input.SortOrderColumn));

            }

            var dbData = _repository.Get(queryFilter, orderBy, includeTables);
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            var mapResult = Mapper.Map<List<Lack2Dto>>(dbData.ToList());

            return mapResult;
        }

        /// <summary>
        /// Gets all LACK2 COMPLETED Documents by parameters
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<Lack2Dto> GetAllCompletedByParam(Lack2GetByParamInput input)
        {
            Expression<Func<LACK2, bool>> queryFilter = PredicateHelper.True<LACK2>();

            if (!string.IsNullOrEmpty((input.PlantId)))
            {
                queryFilter = queryFilter.And(c => c.LEVEL_PLANT_ID == input.PlantId);
            }
            if (!string.IsNullOrEmpty((input.Creator)))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.Creator);
            }
            if (!string.IsNullOrEmpty((input.Poa)))
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY == input.Poa);
            }
            if(input.Status != null || input.Status != 0)
            {
                queryFilter = queryFilter.And(c => c.STATUS == input.Status);
            }
           

            Func<IQueryable<LACK2>, IOrderedQueryable<LACK2>> orderBy = null;

            if (!string.IsNullOrEmpty(input.SortOrderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<LACK2>(input.SortOrderColumn));

            }

            var dbData = _repository.Get(queryFilter, orderBy, includeTables);
            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            var mapResult = Mapper.Map<List<Lack2Dto>>(dbData.ToList());

            return mapResult;
        }

        /// <summary>
        /// Gets Lack2 by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Lack2Dto</returns>
        public Lack2Dto GetById(int id)
        {
            return Mapper.Map<Lack2Dto>(_repository.GetByID(id));
        }

        public Lack2Dto GetByIdAndItem(int id)
        {
            var data = _repositoryItem.Get(x => x.LACK2_ID == id, null, "LACK2, LACK2.MONTH, CK5, LACK2.LACK2_DOCUMENT");
            var lack2dto = new Lack2Dto();
            lack2dto = data.Select(x => Mapper.Map<Lack2Dto>(x.LACK2)).FirstOrDefault();
            lack2dto.Items = data.Select(x => Mapper.Map<Lack2ItemDto>(x)).ToList();
            
            return lack2dto;
        }

        private Enums.ActionType GetActionType(Lack2Dto lack2, string modifiedBy)
        {
            var docStatus = lack2.Status;
            if (docStatus == Enums.DocumentStatus.Draft)
            {
                if (lack2.IsRejected)
                {
                    return Enums.ActionType.Reject;
                }
                if (modifiedBy != null)
                {
                    return Enums.ActionType.Modified;
                }
                return Enums.ActionType.Created;
            }
            if (docStatus == Enums.DocumentStatus.WaitingForApproval)
            {
                return Enums.ActionType.Submit;
            }

            if (docStatus == Enums.DocumentStatus.WaitingForApprovalManager)
            {
                return Enums.ActionType.Approve;
            }
         
            if (docStatus == Enums.DocumentStatus.WaitingGovApproval)
            {
                return Enums.ActionType.Approve;
            }
            if (docStatus == Enums.DocumentStatus.GovApproved)
            {
                return Enums.ActionType.GovPartialApprove;
            }
            if (docStatus == Enums.DocumentStatus.Completed)
            {
                return Enums.ActionType.GovApprove;
            }
            return Enums.ActionType.Reject;
        }

        private string GetActionBy(Lack2Dto lack2)
        {
            if (lack2.Status == Enums.DocumentStatus.Draft )
            {
                if (lack2.IsRejected)
                {
                    return lack2.RejectedBy;
                }
                if (lack2.ModifiedBy != null)
                {
                    return lack2.ModifiedBy;
                }
                return lack2.CreatedBy;
            }
            if (lack2.Status == Enums.DocumentStatus.WaitingForApproval)
            {
                return lack2.CreatedBy;
            }
            if (lack2.Status == Enums.DocumentStatus.WaitingForApprovalManager)
            {
                return lack2.ApprovedBy;
            }
            if (lack2.Status == Enums.DocumentStatus.WaitingGovApproval)
            {
                return lack2.ApprovedByManager;
            }
            if (lack2.Status == Enums.DocumentStatus.Rejected)
            {
                return lack2.RejectedBy;
            }
           
           
            
            return lack2.CreatedBy;
        }

        /// <summary>
        /// Inserts a LACK2 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Lack2Dto Insert(Lack2Dto item)
        {

            if (item == null)
            {
                throw new Exception("Invalid data entry !");
            }

            LACK2 model = new LACK2();
            MONTH month = new MONTH();
            
            month = _monthBll.GetMonth(item.PeriodMonth);
           
            model = AutoMapper.Mapper.Map<LACK2>(item);
            model.MONTH = month;
            model.LACK2_DOCUMENT = item.Documents;
            try
            {
                _repository.InsertOrUpdate(model);
                _uow.SaveChanges();
                var history = new WorkflowHistoryDto();
                history.FORM_ID = model.LACK2_ID;
                history.ACTION = GetActionType(item, item.ModifiedBy);
                history.ACTION_BY = GetActionBy(item);
                history.ACTION_DATE = DateTime.Now;
                history.FORM_NUMBER = item.Lack2Number;
                history.FORM_TYPE_ID = Enums.FormType.LACK2;
                history.COMMENT = item.Comment;
                //set workflow history
                var getUserRole = _poabll.GetUserRole(history.ACTION_BY);
                history.ROLE = getUserRole;
                _workflowHistoryBll.AddHistory(history);
                _uow.SaveChanges();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            return item;
        }

        public void InsertDocument(LACK2_DOCUMENT document)
        {
            _repositoryDocument = _uow.GetGenericRepository<LACK2_DOCUMENT>();
            _repositoryDocument.InsertOrUpdate(document);
            _uow.SaveChanges();
        }

        public int RemoveDoc(int docId)
        {
            try
            {
                _repositoryDocument = _uow.GetGenericRepository<LACK2_DOCUMENT>();
                _repositoryDocument.Delete(docId);
                _uow.SaveChanges();
            }
            catch (Exception)
            {
                return -1;
            }
            return 0;

        }

        public List<Lack2Dto> GetCompletedDocument()
        {
            return Mapper.Map<List<Lack2Dto>>(_repository.Get(x => x.STATUS == Enums.DocumentStatus.Completed, null, includeTables));
        }

        public void RemoveExistingItem(long id)
        {
            _repositoryItem.Delete(id);
            _uow.SaveChanges();
        }
    }
}
