using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class PrintHistoryBLL : IPrintHistoryBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<PRINT_HISTORY> _repository;
        private string includeTables = "USER";

        public PrintHistoryBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<PRINT_HISTORY>();
        }
        
        public List<PrintHistoryDto> GetByFormNumber(string formNumber)
        {
            Expression<Func<PRINT_HISTORY, bool>> queryFilter = c => c.FORM_NUMBER == formNumber;
            var dbData = _repository.Get(queryFilter, null, includeTables);
            return Mapper.Map<List<PrintHistoryDto>>(dbData);
        }

        public List<PrintHistoryDto> GetByFormTypeAndFormId(Enums.FormType formType, long formId)
        {
            Expression<Func<PRINT_HISTORY, bool>> queryFilter = c => c.FORM_TYPE_ID == formType && c.FORM_ID == formId;
            var dbData = _repository.Get(queryFilter, null, includeTables);
            return Mapper.Map<List<PrintHistoryDto>>(dbData);
        }

        public PrintHistoryDto GetById(long id)
        {
            Expression<Func<PRINT_HISTORY, bool>> queryFilter = c => c.PRINT_HOSTORY_ID == id;
            var dbData = _repository.Get(queryFilter, null, includeTables);
            if(dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return Mapper.Map<PrintHistoryDto>(dbData.FirstOrDefault()); 
        }

        public void AddPrintHistory(PrintHistoryDto printHistoryData)
        {
            var dbData = Mapper.Map<PRINT_HISTORY>(printHistoryData);
            _repository.Insert(dbData);
        }

    }
}
