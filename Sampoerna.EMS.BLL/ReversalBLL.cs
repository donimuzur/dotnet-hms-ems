using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Sampoerna.EMS.BLL.Services;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class ReversalBLL : IReversalBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<REVERSAL> _repository;
        private IGenericRepository<CK4C> _repositoryCk4c;
        private IGenericRepository<PRODUCTION> _repositoryProd;

        private IZaapShiftRptService _zaapShiftRptService;
        private IUserPlantMapBLL _userPlantBll;
        private IPOAMapBLL _poaMapBll;
        private IPlantBLL _plantBll;

        public ReversalBLL(ILogger logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<REVERSAL>();
            _repositoryCk4c = _uow.GetGenericRepository<CK4C>();
            _repositoryProd = _uow.GetGenericRepository<PRODUCTION>();

            _zaapShiftRptService = new ZaapShiftRptService(_uow, _logger);
            _userPlantBll = new UserPlantMapBLL(_uow, _logger);
            _poaMapBll = new POAMapBLL(_uow, _logger);
            _plantBll = new PlantBLL(_uow, _logger);
        }

        public List<ReversalDto> GetListDocumentByParam(ReversalGetByParamInput input)
        {
            var queryFilter = ProcessQueryFilter(input);

            return Mapper.Map<List<ReversalDto>>(GetReversalData(queryFilter, input.ShortOrderColumn));
        }

        private Expression<Func<REVERSAL, bool>> ProcessQueryFilter(ReversalGetByParamInput input)
        {
            Expression<Func<REVERSAL, bool>> queryFilter = PredicateHelper.True<REVERSAL>();

            if (!string.IsNullOrEmpty(input.DateProduction))
            {
                var dt = Convert.ToDateTime(input.DateProduction);
                queryFilter = queryFilter.And(c => c.PRODUCTION_DATE == dt);
            }

            if (!string.IsNullOrEmpty(input.PlantId))
            {
                queryFilter = queryFilter.And(c => c.WERKS == input.PlantId);
            }

            if (!string.IsNullOrEmpty(input.UserId))
            {
                if (input.UserRole != Enums.UserRole.SuperAdmin) { 
                    queryFilter = queryFilter.And(c => input.ListUserPlants.Contains(c.WERKS));
                }
            }
            
            return queryFilter;
        }

        private List<REVERSAL> GetReversalData(Expression<Func<REVERSAL, bool>> queryFilter, string orderColumn)
        {
            Func<IQueryable<REVERSAL>, IOrderedQueryable<REVERSAL>> orderBy = null;
            if (!string.IsNullOrEmpty(orderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<REVERSAL>(orderColumn));
            }

            var dbData = _repository.Get(queryFilter);

            return dbData.ToList();
        }

        public ReversalOutput CheckData(ReversalCreateParamInput reversalInput)
        {
            var output = new ReversalOutput();

            var zaapData = _zaapShiftRptService.GetById(reversalInput.ZaapShiftId);

            var reversalList = GetReversalData(x => x.ZAAP_SHIFT_RPT_ID == reversalInput.ZaapShiftId, null);

            var remainingQty = (zaapData == null ? 0 : (zaapData.QTY.HasValue ? (zaapData.QTY.Value * -1) : 0)) - 
                (reversalList.Sum(x => x.REVERSAL_QTY.HasValue ? x.REVERSAL_QTY.Value : 0));

            var reversalListByPlantFacodeDate = GetReversalData(x => x.WERKS == reversalInput.Werks && x.FA_CODE == reversalInput.FaCode
                                                                    && x.PRODUCTION_DATE == reversalInput.ProductionDate, null);

            var totalReversal = reversalListByPlantFacodeDate.Sum(x => x.REVERSAL_QTY.HasValue ? x.REVERSAL_QTY.Value : 0) + reversalInput.ReversalQty;

            output.IsPackedQtyNotExists = true;

            output.PackedQty = 0;

            if (reversalInput.ReversalId > 0)
            {
                var existsReversal = GetById(reversalInput.ReversalId);

                remainingQty = remainingQty + existsReversal.ReversalQty;

                totalReversal = reversalListByPlantFacodeDate.Where(x => x.REVERSAL_ID != reversalInput.ReversalId).Sum(x => x.REVERSAL_QTY.HasValue ? x.REVERSAL_QTY.Value : 0) + reversalInput.ReversalQty;
            }

            if (reversalInput.ProductionDate.HasValue)
            {
                var plant = _plantBll.GetId(reversalInput.Werks);

                var period = 1;

                var month = reversalInput.ProductionDate.Value.Month;

                var year = reversalInput.ProductionDate.Value.Year;

                if (reversalInput.ProductionDate.Value.Day > 14) period = 2;

                var ck4cData = _repositoryCk4c.Get(x => x.NPPBKC_ID == plant.NPPBKC_ID && x.REPORTED_PERIOD == period
                                                        && x.REPORTED_MONTH == month && x.REPORTED_YEAR == year
                                                        && x.STATUS == Enums.DocumentStatus.Completed);

                var prodData = _repositoryProd.Get(p => p.PRODUCTION_DATE == reversalInput.ProductionDate.Value
                                                        && p.WERKS == reversalInput.Werks && p.FA_CODE == reversalInput.FaCode).FirstOrDefault();

                if (ck4cData.Count() > 0) output.IsForCk4cCompleted = true;

                if (prodData != null)
                {
                    if (prodData.QTY_PACKED.Value > 0) { 
                        output.IsPackedQtyNotExists = false;
                        output.PackedQty = prodData.QTY_PACKED.Value;
                    }
                }
            }

            if (reversalInput.ReversalQty > remainingQty) output.IsMoreThanQuota = true;

            if (totalReversal > output.PackedQty) output.IsMoreThanPacked = true;

            output.RemainingQuota = remainingQty;

            return output;
        }

        public ReversalDto Save(ReversalDto item, string userId)
        {
            REVERSAL model;

            try
            {
                model = Mapper.Map<REVERSAL>(item);

                _repository.InsertOrUpdate(model);

                _uow.SaveChanges();
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return Mapper.Map<ReversalDto>(model);
        }

        public ReversalDto GetById(long id)
        {
            var dbData = _repository.Get(c => c.REVERSAL_ID == id).FirstOrDefault();

            var mapResult = Mapper.Map<ReversalDto>(dbData);

            return mapResult;
        }

        public List<ReversalDto> GetListByParam(string plant, string facode, DateTime prodDate)
        {
            Expression<Func<REVERSAL, bool>> queryFilter = PredicateHelper.True<REVERSAL>();

            if (!string.IsNullOrEmpty(plant))
            {
                queryFilter = queryFilter.And(c => c.WERKS == plant);
            }

            if (!string.IsNullOrEmpty(facode))
            {
                queryFilter = queryFilter.And(c => c.FA_CODE == facode);
            }

            if (prodDate != null)
            {
                queryFilter = queryFilter.And(c => c.PRODUCTION_DATE == prodDate);
            }

            var dbData = _repository.Get(queryFilter);

            return Mapper.Map<List<ReversalDto>>(dbData.ToList());
        }
    }
}
