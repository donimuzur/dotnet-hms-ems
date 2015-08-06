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
        private IGenericRepository<PAGE_MAP> _pageMapRepository;

        public PageBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _pageRepository = _uow.GetGenericRepository<PAGE>();
            _pageMapRepository = _uow.GetGenericRepository<PAGE_MAP>();
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

        public void Save(PAGE_MAP pageMap)
        {
            _pageMapRepository.InsertOrUpdate(pageMap);
            _uow.SaveChanges();
        }

        public void DeletePageMap(int id)
        {
            _pageMapRepository.Delete(id);
        }

        public List<PAGE> GetParentPages()
        {
            var arrParent = new List<int?>{ 1, 3 };
            return _pageRepository.Get(p => arrParent.Any(x=>x == p.PARENT_PAGE_ID)).ToList();
        }
    }
}
