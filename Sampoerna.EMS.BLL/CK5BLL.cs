using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class CK5BLL : ICK5BLL
    {
         private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<CK5> _repository;
        private string includeTables = "";
        private IDocumentSequenceNumberBLL _docSeqNumBll;
        private IMasterDataBLL _masterDataBll;

        public CK5BLL(IUnitOfWork uow, ILogger logger, IDocumentSequenceNumberBLL docSeqNumBll, IMasterDataBLL masterDataBll)
        {
            _logger = logger;
            _uow = uow;
            _docSeqNumBll = docSeqNumBll;
            _masterDataBll = masterDataBll;
            _repository = _uow.GetGenericRepository<CK5>();
        }

        public CK5 GetById(long id)
        {
            return _repository.GetByID(id);
        }
        public List<CK5> GetAll()
        {
            includeTables = "T1001W.ZAIDM_EX_NPPBKC, T1001W1.ZAIDM_EX_NPPBKC";
            return _repository.Get(null, null, includeTables).ToList();
        }


        public List<CK5> GetCK5ByType(Enums.CK5Type ck5Type)
        {
            includeTables = "T1001W.ZAIDM_EX_NPPBKC, T1001W1.ZAIDM_EX_NPPBKC";
            return _repository.Get(c => c.CK5_TYPE == ck5Type, null, includeTables).ToList();
        }

        public List<CK5> GetCK5ByParam(CK5Input input)
        {
            Expression<Func<CK5, bool>> queryFilter = PredicateHelper.True<CK5>();

            if (!string.IsNullOrEmpty(input.DocumentNumber))
            {
                queryFilter = queryFilter.And(c => c.SUBMISSION_NUMBER.Contains(input.DocumentNumber));
            }

            if (input.POA.HasValue)
            {
                queryFilter = queryFilter.And(c => c.APPROVED_BY.HasValue && c.APPROVED_BY.Value == input.POA.Value);
            }

            if (input.Creator.HasValue)
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY.HasValue && c.CREATED_BY.Value == input.Creator.Value);
            }

            if (input.NPPBKCOrigin.HasValue)
            {
                queryFilter = queryFilter.And(c => c.SOURCE_PLANT_ID.HasValue && c.SOURCE_PLANT_ID.Value == input.NPPBKCOrigin.Value);
            }

            if (input.NPPBKCDestination.HasValue)
            {
                queryFilter = queryFilter.And(c => c.DEST_PLANT_ID.HasValue && c.DEST_PLANT_ID.Value == input.NPPBKCDestination.Value);
            }

            if (input.Ck5Type != null)
            {
                queryFilter = queryFilter.And(c => c.CK5_TYPE == input.Ck5Type);
            }

            Func<IQueryable<CK5>, IOrderedQueryable<CK5>> orderBy = null;
            if (!string.IsNullOrEmpty(input.SortOrderColumn))
            {
                orderBy = c => c.OrderBy(OrderByHelper.GetOrderByFunction<CK5>(input.SortOrderColumn));
            }

            var rc = _repository.Get(queryFilter, orderBy, includeTables);
            if (rc == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            return rc.ToList();
        }

        public void SaveCk5(CK5 ck5)
        {
            //get generate number


            if (ck5.CK5_ID <= 0)
            {
                //insert
                long plantId = 0;
                if (ck5.SOURCE_PLANT_ID.HasValue)
                    plantId = ck5.SOURCE_PLANT_ID.Value;

                var plant = _masterDataBll.GetPlantById(plantId);
                long nppbkc = 0;
                if (plant.NPPBCK_ID.HasValue)
                    nppbkc = plant.NPPBCK_ID.Value;

                var input = new GenerateDocNumberInput()
                {
                    Year = DateTime.Now.Year,
                    Month = DateTime.Now.Month,
                    NppbkcId = nppbkc
                };
                ck5.SUBMISSION_NUMBER = _docSeqNumBll.GenerateNumber(input);
                //ck5.CREATED_DATE = DateTime.Now;
            }
          
            _repository.InsertOrUpdate(ck5);
            _uow.SaveChanges();
        }
    }
}
