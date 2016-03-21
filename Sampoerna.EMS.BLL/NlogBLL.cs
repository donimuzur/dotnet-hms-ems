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
using Sampoerna.EMS.LinqExtensions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class NlogBLL :INlogBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<NlogLogs> _repository;


        public NlogBLL(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<NlogLogs>();

        }

        public NlogDto GetById(long id)
        {
            var dtData = _repository.GetByID(id);
            if (dtData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return Mapper.Map<NlogDto>(dtData);

        }

        public List<NlogDto> GetAllData()
        {
            var dbData = _repository.Get();

            return Mapper.Map<List<NlogDto>>(dbData.ToList());

        }
        public List<NlogDto> GetNlogByParam(NlogGetByParamInput input)
        {
            Expression<Func<NlogLogs, bool>> queryFilter = PredicateHelper.True<NlogLogs>();

            if (!string.IsNullOrEmpty(input.FileName))
            {
                queryFilter = queryFilter.And(c => c.FileName == input.FileName);
            }

            if (input.Month.HasValue)
            {
                queryFilter = queryFilter.And(c => c.Timestamp.Value.Month == input.Month);
            }


            Func<IQueryable<NlogLogs>, IOrderedQueryable<NlogLogs>> orderByFilter = n => n.OrderByDescending(z => z.Timestamp);
          

            var result = _repository.Get(queryFilter, orderByFilter, "").ToList();

            return Mapper.Map<List<NlogDto>>(result.ToList());
            

        }

        public void DeleteDataByParam(NlogGetByParamInput input)
        {
            var listData = GetNlogByParam(input);

            foreach (var nlogDto in listData)
            {
                //get data 
                var data = _repository.GetByID(nlogDto.Nlog_Id);
                if (data != null)
                {
                    _repository.Delete(data);
                }
            }

           
            _uow.SaveChanges();
        }

        public void BackupXmlLog(BackupXmlLogInput input)
        {
            var inputParam = new NlogGetByParamInput();
            inputParam.FileName = input.FileName;
            inputParam.Month = input.Month;

            var listData = GetNlogByParam(inputParam);

            foreach (var nlogDto in listData)
            {
                //get data 
                var data = _repository.GetByID(nlogDto.Nlog_Id);
                if (data != null)
                {
                    _repository.Delete(data);
                }
            }

            //ZipHelper.CreateZip();
            //backup to zip file
            var listFile = listData.DistinctBy(c => c.FileName).Select(x => x.FileName).ToList();

            //check file
            foreach (var file in listFile)
            {
                if (!System.IO.File.Exists(input.FolderPath + file))
                    throw new BLLException(ExceptionCodes.BLLExceptions.LogXmlNotFound);
            }
            
            ZipHelper.CreateZip(listFile,input.FolderPath, input.FileZipName);

            _uow.SaveChanges();
        }
    }
}
