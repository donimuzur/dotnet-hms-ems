
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class CK2Services : ICK2Services
    {
        private IGenericRepository<CK2> _repository;
        private IGenericRepository<CK2_DOCUMENT> _repositoryCk2Documents;
        private ILogger _logger;
        private IUnitOfWork _uow;

        private string includeTables = "CK2_DOCUMENT";

        public CK2Services(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<CK2>();
            _repositoryCk2Documents = _uow.GetGenericRepository<CK2_DOCUMENT>();
        }

        public CK2 GetCk2ByPbck3Id(int pbck3Id)
        {
            var dbCk2 = _repository.Get(c => c.PBCK3_ID == pbck3Id, null, includeTables).FirstOrDefault();

            return dbCk2;

        }

        public void SaveCk2ByPbck3Id(SaveCk2ByPbck3IdInput input)
        {
            var dbCk2 = _repository.Get(c => c.PBCK3_ID == input.Pbck3Id, null, includeTables).FirstOrDefault() ?? new CK2();

            dbCk2.PBCK3_ID = input.Pbck3Id;
            dbCk2.CK2_NUMBER = input.Ck2Number;
            dbCk2.CK2_DATE = input.Ck2Date;
            dbCk2.CK2_VALUE = input.Ck2Value;

            InsertOrDeleteCk2Item(input.Ck2Documents);

            //foreach (var ck2Document in input.Ck2Documents)
            //{
            //    ck2Document.CK2_ID = dbCk2.CK2_ID;
            //    ck2Document.CK2_DOC_ID = 0;
            //    //dbCk2.CK2_DOCUMENT.Add(ck2Document);
            //    _repositoryCk2Documents.InsertOrUpdate(ck2Document);

            //}

            //dbBack1.BACK1_DOCUMENT = input.Back1Documents;

            _repository.InsertOrUpdate(dbCk2);
        }

        public void InsertOrDeleteCk2Item(List<CK2_DOCUMENTDto> input)
        {
            foreach (var ck2DocumentDto in input)
            {
                if (ck2DocumentDto.IsDeleted)
                {
                    var ck2Doc = _repositoryCk2Documents.GetByID(ck2DocumentDto.CK2_DOC_ID);
                    if (ck2Doc != null)
                        _repositoryCk2Documents.Delete(ck2Doc);
                }
                else
                {
                    if (ck2DocumentDto.CK2_DOC_ID == 0)
                        _repositoryCk2Documents.Insert(Mapper.Map<CK2_DOCUMENT>(ck2DocumentDto));
                }
            }
        }

        public bool IsExistCk2DocumentByPbck3(int pbck3Id)
        {
            var dbCk2 = _repository.Get(c => c.PBCK3_ID == pbck3Id, null, includeTables).ToList();

            return dbCk2.Any(back3 => back3.CK2_DOCUMENT.Count > 0);
        }
    }
}
