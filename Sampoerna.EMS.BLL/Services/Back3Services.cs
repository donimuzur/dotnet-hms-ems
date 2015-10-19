
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class Back3Services : IBack3Services
    {
        private IGenericRepository<BACK3> _repository;
        private IGenericRepository<BACK3_DOCUMENT> _repositoryBack3Documents;
        private ILogger _logger;
        private IUnitOfWork _uow;

         private string includeTables = "BACK3_DOCUMENT";

         public Back3Services(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<BACK3>();
            _repositoryBack3Documents = _uow.GetGenericRepository<BACK3_DOCUMENT>();
        }

         public BACK3 GetBack3ByPbck3Id(int pbck3Id)
         {
             var dbBack3 = _repository.Get(c => c.PBCK3_ID == pbck3Id, null, includeTables).FirstOrDefault();

             return dbBack3;

         }

        public void SaveBack3ByPbck3Id(SaveBack3ByPbck3IdInput input)
        {
            var dbBack3 = _repository.Get(c => c.PBCK3_ID == input.Pbck3Id, null, includeTables).FirstOrDefault() ?? new BACK3();

            dbBack3.PBCK3_ID = input.Pbck3Id;
            dbBack3.BACK3_NUMBER = input.Back3Number;
            dbBack3.BACK3_DATE = input.Back3Date;
           
            ////delete child first
            //foreach (var back3Doc in dbBack3.BACK3_DOCUMENT.ToList())
            //{
            //    _repositoryBack3Documents.Delete(back3Doc);
            //}


            //foreach (var back3Document in input.Back3Documents)
            //{
            //    back3Document.BACK3_ID = dbBack3.BACK3_ID;
            //    back3Document.BACK3_DOC_ID = 0;
            //    dbBack3.BACK3_DOCUMENT.Add(back3Document);

            //}

            foreach (var back3Document in input.Back3Documents)
            {
                back3Document.BACK3_ID = dbBack3.BACK3_ID;
                back3Document.BACK3_DOC_ID = 0;
                _repositoryBack3Documents.InsertOrUpdate(back3Document);

            }
        
            _repository.InsertOrUpdate(dbBack3);
        }

        public bool IsExistBack3DocumentByPbck3(int pbck3Id)
        {
            var dbBack3 = _repository.Get(c => c.PBCK3_ID == pbck3Id, null, includeTables).ToList();

            return dbBack3.Any(back3 => back3.BACK3_DOCUMENT.Count > 0);
        }

    }
}
