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
    public class XmlFileLogBLL : IXmlFileLogBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<XML_LOGS> _repository;


        public XmlFileLogBLL(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<XML_LOGS>();
        }


        public List<XML_LOGSDto> GetXmlLogByParam(GetXmlLogByParamInput input)
        {
            Expression<Func<XML_LOGS, bool>> queryFilter = PredicateHelper.True<XML_LOGS>();

            if (input.DateFrom.HasValue)
            {
                input.DateFrom = new DateTime(input.DateFrom.Value.Year, input.DateFrom.Value.Month, input.DateFrom.Value.Day, 0, 0, 0);
                queryFilter = queryFilter.And(c => c.LAST_ERROR_TIME >= input.DateFrom);
            }

            if (input.DateTo.HasValue)
            {
                input.DateFrom = new DateTime(input.DateTo.Value.Year, input.DateTo.Value.Month, input.DateTo.Value.Day, 23, 59, 59);
                queryFilter = queryFilter.And(c => c.LAST_ERROR_TIME <= input.DateTo);
            }


            Func<IQueryable<XML_LOGS>, IOrderedQueryable<XML_LOGS>> orderByFilter = n => n.OrderByDescending(z => z.LAST_ERROR_TIME);
            
            var result = _repository.Get(queryFilter, orderByFilter, "").ToList();

            return Mapper.Map<List<XML_LOGSDto>>(result.ToList());
            
        }

        public XML_LOGSDto GetByIdIncludeTables(long id)
        {
            var dbData = _repository.Get(c => c.XML_LOGS_ID == id, null, "XML_LOGS_DETAILS").FirstOrDefault();
            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);


            return Mapper.Map<XML_LOGSDto>(dbData);
        }


        public void UpdateXmlFile(UpdateXmlFileInput input)
        {
            var dbData = _repository.GetByID(input.XmlId);

            if (dbData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //update data
            dbData.STATUS = Enums.XmlLogStatus.ReRun;
            dbData.MODIFIED_BY = input.UserId;
            dbData.MODIFIED_DATE = DateTime.Now;

            //check if file is exist in source path
            if (!System.IO.File.Exists(input.SourcePath + dbData.XML_FILENAME))
                throw new Exception("File Not Found : " + input.SourcePath + dbData.XML_FILENAME);

            //if file exist ..in dest path .. remove ??
            if (System.IO.File.Exists(input.DestPath + dbData.XML_FILENAME))
                System.IO.File.Delete(input.DestPath + dbData.XML_FILENAME);

            System.IO.File.Move(input.SourcePath + dbData.XML_FILENAME , input.DestPath + dbData.XML_FILENAME);

            _uow.SaveChanges();
        }
    }
}
