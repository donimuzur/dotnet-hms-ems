using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using AutoMapper;
using AutoMapper.Internal;
using Sampoerna.EMS.BLL.Services;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.MessagingService;
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
        private ILFA1BLL _lfaBll;

        private IDocumentSequenceNumberBLL _docSeqNumBll;
        private IBrandRegistrationService _brandRegistrationServices;
        private IPlantBLL _plantBll;
        private IChangesHistoryBLL _changesHistoryBll;
        private IPOABLL _poaBll;
        private IUserBLL _userBll;
        private IMessageService _messageService;
        private IPrintHistoryBLL _printHistoryBll;

        private IBack1Services _back1Services;
        private IT001KBLL _t001Kbll;
        private IPbck3Services _pbck3Services;
        private CK2Services _ck2Services;
        private IBack3Services _back3Services;
        private IWorkflowBLL _workflowBll;
        private IHeaderFooterBLL _headerFooterBll;
        private IMonthBLL _monthBll;

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
            _brandRegistrationServices = new BrandRegistrationService(_uow, _logger);
            _plantBll = new PlantBLL(_uow,_logger);
            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);
            _poaBll = new POABLL(_uow, _logger);
            _userBll = new UserBLL(_uow, _logger);
            _messageService = new MessageService(_logger);
            _printHistoryBll = new PrintHistoryBLL(_uow, _logger);

            _back1Services = new Back1Services(_uow, _logger);
            _t001Kbll = new T001KBLL(_uow, _logger);
            _pbck3Services = new Pbck3Services(_uow, _logger);
            _ck2Services = new CK2Services(_uow, _logger);
            _back3Services = new Back3Services(_uow, _logger);
            _workflowBll = new WorkflowBLL(_uow, _logger);
            _lfaBll = new LFA1BLL(_uow, _logger);
            _headerFooterBll = new HeaderFooterBLL(_uow, _logger);
            _monthBll = new MonthBLL(_uow, _logger);
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

            var dbData = _repositoryPbck3.Get(queryFilter, orderBy, "PBCK7, CK5");
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
           

            //Func<IQueryable<PBCK7>, IOrderedQueryable<PBCK7>> orderBy = null;
            //if (!string.IsNullOrEmpty(input.ShortOrderColum))
            //{
            //    orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<PBCK7>(input.ShortOrderColum));
            //}
            Func<IQueryable<PBCK7>, IOrderedQueryable<PBCK7>> orderBy = n => n.OrderByDescending(z => z.CREATED_DATE);
        


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
                queryFilter = queryFilter.And(c => (c.PBCK7.NPPBKC == input.NppbkcId || c.CK5.SOURCE_PLANT_NPPBKC_ID == input.NppbkcId));
            }
            if (!string.IsNullOrEmpty(input.PlantId))
            {
                queryFilter = queryFilter.And(c => c.PBCK7.PLANT_ID == input.PlantId || c.CK5.SOURCE_PLANT_ID == input.PlantId);
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
            //Func<IQueryable<PBCK3>, IOrderedQueryable<PBCK3>> orderBy = null;
            //if (!string.IsNullOrEmpty(input.ShortOrderColum))
            //{
            //    orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<PBCK3>(input.ShortOrderColum));
            //}
            Func<IQueryable<PBCK3>, IOrderedQueryable<PBCK3>> orderBy = n => n.OrderByDescending(z => z.CREATED_DATE);
        

            var dbData = _repositoryPbck3.Get(queryFilter, orderBy, "PBCK7, CK5");
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

        //public void Insert(Pbck7AndPbck3Dto pbck7AndPbck3Dto)
        //{
        //    var dataToAdd = Mapper.Map<PBCK3_PBCK7>(pbck7AndPbck3Dto);
        //    _repository.InsertOrUpdate(dataToAdd);
        //    _uow.SaveChanges();

        //    var history = new WorkflowHistoryDto();
        //    history.FORM_ID = dataToAdd.PBCK3_PBCK7_ID;
        //    if (pbck7AndPbck3Dto.Pbck3Dto.Pbck3Status == Enums.DocumentStatus.Draft)
        //    {
        //        history.ACTION = GetActionTypePbck7(pbck7AndPbck3Dto, pbck7AndPbck3Dto.ModifiedBy);
        //        history.ACTION_BY = GetActionByPbck7(pbck7AndPbck3Dto);
        //        history.ACTION_DATE = DateTime.Now;
        //        history.FORM_NUMBER = pbck7AndPbck3Dto.Pbck7Number;
        //    }
        //    history.FORM_TYPE_ID = Enums.FormType.PBCK7;
        //    history.COMMENT = pbck7AndPbck3Dto.Comment;
        //    //set workflow history
        //    var getUserRole = _poabll.GetUserRole(history.ACTION_BY);
        //    history.ROLE = getUserRole;
        //    _workflowHistoryBll.AddHistory(history);
        //    _uow.SaveChanges();
        //}

        //public int? InsertPbck7(Pbck7AndPbck3Dto pbck7AndPbck3Dto)
        //{
        //    var dataToAdd = Mapper.Map<PBCK7>(pbck7AndPbck3Dto);
        //    _repositoryPbck7.InsertOrUpdate(dataToAdd);
        //    _uow.SaveChanges();

        //    var history = new WorkflowHistoryDto();
        //    history.FORM_ID = dataToAdd.PBCK7_ID;
        //    history.ACTION = GetActionTypePbck7(pbck7AndPbck3Dto, pbck7AndPbck3Dto.ModifiedBy);
        //    history.ACTION_BY = GetActionByPbck7(pbck7AndPbck3Dto);
        //    history.ACTION_DATE = DateTime.Now;
        //    history.FORM_NUMBER = pbck7AndPbck3Dto.Pbck7Number;
        //    history.FORM_TYPE_ID = Enums.FormType.PBCK7;
        //    history.COMMENT = pbck7AndPbck3Dto.Comment;
        //    //set workflow history
        //    var getUserRole = _poabll.GetUserRole(history.ACTION_BY);
        //    history.ROLE = getUserRole;
        //    _workflowHistoryBll.AddHistory(history);
        //    _uow.SaveChanges();
        //    return dataToAdd.PBCK7_ID;
        //}

        //public void InsertPbck7Item(Pbck7ItemUpload item)
        //{
        //    var uploadItemToAdd = Mapper.Map<PBCK7_ITEM>(item);
        //    _repositoryPbck7Item.InsertOrUpdate(uploadItemToAdd);
        //    _uow.SaveChanges();
            
        //}

       

        //public void InsertBack1(Back1Dto back1)
        //{
        //    var back1ToAdd = Mapper.Map<BACK1>(back1);
        //    _repositoryBack1.InsertOrUpdate(back1ToAdd);
        //    _uow.SaveChanges();
        //}

        //public void InsertBack3(Back3Dto back3)
        //{
        //    var back3ToAdd = Mapper.Map<BACK3>(back3);
        //    _repositoryBack3.InsertOrUpdate(back3ToAdd);
        //    _uow.SaveChanges();
        //}

        //public void InsertCk2(Ck2Dto ck2)
        //{
        //    var ck2ToAdd = Mapper.Map<CK2>(ck2);
        //    _repositoryCk2.InsertOrUpdate(ck2ToAdd);
        //    _uow.SaveChanges(); 
        //}

        public Back1Dto GetBack1ByPbck7(int pbck7Id)
        {
            var data = _repositoryBack1.Get(x => x.PBCK7_ID == pbck7Id, null, "BACK1_DOCUMENT");
            return Mapper.Map<Back1Dto>(data.LastOrDefault());
        }

        public Pbck3Dto GetPbck3ByPbck7Id(int? id)
        {
            var data = _repositoryPbck3.Get(p => p.PBCK7_ID == id);
            return Mapper.Map<Pbck3Dto>(data.LastOrDefault());
        }

        public Pbck3Dto GetPbck3ById(int id)
        {
            var data = _repositoryPbck3.Get(c => c.PBCK3_ID == id, null, "PBCK7");
            return Mapper.Map<Pbck3Dto>(data.FirstOrDefault());
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

        //public void InsertPbck3(Pbck3Dto pbck3Dto)
        //{
        //    var dataToAdd = Mapper.Map<PBCK3>(pbck3Dto);
        //    _repositoryPbck3.InsertOrUpdate(dataToAdd);
        //    _uow.SaveChanges();

        //    var history = new WorkflowHistoryDto();
        //    history.FORM_ID = dataToAdd.PBCK3_ID;
        //    history.ACTION = GetActionTypePbck3(pbck3Dto, pbck3Dto.ModifiedBy);
        //    history.ACTION_BY = GetActionByPbck3(pbck3Dto);
        //    history.ACTION_DATE = DateTime.Now;
        //    history.FORM_NUMBER = pbck3Dto.Pbck3Number;
        //    history.FORM_TYPE_ID = Enums.FormType.PBCK3;
        //    history.COMMENT = pbck3Dto.Comment;
        //    //set workflow history
        //    var getUserRole = _poabll.GetUserRole(history.ACTION_BY);
        //    history.ROLE = getUserRole;
        //    _workflowHistoryBll.AddHistory(history);
        //    _uow.SaveChanges();
        //}
        //private Core.Enums.ActionType GetActionTypePbck3(Pbck3Dto pbck3, string modifiedBy)
        //{
        //    var docStatus = pbck3.Pbck3Status;
        //    if (docStatus == Core.Enums.DocumentStatus.Draft)
        //    {
        //        if (pbck3.IsRejected)
        //        {
        //            return Core.Enums.ActionType.Reject;
        //        }
        //        if (modifiedBy != null)
        //        {
        //            return Core.Enums.ActionType.Modified;
        //        }
        //        return Core.Enums.ActionType.Created;
        //    }
        //    if (docStatus == Core.Enums.DocumentStatus.WaitingForApproval)
        //    {
        //        return Core.Enums.ActionType.Submit;
        //    }

        //    if (docStatus == Core.Enums.DocumentStatus.WaitingForApprovalManager)
        //    {
        //        return Core.Enums.ActionType.Approve;
        //    }

        //    if (docStatus == Core.Enums.DocumentStatus.WaitingGovApproval)
        //    {
        //        return Core.Enums.ActionType.Approve;
        //    }
        //    if (docStatus == Core.Enums.DocumentStatus.GovApproved)
        //    {
        //        return Core.Enums.ActionType.GovApprove;
        //    }
        //    if (docStatus == Core.Enums.DocumentStatus.Completed)
        //    {
        //        return Core.Enums.ActionType.Completed;
        //    }
        //    return Core.Enums.ActionType.Reject;
        //}

        //private Core.Enums.ActionType GetActionTypePbck7(Pbck7AndPbck3Dto pbck7pbck3, string modifiedBy)
        //{
        //    var docStatus = pbck7pbck3.Pbck7Status;
        //    if (docStatus == Core.Enums.DocumentStatus.Draft)
        //    {
        //        if (pbck7pbck3.IsRejected)
        //        {
        //            return Core.Enums.ActionType.Reject;
        //        }
        //        if (modifiedBy != null)
        //        {
        //            return Core.Enums.ActionType.Modified;
        //        }
        //        return Core.Enums.ActionType.Created;
        //    }
        //    if (docStatus == Core.Enums.DocumentStatus.WaitingForApproval)
        //    {
        //        return Core.Enums.ActionType.Submit;
        //    }

        //    if (docStatus == Core.Enums.DocumentStatus.WaitingForApprovalManager)
        //    {
        //        return Core.Enums.ActionType.Approve;
        //    }

        //    if (docStatus == Core.Enums.DocumentStatus.WaitingGovApproval)
        //    {
        //        return Core.Enums.ActionType.Approve;
        //    }
        //    if (docStatus == Core.Enums.DocumentStatus.GovApproved)
        //    {
        //        return Core.Enums.ActionType.GovApprove;
        //    }
        //    if (docStatus == Core.Enums.DocumentStatus.Completed)
        //    {
        //        return Core.Enums.ActionType.Completed;
        //    }
        //    return Core.Enums.ActionType.Reject;
        //}
        
        //private string GetActionByPbck7(Pbck7AndPbck3Dto pbck3pbkc7)
        //{
        //    if (pbck3pbkc7.Pbck7Status == Core.Enums.DocumentStatus.Draft)
        //    {
        //        if (pbck3pbkc7.IsRejected)
        //        {
        //            return pbck3pbkc7.RejectedBy;
        //        }
        //        if (pbck3pbkc7.ModifiedBy != null)
        //        {
        //            return pbck3pbkc7.ModifiedBy;
        //        }
        //        return pbck3pbkc7.CreatedBy;
        //    }
        //    if (pbck3pbkc7.Pbck7Status == Core.Enums.DocumentStatus.WaitingForApproval)
        //    {
        //        return pbck3pbkc7.CreatedBy;
        //    }
        //    if (pbck3pbkc7.Pbck7Status == Core.Enums.DocumentStatus.WaitingForApprovalManager)
        //    {
        //        if (pbck3pbkc7.ApprovedBy != null)
        //        {
        //            return pbck3pbkc7.ApprovedBy;
        //        }
        //        else
        //        {
        //            return pbck3pbkc7.CreatedBy;
        //        }
        //    }
        //    if (pbck3pbkc7.Pbck7Status == Core.Enums.DocumentStatus.WaitingGovApproval)
        //    {
        //        return pbck3pbkc7.ApprovedByManager;
        //    }
        //    if (pbck3pbkc7.Pbck7Status == Core.Enums.DocumentStatus.Rejected)
        //    {
        //        return pbck3pbkc7.RejectedBy;
        //    }



        //    return pbck3pbkc7.CreatedBy;
        //}

        //private string GetActionByPbck3(Pbck3Dto pbck3)
        //{
        //    if (pbck3.Pbck3Status == Core.Enums.DocumentStatus.Draft)
        //    {
        //        if (pbck3.IsRejected)
        //        {
        //            return pbck3.RejectedBy;
        //        }
        //        if (pbck3.ModifiedBy != null)
        //        {
        //            return pbck3.ModifiedBy;
        //        }
        //        return pbck3.CreatedBy;
        //    }
        //    if (pbck3.Pbck3Status == Core.Enums.DocumentStatus.WaitingForApproval)
        //    {
        //        return pbck3.CreatedBy;
        //    }
        //    if (pbck3.Pbck3Status == Core.Enums.DocumentStatus.WaitingForApprovalManager)
        //    {
        //        if (pbck3.ApprovedBy != null)
        //        {
        //            return pbck3.ApprovedBy;
        //        }
        //        else
        //        {
        //            return pbck3.CreatedBy;
        //        };
        //    }
        //    if (pbck3.Pbck3Status == Core.Enums.DocumentStatus.WaitingGovApproval)
        //    {
        //        return pbck3.ApprovedByManager;
        //    }
        //    if (pbck3.Pbck3Status == Core.Enums.DocumentStatus.Rejected)
        //    {
        //        return pbck3.RejectedBy;
        //    }



        //    return pbck3.CreatedBy;
        //}

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

            if (!input.IsModified && input.ActionType == Enums.ActionType.Submit)
                _workflowHistoryBll.UpdateHistoryModifiedForSubmit(dbData);
            else
                _workflowHistoryBll.Save(dbData);
        }

        private void AddWorkflowHistory(Pbck7Pbck3WorkflowDocumentInput input)
        {
            var inputWorkflowHistory = new Pbck7Pbck3WorkflowHistoryInput();

            inputWorkflowHistory.DocumentId = input.DocumentId;
            inputWorkflowHistory.DocumentNumber = input.DocumentNumber;
            inputWorkflowHistory.UserId = input.UserId;
            inputWorkflowHistory.UserRole = input.UserRole;
            inputWorkflowHistory.ActionType = input.ActionType;
            inputWorkflowHistory.Comment = input.Comment;
            inputWorkflowHistory.IsModified = input.IsModified;
            inputWorkflowHistory.FormType = input.FormType;
            AddWorkflowHistory(inputWorkflowHistory);
        }

        private void AddWorkflowHistoryPbck3(Pbck3WorkflowDocumentInput input)
        {
            var inputWorkflowHistory = new Pbck7Pbck3WorkflowHistoryInput();

            inputWorkflowHistory.DocumentId = input.DocumentId;
            inputWorkflowHistory.DocumentNumber = input.DocumentNumber;
            inputWorkflowHistory.UserId = input.UserId;
            inputWorkflowHistory.UserRole = input.UserRole;
            inputWorkflowHistory.ActionType = input.ActionType;
            inputWorkflowHistory.Comment = input.Comment;
            inputWorkflowHistory.IsModified = input.IsModified;
            inputWorkflowHistory.FormType = input.FormType;
            AddWorkflowHistory(inputWorkflowHistory);
        }

        private bool SetChangesHistory(Pbck7AndPbck3Dto origin, Pbck7AndPbck3Dto dataModified, string userId)
        {
            bool isModified = false;

            var changesData = new Dictionary<string, bool>();

            changesData.Add("DATE", origin.Pbck7Date == dataModified.Pbck7Date);
            changesData.Add("EXEC_FROM", origin.ExecDateFrom == dataModified.ExecDateFrom);
            changesData.Add("EXEC_TO", origin.ExecDateTo == dataModified.ExecDateTo);
            changesData.Add("LAMPIRAN", origin.Lampiran == dataModified.Lampiran);
            changesData.Add("DOC_TYPE", origin.DocumentType == dataModified.DocumentType);


            foreach (var listChange in changesData)
            {
                if (listChange.Value == false)
                {
                    var changes = new CHANGES_HISTORY();
                    changes.FORM_TYPE_ID = Enums.MenuList.PBCK7;
                    changes.FORM_ID = origin.Pbck7Id.ToString();
                    changes.FIELD_NAME = listChange.Key;
                    changes.MODIFIED_BY = userId;
                    changes.MODIFIED_DATE = DateTime.Now;
                    switch (listChange.Key)
                    {
                        case "DATE":
                            changes.OLD_VALUE = origin.Pbck7Date.ToString("dd MMM yyyy");
                            changes.NEW_VALUE = dataModified.Pbck7Date.ToString("dd MMM yyyy");
                            break;
                        case "EXEC_FROM":
                            changes.OLD_VALUE = origin.ExecDateFrom.HasValue ? origin.ExecDateFrom.Value.ToString("dd MMM yyyy") : string.Empty;
                            changes.NEW_VALUE = dataModified.ExecDateFrom.HasValue ? dataModified.ExecDateFrom.Value.ToString("dd MMM yyyy") : string.Empty;
                            break;
                        case "EXEC_TO":
                            changes.OLD_VALUE = origin.ExecDateTo.HasValue ? origin.ExecDateTo.Value.ToString("dd MMM yyyy") : string.Empty;
                            changes.NEW_VALUE = dataModified.ExecDateTo.HasValue ? dataModified.ExecDateTo.Value.ToString("dd MMM yyyy") : string.Empty;
                            break;
                        case "LAMPIRAN":
                            changes.OLD_VALUE = origin.Lampiran;
                            changes.NEW_VALUE = dataModified.Lampiran;
                            break;
                        case "DOC_TYPE":
                            changes.OLD_VALUE = EnumHelper.GetDescription(origin.DocumentType);
                            changes.NEW_VALUE = EnumHelper.GetDescription(dataModified.DocumentType);
                            break;
                    }
                    _changesHistoryBll.AddHistory(changes);
                    isModified = true;
                }
            }

            return isModified;
        }

        private void SetChangeHistory(string oldValue, string newValue, string fieldName, string userId, string pbck7Id)
        {
            var changes = new CHANGES_HISTORY();
            changes.FORM_TYPE_ID = Enums.MenuList.PBCK7;
            changes.FORM_ID = pbck7Id;
            changes.FIELD_NAME = fieldName;
            changes.MODIFIED_BY = userId;
            changes.MODIFIED_DATE = DateTime.Now;

            changes.OLD_VALUE = oldValue;
            changes.NEW_VALUE = newValue;

            _changesHistoryBll.AddHistory(changes);

        }

        private void SetChangeHistoryPbck3(string oldValue, string newValue, string fieldName, string userId, string pbck7Id)
        {
            var changes = new CHANGES_HISTORY();
            changes.FORM_TYPE_ID = Enums.MenuList.PBCK3;
            changes.FORM_ID = pbck7Id;
            changes.FIELD_NAME = fieldName;
            changes.MODIFIED_BY = userId;
            changes.MODIFIED_DATE = DateTime.Now;

            changes.OLD_VALUE = oldValue;
            changes.NEW_VALUE = newValue;

            _changesHistoryBll.AddHistory(changes);

        }
        
        public Pbck7AndPbck3Dto SavePbck7(Pbck7Pbck3SaveInput input)
        {
            bool isModified = false;

            //workflowhistory
            var inputWorkflowHistory = new Pbck7Pbck3WorkflowHistoryInput();

            PBCK7 dbData = null;

            if (input.Pbck7Pbck3Dto.Pbck7Id > 0)
            {
                //update
                dbData = _repositoryPbck7.Get(c => c.PBCK7_ID == input.Pbck7Pbck3Dto.Pbck7Id, null, includeTable).FirstOrDefault();
                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                //set changes history
                var origin = Mapper.Map<Pbck7AndPbck3Dto>(dbData);

                isModified = SetChangesHistory(origin, input.Pbck7Pbck3Dto, input.UserId);

                Mapper.Map<Pbck7AndPbck3Dto, PBCK7>(input.Pbck7Pbck3Dto, dbData);

                //get plant detail
                var dbPlant = _plantBll.GetT001WById(dbData.PLANT_ID);
                if (dbPlant != null)
                {
                    dbData.PLANT_NAME = dbPlant.NAME1;
                    dbData.PLANT_CITY = dbPlant.ORT01;
                }

                if (dbData.STATUS == Enums.DocumentStatus.Rejected)
                {
                    dbData.STATUS = Enums.DocumentStatus.Draft;
                }

                dbData.MODIFIED_DATE = DateTime.Now;
                dbData.MODIFIED_BY = input.UserId;
                
                //delete child first
                foreach (var pbck7Item in dbData.PBCK7_ITEM.ToList())
                {
                    _repositoryPbck7Item.Delete(pbck7Item);
                }

         

                inputWorkflowHistory.ActionType = Enums.ActionType.Modified;



                //////insert new data
                foreach (var pbck7Items in input.Pbck7Pbck3Items)
                {
                    var pbck7Item = Mapper.Map<PBCK7_ITEM>(pbck7Items);
                 
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

                //get plant detail
                var dbPlant = _plantBll.GetT001WById(dbData.PLANT_ID);
                if (dbPlant != null)
                {
                    dbData.PLANT_NAME = dbPlant.NAME1;
                    dbData.PLANT_CITY = dbPlant.ORT01;
                }

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

            var resultDto = Mapper.Map<Pbck7AndPbck3Dto>(dbData);
            resultDto.IsModifiedHistory = isModified;

            return resultDto;
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

            //change history data
            output.ListChangesHistorys = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK7, output.Pbck7Dto.Pbck7Id.ToString());

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

            output.ListPrintHistorys = _printHistoryBll.GetByFormTypeAndFormId(Enums.FormType.PBCK7, result.PBCK7_ID);

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
                output.Message = "";
                output.ProdTypeAlias = "";
                output.Brand = "";
                output.Content = "0";
                output.SeriesValue = "";
                output.Hje = "0";
                output.Tariff = "0";
                output.ExciseValue = "0";

                var dbBrand = _brandRegistrationServices.GetByPlantIdAndFaCode(pbck7ItemInput.Plant, pbck7ItemInput.FaCode);
                if (dbBrand == null)
                    messageList.Add("FA Code Not Exist");


                if (!ConvertHelper.IsNumeric(pbck7ItemInput.Pbck7Qty))
                    messageList.Add("PBCK-7 Qty not valid");

                if (ConvertHelper.ConvertToDecimalOrZero(pbck7ItemInput.Pbck7Qty) <= 0)
                    messageList.Add("PBCK-7 Qty Must > 0");

                if (!ConvertHelper.IsNumeric(pbck7ItemInput.FiscalYear))
                    messageList.Add("Fiscal Year not valid");

            
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

        private Pbck7ItemsOutput GetAdditionalValuePbck7Items(Pbck7ItemsOutput input)
        {
            var dbBrand = _brandRegistrationServices.GetByPlantIdAndFaCode(input.PlantId, input.FaCode);
            if (dbBrand == null)
            {
                input.Brand = "";
                input.ProdTypeAlias = "";
                input.Content = "0";
                input.Hje = "0";
                input.Tariff = "0";
                input.ExciseValue = "0";

            }
            else
            {
                input.Brand = dbBrand.BRAND_CE;
                if (dbBrand.ZAIDM_EX_SERIES != null)
                    input.SeriesValue = dbBrand.ZAIDM_EX_SERIES.SERIES_CODE;

                if (dbBrand.ZAIDM_EX_PRODTYP != null)
                    input.ProdTypeAlias = dbBrand.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS;
                

                input.Content = ConvertHelper.ConvertToDecimalOrZero(dbBrand.BRAND_CONTENT).ToString();

                input.Hje = dbBrand.HJE_IDR.HasValue ? dbBrand.HJE_IDR.Value.ToString() : "0";
                input.Tariff = dbBrand.TARIFF.HasValue ? dbBrand.TARIFF.Value.ToString() : "0";

                input.ExciseValue = (ConvertHelper.GetDecimal(input.Content) * ConvertHelper.GetDecimal(input.Tariff) * ConvertHelper.GetDecimal(input.Pbck7Qty)).ToString("f2");
                
            }
        
            return input;
        }

        public List<Pbck7ItemsOutput> Pbck7ItemProcess(List<Pbck7ItemsInput> inputs)
        {
            var outputList = ValidatePbck7Items(inputs);

            if (!outputList.All(c => c.IsValid))
                return outputList;

            foreach (var output in outputList)
            {
                var resultValue = GetAdditionalValuePbck7Items(output);


                output.Brand = resultValue.Brand;
                output.SeriesValue = resultValue.SeriesValue;
                output.ProdTypeAlias = resultValue.ProdTypeAlias;
              
                output.Content = resultValue.Content;
                output.Hje = resultValue.Hje;
                output.Tariff = resultValue.Tariff;
                output.ExciseValue = resultValue.ExciseValue;

            }

            return outputList;
        }

        public void PBCK7Workflow(Pbck7Pbck3WorkflowDocumentInput input)
        {
            var isNeedSendNotif = false;

            switch (input.ActionType)
            {
                case Enums.ActionType.Submit:
                    SubmitDocument(input);
                    isNeedSendNotif = true;
                    break;
                case Enums.ActionType.Approve:
                    ApproveDocument(input);
                    isNeedSendNotif = true;
                    break;
                case Enums.ActionType.Reject:
                    RejectDocument(input);
                    isNeedSendNotif = true;
                    break;
              
                case Enums.ActionType.GovApprove:
                case Enums.ActionType.GovPartialApprove:
                    GovApproveDocument(input);
                    isNeedSendNotif = true;
                    break;
                case Enums.ActionType.GovReject:
                    GovRejectedDocument(input);
                    break;
               
                  

            }

            //todo sent mail
            if (isNeedSendNotif)
                SendEmailWorkflow(input);


            _uow.SaveChanges();
        }

       

        private void SendEmailWorkflow(Pbck7Pbck3WorkflowDocumentInput input)
        {

            var pbck7Dto = Mapper.Map<Pbck7AndPbck3Dto>(_repositoryPbck7.Get(c => c.PBCK7_ID == input.DocumentId).FirstOrDefault());

            if ((input.ActionType == Enums.ActionType.GovApprove || input.ActionType == Enums.ActionType.GovPartialApprove)
                && pbck7Dto.Pbck7Status != Enums.DocumentStatus.Completed)
                return;

            var mailProcess = ProsesMailNotificationBody(pbck7Dto, input);

            //distinct double To email
            List<string> ListTo = mailProcess.To.Distinct().ToList();

            if (mailProcess.IsCCExist)
                //Send email with CC
                _messageService.SendEmailToListWithCC(ListTo, mailProcess.CC, mailProcess.Subject, mailProcess.Body, true);
            else
                _messageService.SendEmailToList(ListTo, mailProcess.Subject, mailProcess.Body, true);

        }

        private MailNotification ProsesMailNotificationBody(Pbck7AndPbck3Dto pbck7Dto, Pbck7Pbck3WorkflowDocumentInput input)
        {
            var bodyMail = new StringBuilder();
            var rc = new MailNotification();

            var rejected = _workflowHistoryBll.GetApprovedOrRejectedPOAStatusByDocumentNumber(new GetByFormTypeAndFormIdInput() { FormId = pbck7Dto.Pbck7Id, FormType = Enums.FormType.PBCK7 });
            var poaList = _poaBll.GetPoaByNppbkcId(pbck7Dto.NppbkcId);

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            string companyCode = "";
            var t001K = _t001Kbll.GetByBwkey(pbck7Dto.PlantId);
            if (t001K != null)
                companyCode = t001K.BUKRS;

            rc.Subject = "PBCK-7 " + pbck7Dto.Pbck7Number + " is " + EnumHelper.GetDescription(pbck7Dto.Pbck7Status);
            bodyMail.Append("Dear Team,<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Kindly be informed, " + rc.Subject + ". <br />");
            bodyMail.AppendLine();
            bodyMail.Append("<table><tr><td>Company Code </td><td>: " + companyCode + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>NPPBKC </td><td>: " + pbck7Dto.NppbkcId + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Document Number</td><td> : " + pbck7Dto.Pbck7Number + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Document Type</td><td> : PBCK-7</td></tr>");
            bodyMail.AppendLine();
            if (input.ActionType == Enums.ActionType.Reject)
            {
                bodyMail.Append("<tr><td>Comment</td><td> : " + input.Comment + "</td></tr>");
                bodyMail.AppendLine();
            }
            else if (input.ActionType == Enums.ActionType.GovApprove || input.ActionType == Enums.ActionType.GovPartialApprove)
            {
                var dbBack1 = _back1Services.GetBack1ByPbck7Id(pbck7Dto.Pbck7Id);

                string back1Date = ConvertHelper.ConvertDateToString(dbBack1.BACK1_DATE, "dd MMMM yyyy");
                
                bodyMail.Append("<tr><td>BACK-1 Number</td><td> : " + dbBack1.BACK1_NUMBER+ "</td></tr>");
                bodyMail.AppendLine();
                bodyMail.Append("<tr><td>BACK-1 Date</td><td> : " + back1Date + "</td></tr>");
                bodyMail.AppendLine();
              
            }
            bodyMail.Append("<tr colspan='2'><td><i>Please click this <a href='" + webRootUrl + "/PBCK7AndPBCK3/Edit/" + pbck7Dto.Pbck7Id + "'>link</a> to show detailed information</i></td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("</table>");
            bodyMail.AppendLine();
            bodyMail.Append("<br />Regards,<br />");
            switch (input.ActionType)
            {
                case Enums.ActionType.Submit:
                    if (pbck7Dto.Pbck7Status == Enums.DocumentStatus.WaitingForApproval)
                    {
                        if (rejected != null)
                        {
                            rc.To.Add(_poaBll.GetById(rejected.ACTION_BY).POA_EMAIL);
                        }
                        else
                        {
                            foreach (var poaDto in poaList)
                            {
                                rc.To.Add(poaDto.POA_EMAIL);
                            }
                        }

                        rc.CC.Add(_userBll.GetUserById(pbck7Dto.CreatedBy).EMAIL);
                    }
                    else if (pbck7Dto.Pbck7Status == Enums.DocumentStatus.WaitingForApprovalManager)
                    {
                        var poaData = _poaBll.GetById(pbck7Dto.CreatedBy);
                        rc.To.Add(GetManagerEmail(pbck7Dto.CreatedBy));
                        rc.CC.Add(poaData.POA_EMAIL);

                        foreach (var poaDto in poaList)
                        {
                            if (poaData.POA_ID != poaDto.POA_ID)
                                rc.To.Add(poaDto.POA_EMAIL);
                        }
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Approve:
                    if (pbck7Dto.Pbck7Status == Enums.DocumentStatus.WaitingForApprovalManager)
                    {
                        rc.To.Add(GetManagerEmail(pbck7Dto.ApprovedBy));

                        if (rejected != null)
                        {
                            rc.CC.Add(_poaBll.GetById(rejected.ACTION_BY).POA_EMAIL);
                        }
                        else
                        {
                            foreach (var poaDto in poaList)
                            {
                                rc.CC.Add(poaDto.POA_EMAIL);
                            }
                        }

                        rc.CC.Add(_userBll.GetUserById(pbck7Dto.CreatedBy).EMAIL);

                    }
                    else if (pbck7Dto.Pbck7Status == Enums.DocumentStatus.WaitingGovApproval)
                    {
                        var poaData = _poaBll.GetById(pbck7Dto.CreatedBy);
                        if (poaData != null)
                        {
                            //creator is poa user
                            rc.To.Add(poaData.POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(pbck7Dto.CreatedBy));
                        }
                        else
                        {
                            //creator is excise executive
                            var userData = _userBll.GetUserById(pbck7Dto.CreatedBy);
                            rc.To.Add(userData.EMAIL);
                            rc.CC.Add(_poaBll.GetById(pbck7Dto.ApprovedBy).POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(pbck7Dto.ApprovedBy));
                        }
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Reject:
                    //send notification to creator
                    var userDetail = _userBll.GetUserById(pbck7Dto.CreatedBy);
                    var poaData2 = _poaBll.GetById(pbck7Dto.CreatedBy);

                    if (pbck7Dto.ApprovedBy != null || poaData2 != null)
                    {
                        if (poaData2 == null)
                        {
                            var poa = _poaBll.GetById(pbck7Dto.ApprovedBy);
                            rc.To.Add(userDetail.EMAIL);
                            rc.CC.Add(poa.POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(pbck7Dto.ApprovedBy));
                        }
                        else
                        {
                            rc.To.Add(poaData2.POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(pbck7Dto.CreatedBy));
                        }
                    }
                    else
                    {
                        rc.To.Add(userDetail.EMAIL);

                        foreach (var poaDto in poaList)
                        {
                            rc.CC.Add(poaDto.POA_EMAIL);
                        }
                    }

                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.GovApprove:
                case Enums.ActionType.GovPartialApprove:
                case Enums.ActionType.Completed:
                    if (pbck7Dto.Pbck7Status == Enums.DocumentStatus.Completed)
                    {

                        var userData = _userBll.GetUserById(pbck7Dto.CreatedBy);
                        rc.To.Add(userData.EMAIL);
                        var poaData3 = _poaBll.GetById(pbck7Dto.CreatedBy);

                        if (poaData3 != null)
                        {
                            //creator is poa user
                            rc.CC.Add(GetManagerEmail(pbck7Dto.CreatedBy));

                        }
                        else
                        {
                            //creator is excise executive
                            rc.CC.Add(_poaBll.GetById(pbck7Dto.ApprovedBy).POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(pbck7Dto.ApprovedBy));


                        }
                        rc.IsCCExist = true;
                    }
                    break;



            }
            rc.Body = bodyMail.ToString();
            return rc;
        }

        private string GetManagerEmail(string poaId)
        {
            var managerId = _poaBll.GetManagerIdByPoaId(poaId);
            var managerDetail = _userBll.GetUserById(managerId);
            return managerDetail.EMAIL;
        }

        private void SubmitDocument(Pbck7Pbck3WorkflowDocumentInput input)
        {
            var dbData = _repositoryPbck7.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS != Enums.DocumentStatus.Draft && dbData.STATUS != Enums.DocumentStatus.Rejected)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            string newValue = "";
            string oldValue = EnumHelper.GetDescription(dbData.STATUS);

            dbData.STATUS = Enums.DocumentStatus.WaitingForApproval;

            input.DocumentNumber = dbData.PBCK7_NUMBER;

            AddWorkflowHistory(input);

            switch (input.UserRole)
            {
                case Enums.UserRole.User:
                    dbData.STATUS = Enums.DocumentStatus.WaitingForApproval;
                    newValue = EnumHelper.GetDescription(Enums.DocumentStatus.WaitingForApproval);
                    break;
                case Enums.UserRole.POA:
                    dbData.STATUS = Enums.DocumentStatus.WaitingForApprovalManager;
                    newValue = EnumHelper.GetDescription(Enums.DocumentStatus.WaitingForApprovalManager);
                    break;
                default:
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
            }

            //set change history
            SetChangeHistory(oldValue, newValue, "STATUS", input.UserId, dbData.PBCK7_ID.ToString());


        }

        private void ApproveDocument(Pbck7Pbck3WorkflowDocumentInput input)
        {
            var dbData = _repositoryPbck7.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //if (dbData.STATUS != Enums.DocumentStatus.WaitingForApproval &&
            //    dbData.STATUS != Enums.DocumentStatus.WaitingForApprovalManager)
            //    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
            //var isOperationAllow = _workflowBll.AllowApproveAndReject(new WorkflowAllowApproveAndRejectInput()
            //{
            //    CreatedUser = dbData.CREATED_BY,
            //    CurrentUser = input.UserId,
            //    DocumentStatus = dbData.STATUS,
            //    UserRole = input.UserRole,
            //    NppbkcId = dbData.NPPBKC,
            //    DocumentNumber = dbData.PBCK7_NUMBER
            //});

            //if (!isOperationAllow)
            //    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);


            string oldValue = EnumHelper.GetDescription(dbData.STATUS);
            string newValue = "";

            if (input.UserRole == Enums.UserRole.POA)
            {
                if (dbData.STATUS == Enums.DocumentStatus.WaitingForApproval)
                {
                    dbData.STATUS = Enums.DocumentStatus.WaitingForApprovalManager;
                    dbData.APPROVED_BY = input.UserId;
                    dbData.APPROVED_DATE = DateTime.Now;

                    ////get poa printed name
                    //string poaPrintedName = "";
                    //var poaData = _poaBll.GetDetailsById(input.UserId);
                    //if (poaData != null)
                    //    poaPrintedName = poaData.PRINTED_NAME;

                    ////todo add field poa
                    ////dbData.POA_PRINTED_NAME = poaPrintedName;

                    newValue = EnumHelper.GetDescription(Enums.DocumentStatus.WaitingForApprovalManager);
                }
                else
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
            }
            else if (input.UserRole == Enums.UserRole.Manager)
            {
                if (dbData.STATUS == Enums.DocumentStatus.WaitingForApprovalManager)
                {
                    dbData.STATUS = Enums.DocumentStatus.WaitingGovApproval;
                    dbData.APPROVED_BY_MANAGER = input.UserId;
                    dbData.APPROVED_BY_MANAGER_DATE = DateTime.Now;
                    newValue = EnumHelper.GetDescription(Enums.DocumentStatus.WaitingGovApproval);
                }
                else
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
            }


            input.DocumentNumber = dbData.PBCK7_NUMBER;

            AddWorkflowHistory(input);

            //set change history
            SetChangeHistory(oldValue, newValue, "STATUS", input.UserId, dbData.PBCK7_ID.ToString());

        }

        private void RejectDocument(Pbck7Pbck3WorkflowDocumentInput input)
        {
            var dbData = _repositoryPbck7.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS != Enums.DocumentStatus.WaitingForApproval &&
                dbData.STATUS != Enums.DocumentStatus.WaitingForApprovalManager &&
                dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            string oldValue = EnumHelper.GetDescription(dbData.STATUS);
            string newValue = "";

            //change back to draft
            dbData.STATUS = Enums.DocumentStatus.Rejected;
            newValue = EnumHelper.GetDescription(Enums.DocumentStatus.Rejected);

            dbData.REJECTED_BY = input.UserId;
            dbData.REJECTED_DATE = DateTime.Now;

            input.DocumentNumber = dbData.PBCK7_NUMBER;

            AddWorkflowHistory(input);

            //set change history
            SetChangeHistory(oldValue, newValue, "STATUS", input.UserId, dbData.PBCK7_ID.ToString());
        }

        private void WorkflowStatusGovAddChanges(Pbck7Pbck3WorkflowDocumentInput input, Enums.DocumentStatusGov? oldStatus, Enums.DocumentStatusGov newStatus)
        {
            //set changes log
            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Enums.MenuList.PBCK7,
                FORM_ID = input.DocumentId.ToString(),
                FIELD_NAME = "GOV_STATUS",
                NEW_VALUE = EnumHelper.GetDescription(newStatus),
                OLD_VALUE = oldStatus.HasValue ? EnumHelper.GetDescription(oldStatus) : "NULL",
                MODIFIED_BY = input.UserId,
                MODIFIED_DATE = DateTime.Now
            };

            _changesHistoryBll.AddHistory(changes);
        }

        private void WorkflowStatusGovAddChangesPbck3(Pbck3WorkflowDocumentInput input, Enums.DocumentStatusGov? oldStatus, Enums.DocumentStatusGov newStatus)
        {
            //set changes log
            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Enums.MenuList.PBCK3,
                FORM_ID = input.DocumentId.ToString(),
                FIELD_NAME = "GOV_STATUS",
                NEW_VALUE = EnumHelper.GetDescription(newStatus),
                OLD_VALUE = oldStatus.HasValue ? EnumHelper.GetDescription(oldStatus) : "NULL",
                MODIFIED_BY = input.UserId,
                MODIFIED_DATE = DateTime.Now
            };

            _changesHistoryBll.AddHistory(changes);
        }
        
        private void WorkflowStatusAddChanges(Pbck7Pbck3WorkflowDocumentInput input, Enums.DocumentStatus oldStatus, Enums.DocumentStatus newStatus)
        {
            //set changes log
            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Enums.MenuList.PBCK7,
                FORM_ID = input.DocumentId.ToString(),
                FIELD_NAME = "STATUS",
                NEW_VALUE = EnumHelper.GetDescription(newStatus),
                OLD_VALUE = EnumHelper.GetDescription(oldStatus),
                MODIFIED_BY = input.UserId,
                MODIFIED_DATE = DateTime.Now
            };
            _changesHistoryBll.AddHistory(changes);
        }

        private void WorkflowStatusAddChangesPbck3(Pbck3WorkflowDocumentInput input, Enums.DocumentStatus oldStatus, Enums.DocumentStatus newStatus)
        {
            //set changes log
            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Enums.MenuList.PBCK3,
                FORM_ID = input.DocumentId.ToString(),
                FIELD_NAME = "STATUS",
                NEW_VALUE = EnumHelper.GetDescription(newStatus),
                OLD_VALUE = EnumHelper.GetDescription(oldStatus),
                MODIFIED_BY = input.UserId,
                MODIFIED_DATE = DateTime.Now
            };
            _changesHistoryBll.AddHistory(changes);
        }


        private bool IsCompletedWorkflow(Pbck7Pbck3WorkflowDocumentInput input)
        {
            if (string.IsNullOrEmpty(input.AdditionalDocumentData.Back1No))
                return false;

            if (!input.AdditionalDocumentData.Back1Date.HasValue)
                return false;

            if (input.AdditionalDocumentData.Back1FileUploadList.Count == 0)
                return false;

            return true;
        }

        public void UpdateUploadedFileCompletedPbck7(List<BACK1_DOCUMENTDto> input)
        {
            
            _back1Services.InsertOrDeleteBack1Documents(input);
            _uow.SaveChanges();
        }

        private void UpdatePbck7ItemGovApproval(Enums.DocumentStatusGov govStatus, List<PBCK7_ITEMDto> listPbck7Item)
        {
            //string oldValue = "";
            //string newValue = "";

            foreach (var pbck7ItemDto in listPbck7Item)
            {
                var pbck7Item = _repositoryPbck7Item.GetByID(pbck7ItemDto.PBCK7_ITEM_ID);
                if (pbck7Item != null)
                {

                    if (govStatus == Enums.DocumentStatusGov.FullApproved)
                        pbck7Item.BACK1_QTY = pbck7Item.PBCK7_QTY;
                    else if (govStatus == Enums.DocumentStatusGov.PartialApproved)
                    {
                        //back1 qty must be > 0
                        if (!pbck7ItemDto.BACK1_QTY.HasValue
                            || pbck7ItemDto.BACK1_QTY.Value <= 0)
                            throw new BLLException(ExceptionCodes.BLLExceptions.Pbck7ItemErrorBack1QtyValue);

                        if (pbck7ItemDto.BACK1_QTY >= pbck7ItemDto.PBCK7_QTY)
                            throw new BLLException(ExceptionCodes.BLLExceptions.Pbck4ItemBack1MoreThanQtyValue);

                        pbck7Item.BACK1_QTY = pbck7ItemDto.BACK1_QTY;
                    }
                    _repositoryPbck7Item.Update(pbck7Item);

                    ////set change history
                    //SetChangeHistoryPbck3(oldValue, newValue, "STATUS", input.UserId, dbData.PBCK3_ID.ToString());
                }
            }
        }

        private void GovApproveDocument(Pbck7Pbck3WorkflowDocumentInput input)
        {
            var dbData = _repositoryPbck7.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            //Add Changes
            if (dbData.GOV_STATUS != input.StatusGovInput)
                WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, input.StatusGovInput);

            UpdatePbck7ItemGovApproval(input.StatusGovInput, input.Pbck7ItemDtos);

            dbData.GOV_STATUS = input.StatusGovInput;

            dbData.MODIFIED_DATE = DateTime.Now;
            dbData.MODIFIED_BY = input.UserId;

            input.DocumentNumber = dbData.PBCK7_NUMBER;

            var latestAction = _workflowHistoryBll.GetByFormNumber(input.DocumentNumber);

            if (latestAction.LastOrDefault().ACTION == input.ActionType && latestAction.LastOrDefault().UserId == input.UserId)
            {
                var latestWorkflow = latestAction.LastOrDefault();

                latestWorkflow.ACTION_DATE = DateTime.Now;

                _workflowHistoryBll.Save(latestWorkflow);
            }
            else
            {
                AddWorkflowHistory(input);
            }


            if (IsCompletedWorkflow(input))
            {
                //insert/update back1 based on pbck7Id
                var inputBack1 = new SaveBack1ByPbck7IdInput();
                inputBack1.Pbck7Id = dbData.PBCK7_ID;
                inputBack1.Back1Number = input.AdditionalDocumentData.Back1No;
                inputBack1.Back1Date = input.AdditionalDocumentData.Back1Date.HasValue
                    ? input.AdditionalDocumentData.Back1Date.Value
                    : DateTime.Now;

                inputBack1.Back1Documents = input.AdditionalDocumentData.Back1FileUploadList;

                _back1Services.SaveBack1ByPbck7Id(inputBack1);

                WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Completed);

                dbData.STATUS = Enums.DocumentStatus.Completed;
                input.ActionType = Enums.ActionType.Completed;

                //insert to pbck3
                var inputPbck3 = new InsertPbck3FromPbck7Input();
                inputPbck3.Pbck7Id = dbData.PBCK7_ID;
                inputPbck3.NppbkcId = dbData.NPPBKC;
                inputPbck3.UserId = input.UserId;
                inputPbck3.Pbck7ExecFrom = dbData.EXEC_DATE_FROM;
                inputPbck3.Pbck7ExecTo = dbData.EXEC_DATE_TO;

               var pbck3Number = _pbck3Services.InsertPbck3FromPbck7(inputPbck3);

                var inputWorkflowHistoryPbck3 = new Pbck3WorkflowDocumentInput();
                inputWorkflowHistoryPbck3.DocumentNumber = pbck3Number;
                inputWorkflowHistoryPbck3.UserId = input.UserId;
                inputWorkflowHistoryPbck3.UserRole = input.UserRole;
                inputWorkflowHistoryPbck3.ActionType = Enums.ActionType.Created;

                AddWorkflowHistoryPbck3(inputWorkflowHistoryPbck3);

                AddWorkflowHistory(input);
            }
        }

        private void GovRejectedDocument(Pbck7Pbck3WorkflowDocumentInput input)
        {
            var dbData = _repositoryPbck7.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            ////Add Changes
            WorkflowStatusAddChanges(input, dbData.STATUS, Enums.DocumentStatus.Rejected);
            WorkflowStatusGovAddChanges(input, dbData.GOV_STATUS, Enums.DocumentStatusGov.Rejected);

            dbData.STATUS = Enums.DocumentStatus.Rejected;
            dbData.GOV_STATUS = Enums.DocumentStatusGov.Rejected;

            dbData.MODIFIED_DATE = DateTime.Now;
            dbData.MODIFIED_BY = input.UserId;

            input.DocumentNumber = dbData.PBCK7_NUMBER;

            AddWorkflowHistory(input);

        }

        public Pbck3Output GetPbck3DetailsById(int id)
        {
            var output = new Pbck3Output();

            var data = _repositoryPbck3.Get(p => p.PBCK3_ID == id, null, "PBCK7, CK5, PBCK7.BACK1, PBCK7.PBCK7_ITEM, CK5.CK5_MATERIAL").FirstOrDefault();
            if (data == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //only mapper pbck3
            var result = Mapper.Map<Pbck3CompositeDto>(data);

            //map pbck7
            if (data.PBCK7 != null)
            {
                result.FromPbck7 = true;
                result.Pbck7Composite = Mapper.Map<Pbck3Pbck7DtoComposite>(data.PBCK7);
               
            }
            else//from ck5 market return
            {
                result.FromPbck7 = false;

                result.Ck5Composite.Ck5Dto = Mapper.Map<CK5Dto>(data.CK5);
                //details
                result.Ck5Composite.Ck5MaterialDto = Mapper.Map<List<CK5MaterialDto>>(data.CK5.CK5_MATERIAL);

                //workflow history
                var input = new GetByFormNumberInput();
                input.FormNumber = data.CK5.SUBMISSION_NUMBER;
                input.DocumentStatus = data.CK5.STATUS_ID;
                input.NPPBKC_Id = data.CK5.SOURCE_PLANT_NPPBKC_ID;

                result.Ck5Composite.ListWorkflowHistorys = _workflowHistoryBll.GetByFormNumber(input);

            }

            //back1
            BACK1 dbBack1 = null;
            dbBack1 = result.FromPbck7
                ? _back1Services.GetBack1ByPbck7Id(result.Pbck7Composite.Pbck7Id)
                : _back1Services.GetBack1ByCk5Id(result.Ck5Composite.Ck5Dto.CK5_ID);
            

            if (dbBack1 != null)
            {
                result.Back1Id = dbBack1.BACK1_ID;
                result.Back1Number = dbBack1.BACK1_NUMBER;
                result.Back1Date = dbBack1.BACK1_DATE;
                result.Back1Documents = new List<BACK1_DOCUMENT>();
                if (dbBack1.BACK1_DOCUMENT != null)
                {
                    result.Back1Documents = dbBack1.BACK1_DOCUMENT.ToList();
                }
            }

            //CK2

            var dbCk2 = _ck2Services.GetCk2ByPbck3Id(result.PBCK3_ID);
            if (dbCk2 != null)
            {
                result.Ck2Id = dbCk2.CK2_ID;
                result.Ck2Number = dbCk2.CK2_NUMBER;
                result.Ck2Date = dbCk2.CK2_DATE;
                result.Ck2Value = dbCk2.CK2_VALUE;
                result.Ck2Documents = new List<CK2_DOCUMENT>();
                if (dbCk2.CK2_DOCUMENT != null)
                {
                    result.Ck2Documents = dbCk2.CK2_DOCUMENT.ToList();
                }
            }

            //BACK3
            var dbBack3 = _back3Services.GetBack3ByPbck3Id(result.PBCK3_ID);
            if (dbBack3 != null)
            {
                result.Back3Id = dbBack3.BACK3_ID;
                result.Back3Number = dbBack3.BACK3_NUMBER;
                result.Back3Date = dbBack3.BACK3_DATE;
                if (dbBack3.BACK3_DOCUMENT != null)
                {
                    result.Back3Documents = dbBack3.BACK3_DOCUMENT.ToList();
                }
            }

            output.Pbck3CompositeDto = result;



            //change history data
            output.ListChangesHistorys = _changesHistoryBll.GetByFormTypeAndFormId(Enums.MenuList.PBCK3, output.Pbck3CompositeDto.PBCK3_ID.ToString());

            //workflow history
            var workflowInput = new GetByFormNumberInput();
            workflowInput.FormId = output.Pbck3CompositeDto.PBCK3_ID;
            workflowInput.FormNumber = output.Pbck3CompositeDto.PBCK3_NUMBER;
            workflowInput.DocumentStatus = output.Pbck3CompositeDto.STATUS.HasValue ? output.Pbck3CompositeDto.STATUS.Value : Enums.DocumentStatus.Draft;
            
            workflowInput.FormType = Enums.FormType.PBCK3;

            output.WorkflowHistoryPbck3 = _workflowHistoryBll.GetByFormNumber(workflowInput);

            if (output.Pbck3CompositeDto.FromPbck7)
            {
                workflowInput.FormId = output.Pbck3CompositeDto.PBCK7_ID.HasValue ? output.Pbck3CompositeDto.PBCK7_ID.Value : 0;
                workflowInput.FormNumber = output.Pbck3CompositeDto.Pbck7Composite.Pbck7Number;
                workflowInput.DocumentStatus = output.Pbck3CompositeDto.Pbck7Composite.Pbck7Status;
                workflowInput.FormType = Enums.FormType.PBCK7;

                output.WorkflowHistoryPbck7 = _workflowHistoryBll.GetByFormNumber(workflowInput);
            }

            


            return output;
        }

        private bool SetChangesHistoryPbck3(Pbck3Dto origin, Pbck3Dto dataModified, string userId)
        {
            bool isModified = false;

            var changesData = new Dictionary<string, bool>();

            changesData.Add("PBCK-3 DATE", origin.Pbck3Date == dataModified.Pbck3Date);
            changesData.Add("EXEC_DATE_FROM", origin.EXEC_DATE_FROM == dataModified.EXEC_DATE_FROM);
            changesData.Add("EXEC_DATE_TO", origin.EXEC_DATE_TO == dataModified.EXEC_DATE_TO);
            
            foreach (var listChange in changesData)
            {
                if (listChange.Value == false)
                {
                    var changes = new CHANGES_HISTORY();
                    changes.FORM_TYPE_ID = Enums.MenuList.PBCK3;
                    changes.FORM_ID = origin.Pbck3Id.ToString();
                    changes.FIELD_NAME = listChange.Key;
                    changes.MODIFIED_BY = userId;
                    changes.MODIFIED_DATE = DateTime.Now;

                    switch (listChange.Key)
                    {
                        case "PBCK-3 DATE":
                            changes.OLD_VALUE = origin.Pbck3Date.HasValue ? origin.Pbck3Date.Value.ToString("dd MMM yyyy") : string.Empty;
                            changes.NEW_VALUE = dataModified.Pbck3Date.HasValue ? dataModified.Pbck3Date.Value.ToString("dd MMM yyyy") : string.Empty; 
                            break;
                        case "EXEC_DATE_FROM":
                            changes.OLD_VALUE = origin.EXEC_DATE_FROM.ToString("dd MMM yyyy");
                            changes.NEW_VALUE = dataModified.EXEC_DATE_FROM.ToString("dd MMM yyyy");
                            break;
                        case "EXEC_DATE_TO":
                             changes.OLD_VALUE = origin.EXEC_DATE_TO.ToString("dd MMM yyyy");
                            changes.NEW_VALUE = dataModified.EXEC_DATE_TO.ToString("dd MMM yyyy");
                            break;
                    }

                    _changesHistoryBll.AddHistory(changes);
                    isModified = true;
                }
            }

            return isModified;
        }

        public Pbck3Dto SavePbck3(Pbck3SaveInput input)
        {
            bool isModified = false;

            //workflowhistory
            var inputWorkflowHistory = new Pbck7Pbck3WorkflowHistoryInput();

            PBCK3 dbData = null;

            if (input.Pbck3Dto.Pbck3Id > 0)
            {
                //update
                dbData = _repositoryPbck3.Get(c => c.PBCK3_ID == input.Pbck3Dto.Pbck3Id).FirstOrDefault();
                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                //set changes history
                var origin = Mapper.Map<Pbck3Dto>(dbData);


                isModified = SetChangesHistoryPbck3(origin, input.Pbck3Dto, input.UserId);

                //Mapper.Map<Pbck3Dto, PBCK3>(input.Pbck3Dto, dbData);
                DateTime pbck3Date = DateTime.Now;
                if (input.Pbck3Dto.Pbck3Date.HasValue)
                    pbck3Date = input.Pbck3Dto.Pbck3Date.Value;
                dbData.PBCK3_DATE = pbck3Date;


                dbData.EXEC_DATE_FROM = input.Pbck3Dto.EXEC_DATE_FROM;
                dbData.EXEC_DATE_TO = input.Pbck3Dto.EXEC_DATE_TO;

                if (dbData.STATUS == Enums.DocumentStatus.Rejected)
                {
                    dbData.STATUS = Enums.DocumentStatus.Draft;
                }

                dbData.MODIFIED_DATE = DateTime.Now;
                dbData.MODIFIED_BY = input.UserId;
                
                inputWorkflowHistory.ActionType = Enums.ActionType.Modified;
                
            }
            else
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            inputWorkflowHistory.DocumentId = dbData.PBCK3_ID;
            inputWorkflowHistory.DocumentNumber = dbData.PBCK3_NUMBER;
            inputWorkflowHistory.UserId = input.UserId;
            inputWorkflowHistory.UserRole = input.UserRole;
            inputWorkflowHistory.FormType = Enums.FormType.PBCK3;

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

            var resultDto = Mapper.Map<Pbck3Dto>(dbData);
            resultDto.IsModifiedHistory = isModified;

            return resultDto;
         
        }

        public void PBCK3Workflow(Pbck3WorkflowDocumentInput input)
        {
            var isNeedSendNotif = false;

            switch (input.ActionType)
            {
                case Enums.ActionType.Submit:
                    SubmitDocumentPbck3(input);
                    isNeedSendNotif = true;
                    break;
                case Enums.ActionType.Approve:
                    ApproveDocumentPbck3(input);
                    isNeedSendNotif = true;
                    break;
                case Enums.ActionType.Reject:
                    RejectDocumentPbck3(input);
                    isNeedSendNotif = true;
                    break;

                case Enums.ActionType.GovApprove:
                case Enums.ActionType.GovPartialApprove:
                    GovApproveDocumentPbck3(input);

                    break;
                case Enums.ActionType.GovReject:
                    GovRejectedDocumentPbck3(input);
                    break;
              

            }

         
            if (isNeedSendNotif)
                SendEmailWorkflowPbck3(input);


            _uow.SaveChanges();
        }

        private void SubmitDocumentPbck3(Pbck3WorkflowDocumentInput input)
        {
            var dbData = _repositoryPbck3.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS != Enums.DocumentStatus.Draft && dbData.STATUS != Enums.DocumentStatus.Rejected)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            string newValue = "";
            string oldValue = EnumHelper.GetDescription(dbData.STATUS);

            dbData.STATUS = Enums.DocumentStatus.WaitingForApproval;

            input.DocumentNumber = dbData.PBCK3_NUMBER;

            AddWorkflowHistoryPbck3(input);

            switch (input.UserRole)
            {
                case Enums.UserRole.User:
                    dbData.STATUS = Enums.DocumentStatus.WaitingForApproval;
                    newValue = EnumHelper.GetDescription(Enums.DocumentStatus.WaitingForApproval);
                    break;
                case Enums.UserRole.POA:
                    dbData.STATUS = Enums.DocumentStatus.WaitingForApprovalManager;
                    newValue = EnumHelper.GetDescription(Enums.DocumentStatus.WaitingForApprovalManager);
                    break;
                default:
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
            }

            //set change history
            SetChangeHistoryPbck3(oldValue, newValue, "STATUS", input.UserId, dbData.PBCK3_ID.ToString());


        }

        private void ApproveDocumentPbck3(Pbck3WorkflowDocumentInput input)
        {
            var dbData = _repositoryPbck3.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //if (dbData.STATUS != Enums.DocumentStatus.WaitingForApproval &&
            //    dbData.STATUS != Enums.DocumentStatus.WaitingForApprovalManager)
            //    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
            //string nppbkcId = "";
            //var outputResult = GetPbck3DetailsById(input.DocumentId);
            //if (outputResult.Pbck3CompositeDto.FromPbck7)
            //    nppbkcId = outputResult.Pbck3CompositeDto.Pbck7Composite.NppbkcId;
            //else
            //    nppbkcId = outputResult.Pbck3CompositeDto.Ck5Composite.Ck5Dto.SOURCE_PLANT_NPPBKC_ID;

            //var isOperationAllow = _workflowBll.AllowApproveAndReject(new WorkflowAllowApproveAndRejectInput()
            //{
            //    CreatedUser = dbData.CREATED_BY,
            //    CurrentUser = input.UserId,
            //    DocumentStatus = dbData.STATUS.HasValue ? dbData.STATUS.Value : Enums.DocumentStatus.Draft,
            //    UserRole = input.UserRole,
            //    NppbkcId = nppbkcId,
            //    DocumentNumber = dbData.PBCK3_NUMBER
            //});

            //if (!isOperationAllow)
            //    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);


            string oldValue = EnumHelper.GetDescription(dbData.STATUS);
            string newValue = "";

            if (input.UserRole == Enums.UserRole.POA)
            {
                if (dbData.STATUS == Enums.DocumentStatus.WaitingForApproval)
                {
                    dbData.STATUS = Enums.DocumentStatus.WaitingForApprovalManager;
                    dbData.APPROVED_BY = input.UserId;
                    dbData.APPROVED_DATE = DateTime.Now;

                    ////get poa printed name
                    //string poaPrintedName = "";
                    //var poaData = _poaBll.GetDetailsById(input.UserId);
                    //if (poaData != null)
                    //    poaPrintedName = poaData.PRINTED_NAME;

                    ////todo add field poa
                    ////dbData.POA_PRINTED_NAME = poaPrintedName;

                    newValue = EnumHelper.GetDescription(Enums.DocumentStatus.WaitingForApprovalManager);
                }
                else
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
            }
            else if (input.UserRole == Enums.UserRole.Manager)
            {
                if (dbData.STATUS == Enums.DocumentStatus.WaitingForApprovalManager)
                {
                    dbData.STATUS = Enums.DocumentStatus.WaitingGovApproval;
                    dbData.APPROVED_BY_MANAGER = input.UserId;
                    dbData.APPROVED_BY_MANAGER_DATE = DateTime.Now;
                    newValue = EnumHelper.GetDescription(Enums.DocumentStatus.WaitingGovApproval);
                }
                else
                    throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);
            }


            input.DocumentNumber = dbData.PBCK3_NUMBER;

            AddWorkflowHistoryPbck3(input);

            //set change history
            SetChangeHistoryPbck3(oldValue, newValue, "STATUS", input.UserId, dbData.PBCK3_ID.ToString());

        }

        private void RejectDocumentPbck3(Pbck3WorkflowDocumentInput input)
        {
            var dbData = _repositoryPbck3.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS != Enums.DocumentStatus.WaitingForApproval &&
                dbData.STATUS != Enums.DocumentStatus.WaitingForApprovalManager &&
                dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            string oldValue = EnumHelper.GetDescription(dbData.STATUS);
            string newValue = "";

            //change back to draft
            dbData.STATUS = Enums.DocumentStatus.Rejected;
            newValue = EnumHelper.GetDescription(Enums.DocumentStatus.Rejected);

            dbData.REJECTED_BY = input.UserId;
            dbData.REJECTED_DATE = DateTime.Now;

            input.DocumentNumber = dbData.PBCK3_NUMBER;

            AddWorkflowHistoryPbck3(input);

            //set change history
            SetChangeHistoryPbck3(oldValue, newValue, "STATUS", input.UserId, dbData.PBCK3_ID.ToString());
        }

        private bool IsCompletedWorkflowPbck3(Pbck3WorkflowDocumentInput input)
        {
            if (string.IsNullOrEmpty(input.AdditionalDocumentData.Back3No))
                return false;

            if (!input.AdditionalDocumentData.Back3Date.HasValue)
                return false;

            if (input.AdditionalDocumentData.Back3FileUploadList.Count == 0)
            {
                if (!_back3Services.IsExistBack3DocumentByPbck3(input.DocumentId))
                    return false;
            }

            if (string.IsNullOrEmpty(input.AdditionalDocumentData.Ck2No))
                return false;

            if (!input.AdditionalDocumentData.Ck2Date.HasValue)
                return false;

            if (!input.AdditionalDocumentData.Ck2Value.HasValue)
                return false;

            if (input.AdditionalDocumentData.Ck2FileUploadList.Count == 0)
                if (!_ck2Services.IsExistCk2DocumentByPbck3(input.DocumentId))
                    return false;

            return true;
        }

        public void UpdateUploadedFileCompletedPbck3(List<BACK3_DOCUMENTDto> inputBack3, List<CK2_DOCUMENTDto> inputCk2)
        {

            _back3Services.InsertOrDeleteBack3Item(inputBack3);
            _ck2Services.InsertOrDeleteCk2Item(inputCk2);
            _uow.SaveChanges();
        }


        private void GovApproveDocumentPbck3(Pbck3WorkflowDocumentInput input)
        {
            var dbData = _repositoryPbck3.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);


            //Add Changes
            if (dbData.GOV_STATUS != input.GovStatusInput)
                WorkflowStatusGovAddChangesPbck3(input, dbData.GOV_STATUS, input.GovStatusInput);


            dbData.GOV_STATUS = input.GovStatusInput;

            dbData.MODIFIED_DATE = DateTime.Now;
            dbData.MODIFIED_BY = input.UserId;

            //prepare for set changes history
            //get origin data
            var originBack3 = _back3Services.GetBack3ByPbck3Id(input.DocumentId);
            if (originBack3 == null)
                originBack3 = new BACK3();
            var modifiedBack3 = Mapper.Map<Back3Dto>(input.AdditionalDocumentData);
            //add to change log
            SetChangesHistoryBack3(Mapper.Map<Back3Dto>(originBack3), modifiedBack3, input.UserId, input.DocumentId.ToString());
            
            //insert/update back3
            var inputBack3 = new SaveBack3ByPbck3IdInput();
            inputBack3.Back3Number = input.AdditionalDocumentData.Back3No;
            inputBack3.Back3Date = input.AdditionalDocumentData.Back3Date;
            inputBack3.Pbck3Id = dbData.PBCK3_ID;
            inputBack3.Back3Documents = input.AdditionalDocumentData.Back3FileUploadList;
            _back3Services.SaveBack3ByPbck3Id(inputBack3);


            //prepare for set changes history
            //get origin data
            var originCk2 = _ck2Services.GetCk2ByPbck3Id(input.DocumentId);
            if (originCk2 == null)
                originCk2 = new CK2();
            var modifiedCk2 = Mapper.Map<Ck2Dto>(input.AdditionalDocumentData);
            //add to change log
            SetChangesHistoryCk2(Mapper.Map<Ck2Dto>(originCk2), modifiedCk2, input.UserId, input.DocumentId.ToString());


            //insert/update ck2
            var inputCk2 = new SaveCk2ByPbck3IdInput();
            inputCk2.Ck2Number = input.AdditionalDocumentData.Ck2No;
            inputCk2.Ck2Date = input.AdditionalDocumentData.Ck2Date;
            inputCk2.Ck2Value = input.AdditionalDocumentData.Ck2Value;
            inputCk2.Pbck3Id = dbData.PBCK3_ID;
            inputCk2.Ck2Documents = input.AdditionalDocumentData.Ck2FileUploadList;
            _ck2Services.SaveCk2ByPbck3Id(inputCk2);

            input.DocumentNumber = dbData.PBCK3_NUMBER;

            var latestAction = _workflowHistoryBll.GetByFormNumber(input.DocumentNumber);

            if (latestAction.LastOrDefault().ACTION == input.ActionType && latestAction.LastOrDefault().UserId == input.UserId)
            {
                var latestWorkflow = latestAction.LastOrDefault();

                latestWorkflow.ACTION_DATE = DateTime.Now;

                _workflowHistoryBll.Save(latestWorkflow);
            }
            else
            {
                AddWorkflowHistoryPbck3(input);
            }

            if (IsCompletedWorkflowPbck3(input))
            {
                WorkflowStatusAddChangesPbck3(input, dbData.STATUS.Value, Enums.DocumentStatus.Completed);

                dbData.STATUS = Enums.DocumentStatus.Completed;
                input.ActionType = Enums.ActionType.Completed;

                AddWorkflowHistoryPbck3(input);
            }


            

        }

        private void SetChangesHistoryBack3(Back3Dto origin, Back3Dto dataModified, string userId, string formId)
        {
            var changesData = new Dictionary<string, bool>();

            changesData.Add("BACK3_NUMBER", origin.Back3Number == dataModified.Back3Number);
            changesData.Add("BACK3_DATE", origin.Back3Date == dataModified.Back3Date);
            

            foreach (var listChange in changesData)
            {
                if (listChange.Value == false)
                {
                    var changes = new CHANGES_HISTORY();
                    changes.FORM_TYPE_ID = Enums.MenuList.PBCK3;
                    changes.FORM_ID = formId;
                    changes.FIELD_NAME = listChange.Key;
                    changes.MODIFIED_BY = userId;
                    changes.MODIFIED_DATE = DateTime.Now;
                    switch (listChange.Key)
                    {
                        case "BACK3_NUMBER":
                            changes.OLD_VALUE = origin.Back3Number;
                            changes.NEW_VALUE = dataModified.Back3Number;
                            break;
                        case "BACK3_DATE":
                            changes.OLD_VALUE = origin.Back3Date.HasValue ? origin.Back3Date.Value.ToString("dd MMM yyyy") : string.Empty;
                            changes.NEW_VALUE = dataModified.Back3Date.HasValue ? dataModified.Back3Date.Value.ToString("dd MMM yyyy") : string.Empty;
                            break;
                      
                    }
                    _changesHistoryBll.AddHistory(changes);
                }
            }
        }

        private void SetChangesHistoryCk2(Ck2Dto origin, Ck2Dto dataModified, string userId, string formId)
        {
            var changesData = new Dictionary<string, bool>();

            changesData.Add("CK2_NUMBER", origin.Ck2Number == dataModified.Ck2Number);
            changesData.Add("CK2_DATE", origin.Ck2Date == dataModified.Ck2Date);
            changesData.Add("CK2_VALUE", origin.Ck2Value == dataModified.Ck2Value);

            foreach (var listChange in changesData)
            {
                if (listChange.Value == false)
                {
                    var changes = new CHANGES_HISTORY();
                    changes.FORM_TYPE_ID = Enums.MenuList.PBCK3;
                    changes.FORM_ID = formId;
                    changes.FIELD_NAME = listChange.Key;
                    changes.MODIFIED_BY = userId;
                    changes.MODIFIED_DATE = DateTime.Now;
                    switch (listChange.Key)
                    {
                        case "CK2_NUMBER":
                            changes.OLD_VALUE = origin.Ck2Number;
                            changes.NEW_VALUE = dataModified.Ck2Number;
                            break;
                        case "CK2_DATE":
                            changes.OLD_VALUE = origin.Ck2Date.HasValue ? origin.Ck2Date.Value.ToString("dd MMM yyyy") : string.Empty;
                            changes.NEW_VALUE = dataModified.Ck2Date.HasValue ? dataModified.Ck2Date.Value.ToString("dd MMM yyyy") : string.Empty;
                            break;
                        case "CK2_VALUE":
                            changes.OLD_VALUE = ConvertHelper.ConvertDecimalToString(origin.Ck2Value);
                            changes.NEW_VALUE = ConvertHelper.ConvertDecimalToString(dataModified.Ck2Value); 
                            break;

                    }
                    _changesHistoryBll.AddHistory(changes);
                }
            }
        }

        private void GovRejectedDocumentPbck3(Pbck3WorkflowDocumentInput input)
        {
            var dbData = _repositoryPbck3.GetByID(input.DocumentId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbData.STATUS != Enums.DocumentStatus.WaitingGovApproval)
                throw new BLLException(ExceptionCodes.BLLExceptions.OperationNotAllowed);

            ////Add Changes
            WorkflowStatusAddChangesPbck3(input, dbData.STATUS.Value, Enums.DocumentStatus.Rejected);
            WorkflowStatusGovAddChangesPbck3(input, dbData.GOV_STATUS, Enums.DocumentStatusGov.Rejected);

            dbData.STATUS = Enums.DocumentStatus.Rejected;
            dbData.GOV_STATUS = Enums.DocumentStatusGov.Rejected;

            dbData.MODIFIED_DATE = DateTime.Now;
            dbData.MODIFIED_BY = input.UserId;

            input.DocumentNumber = dbData.PBCK3_NUMBER;

            AddWorkflowHistoryPbck3(input);

        }

        public void SendMailCompletedPbck3Document(Pbck3WorkflowDocumentInput input)
        {
            input.ActionType = Enums.ActionType.GovApprove;
            SendEmailWorkflowPbck3(input);
        }
        private void SendEmailWorkflowPbck3(Pbck3WorkflowDocumentInput input)
        {

            //var pbck3Dto = Mapper.Map<Pbck3Dto>(_repositoryPbck3.Get(c => c.PBCK3_ID == input.DocumentId).FirstOrDefault());

            var pbck3CompositeDto = GetPbck3DetailsById(input.DocumentId).Pbck3CompositeDto;
            //completed from controller after create xml file
            //if ((input.ActionType == Enums.ActionType.GovApprove || input.ActionType == Enums.ActionType.GovPartialApprove)
            //    && pbck7Dto.Pbck7Status != Enums.DocumentStatus.Completed)
            //    return;

            var mailProcess = ProsesMailNotificationBodyPbck3(pbck3CompositeDto, input);

            //distinct double To email
            List<string> ListTo = mailProcess.To.Distinct().ToList();

            if (mailProcess.IsCCExist)
                //Send email with CC
                _messageService.SendEmailToListWithCC(ListTo, mailProcess.CC, mailProcess.Subject, mailProcess.Body, true);
            else
                _messageService.SendEmailToList(ListTo, mailProcess.Subject, mailProcess.Body, true);

        }

        private MailNotification ProsesMailNotificationBodyPbck3(Pbck3CompositeDto pbck3CompositeDto, Pbck3WorkflowDocumentInput input)
        {
            var bodyMail = new StringBuilder();
            var rc = new MailNotification();

            var rejected = _workflowHistoryBll.GetApprovedOrRejectedPOAStatusByDocumentNumber(new GetByFormTypeAndFormIdInput() { FormId = pbck3CompositeDto.PBCK3_ID, FormType = Enums.FormType.PBCK3 });
            string nppbkcId = "";
            string plantId = "";
            if (pbck3CompositeDto.FromPbck7)
            {
                nppbkcId = pbck3CompositeDto.Pbck7Composite.NppbkcId;
                plantId = pbck3CompositeDto.Pbck7Composite.PlantId;
            }
            else
            {
                nppbkcId = pbck3CompositeDto.Ck5Composite.Ck5Dto.SOURCE_PLANT_NPPBKC_ID;
                plantId = pbck3CompositeDto.Ck5Composite.Ck5Dto.SOURCE_PLANT_ID;
            }

            var poaList = _poaBll.GetPoaByNppbkcId(nppbkcId);

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            string companyCode = "";
            var t001K = _t001Kbll.GetByBwkey(plantId);
            if (t001K != null)
                companyCode = t001K.BUKRS;

            rc.Subject = "PBCK-3 " + pbck3CompositeDto.PBCK3_NUMBER + " is " + EnumHelper.GetDescription(pbck3CompositeDto.STATUS);
            bodyMail.Append("Dear Team,<br />");
            bodyMail.AppendLine();
            bodyMail.Append("Kindly be informed, " + rc.Subject + ". <br />");
            bodyMail.AppendLine();
            bodyMail.Append("<table><tr><td>Company Code </td><td>: " + companyCode + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>NPPBKC </td><td>: " + nppbkcId + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Document Number</td><td> : " + pbck3CompositeDto.PBCK3_NUMBER + "</td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("<tr><td>Document Type</td><td> : PBCK-3</td></tr>");
            bodyMail.AppendLine();
            if (input.ActionType == Enums.ActionType.Reject)
            {
                bodyMail.Append("<tr><td>Comment</td><td> : " + input.Comment + "</td></tr>");
                bodyMail.AppendLine();
            }
            else if (input.ActionType == Enums.ActionType.GovApprove || input.ActionType == Enums.ActionType.GovPartialApprove)
            {
                var dbBack3 = _back3Services.GetBack3ByPbck3Id(pbck3CompositeDto.PBCK3_ID);

                string back3Date = ConvertHelper.ConvertDateToString(dbBack3.BACK3_DATE, "dd MMMM yyyy");

                bodyMail.Append("<tr><td>BACK-3 Number</td><td> : " + dbBack3.BACK3_NUMBER + "</td></tr>");
                bodyMail.AppendLine();
                bodyMail.Append("<tr><td>BACK-3 Date</td><td> : " + back3Date + "</td></tr>");
                bodyMail.AppendLine();

                var dbCk2 = _ck2Services.GetCk2ByPbck3Id(pbck3CompositeDto.PBCK3_ID);

                string ck2Date = ConvertHelper.ConvertDateToString(dbCk2.CK2_DATE, "dd MMMM yyyy");

                bodyMail.Append("<tr><td>CK-2 Number</td><td> : " + dbCk2.CK2_NUMBER + "</td></tr>");
                bodyMail.AppendLine();
                bodyMail.Append("<tr><td>CK-2 Date</td><td> : " + ck2Date + "</td></tr>");
                bodyMail.AppendLine();
                bodyMail.Append("<tr><td>CK-2 Value</td><td> : " + ConvertHelper.ConvertDecimalToString(dbCk2.CK2_VALUE) + "</td></tr>");
                bodyMail.AppendLine();


            }
            bodyMail.Append("<tr colspan='2'><td><i>Please click this <a href='" + webRootUrl + "/PBCK7AndPBCK3/EditPbck3/" + pbck3CompositeDto.PBCK3_ID + "'>link</a> to show detailed information</i></td></tr>");
            bodyMail.AppendLine();
            bodyMail.Append("</table>");
            bodyMail.AppendLine();
            bodyMail.Append("<br />Regards,<br />");
            switch (input.ActionType)
            {
                case Enums.ActionType.Submit:
                    if (pbck3CompositeDto.STATUS == Enums.DocumentStatus.WaitingForApproval)
                    {
                        if (rejected != null)
                        {
                            rc.To.Add(_poaBll.GetById(rejected.ACTION_BY).POA_EMAIL);
                        }
                        else
                        {
                            foreach (var poaDto in poaList)
                            {
                                rc.To.Add(poaDto.POA_EMAIL);
                            }
                        }

                        rc.CC.Add(_userBll.GetUserById(pbck3CompositeDto.CREATED_BY).EMAIL);
                    }
                    else if (pbck3CompositeDto.STATUS == Enums.DocumentStatus.WaitingForApprovalManager)
                    {
                        var poaData = _poaBll.GetById(pbck3CompositeDto.CREATED_BY);
                        rc.To.Add(GetManagerEmail(pbck3CompositeDto.CREATED_BY));
                        rc.CC.Add(poaData.POA_EMAIL);

                        foreach (var poaDto in poaList)
                        {
                            if (poaData.POA_ID != poaDto.POA_ID)
                                rc.To.Add(poaDto.POA_EMAIL);
                        }
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Approve:
                    if (pbck3CompositeDto.STATUS == Enums.DocumentStatus.WaitingForApprovalManager)
                    {
                        rc.To.Add(GetManagerEmail(pbck3CompositeDto.APPROVED_BY));

                        if (rejected != null)
                        {
                            rc.CC.Add(_poaBll.GetById(rejected.ACTION_BY).POA_EMAIL);
                        }
                        else
                        {
                            foreach (var poaDto in poaList)
                            {
                                rc.CC.Add(poaDto.POA_EMAIL);
                            }
                        }

                        rc.CC.Add(_userBll.GetUserById(pbck3CompositeDto.CREATED_BY).EMAIL);

                    }
                    else if (pbck3CompositeDto.STATUS == Enums.DocumentStatus.WaitingGovApproval)
                    {
                        var poaData = _poaBll.GetById(pbck3CompositeDto.CREATED_BY);
                        if (poaData != null)
                        {
                            //creator is poa user
                            rc.To.Add(poaData.POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(pbck3CompositeDto.CREATED_BY));
                        }
                        else
                        {
                            //creator is excise executive
                            var userData = _userBll.GetUserById(pbck3CompositeDto.CREATED_BY);
                            rc.To.Add(userData.EMAIL);
                            rc.CC.Add(_poaBll.GetById(pbck3CompositeDto.APPROVED_BY).POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(pbck3CompositeDto.APPROVED_BY));
                        }
                    }
                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.Reject:
                    //send notification to creator
                    var userDetail = _userBll.GetUserById(pbck3CompositeDto.CREATED_BY);
                    var poaData2 = _poaBll.GetById(pbck3CompositeDto.CREATED_BY);

                    if (pbck3CompositeDto.APPROVED_BY != null || poaData2 != null)
                    {
                        if (poaData2 == null)
                        {
                            var poa = _poaBll.GetById(pbck3CompositeDto.APPROVED_BY);
                            rc.To.Add(userDetail.EMAIL);
                            rc.CC.Add(poa.POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(pbck3CompositeDto.APPROVED_BY));
                        }
                        else
                        {
                            rc.To.Add(poaData2.POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(pbck3CompositeDto.CREATED_BY));
                        }
                    }
                    else
                    {
                        rc.To.Add(userDetail.EMAIL);

                        foreach (var poaDto in poaList)
                        {
                            rc.CC.Add(poaDto.POA_EMAIL);
                        }
                    }

                    rc.IsCCExist = true;
                    break;
                case Enums.ActionType.GovApprove:
                case Enums.ActionType.GovPartialApprove:
                    if (pbck3CompositeDto.STATUS == Enums.DocumentStatus.Completed)
                    {

                        var userData = _userBll.GetUserById(pbck3CompositeDto.CREATED_BY);
                        rc.To.Add(userData.EMAIL);
                        var poaData3 = _poaBll.GetById(pbck3CompositeDto.CREATED_BY);

                        if (poaData3 != null)
                        {
                            //creator is poa user
                            rc.CC.Add(GetManagerEmail(pbck3CompositeDto.CREATED_BY));

                        }
                        else
                        {
                            //creator is excise executive
                            rc.CC.Add(_poaBll.GetById(pbck3CompositeDto.APPROVED_BY).POA_EMAIL);
                            rc.CC.Add(GetManagerEmail(pbck3CompositeDto.APPROVED_BY));


                        }
                        rc.IsCCExist = true;
                    }
                    break;



            }
            rc.Body = bodyMail.ToString();
            return rc;
        }



        public List<GetListFaCodeByPlantOutput> GetListFaCodeByPlant(string plantId)
        {
            var dbBrand = _brandRegistrationServices.GetBrandByPlant(plantId);

            return Mapper.Map<List<GetListFaCodeByPlantOutput>>(dbBrand);
        }

        public GetBrandItemsByPlantAndFaCodeOutput GetBrandItemsByPlantAndFaCode(string plantId , string faCode)
        {
            var result = new GetBrandItemsByPlantAndFaCodeOutput();
            var dbBrand = _brandRegistrationServices.GetByPlantIdAndFaCode(plantId, faCode);
            if (dbBrand == null)
            {
                result.PlantId = plantId;
                result.FaCode = faCode;
                result.ProductAlias = "";
                result.BrandName = "";
                result.BrandContent = "0";
                result.Hje = "0";
                result.Tariff = "0";
                result.SeriesValue = "";
            }
            else
            {

                if (dbBrand.ZAIDM_EX_PRODTYP != null)
                    result.ProductAlias = dbBrand.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS;


                result.BrandName = dbBrand.BRAND_CE;
                if (dbBrand.ZAIDM_EX_SERIES != null)
                    result.SeriesValue = dbBrand.ZAIDM_EX_SERIES.SERIES_CODE;

                result.BrandContent = ConvertHelper.ConvertToDecimalOrZero(dbBrand.BRAND_CONTENT).ToString();

                result.Hje = dbBrand.HJE_IDR.HasValue ? dbBrand.HJE_IDR.Value.ToString() : "0";
                result.Tariff = dbBrand.TARIFF.HasValue ? dbBrand.TARIFF.Value.ToString() : "0";
                

            }

            return result;
        }

        #region ------------- Get Print Out Data -------------

        public Pbck73PrintOutDto GetPbck7PrintOutData(int pbck7Id)
        {
            var dbData = _repositoryPbck7.Get(c => c.PBCK7_ID == pbck7Id, null, "PBCK7_ITEM").FirstOrDefault();

            if (dbData == null) return null;

            var rc = Mapper.Map<Pbck73PrintOutDto>(dbData);

            //set exclude on mapper
            //get POA Data
            var poaId = string.IsNullOrEmpty(dbData.APPROVED_BY) ? dbData.CREATED_BY : dbData.APPROVED_BY;
            rc = SetPoaData(rc, poaId);

            //Get Company Data
            rc = SetCompanyData(rc, dbData.PLANT_ID);

            //get nppbkc data
            rc = SetNppbkcData(rc, dbData.NPPBKC);

            //set header footer data by CompanyCode and FormTypeId
            var headerFooterData = _headerFooterBll.GetByComanyAndFormType(new HeaderFooterGetByComanyAndFormTypeInput()
            {
                FormTypeId = Enums.FormType.PBCK7,
                CompanyCode = rc.CompanyCode
            });

            rc.HeaderFooter = headerFooterData;
            rc.PrintedDate = DateReportString(dbData.PBCK7_DATE);
            rc = SetExecutionDate(rc);
            return rc;
        }

        public Pbck73PrintOutDto GetPbck3PrintOutData(int pbck3Id)
        {
            var dbData = _repositoryPbck3.Get(c => c.PBCK3_ID == pbck3Id, null, "PBCK7, PBCK7.PBCK7_ITEM, CK5, CK5.CK5_MATERIAL").FirstOrDefault();

            if (dbData == null) return null;

            var rc = Mapper.Map<Pbck73PrintOutDto>(dbData);

            //set exclude on mapper
            //get POA Data
            var poaId = string.IsNullOrEmpty(dbData.APPROVED_BY) ? dbData.CREATED_BY : dbData.APPROVED_BY;
            rc = SetPoaData(rc, poaId);

            
            if (dbData.PBCK7_ID.HasValue)
            {
                //data come from PBCK7
                if (dbData.PBCK7 != null)
                {
                    rc = SetCompanyData(rc, dbData.PBCK7.PLANT_ID);
                    rc = SetNppbkcData(rc, dbData.PBCK7.NPPBKC);

                    //set Items
                    rc.Items = Mapper.Map<List<Pbck73ItemPrintOutDto>>(dbData.PBCK7.PBCK7_ITEM);
                }
            }
            else
            {
                //data come from CK5
                if (dbData.CK5 != null)
                {
                    rc = SetCompanyData(rc, dbData.CK5.SOURCE_PLANT_ID);
                    rc = SetNppbkcData(rc, dbData.CK5.SOURCE_PLANT_NPPBKC_ID);
                    //get from CK5_Material, but need to process
                    //todo: ask to get Items//rc.Items = Mapper.Map<List<Pbck73ItemPrintOutDto>>(dbData.CK5.CK5_MATERIAL);
                    rc.Items = new List<Pbck73ItemPrintOutDto>();
                    if (dbData.CK5.CK5_MATERIAL != null && dbData.CK5.CK5_MATERIAL.Count > 0)
                    {
                        //proses
                        foreach (var item in dbData.CK5.CK5_MATERIAL)
                        {
                            var itemToInsert = new Pbck73ItemPrintOutDto()
                            {
                                FaCode = item.BRAND,
                                ProdTypeAlias = "",
                                Brand = "",
                                Qty = item.CONVERTED_QTY,
                                Content = null, //todo: ask to analyst
                                SeriesValue = "",
                                Hje = item.HJE,
                                Tariff = item.TARIFF,
                                ExciseValue = item.EXCISE_VALUE
                            };

                            var brandData = _brandRegistrationServices.GetByPlantIdAndFaCode(item.PLANT_ID, item.BRAND);
                            if (brandData != null)
                            {
                                itemToInsert.Brand = brandData.BRAND_CE;
                                itemToInsert.ProdTypeAlias = brandData.ZAIDM_EX_PRODTYP != null
                                    ? brandData.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS
                                    : "-";
                                itemToInsert.SeriesValue = brandData.SERIES_CODE;
                            }
                            rc.Items.Add(itemToInsert);
                        }
                    }
                }
            }

            //set header footer data by CompanyCode and FormTypeId
            var headerFooterData = _headerFooterBll.GetByComanyAndFormType(new HeaderFooterGetByComanyAndFormTypeInput()
            {
                FormTypeId = Enums.FormType.PBCK3,
                CompanyCode = rc.CompanyCode
            });

            rc.HeaderFooter = headerFooterData;
            rc.PrintedDate = DateReportString(dbData.PBCK3_DATE);
            rc = SetExecutionDate(rc);
            return rc;
        }

        private string DateReportString(DateTime dt)
        {
            var monthPeriodFrom = _monthBll.GetMonth(dt.Month);
            return dt.ToString("dd") + " " + monthPeriodFrom.MONTH_NAME_IND +
                                   " " + dt.ToString("yyyy");
        }

        private Pbck73PrintOutDto SetPoaData(Pbck73PrintOutDto data, string poaId)
        {
            var poaData = _poabll.GetById(poaId);
            if (poaData != null)
            {
                data.PoaId = poaData.POA_ID;
                data.PoaName = poaData.PRINTED_NAME;
                data.PoaTitle = poaData.TITLE;
            }
            else
            {
                data.PoaId = "-";
                data.PoaName = "-";
                data.PoaTitle = "-";
            }
            return data;
        }

        private Pbck73PrintOutDto SetCompanyData(Pbck73PrintOutDto data, string plantId)
        {
            var t001KData = _t001Kbll.GetByBwkey(plantId);
            if (t001KData == null) return data;
            data.CompanyCode = t001KData.BUKRS;
            data.CompanyName = t001KData.BUTXT;
            data.CompanyAddress = t001KData.SPRAS;
            return data;
        }

        private Pbck73PrintOutDto SetNppbkcData(Pbck73PrintOutDto data, string nppbkcId)
        {
            var nppbkcData = _nppbkcbll.GetById(nppbkcId);
            if (nppbkcData == null) return data;
            data.NppbkcId = nppbkcData.NPPBKC_ID;
            data.NppbkcTextTo = nppbkcData.TEXT_TO;
            data.NppbkcCity = nppbkcData.CITY;
            var vendor = _lfaBll.GetById(nppbkcData.KPPBC_ID);
            if (vendor != null)
            {
                data.VendorCity = vendor.ORT01;
            }
            if (nppbkcData.START_DATE.HasValue)
                data.NppbkcStartDate = DateReportString(nppbkcData.START_DATE.Value);
            return data;
        }

        private Pbck73PrintOutDto SetExecutionDate(Pbck73PrintOutDto data)
        {
            if (data.ExecDateFrom.HasValue && data.ExecDateTo.HasValue)
            {
                data.ExecDateDisplayString = DateReportString(data.ExecDateFrom.Value) + " - " +
                                             DateReportString(data.ExecDateTo.Value);
            }
            return data;
        }

        #endregion

    }


}
