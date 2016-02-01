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

namespace Sampoerna.EMS.BLL
{
    public class ReversalBLL : IReversalBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<REVERSAL> _repository;

        private IZaapShiftRptService _zaapShiftRptService;
        private IUserPlantMapBLL _userPlantBll;
        private IPOAMapBLL _poaMapBll;

        public ReversalBLL(ILogger logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<REVERSAL>();

            _zaapShiftRptService = new ZaapShiftRptService(_uow, _logger);
            _userPlantBll = new UserPlantMapBLL(_uow, _logger);
            _poaMapBll = new POAMapBLL(_uow, _logger);
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
                var listUserPlant = _userPlantBll.GetPlantByUserId(input.UserId);

                var listPoaPlant = _poaMapBll.GetPlantByPoaId(input.UserId);

                queryFilter = queryFilter.And(c => listUserPlant.Contains(c.WERKS) || listPoaPlant.Contains(c.WERKS));
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

            var remainingQty = (zaapData.QTY.HasValue ? zaapData.QTY.Value : 0) - (reversalList.Sum(x => x.REVERSAL_QTY.Value));

            if (reversalInput.ReversalQty > remainingQty) output.IsMoreThanQuota = true;

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
    }
}
