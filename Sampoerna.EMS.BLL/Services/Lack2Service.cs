using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class Lack2Service : ILack2Service
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<LACK2> _repository;
        private IGenericRepository<LACK2_ITEM> _repositoryItem;
        private IGenericRepository<LACK2_DOCUMENT> _repositoryDocument;

        private string includeTables = "MONTH";

        public Lack2Service(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<LACK2>();
            _repositoryItem = _uow.GetGenericRepository<LACK2_ITEM>();
            _repositoryDocument = _uow.GetGenericRepository<LACK2_DOCUMENT>();
        }

        public List<LACK2> GetAll()
        {
            return _repository.Get().ToList();
        }

        public List<LACK2> GetOpenDocument(Login user)
        {
            Expression<Func<LACK2, bool>> queryFilter = PredicateHelper.True<LACK2>();

            if (user.UserRole == Core.Enums.UserRole.POA)
            {
                var nppbkc = _nppbkcbll.GetNppbkcsByPOA(user.USER_ID).Select(d => d.NPPBKC_ID).ToList();

                queryFilter = queryFilter.And(c => (c.CREATED_BY == user.USER_ID || (c.STATUS != Core.Enums.DocumentStatus.Draft && nppbkc.Contains(c.NPPBKC_ID))));

            }
            else if (user.UserRole == Enums.UserRole.Manager)
            {
                var poaList = _poabll.GetPOAIdByManagerId(user.USER_ID);
                var document = _workflowHistoryBll.GetDocumentByListPOAId(poaList);

                queryFilter = queryFilter.And(c => c.STATUS != Enums.DocumentStatus.Draft && c.STATUS != Enums.DocumentStatus.WaitingForApproval && document.Contains(c.LACK2_NUMBER));
            }
            else
            {
                queryFilter = queryFilter.And(c => c.CREATED_BY == user.USER_ID);
            }

            queryFilter = queryFilter.And(c => c.STATUS != Enums.DocumentStatus.Completed);
        }
    }
}
