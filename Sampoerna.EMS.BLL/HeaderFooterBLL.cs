using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class HeaderFooterBLL : IHeaderFooterBLL
    {

        private IGenericRepository<HEADER_FOOTER> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private string includeTables = "T1001, HEADER_FOOTER_FORM_MAP";

        public HeaderFooterBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<HEADER_FOOTER>();
        }

        public HeaderFooterDetails GetDetailsById(int id)
        {
            return Mapper.Map<HeaderFooterDetails>(_repository.Get(c => c.HEADER_FOOTER_ID == id, null, includeTables).FirstOrDefault());
        }

        public List<HeaderFooter> GetAll()
        {
            return Mapper.Map<List<HeaderFooter>>(_repository.Get(null, null, includeTables).ToList());
        }

        public SaveHeaderFooterOutput Save(HeaderFooterDetails headerFooterData)
        {
            var data = Mapper.Map<HEADER_FOOTER>(headerFooterData);
            if (headerFooterData.HEADER_FOOTER_ID > 0)
            {
                //update
                _repository.Update(data);
            }
            else
            {
                //Insert
                _repository.Insert(data);
            }

            var output = new SaveHeaderFooterOutput();

            try
            {
                _uow.SaveChanges();
                output.Success = true;
                output.HeaderFooterId = headerFooterData.HEADER_FOOTER_ID;
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
                output.Success = false;
                output.ErrorCode = ExceptionCodes.BaseExceptions.unhandled_exception.ToString();
                output.ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
            }
            return output;
        }
    }
}
