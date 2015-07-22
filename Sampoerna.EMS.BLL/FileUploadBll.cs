using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class FileUploadBLL : IFileUploadBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<FILE_UPLOAD> _repository;
 

        public FileUploadBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<FILE_UPLOAD>();
        }
        public void Save(FILE_UPLOAD file_upload)
        {
            _repository.InsertOrUpdate(file_upload);
        }
    }
}
