using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class ZaidmExNPPBKCBLL : IZaidmExNPPBKCBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAIDM_EX_NPPBKC> _repository;
        private IGenericRepository<REGION_OFFICE_ID> _repositoryRegionOffice;

        public ZaidmExNPPBKCBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_NPPBKC>();
            _repositoryRegionOffice = _uow.GetGenericRepository<REGION_OFFICE_ID>();


        }

        public ZAIDM_EX_NPPBKC GetById(long id)
        {
            return _repository.GetByID(id);
        }

        public List<ZAIDM_EX_NPPBKC> GetAllNppbkc()
        {
            return _repository.Get().ToList();
        }

        public List<ZaidmExNPPBKCOutput> GetAll()
        {    
            var repoNppbkc = _repository.Get().ToList();
            var repoRegionOffice = _repositoryRegionOffice.Get().ToList();

            var result = repoNppbkc.Join(repoRegionOffice,
                nppbkcList => nppbkcList.REGION_OFFICE_ID,
                regionOffice => regionOffice.REGION_OFFICE_ID1,
                (nppbkcList, regionOffice) => new ZaidmExNPPBKCOutput()
                {
                    NppbckId = nppbkcList.NPPBKC_ID,
                    Addr1 = nppbkcList.ADDR1,
                    City = nppbkcList.CITY,
                    RegionOfficeIdNppbkc = regionOffice.REGION_NAME,
                    TextTo = nppbkcList.TEXT_TO

                }).ToList();
            return result;
        }
    }
}
