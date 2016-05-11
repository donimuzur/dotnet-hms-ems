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

        public List<MaterialBalanceDto> GetByPlantAndMaterialList(List<string> plantId, List<string> materialList, int month, int year,string uomId)
        {
            Expression<Func<ZAIDM_EX_MATERIAL_BALANCE, bool>> queryFilter =
                c => plantId.Contains(c.WERKS) && materialList.Contains(c.MATERIAL_ID)
                    && c.PERIOD_MONTH == month && c.PERIOD_YEAR == year;

            var data = _repository.Get(queryFilter, null, "ZAIDM_EX_MATERIAL,ZAIDM_EX_MATERIAL.MATERIAL_UOM").ToList();
            var result = ProcessConvertionMaterialbalance(data, uomId);
            return result;
        }

        private List<MaterialBalanceDto> ProcessConvertionMaterialbalance(List<ZAIDM_EX_MATERIAL_BALANCE> data, string uomId)
        {
            var result = new List<MaterialBalanceDto>();
            foreach (var zaidmExMaterialBalance in data)
            {
                var resultData = new MaterialBalanceDto();
                resultData.Werks = zaidmExMaterialBalance.WERKS;
                resultData.MaterialId = zaidmExMaterialBalance.MATERIAL_ID;
                resultData.StorLoc = zaidmExMaterialBalance.LGORT;
                if (zaidmExMaterialBalance.PERIOD_MONTH != null)
                    resultData.Month = zaidmExMaterialBalance.PERIOD_MONTH.Value;
                if (zaidmExMaterialBalance.PERIOD_YEAR != null)
                    resultData.Year = zaidmExMaterialBalance.PERIOD_YEAR.Value;

                var materialUom =
                    zaidmExMaterialBalance.ZAIDM_EX_MATERIAL.MATERIAL_UOM.FirstOrDefault(x => x.MEINH == uomId);

                if (materialUom != null)
                {
                    resultData.OpenBalance = (decimal) (zaidmExMaterialBalance.OPEN_BALANCE / materialUom.UMREN);
                    resultData.CloseBalance = (decimal) (zaidmExMaterialBalance.CLOSE_BALANCE / materialUom.UMREN);
                }
                else
                {
                    if (zaidmExMaterialBalance.OPEN_BALANCE != null)
                        resultData.OpenBalance = zaidmExMaterialBalance.OPEN_BALANCE.Value;
                    if (zaidmExMaterialBalance.CLOSE_BALANCE != null)
                        resultData.CloseBalance = zaidmExMaterialBalance.CLOSE_BALANCE.Value;
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

        public MaterialBalanceTotalDto GetByMaterialListAndPlant(string plantId, List<string> materialList, int month,
            int year)
        {
            var data = new MaterialBalanceTotalDto();
            data.BeginningBalance = new List<string>();
            data.BeginningBalanceUom = new List<string>();
            data.TotalBeginningBalance = 0; 

            foreach (var item in materialList)
            {
                Expression<Func<ZAIDM_EX_MATERIAL_BALANCE, bool>> queryFilter =
                c => c.WERKS == plantId && c.MATERIAL_ID == item && c.PERIOD_MONTH == month && c.PERIOD_YEAR == year;

                var openBalanceList = _repository.Get(queryFilter);

                if (openBalanceList.ToList().Count > 0)
                {
                    var openBalance = openBalanceList.Sum(x => x.OPEN_BALANCE.Value);
                    data.BeginningBalance.Add(openBalance.ToString("N2"));
                    data.BeginningBalanceUom.Add("Gram");
                    data.TotalBeginningBalance += openBalance;
                }
            }

            return data;
        }
    }
}
