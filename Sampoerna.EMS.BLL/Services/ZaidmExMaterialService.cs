using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class ZaidmExMaterialService : IZaidmExMaterialService
    {
        private IGenericRepository<ZAIDM_EX_MATERIAL> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private ChangesHistoryBLL _changesHistoryBll;

        public ZaidmExMaterialService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_MATERIAL>();
            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);
        }

        public ZAIDM_EX_MATERIAL GetByMaterialAndPlantId(string materialId, string plantId)
        {
            return _repository.Get(c => c.STICKER_CODE == materialId && c.WERKS == plantId, null, "ZAIDM_EX_GOODTYP, UOM").FirstOrDefault();
        }

        public List<ZAIDM_EX_MATERIAL> GetByPlantId(string plantId)
        {
            return _repository.Get(c => c.WERKS == plantId).ToList();
        }

        public List<ZAIDM_EX_MATERIAL> GetByMaterialListAndPlantId(List<string> materialList, string plantId)
        {
            Expression<Func<ZAIDM_EX_MATERIAL, bool>> queryFilter =
                c => c.WERKS == plantId && materialList.Contains(c.STICKER_CODE);
            return _repository.Get(queryFilter, null, "UOM").ToList();
        }

        public List<ZAIDM_EX_MATERIAL> GetByPlantIdAndExGoodType(List<string> plantId, string exGoodType)
        {
            return _repository.Get(c => plantId.Contains(c.WERKS) && c.EXC_GOOD_TYP == exGoodType).ToList();
        }

        public List<ZAIDM_EX_MATERIAL> GetAll()
        {
            return _repository.Get().ToList();
        }

        public void ClientDeletion(MaterialDto data, string userId)
        {
            var original = _repository.Get(x => x.STICKER_CODE == data.STICKER_CODE && x.WERKS == data.WERKS, null, "").FirstOrDefault();


            var datatobeclientdeleted = _repository.Get(x => x.STICKER_CODE == data.STICKER_CODE, null, "").ToList();

            foreach (var detail in datatobeclientdeleted)
            {


                if (original.CLIENT_DELETION != data.CLIENT_DELETION)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Core.Enums.MenuList.MaterialMaster,
                        FORM_ID = detail.STICKER_CODE + detail.WERKS,
                        FIELD_NAME = "CLIENT_DELETION",
                        MODIFIED_BY = userId,
                        MODIFIED_DATE = DateTime.Now,
                        OLD_VALUE = detail.CLIENT_DELETION.HasValue ? detail.CLIENT_DELETION.Value.ToString() : "NULL",
                        NEW_VALUE = data.CLIENT_DELETION.HasValue ? data.CLIENT_DELETION.ToString() : "NULL"
                    };

                    _changesHistoryBll.AddHistory(changes);
                }
                detail.CLIENT_DELETION = data.CLIENT_DELETION;

            }


            //_uow.SaveChanges();
        }

        public void PlantDeletion(MaterialDto data, string userId)
        {

            var original = _repository.Get(x => x.STICKER_CODE == data.STICKER_CODE && x.WERKS == data.WERKS, null, "").FirstOrDefault();
            if (original.PLANT_DELETION != data.PLANT_DELETION)
            {
                var changes = new CHANGES_HISTORY
                {
                    FORM_TYPE_ID = Core.Enums.MenuList.MaterialMaster,
                    FORM_ID = data.STICKER_CODE + data.WERKS,
                    FIELD_NAME = "PLANT_DELETION",
                    MODIFIED_BY = userId,
                    MODIFIED_DATE = DateTime.Now,
                    OLD_VALUE = data.PLANT_DELETION.HasValue ? data.PLANT_DELETION.Value.ToString() : "NULL",
                    NEW_VALUE = true.ToString()
                };

                _changesHistoryBll.AddHistory(changes);
            }

            original.PLANT_DELETION = data.PLANT_DELETION;
        }
    }
}
