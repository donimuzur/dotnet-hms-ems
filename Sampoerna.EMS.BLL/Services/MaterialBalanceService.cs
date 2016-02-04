using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class MaterialBalanceService : IMaterialBalanceService
    {
        private IGenericRepository<ZAIDM_EX_MATERIAL_BALANCE> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public MaterialBalanceService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_MATERIAL_BALANCE>();
        }

        public List<ZAIDM_EX_MATERIAL_BALANCE> GetByPlantAndMaterialList(List<string> plantId, List<string> materialList, int month, int year,string uomId)
        {
            Expression<Func<ZAIDM_EX_MATERIAL_BALANCE, bool>> queryFilter =
                c => plantId.Contains(c.WERKS) && materialList.Contains(c.MATERIAL_ID)
                    && c.PERIOD_MONTH == month && c.PERIOD_YEAR == year;

            var data = _repository.Get(queryFilter,null,"ZAIDM_EX_MATERIAL,MATERIAL_UOM").ToList();
            data = ProcessConvertionMaterialbalance(data, uomId);
            return data;
        }

        private List<ZAIDM_EX_MATERIAL_BALANCE> ProcessConvertionMaterialbalance(List<ZAIDM_EX_MATERIAL_BALANCE> data,string uomId)
        {
            var result = new List<ZAIDM_EX_MATERIAL_BALANCE>();
            foreach (var zaidmExMaterialBalance in data)
            {
                var resultData = zaidmExMaterialBalance;
                var materialUom =
                    zaidmExMaterialBalance.ZAIDM_EX_MATERIAL.MATERIAL_UOM.FirstOrDefault(x => x.MEINH == uomId);

                if (materialUom != null)
                {
                    resultData.OPEN_BALANCE = zaidmExMaterialBalance.OPEN_BALANCE / materialUom.UMREN;
                    resultData.CLOSE_BALANCE = zaidmExMaterialBalance.CLOSE_BALANCE / materialUom.UMREN;
                }
                else
                {
                    resultData.OPEN_BALANCE = zaidmExMaterialBalance.OPEN_BALANCE;
                    resultData.CLOSE_BALANCE = zaidmExMaterialBalance.CLOSE_BALANCE;
                }

                result.Add(resultData);
            }

            return result;
        }

        public List<ZAIDM_EX_MATERIAL_BALANCE> GetByPlantListAndMaterialList(List<string> plantId, List<string> materialList)
        {
            Expression<Func<ZAIDM_EX_MATERIAL_BALANCE, bool>> queryFilter =
                c => plantId.Contains(c.WERKS) && materialList.Contains(c.MATERIAL_ID);

            return _repository.Get(queryFilter).ToList();
        }
    }
}
