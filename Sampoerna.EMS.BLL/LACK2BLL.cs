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
        private IPlantBLL _plantBll;

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
            _plantBll = new PlantBLL(_uow, _logger);
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

        public List<Lack2SummaryReportDto> GetSummaryReportsByParam(Lack2GetSummaryReportByParamInput input)
        {

            Expression<Func<LACK2, bool>> queryFilter = PredicateHelper.True<LACK2>();

            if (!string.IsNullOrEmpty(input.CompanyCode))
            {
                queryFilter = queryFilter.And(c => c.BUKRS.Contains(input.CompanyCode));
            }

            if (!string.IsNullOrEmpty(input.NppbkcId))
            {
                queryFilter = queryFilter.And(c => c.NPPBKC_ID.Contains(input.NppbkcId));
            }

            if (!string.IsNullOrEmpty(input.SendingPlantId))
            {
                queryFilter = queryFilter.And(c => c.LEVEL_PLANT_ID.Contains(input.SendingPlantId));
            }

            if (!string.IsNullOrEmpty(input.GoodType))
            {
                queryFilter = queryFilter.And(c => c.EX_GOOD_TYP.Contains(input.GoodType));
            }

            if (input.PeriodMonth.HasValue)
                queryFilter =
                    queryFilter.And(c => c.PERIOD_MONTH == input.PeriodMonth.Value);

            if (input.PeriodYear.HasValue)
                queryFilter =
                    queryFilter.And(c => c.PERIOD_YEAR == input.PeriodYear.Value);

            if (input.DocumentStatus.HasValue)
            {
                queryFilter = queryFilter.And(c => c.STATUS == input.DocumentStatus.Value);
            }

            if (input.CreatedDate.HasValue)
            {
                queryFilter =
                    queryFilter.And(
                        c =>
                            c.CREATED_DATE.Year == input.CreatedDate.Value.Year &&
                            c.CREATED_DATE.Month == input.CreatedDate.Value.Month &&
                            c.CREATED_DATE.Day == input.CreatedDate.Value.Day);
            }
            if (!string.IsNullOrEmpty(input.CreatedBy))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.CreatedBy);
            }
            if (input.ApprovedDate.HasValue)
            {
                queryFilter =
                    queryFilter.And(
                        c =>
                            c.APPROVED_DATE.HasValue &&
                            c.APPROVED_DATE.Value.Year == input.ApprovedDate.Value.Year &&
                            c.APPROVED_DATE.Value.Month == input.ApprovedDate.Value.Month &&
                            c.APPROVED_DATE.Value.Day == input.ApprovedDate.Value.Day);
            }
            if (!string.IsNullOrEmpty(input.ApprovedBy))
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY == input.ApprovedBy);
            }
            if (!string.IsNullOrEmpty(input.Creator))
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == input.Creator);
            }
            if (!string.IsNullOrEmpty(input.Approver))
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY_MANAGER == input.Approver);
            }
          


            var rc = _repository.Get(queryFilter, null, includeTables).ToList();
            if (rc == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            return SetDataSummaryReport(rc);
            

        }

        private List<Lack2SummaryReportDto> SetDataSummaryReport(List<LACK2> listLack2)
        {
            var result = new List<Lack2SummaryReportDto>();

            foreach (var dtData in listLack2)
            {

                var summaryDto = new Lack2SummaryReportDto();

                summaryDto.Lack2Number = dtData.LACK2_NUMBER;
                summaryDto.DocumentType = EnumHelper.GetDescription(Enums.FormType.LACK2);

                summaryDto.CompanyCode = dtData.BUKRS;
                summaryDto.CompanyName = dtData.BUTXT;
                summaryDto.NppbkcId = dtData.NPPBKC_ID;
                summaryDto.Ck5SendingPlant = dtData.LEVEL_PLANT_ID;

                var dbPlant = _plantBll.GetT001WById(dtData.LEVEL_PLANT_ID);
                if (dbPlant != null)
                {
                    summaryDto.SendingPlantAddress = dbPlant.ADDRESS;
                }

                var monthData = _monthBll.GetMonth(dtData.PERIOD_MONTH);
                if (monthData != null)
                {
                    summaryDto.Lack2Period = monthData.MONTH_NAME_IND + " " + dtData.PERIOD_YEAR;
                }

                summaryDto.Lack2Date = ConvertHelper.ConvertDateToStringddMMMyyyy(dtData.SUBMISSION_DATE);

                summaryDto.TypeExcisableGoods = dtData.EX_GOOD_TYP;
                summaryDto.TypeExcisableGoodsDesc = dtData.EX_TYP_DESC;

                summaryDto.TotalDeliveryExcisable = "0";
                summaryDto.Uom = "";
                summaryDto.Poa = "";
                summaryDto.PoaManager = "";
                summaryDto.LegalizeData = "";

                summaryDto.CreatedDate = ConvertHelper.ConvertDateToStringddMMMyyyy(dtData.CREATED_DATE);
                summaryDto.CreatedTime = ConvertHelper.ConvertDateToStringHHmm(dtData.CREATED_DATE);
                summaryDto.CreatedBy = dtData.CREATED_BY;

                summaryDto.ApprovedDate = ConvertHelper.ConvertDateToStringddMMMyyyy(dtData.APPROVED_DATE);
                summaryDto.ApprovedTime = ConvertHelper.ConvertDateToStringHHmm(dtData.APPROVED_DATE);
                summaryDto.ApprovedBy = dtData.APPROVED_BY;

                summaryDto.LastChangedDate = ConvertHelper.ConvertDateToStringddMMMyyyy(dtData.MODIFIED_DATE);
                summaryDto.LastChangedTime = ConvertHelper.ConvertDateToStringHHmm(dtData.MODIFIED_DATE);

                summaryDto.Status = EnumHelper.GetDescription(dtData.STATUS);

                //search
                summaryDto.PeriodYear = dtData.PERIOD_YEAR.ToString();
                result.Add(summaryDto);

            }

            return result;
        }

        public List<Lack2DetailReportDto> GetDetailReportsByParam(Lack2GetDetailReportByParamInput input)
        {

            Expression<Func<LACK2, bool>> queryFilter = PredicateHelper.True<LACK2>();

            if (!string.IsNullOrEmpty(input.CompanyCode))
            {
                queryFilter = queryFilter.And(c => c.BUKRS.Contains(input.CompanyCode));
            }

            if (!string.IsNullOrEmpty(input.NppbkcId))
            {
                queryFilter = queryFilter.And(c => c.NPPBKC_ID.Contains(input.NppbkcId));
            }

            if (!string.IsNullOrEmpty(input.SendingPlantId))
            {
                queryFilter = queryFilter.And(c => c.LEVEL_PLANT_ID.Contains(input.SendingPlantId));
            }

            if (!string.IsNullOrEmpty(input.GoodType))
            {
                queryFilter = queryFilter.And(c => c.EX_GOOD_TYP.Contains(input.GoodType));
            }


            if (input.PeriodMonth.HasValue)
                queryFilter =
                    queryFilter.And(c => c.PERIOD_MONTH == input.PeriodMonth.Value);

            if (input.PeriodYear.HasValue)
                queryFilter =
                    queryFilter.And(c => c.PERIOD_YEAR == input.PeriodYear.Value);

            //if (input.CreatedFrom.HasValue)
            //{
            //    queryFilter =
            //        queryFilter.And(
            //            c =>
            //                c.CREATED_DATE.Year == input.CreatedDate.Value.Year &&
            //                c.CREATED_DATE.Month == input.CreatedDate.Value.Month &&
            //                c.CREATED_DATE.Day == input.CreatedDate.Value.Day);
            //}
         
            //if (input.CreatedTo.HasValue)
            //{
            //    queryFilter =
            //        queryFilter.And(
            //            c =>
            //                c.APPROVED_DATE.HasValue &&
            //                c.APPROVED_DATE.Value.Year == input.ApprovedDate.Value.Year &&
            //                c.APPROVED_DATE.Value.Month == input.ApprovedDate.Value.Month &&
            //                c.APPROVED_DATE.Value.Day == input.ApprovedDate.Value.Day);
            //}



            var rc = _repository.Get(queryFilter, null, "LACK2_ITEM, LACK2_ITEM.CK5").ToList();
            if (rc == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            var result = SetDataDetailReport(rc);

            if (input.DateFrom.HasValue)
            {
                input.DateFrom = new DateTime(input.DateFrom.Value.Year, input.DateFrom.Value.Month, input.DateFrom.Value.Day, 0, 0, 0);
                result = result.Where(c => c.GiDate >= input.DateFrom).ToList();
            }

            if (input.DateTo.HasValue)
            {
                input.DateFrom = new DateTime(input.DateTo.Value.Year, input.DateTo.Value.Month, input.DateTo.Value.Day, 23, 59, 59);
                result = result.Where(c => c.GiDate <= input.DateTo).ToList();
            }

            return result;

        }

        private List<Lack2DetailReportDto> SetDataDetailReport(List<LACK2> listLack2)
        {
            var result = new List<Lack2DetailReportDto>();

            foreach (var dtData in listLack2)
            {
                foreach (var lack2Item in dtData.LACK2_ITEM)
                {
                    var summaryDto = new Lack2DetailReportDto();

                    summaryDto.Lack2Number = dtData.LACK2_NUMBER;

                    if (lack2Item.CK5 != null)
                    {
                        summaryDto.GiDate = lack2Item.CK5.GI_DATE;
                        summaryDto.Ck5GiDate = ConvertHelper.ConvertDateToStringddMMMyyyy(lack2Item.CK5.GI_DATE);
                        summaryDto.Ck5RegistrationNumber = lack2Item.CK5.REGISTRATION_NUMBER;
                        summaryDto.Ck5RegistrationDate = ConvertHelper.ConvertDateToStringddMMMyyyy(lack2Item.CK5.REGISTRATION_DATE);
                        summaryDto.Ck5Total = ConvertHelper.ConvertDecimalToStringMoneyFormat(lack2Item.CK5.GRAND_TOTAL_EX);
                        
                        summaryDto.ReceivingCompanyCode = lack2Item.CK5.DEST_PLANT_COMPANY_CODE;
                        summaryDto.ReceivingCompanyName = lack2Item.CK5.DEST_PLANT_COMPANY_NAME;
                        summaryDto.ReceivingNppbkc = lack2Item.CK5.DEST_PLANT_NPPBKC_ID;
                        summaryDto.ReceivingAddress = lack2Item.CK5.DEST_PLANT_ADDRESS;
                    }

                    summaryDto.Ck5SendingPlant = dtData.LEVEL_PLANT_ID;
                    var dbPlant = _plantBll.GetT001WById(dtData.LEVEL_PLANT_ID);
                    if (dbPlant != null)
                    {
                        summaryDto.SendingPlantAddress = dbPlant.ADDRESS;
                    }
                    summaryDto.CompanyCode = dtData.BUKRS;
                    summaryDto.CompanyName = dtData.BUTXT;
                    summaryDto.NppbkcId = dtData.NPPBKC_ID;
                    summaryDto.TypeExcisableGoods = dtData.EX_GOOD_TYP;
                    summaryDto.TypeExcisableGoodsDesc = dtData.EX_TYP_DESC;
                    
                    result.Add(summaryDto);

                }
            }

            return result;
        }
    }
}
