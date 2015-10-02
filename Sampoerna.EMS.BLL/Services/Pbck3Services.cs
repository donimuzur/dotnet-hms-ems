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


        public void InsertPbck3FromCk5MarketReturn(InsertPbck3FromCk5MarketReturnInput input)
        {
            //generate pbck3number
           
            var generateNumberInput = new GenerateDocNumberInput()
            {
                Year = DateTime.Now.Year,
                Month = DateTime.Now.Month,
                NppbkcId = input.NppbkcId,
                FormType = Enums.FormType.CK5MarketReturn
            };

            var dbPbck3 = new PBCK3();

            dbPbck3.PBCK3_NUMBER = _documentSequenceNumberBll.GenerateNumberNoReset(generateNumberInput);
            dbPbck3.PBCK3_DATE = DateTime.Now;
            dbPbck3.STATUS = Enums.DocumentStatus.Draft;
            dbPbck3.CREATED_BY = input.UserId;
            dbPbck3.CREATED_DATE = DateTime.Now;
            dbPbck3.CK5_ID = input.Ck5Id;

            _repository.InsertOrUpdate(dbPbck3);

        }
    }
}
