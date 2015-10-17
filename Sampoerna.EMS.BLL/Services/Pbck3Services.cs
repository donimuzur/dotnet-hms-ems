using System;
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

            _documentSequenceNumberBll = new DocumentSequenceNumberBLL(_uow,_logger);
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
                CK5_ID = input.Ck5Id
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
                PBCK7_ID = input.Pbck7Id
            };

            _repository.InsertOrUpdate(dbPbck3);

            return dbPbck3.PBCK3_NUMBER;
        }
    }
}
