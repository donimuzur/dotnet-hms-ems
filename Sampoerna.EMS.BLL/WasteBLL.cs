using System;
using System.Collections.Generic;
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

namespace Sampoerna.EMS.BLL
{
    public class WasteBLL : IWasteBLL
    {
        private IGenericRepository<WASTE> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAIDM_EX_BRAND> _repositoryBrand;
        private IGenericRepository<ZAIDM_EX_GOODTYP> _repositoryGood;
        private IGenericRepository<UOM> _repositoryUom;
        private IGenericRepository<T001W> _repositoryPlant;

        public WasteBLL(ILogger logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<WASTE>();
            _repositoryBrand = _uow.GetGenericRepository<ZAIDM_EX_BRAND>();
            _repositoryGood = _uow.GetGenericRepository<ZAIDM_EX_GOODTYP>();
            _repositoryUom = _uow.GetGenericRepository<UOM>();
            _repositoryPlant = _uow.GetGenericRepository<T001W>();
        }
        public List<WasteDto> GetAllByParam(WasteGetByParamInput input)
        {
            Expression<Func<WASTE, bool>> queryFilter = PredicateHelper.True<WASTE>();
            if (!string.IsNullOrEmpty(input.Company))
            {
                queryFilter = queryFilter.And(c => c.COMPANY_CODE == input.Company);
            }
            if (!string.IsNullOrEmpty(input.Plant))
            {
                queryFilter = queryFilter.And(c => c.WERKS == input.Plant);
            }
            if (!string.IsNullOrEmpty(input.WasteProductionDate))
            {
                var dt = Convert.ToDateTime(input.WasteProductionDate);
                queryFilter = queryFilter.And(c => c.WASTE_PROD_DATE == dt);
            }

            Func<IQueryable<WASTE>, IOrderedQueryable<WASTE>> orderBy = null;
            {
                if (!string.IsNullOrEmpty(input.ShortOrderColumn))
                {
                    orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<WASTE>(input.ShortOrderColumn));
                }

                var dbData = _repository.Get(queryFilter, orderBy);
                if (dbData == null)
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
                }
                var mapResult = Mapper.Map<List<WasteDto>>(dbData.ToList());

                return mapResult;
            }
        }

        public List<WasteDto> GetAllWaste()
        {
            var dbData = _repository.Get().ToList();
            return Mapper.Map<List<WasteDto>>(dbData);

        }

        public void Save(WasteDto wasteDto)
        {

            var dbWaste = Mapper.Map<WASTE>(wasteDto);

            _repository.InsertOrUpdate(dbWaste);
            _uow.SaveChanges();
        }

        public WasteDto GetById(string companyCode, string plantWerk, string faCode, DateTime wasteProductionDate)
        {
            var dbData = _repository.GetByID(companyCode, plantWerk, faCode, wasteProductionDate);
            var item = Mapper.Map<WasteDto>(dbData);

            if (dbData == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            return item;
        }

        public WASTE GetExistDto(string companyCode, string plantWerk, string faCode, DateTime wasteProductionDate)
        {
            return
                _uow.GetGenericRepository<WASTE>()
                    .Get(
                        p =>
                            p.COMPANY_CODE == companyCode && p.WERKS == plantWerk && p.FA_CODE == faCode &&
                            p.WASTE_PROD_DATE == wasteProductionDate)
                    .FirstOrDefault();
        }


        public void SaveUpload(WasteUploadItems wasteUpload)
        {
            var dbUpload = Mapper.Map<WASTE>(wasteUpload);
            _repository.InsertOrUpdate(dbUpload);
            _uow.SaveChanges();
        }
    }
}
