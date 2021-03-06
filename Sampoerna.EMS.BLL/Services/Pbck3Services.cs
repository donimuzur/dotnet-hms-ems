﻿using System;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL.Services
{
    public class Pbck3Services : IPbck3Services
    {
        private IGenericRepository<PBCK3> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        private string includeTables = "";

        private DocumentSequenceNumberBLL _documentSequenceNumberBll;

        public Pbck3Services(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<PBCK3>();

            _documentSequenceNumberBll = new DocumentSequenceNumberBLL(_uow, _logger);
        }


        public string InsertPbck3FromCk5MarketReturn(InsertPbck3FromCk5MarketReturnInput input)
        {
            //generate pbck3number

            var generateNumberInput = new GenerateDocNumberInput()
            {
                Year = DateTime.Now.Year,
                Month = DateTime.Now.Month,
                NppbkcId = input.NppbkcId,
                FormType = Enums.FormType.CK5MarketReturn
            };

            var dbPbck3 = new PBCK3
            {
                PBCK3_NUMBER = _documentSequenceNumberBll.GenerateNumber(generateNumberInput),
                PBCK3_DATE = DateTime.Now,
                STATUS = Enums.DocumentStatus.Draft,
                CREATED_BY = input.UserId,
                CREATED_DATE = DateTime.Now,
                CK5_ID = input.Ck5Id,
                EXEC_DATE_FROM = DateTime.Now,
                EXEC_DATE_TO = DateTime.Now
                
            };
            
            _repository.InsertOrUpdate(dbPbck3);

            return dbPbck3.PBCK3_NUMBER;
        }

        public string InsertPbck3FromPbck7(InsertPbck3FromPbck7Input input)
        {
            //generate pbck3number

            var generateNumberInput = new GenerateDocNumberInput()
            {
                Year = DateTime.Now.Year,
                Month = DateTime.Now.Month,
                NppbkcId = input.NppbkcId,
                FormType = Enums.FormType.PBCK3
            };

            var dbPbck3 = new PBCK3
            {
                PBCK3_NUMBER = _documentSequenceNumberBll.GenerateNumber(generateNumberInput),
                PBCK3_DATE = DateTime.Now,
                STATUS = Enums.DocumentStatus.Draft,
                CREATED_BY = input.UserId,
                CREATED_DATE = DateTime.Now,
                PBCK7_ID = input.Pbck7Id,
                EXEC_DATE_FROM = input.Pbck7ExecFrom,
                EXEC_DATE_TO = input.Pbck7ExecTo
            };

            _repository.InsertOrUpdate(dbPbck3);

            return dbPbck3.PBCK3_NUMBER;
        }

        public PBCK3 GetPbck3ByCk5Id(long ck5Id)
        {
            return _repository.Get(c => c.CK5_ID == ck5Id, null, "CK2, PBCK7, PBCK7.BACK1").FirstOrDefault();
        }

        public PBCK3 GetPbck3ByPbck7Id(int pbck7Id)
        {
            return _repository.Get(c => c.PBCK7_ID == pbck7Id, null, "CK2").FirstOrDefault();
        }
    }
}
