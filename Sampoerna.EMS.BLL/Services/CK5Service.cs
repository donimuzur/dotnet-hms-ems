using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Sampoerna.EMS.Contract.Services;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL.Services
{
    public class CK5Service : ICK5Service
    {
        private IGenericRepository<CK5> _repository;
        private IGenericRepository<CK5_MATERIAL> _repositoryMaterial;
        private ILogger _logger;
        private IUnitOfWork _uow;

        public CK5Service(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<CK5>();
            _repositoryMaterial = _uow.GetGenericRepository<CK5_MATERIAL>();
        }

        public CK5 GetById(long id)
        {
            var dbData = _repository.GetByID(id);
            return dbData;
        }
        public List<CK5> GetForLack1ByParam(Ck5GetForLack1ByParamInput input)
        {
            //&& !string.IsNullOrEmpty(c.STO_RECEIVER_NUMBER)
            Expression<Func<CK5, bool>> queryFilterCk5 =
                c => c.DEST_PLANT_NPPBKC_ID == input.NppbkcId && c.DEST_PLANT_COMPANY_CODE == input.CompanyCode
                     && (int) c.EX_GOODS_TYPE == input.ExGroupTypeId && c.SOURCE_PLANT_ID == input.SupplierPlantId
                     &&
                     (c.GR_DATE.HasValue 
                     && c.GR_DATE.Value.Month == input.PeriodMonth 
                     && c.GR_DATE.Value.Year == input.PeriodYear)
                     && c.STATUS_ID == Enums.DocumentStatus.Completed
                ;

            if (input.Lack1Level == Enums.Lack1Level.Plant)
            {
                queryFilterCk5 = queryFilterCk5.And(c => (c.DEST_PLANT_ID == input.ReceivedPlantId
                    && (c.CK5_TYPE != Enums.CK5Type.Waste && c.CK5_TYPE != Enums.CK5Type.Return))
                    ||  (c.SOURCE_PLANT_ID == input.ReceivedPlantId
                    && (c.CK5_TYPE == Enums.CK5Type.Return) && c.GI_DATE.HasValue));
            }

            if (input.IsExcludeSameNppbkcId)
            {
                queryFilterCk5 = queryFilterCk5.And(c => c.SOURCE_PLANT_NPPBKC_ID != c.DEST_PLANT_NPPBKC_ID);
            }

            if (input.Pbck1DecreeIdList.Count > 0)
            {
                queryFilterCk5 =
                    queryFilterCk5.And(
                        c => c.PBCK1_DECREE_ID.HasValue && input.Pbck1DecreeIdList.Contains(c.PBCK1_DECREE_ID.Value));
            }

            /* story : http://192.168.62.216/TargetProcess/entity/1637 */
            queryFilterCk5 = queryFilterCk5.And(c => (c.CK5_TYPE != Enums.CK5Type.Manual || (c.CK5_TYPE == Enums.CK5Type.Manual && c.REDUCE_TRIAL.HasValue && c.REDUCE_TRIAL.Value)));

            return _repository.Get(queryFilterCk5, null, "UOM").ToList();
        }

        public List<CK5> GetForLack2ByParam(Ck5GetForLack2ByParamInput input)
        {
            var data = _repository.Get(
                p => p.SOURCE_PLANT_NPPBKC_ID == input.NppbkcId && //p.SOURCE_PLANT_COMPANY_CODE == input.CompanyCode &&
                     p.GI_DATE.HasValue && p.GI_DATE.Value.Month == input.PeriodMonth &&
                     p.GI_DATE.Value.Year == input.PeriodYear &&
                     p.PBCK1_DECREE_ID != null &&
                     p.CK5_TYPE != Enums.CK5Type.Export && p.STATUS_ID == Enums.DocumentStatus.Completed &&
                     p.SOURCE_PLANT_ID == input.SourcePlantId && (int) p.EX_GOODS_TYPE == input.ExGroupTypeId).ToList();

            return data;

        }

        public WasteStockQuotaOutput GetWasteStockQuota(decimal wasteStock, string plantId, string materialNumber)
        {
            var result = new WasteStockQuotaOutput();

            //var dbWaste = _wasteStockServices.GetByPlantAndMaterialNumber(plantId, materialNumber);

            //get from ck5 
            if (wasteStock > 0)
            {
                var dbCk5 = _repository.Get(c => c.CK5_TYPE == Enums.CK5Type.Waste
                                                 && c.SOURCE_PLANT_ID == plantId &&
                                                 (c.STATUS_ID != Enums.DocumentStatus.Cancelled), null, "CK5_MATERIAL");

                decimal wasteStockUsed =
                    dbCk5.Sum(
                        c =>
                            c.CK5_MATERIAL.Where(ck5Material => ck5Material.BRAND == materialNumber)
                                .Sum(
                                    ck5Material =>
                                        ck5Material.CONVERTED_QTY.HasValue ? ck5Material.CONVERTED_QTY.Value : 0));



                result.WasteStock = ConvertHelper.ConvertDecimalToStringMoneyFormat(wasteStock);
                result.WasteStockUsed = ConvertHelper.ConvertDecimalToStringMoneyFormat(wasteStockUsed);
                result.WasteStockRemaining =
                    ConvertHelper.ConvertDecimalToStringMoneyFormat((wasteStock - wasteStockUsed));
                result.WasteStockRemainingCount = wasteStock - wasteStockUsed;
            }
            else
            {
                result.WasteStock = "0";
                result.WasteStockUsed = "0";
                result.WasteStockRemaining = "0";
                result.WasteStockRemainingCount = 0;
            }
            return result;

        }

        public List<CK5> GetByStoNumberList(List<string> stoNumberList)
        {
            Expression<Func<CK5, bool>> queryFilter =
                c => stoNumberList.Contains(c.STO_RECEIVER_NUMBER) || stoNumberList.Contains(c.STO_SENDER_NUMBER) ||
                    stoNumberList.Contains(c.DN_NUMBER);

            return _repository.Get(queryFilter, null, "UOM").ToList();
        }

        public List<CK5> GetReconciliationLack1()
        {
            Expression<Func<CK5, bool>> queryFilter =
                c => c.STATUS_ID == Enums.DocumentStatus.Completed && c.GR_DATE != null &&
                    ((c.PBCK1_DECREE_ID != null) || (c.CK5_TYPE == Enums.CK5Type.Waste || c.CK5_TYPE == Enums.CK5Type.Return));

            return _repository.Get(queryFilter, null, "PBCK1, CK5_MATERIAL").OrderBy(x => x.GR_DATE).ToList();
        }

        public List<string> GetCk5AssignedMatdoc()
        {
            Expression<Func<CK5_MATERIAL, bool>> queryFilterMaterial = c => !string.IsNullOrEmpty(c.MATDOC);

            return _repositoryMaterial.Get(queryFilterMaterial).Select(x => x.MATDOC).ToList();
        }


        public List<CK5> GetAllPreviousForLack1(Ck5GetForLack1ByParamInput input)
        {
            var tempyear = input.PeriodMonth == 12 ? input.PeriodYear + 1 : input.PeriodYear;
            var tempmonth = input.PeriodMonth == 12 ? 1 : input.PeriodMonth + 1;

            
            Expression<Func<CK5, bool>> queryFilterCk5 =
                c => c.DEST_PLANT_NPPBKC_ID == input.NppbkcId && c.DEST_PLANT_COMPANY_CODE == input.CompanyCode
                     && (int)c.EX_GOODS_TYPE == input.ExGroupTypeId && c.SOURCE_PLANT_ID == input.SupplierPlantId
                     && (c.GR_DATE.HasValue && c.GR_DATE.Value < new DateTime(tempyear, tempmonth, 1))
                     //original irman
                     //(c.GR_DATE.HasValue && c.GR_DATE.Value.Month == input.PeriodMonth &&
                     // c.GR_DATE.Value.Year == input.PeriodYear)
                     && c.STATUS_ID == Enums.DocumentStatus.Completed
                ;

            if (input.Lack1Level == Enums.Lack1Level.Plant)
            {
                queryFilterCk5 = queryFilterCk5.And(c => (c.DEST_PLANT_ID == input.ReceivedPlantId
                    && (c.CK5_TYPE != Enums.CK5Type.Waste))
                    || (c.SOURCE_PLANT_ID == input.ReceivedPlantId
                    && (c.CK5_TYPE == Enums.CK5Type.Waste)));
            }

            if (input.IsExcludeSameNppbkcId)
            {
                queryFilterCk5 = queryFilterCk5.And(c => c.SOURCE_PLANT_NPPBKC_ID != c.DEST_PLANT_NPPBKC_ID);
            }

            

            /* story : http://192.168.62.216/TargetProcess/entity/1637 */
            queryFilterCk5 = queryFilterCk5.And(c => (c.CK5_TYPE != Enums.CK5Type.Manual || (c.CK5_TYPE == Enums.CK5Type.Manual && c.REDUCE_TRIAL.HasValue && c.REDUCE_TRIAL.Value)));

            return _repository.Get(queryFilterCk5, null, "UOM").ToList();
        }

        public List<CK5> GetCk5WasteByParam(Ck5GetForLack1ByParamInput input)
        {
            //&& !string.IsNullOrEmpty(c.STO_RECEIVER_NUMBER)
            Expression<Func<CK5, bool>> queryFilterCk5 =
                c => c.SOURCE_PLANT_NPPBKC_ID == input.NppbkcId && c.DEST_PLANT_COMPANY_CODE == input.CompanyCode
                     &&
                     (c.GI_DATE.HasValue
                     && c.GI_DATE.Value.Month == input.PeriodMonth
                     && c.GI_DATE.Value.Year == input.PeriodYear)
                     && (c.STATUS_ID == Enums.DocumentStatus.Completed || c.STATUS_ID == Enums.DocumentStatus.GRCompleted || 
                     c.STATUS_ID == Enums.DocumentStatus.WasteDisposal || c.STATUS_ID == Enums.DocumentStatus.WasteApproval
                     || c.STATUS_ID == Enums.DocumentStatus.GoodReceive)
                ;

            if (input.Lack1Level == Enums.Lack1Level.Plant)
            {
                queryFilterCk5 = queryFilterCk5.And(c => (c.SOURCE_PLANT_ID == input.ReceivedPlantId
                    && (c.CK5_TYPE == Enums.CK5Type.Waste)));
            }

            return _repository.Get(queryFilterCk5, null, "UOM").ToList();
        }

        public List<CK5> GetCk5ReturnByParam(Ck5GetForLack1ByParamInput input)
        {
            //&& !string.IsNullOrEmpty(c.STO_RECEIVER_NUMBER)
            Expression<Func<CK5, bool>> queryFilterCk5 =
                c => c.SOURCE_PLANT_NPPBKC_ID == input.NppbkcId //&& c.DEST_PLANT_COMPANY_CODE == input.CompanyCode
                     &&
                     (c.GR_DATE.HasValue
                     && c.GR_DATE.Value.Month == input.PeriodMonth
                     && c.GR_DATE.Value.Year == input.PeriodYear)
                     && c.STATUS_ID == Enums.DocumentStatus.Completed
                ;

            if (input.Lack1Level == Enums.Lack1Level.Plant)
            {
                queryFilterCk5 = queryFilterCk5.And(c => (c.SOURCE_PLANT_ID == input.ReceivedPlantId
                    && c.DEST_PLANT_ID == input.SupplierPlantId
                    && (c.CK5_TYPE == Enums.CK5Type.Return)));
            }

            return _repository.Get(queryFilterCk5, null, "UOM").ToList();
        }
    }

}
