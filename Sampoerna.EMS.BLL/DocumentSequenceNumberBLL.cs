using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using System.Collections.Generic;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class DocumentSequenceNumberBLL : IDocumentSequenceNumberBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<DOC_NUMBER_SEQ> _repository;
        private IGenericRepository<WORKFLOW_HISTORY> _workflowHitoryRepository;
        //private IGenericRepository<ZAIDM_EX_NPPBKC> _nppbkcRepository;
        private IGenericRepository<T001K> _t001KReporRepository;


        public DocumentSequenceNumberBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<DOC_NUMBER_SEQ>();
            _t001KReporRepository = _uow.GetGenericRepository<T001K>();
            _workflowHitoryRepository = _uow.GetGenericRepository<WORKFLOW_HISTORY>();
        }

        //public string GenerateNumber(GenerateDocNumberInput input)
        //{
        //    string docNumber;

        //    var lastSeqData = _repository.Get(c => c.MONTH == input.Month && c.YEAR == input.Year).FirstOrDefault();

        //    if (lastSeqData == null)
        //    {
        //        //insert new record
        //        lastSeqData = new DOC_NUMBER_SEQ()
        //        {
        //            YEAR = input.Year, 
        //            MONTH = input.Month, 
        //            DOC_NUMBER_SEQ_LAST = 1
        //        };
        //        _repository.Insert(lastSeqData);
        //    }
        //    else
        //    {
        //        lastSeqData.DOC_NUMBER_SEQ_LAST += 1;
        //        _repository.Update(lastSeqData);
        //    }

        //    docNumber = lastSeqData.DOC_NUMBER_SEQ_LAST.ToString("0000000000");

        //    if (input.FormType != Enums.FormType.CK5)
        //    {
        //        var t001Data =
        //            _t001KReporRepository.Get(
        //                c =>
        //                    c.T001W.NPPBKC_ID == input.NppbkcId && c.T001W.IS_MAIN_PLANT.HasValue &&
        //                    c.T001W.IS_MAIN_PLANT.Value, null, "T001, T001W, T001W.ZAIDM_EX_NPPBKC").FirstOrDefault();

        //        if (t001Data == null || t001Data.T001 == null)
        //        {
        //            throw new BLLException(ExceptionCodes.BLLExceptions.GenerateNumberCompanyDataHasNotSet);
        //        }
                
        //        if (string.IsNullOrEmpty(t001Data.T001.BUTXT_ALIAS))
        //        {
        //            throw new BLLException(ExceptionCodes.BLLExceptions.GenerateNumberCompanyAliasHasNotSet);
        //        }

        //        if (t001Data.T001W == null || t001Data.T001W.ZAIDM_EX_NPPBKC == null)
        //        {
        //            throw new BLLException(ExceptionCodes.BLLExceptions.GenerateNumberPlantDataHasNotSet);
        //        }

        //        if (string.IsNullOrEmpty(t001Data.T001W.ZAIDM_EX_NPPBKC.CITY_ALIAS))
        //        {
        //            throw new BLLException(ExceptionCodes.BLLExceptions.GenerateNumberCityAliasHasNotSet);
        //        }

        //        //generate number
        //        docNumber = docNumber + "/" + t001Data.T001.BUTXT_ALIAS + "/" + t001Data.T001W.ZAIDM_EX_NPPBKC.CITY_ALIAS 
        //            + "/" + MonthHelper.ConvertToRomansNumeral(input.Month) + "/" + input.Year;

        //    }
            
        //    _uow.SaveChanges();

        //    return docNumber;

        //}

        /// <summary>
        /// Infinitive sequence number by Form Type
        /// </summary>
        /// <param name="formType"></param>
        /// <returns></returns>
        public string GenerateNumberByFormType(Enums.FormType formType)
        {
            while (true)
            {
                DOC_NUMBER_SEQ docSeqNumberToInsertOrUpdate;

                var docNumber = GetGenerateNumberByFormType(formType, out docSeqNumberToInsertOrUpdate);
                var isAlreadyExists = IsDocumentNumberAlreadyExists(docNumber);

                if (isAlreadyExists)
                    continue;

                if (docSeqNumberToInsertOrUpdate.DOC_NUMBER_SEQ_ID > 0)
                {
                    //update
                    _repository.Update(docSeqNumberToInsertOrUpdate);
                }
                else
                {
                    //insert new record
                    _repository.Insert(docSeqNumberToInsertOrUpdate);
                }

                _uow.SaveChanges();

                return docNumber;
            }
        }

        private string GetGenerateNumberByFormType(Enums.FormType formType, out DOC_NUMBER_SEQ docSeqNumberToInsertOrUpdate)
        {
            var lastSeqData = _repository.Get(c => c.FORM_TYPE_ID == formType).FirstOrDefault();

            if (lastSeqData == null)
            {
                //insert new record
                lastSeqData = new DOC_NUMBER_SEQ()
                {
                    YEAR = 0,
                    MONTH = 1, //have rellation with table MONTH so default will be 1
                    FORM_TYPE_ID = formType,
                    DOC_NUMBER_SEQ_LAST = 1
                };
            }
            else
            {
                lastSeqData.DOC_NUMBER_SEQ_LAST += 1;
            }

            docSeqNumberToInsertOrUpdate = lastSeqData;

            var docNumber = lastSeqData.DOC_NUMBER_SEQ_LAST.ToString("0000000000");
            return docNumber;
        }

        public string GenerateNumber(GenerateDocNumberInput input)
        {
            while (true)
            {
                DOC_NUMBER_SEQ docSeqNumberToInsertOrUpdate;

                var docNumber = GetGenerateNumber(input, out docSeqNumberToInsertOrUpdate);
                var isAlreadyExists = IsDocumentNumberAlreadyExists(docNumber);

                if (isAlreadyExists)
                    continue;

                if (docSeqNumberToInsertOrUpdate.DOC_NUMBER_SEQ_ID > 0)
                {
                    //update
                    _repository.Update(docSeqNumberToInsertOrUpdate);
                }
                else
                {
                    //insert new record
                    _repository.Insert(docSeqNumberToInsertOrUpdate);
                }

                _uow.SaveChanges();

                return docNumber;
            }
        }

        private string GetGenerateNumber(GenerateDocNumberInput input, out DOC_NUMBER_SEQ docSeqNumberToInsertOrUpdate)
        {
            var lastSeqData = _repository.Get(c => c.FORM_TYPE_ID != Enums.FormType.CK5).FirstOrDefault();

            if (lastSeqData == null)
            {
                //insert new record
                lastSeqData = new DOC_NUMBER_SEQ()
                {
                    YEAR = input.Year,
                    MONTH = input.Month,
                    DOC_NUMBER_SEQ_LAST = 1
                };
            }
            else
            {
                lastSeqData.DOC_NUMBER_SEQ_LAST += 1;
            }

            //var docNumber = lastSeqData.DOC_NUMBER_SEQ_LAST.ToString();//log : 2015-10-12
            var docNumber = lastSeqData.DOC_NUMBER_SEQ_LAST.ToString("0000000000"); //log : 2015-1013

            if (input.FormType != Enums.FormType.CK5)
            {
                var t001Data =
                    _t001KReporRepository.Get(
                        c =>
                            (c.T001W.NPPBKC_ID == input.NppbkcId || c.T001W.NPPBKC_IMPORT_ID == input.NppbkcId) && c.T001W.IS_MAIN_PLANT.HasValue &&
                            c.T001W.IS_MAIN_PLANT.Value, null, "T001, T001W, T001W.ZAIDM_EX_NPPBKC").FirstOrDefault();

                if (t001Data == null || t001Data.T001 == null)
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.GenerateNumberCompanyDataHasNotSet);
                }

                if (string.IsNullOrEmpty(t001Data.T001.BUTXT_ALIAS))
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.GenerateNumberCompanyAliasHasNotSet);
                }

                if (t001Data.T001W == null || t001Data.T001W.ZAIDM_EX_NPPBKC == null)
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.GenerateNumberPlantDataHasNotSet);
                }

                if (string.IsNullOrEmpty(t001Data.T001W.ZAIDM_EX_NPPBKC.CITY_ALIAS))
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.GenerateNumberCityAliasHasNotSet);
                }

                //generate number
                docNumber = docNumber + "/" + t001Data.T001.BUTXT_ALIAS + "/" + t001Data.T001W.ZAIDM_EX_NPPBKC.CITY_ALIAS
                            + "/" + MonthHelper.ConvertToRomansNumeral(input.Month) + "/" + input.Year;

            }

            docSeqNumberToInsertOrUpdate = lastSeqData;

            return docNumber;
        }

        public List<DOC_NUMBER_SEQ> GetDocumentSequenceList()
        {
            return _repository. Get(null, null, "MONTH1").ToList();
        }

        private bool IsDocumentNumberAlreadyExists(string documentNumber)
        {
            var dbCheck = _workflowHitoryRepository.Get(c => c.FORM_NUMBER == documentNumber).FirstOrDefault();
            return dbCheck != null;
        }

    }
}
