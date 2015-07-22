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
    public class WorkflowSettingBLL : IWorkflowSettingBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;

        private IGenericRepository<WORKFLOW_STATE> _repository;
        public WorkflowSettingBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;

            _repository = _uow.GetGenericRepository<WORKFLOW_STATE>();
        }
        public void SaveWorkflowState(WORKFLOW_STATE data)
        {
            if (data.ACTION_ID != 0)
            {
                var old = _repository.GetByID(data.ACTION_ID);

                old.ACTION_NAME = data.ACTION_NAME;
                old.EMAIL_TEMPLATE_ID = data.EMAIL_TEMPLATE_ID;
                old.USER = data.USER;

                _repository.Update(old);
            }
            else {
                _repository.Insert(data);
            }

            _uow.SaveChanges();
            //throw new NotImplementedException();
        }

        public List<WORKFLOW_STATE> GetAllByFormId(long id)
        {
            return _repository.Get(obj => obj.FORM_ID == id, null, "USERS,EMAIL_TEMPLATE").ToList();
        }

        public WORKFLOW_STATE GetAllById(long id)
        {
            return _repository.GetByID(id);
        }
    }
}
