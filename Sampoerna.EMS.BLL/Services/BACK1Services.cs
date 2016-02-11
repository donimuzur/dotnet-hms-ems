﻿using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class Back1Services : IBack1Services
    {
        private IGenericRepository<BACK1> _repository;
        private IGenericRepository<BACK1_DOCUMENT> _repositoryBac1Documents;
        private ILogger _logger;
        private IUnitOfWork _uow;

        private string includeTables = "BACK1_DOCUMENT";

        public Back1Services(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<BACK1>();
            _repositoryBac1Documents = _uow.GetGenericRepository<BACK1_DOCUMENT>();
        }

        public void SaveBack1ByCk5Id(SaveBack1ByCk5IdInput input)
        {
            var dbBack1 = _repository.Get(c => c.CK5_ID == input.Ck5Id).FirstOrDefault() ?? new BACK1();

            dbBack1.CK5_ID = input.Ck5Id;
            dbBack1.BACK1_NUMBER = input.Back1Number;
            dbBack1.BACK1_DATE = input.Back1Date;

            //delete child first
            foreach (var back1Doc in dbBack1.BACK1_DOCUMENT.ToList())
            {
                _repositoryBac1Documents.Delete(back1Doc);
            }

            foreach (var back1Documents in input.Back1Documents)
            {
                back1Documents.BACK1 = dbBack1.BACK1_ID;
                dbBack1.BACK1_DOCUMENT.Add(back1Documents);

            }

            _repository.InsertOrUpdate(dbBack1);

        }

        public BACK1 GetBack1ByCk5Id(long ck5Id)
        {
            var dbBack1 = _repository.Get(c => c.CK5_ID == ck5Id, null, includeTables).FirstOrDefault();

            return dbBack1;

        }

        public void SaveBack1ByPbck7Id(SaveBack1ByPbck7IdInput input)
        {
            var dbBack1 = _repository.Get(c => c.PBCK7_ID == input.Pbck7Id, null, includeTables).FirstOrDefault() ?? new BACK1();

            dbBack1.PBCK7_ID = input.Pbck7Id;
            dbBack1.BACK1_NUMBER = input.Back1Number;
            dbBack1.BACK1_DATE = input.Back1Date;

            //delete child first
            foreach (var back1Doc in dbBack1.BACK1_DOCUMENT.ToList())
            {
                _repositoryBac1Documents.Delete(back1Doc);
            }

            foreach (var back1Documents in input.Back1Documents)
            {
                back1Documents.BACK1 = dbBack1.BACK1_ID;
                dbBack1.BACK1_DOCUMENT.Add(AutoMapper.Mapper.Map<BACK1_DOCUMENT>(back1Documents));

            }

            //dbBack1.BACK1_DOCUMENT = input.Back1Documents;

            _repository.InsertOrUpdate(dbBack1);

        }

        public BACK1 GetBack1ByPbck7Id(int pbck7Id)
        {
            var dbBack1 = _repository.Get(c => c.PBCK7_ID == pbck7Id, null, includeTables).FirstOrDefault();

            return dbBack1;

        }

        public void InsertOrDeleteBack1Documents(List<BACK1_DOCUMENTDto> input)
        {
            foreach (var back1DocumentDto in input)
            {
                if (back1DocumentDto.IsDeleted)
                {
                    var back1Doc = _repositoryBac1Documents.GetByID(back1DocumentDto.BACK1_DOCUMENT_ID);
                    if (back1Doc != null)
                        _repositoryBac1Documents.Delete(back1Doc);
                }
                else
                {
                    if (back1DocumentDto.BACK1_DOCUMENT_ID == 0)
                        _repositoryBac1Documents.Insert(Mapper.Map<BACK1_DOCUMENT>(back1DocumentDto));
                }
            }
        }

        public void UpdateBack1ByPbck7Id(SaveBack1ByPbck7IdInput input)
        {
            var dbBack1 = _repository.Get(c=>c.PBCK7_ID == input.Pbck7Id).FirstOrDefault();
            if (dbBack1 != null)
            {
                dbBack1.PBCK7_ID = input.Pbck7Id;
                dbBack1.BACK1_NUMBER = input.Back1Number;
                dbBack1.BACK1_DATE = input.Back1Date;

                _repository.InsertOrUpdate(dbBack1);
            }
            

        }

        public BACK1_DOCUMENT GetBack1DocumentById(long id)
        {
            return _repositoryBac1Documents.GetByID(id);
        }
    }
}
