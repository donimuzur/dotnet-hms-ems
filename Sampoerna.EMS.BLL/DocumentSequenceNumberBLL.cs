using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using System.Collections.Generic;

namespace Sampoerna.EMS.BLL
{
    public class DocumentSequenceNumberBLL : IDocumentSequenceNumberBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<DOC_NUMBER_SEQ> _repository;
        //private IGenericRepository<ZAIDM_EX_NPPBKC> _nppbkcRepository;
        private IGenericRepository<T001K> _t001KReporRepository;


        public DocumentSequenceNumberBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<DOC_NUMBER_SEQ>();
            _t001KReporRepository = _uow.GetGenericRepository<T001K>();
        }

        public string GenerateNumber(GenerateDocNumberInput input)
        {
            var lastSeqData = _repository.Get(c => c.MONTH == input.Month && c.YEAR == input.Year).FirstOrDefault();

            if (lastSeqData == null)
            {
                //insert new record
                lastSeqData = new DOC_NUMBER_SEQ() { YEAR = input.Year, MONTH = input.Month, DOC_NUMBER_SEQ_LAST = 1 };
                _repository.Insert(lastSeqData);
            }
            else
            {
                lastSeqData.DOC_NUMBER_SEQ_LAST += 1;
                _repository.Update(lastSeqData);
            }

            //var nppbkcData = _nppbkcRepository.GetByID(input.NppbkcId);
            //var nppbkcData = _nppbkcRepository.Get(c => c.NPPBKC_ID == input.NppbkcId, null, "T001").FirstOrDefault();
            var t001Data = _t001KReporRepository.Get(c => c.T001W.NPPBKC_ID == input.NppbkcId && c.T001W.IS_MAIN_PLANT.HasValue && c.T001W.IS_MAIN_PLANT.Value, null, "T001, T001W, T001W.ZAIDM_EX_NPPBKC").FirstOrDefault();

            //if (nppbkcData == null)
            //    throw new BLLException(ExceptionCodes.BLLExceptions.NppbkcNotFound);

            //generate number
            string rc = lastSeqData.DOC_NUMBER_SEQ_LAST.ToString("00000") + "/" + ((t001Data != null && t001Data.T001 != null) ? t001Data.T001.BUTXT_ALIAS : "-") + "/" + (t001Data != null && t001Data.T001W != null && t001Data.T001W.ZAIDM_EX_NPPBKC != null ? t001Data.T001W.ZAIDM_EX_NPPBKC.CITY_ALIAS : "-") + "/" + MonthHelper.ConvertToRomansNumeral(input.Month) + "/" + input.Year.ToString();

            _uow.SaveChanges();

            return rc;

        }




        public List<DOC_NUMBER_SEQ> GetDocumentSequenceList()
        {
            return _repository. Get(null, null, "MONTH1").ToList();
        }
    }
}
