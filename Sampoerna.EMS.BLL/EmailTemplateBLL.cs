using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class EmailTemplateBLL : IEmailTemplateBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<EMAIL_TEMPLATE> _repository;
        private string _includeTables = "";

        public EmailTemplateBLL(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<EMAIL_TEMPLATE>();
        }

        public List<EMAIL_TEMPLATE> getAllEmailTemplates()
        {
            return _repository.Get(null, null, _includeTables).ToList();
        }

        public EMAIL_TEMPLATE getEmailTemplateById(long id)
        {
            return _repository.GetByID(id);
        }

        public void Save(EMAIL_TEMPLATE record)
        {
            if (record.EMAIL_TEMPLATE_ID != 0)
            {
                _repository.InsertOrUpdate(record);

            }
            else {
                _repository.Insert(record);
            }

            _uow.SaveChanges();
        }
    }
}
