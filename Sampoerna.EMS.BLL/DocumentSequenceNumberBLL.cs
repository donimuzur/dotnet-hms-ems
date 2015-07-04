using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class DocumentSequenceNumberBLL : IDocumentSequenceNumberBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<DOC_NUMBER_SEQ> _repository;
        private IGenericRepository<ZAIDM_EX_NPPBKC> _nppbkcRepository;

        public DocumentSequenceNumberBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<DOC_NUMBER_SEQ>();
            _nppbkcRepository = _uow.GetGenericRepository<ZAIDM_EX_NPPBKC>();
        }

        public string GenerateNumber(GenerateDocNumberInput input)
        {
            var lastSeqData = _repository.Get(c => c.MONTH == input.Month && c.YEAR == input.Year).FirstOrDefault();
            
            if (lastSeqData == null)
            {
                //insert new record
                lastSeqData = new DOC_NUMBER_SEQ() {YEAR = input.Year, MONTH = input.Month, DOC_NUMBER_SEQ_LAST = 1};
                _repository.Insert(lastSeqData);
            }
            else
            {
                lastSeqData.DOC_NUMBER_SEQ_LAST += 1;
                _repository.Update(lastSeqData);
            }

            var nppbkcData = _nppbkcRepository.GetByID(input.NppbkcId);

            if(nppbkcData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.NppbkcNotFound);
            
            //generate number
            string rc = lastSeqData.DOC_NUMBER_SEQ_LAST.ToString("00000") + "/" + nppbkcData.T001.BUTXT + "/" + nppbkcData.CITY_ALIAS + "/" + MonthHelper.ConvertToRomansNumeral(input.Month) + "/" + input.Year.ToString();

            _uow.SaveChanges();

            return rc;

        }
    }
}
