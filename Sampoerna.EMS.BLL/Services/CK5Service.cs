﻿using System;
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
        private ILogger _logger;
        private IUnitOfWork _uow;

        public CK5Service(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<CK5>();

        }

        public List<CK5> GetForLack1ByParam(Ck5GetForLack1ByParamInput input)
        {
            //&& !string.IsNullOrEmpty(c.STO_RECEIVER_NUMBER)
            Expression<Func<CK5, bool>> queryFilterCk5 =
                c => c.DEST_PLANT_NPPBKC_ID == input.NppbkcId && c.DEST_PLANT_COMPANY_CODE == input.CompanyCode
                     && (int) c.EX_GOODS_TYPE == input.ExGroupTypeId && c.SOURCE_PLANT_ID == input.SupplierPlantId
                     &&
                     (c.GR_DATE.HasValue && c.GR_DATE.Value.Month == input.PeriodMonth &&
                      c.GR_DATE.Value.Year == input.PeriodYear)
                     && c.STATUS_ID == Enums.DocumentStatus.Completed
                ;

            if (input.Lack1Level == Enums.Lack1Level.Plant)
            {
                queryFilterCk5 = queryFilterCk5.And(c => c.DEST_PLANT_ID == input.ReceivedPlantId);
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

            return _repository.Get(queryFilterCk5, null, "UOM").ToList();
        }

        public List<CK5> GetForLack2ByParam(Ck5GetForLack2ByParamInput input)
        {
            var data = _repository.Get(
                p => p.SOURCE_PLANT_NPPBKC_ID == input.NppbkcId && p.SOURCE_PLANT_COMPANY_CODE == input.CompanyCode &&
                     p.GI_DATE.HasValue && p.GI_DATE.Value.Month == input.PeriodMonth &&
                     p.GI_DATE.Value.Year == input.PeriodYear &&
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

    }

}
