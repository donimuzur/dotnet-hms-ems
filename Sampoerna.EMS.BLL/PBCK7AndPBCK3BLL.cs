using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
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
       
        private IUnitOfWork _uow;
        private IBACK1BLL _back1Bll;
        private IPOABLL _poabll;
        private string includeTable = "PBCK7_ITEM";
        private WorkflowHistoryBLL _workflowHistoryBll;
       
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
            
            return mapResult;
        }

        public List<Pbck7AndPbck3Dto> GetPbck7ByParam(Pbck7AndPbck3Input input)
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

        public List<Pbck3Dto> GetPbck3ByParam(Pbck7AndPbck3Input input)
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

        public void InsertPbck7(Pbck7AndPbck3Dto pbck7AndPbck3Dto)
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
                return pbck3pbkc7.ApprovedBy;
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
                return pbck3.ApprovedBy;
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

    }


}
