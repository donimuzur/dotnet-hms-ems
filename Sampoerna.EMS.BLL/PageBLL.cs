using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class PageBLL : IPageBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<PAGE> _pageRepository;

        public PageBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _pageRepository = _uow.GetGenericRepository<PAGE>();
        }

        public List<PAGE> GetPages()
        {
            return _pageRepository.Get().ToList();
        }

        public PAGE GetPageByID(int id)
        {
            return _pageRepository.GetByID(id);
        }

        public List<PAGE> GetModulePages() {
            return _pageRepository.Get(q => q.PARENT_PAGE_ID != null && q.PARENT_PAGE_ID != 1, null, "").ToList();
        }

    }
}
