using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
   public class PBCK4BLL : IPBCK4BLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;

        private IGenericRepository<PBCK4> _repository;

        private IMonthBLL _monthBll;
        private IDocumentSequenceNumberBLL _docSeqNumBll;

       private string includeTables = "";// "CK5_MATERIAL, PBCK1, UOM, USER, USER1, CK5_FILE_UPLOAD";

       public PBCK4BLL(IUnitOfWork uow, ILogger logger)
       {
           _logger = logger;
           _uow = uow;

           _repository = _uow.GetGenericRepository<PBCK4>();

           _monthBll = new MonthBLL(_uow, _logger);
           _docSeqNumBll = new DocumentSequenceNumberBLL(_uow, _logger);


       }

       public List<Pbck4Dto> GetPbck4ByParam(Pbck4GetByParamInput input)
       {

           Expression<Func<PBCK4, bool>> queryFilter = PredicateHelper.True<PBCK4>();

           //if (!string.IsNullOrEmpty(input.NppbkcId))
           //{
           //    queryFilter = queryFilter.And(c => c.SOURCE_PLANT_NPPBKC_ID.Contains(input.NppbkcId));
           //}

           //if (!string.IsNullOrEmpty(input.PlantId))
           //{
           //    queryFilter = queryFilter.And(c => c.SOURCE_PLANT_ID.Contains(input.PlantId));

           //}

           if (input.ReportedOn.HasValue)
           {
               queryFilter =
                   queryFilter.And(
                       c => c.CREATED_DATE == input.ReportedOn.Value);
           }


           if (!string.IsNullOrEmpty(input.Poa))
           {
               queryFilter = queryFilter.And(c => c.APPROVED_BY_POA.Contains(input.Poa));
           }

           if (!string.IsNullOrEmpty(input.Creator))
           {
               queryFilter = queryFilter.And(c => c.CREATED_BY.Contains(input.Creator));
           }

           //if (input.IsCompletedDocument)
           //{
           //    queryFilter = queryFilter.And(c => c.STATUS_ID == Enums.DocumentStatus.Completed);
           //}
           //else
           //{
           //    queryFilter = queryFilter.And(c => c.STATUS_ID != Enums.DocumentStatus.Completed);
           //}

           Func<IQueryable<PBCK4>, IOrderedQueryable<PBCK4>> orderByFilter = n => n.OrderByDescending(z => z.CREATED_DATE);
        

           var rc = _repository.Get(queryFilter, orderByFilter, includeTables);
           if (rc == null)
           {
               throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
           }

           var mapResult = Mapper.Map<List<Pbck4Dto>>(rc.ToList());

           return mapResult;


       }

    }
}
