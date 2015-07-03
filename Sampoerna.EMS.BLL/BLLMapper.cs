using System.Collections.Generic;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.BLL
{
    public class BLLMapper
    {
        public static void Initialize()
        {
            //AutoMapper
            Mapper.CreateMap<USER, UserTree>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Manager, opt => opt.MapFrom(src => src.USER2))
                .ForMember(dest => dest.Employees, opt => opt.MapFrom(src => src.USER1));
            Mapper.CreateMap<USER, Login>().IgnoreAllNonExisting();

            Mapper.CreateMap<HEADER_FOOTER, HeaderFooter>().IgnoreAllNonExisting()
                .ForMember(dest => dest.COMPANY_CODE, opt => opt.MapFrom(src => src.T1001.BUKRS))
                .ForMember(dest => dest.COMPANY_NAME, opt => opt.MapFrom(src => src.T1001.BUKRSTXT))
                .ForMember(dest => dest.COMPANY_NPWP, opt => opt.MapFrom(src => src.T1001.NPWP));

            Mapper.CreateMap<HEADER_FOOTER_FORM_MAP, HeaderFooterMap>().IgnoreAllNonExisting();

            Mapper.CreateMap<HEADER_FOOTER, HeaderFooterDetails>().IgnoreAllNonExisting()
                .ForMember(dest => dest.COMPANY_CODE, opt => opt.MapFrom(src => src.T1001.BUKRS))
                .ForMember(dest => dest.COMPANY_NAME, opt => opt.MapFrom(src => src.T1001.BUKRSTXT))
                .ForMember(dest => dest.COMPANY_NPWP, opt => opt.MapFrom(src => src.T1001.NPWP))
                .ForMember(dest => dest.HeaderFooterMapList, opt => opt.MapFrom(src => Mapper.Map<List<HeaderFooterMap>>(src.HEADER_FOOTER_FORM_MAP)));

            Mapper.CreateMap<HeaderFooterMap, HEADER_FOOTER_FORM_MAP>().IgnoreAllUnmapped()
                .ForMember(dest => dest.HEADER_FOOTER_FORM_MAP_ID, opt => opt.MapFrom(src => src.HEADER_FOOTER_FORM_MAP_ID))
                .ForMember(dest => dest.FORM_TYPE_ID, opt => opt.MapFrom(src => src.FORM_TYPE_ID))
                .ForMember(dest => dest.IS_HEADER_SET, opt => opt.MapFrom(src => src.IS_HEADER_SET))
                .ForMember(dest => dest.IS_FOOTER_SET, opt => opt.MapFrom(src => src.IS_FOOTER_SET))
                .ForMember(dest => dest.HEADER_FOOTER_ID, opt => opt.MapFrom(src => src.HEADER_FOOTER_ID));

            Mapper.CreateMap<HeaderFooterDetails, HEADER_FOOTER>().IgnoreAllNonExisting()
                .ForMember(dest => dest.HEADER_FOOTER_FORM_MAP, opt => opt.MapFrom(src => Mapper.Map<List<HEADER_FOOTER_FORM_MAP>>(src.HeaderFooterMapList)));

            Mapper.CreateMap<T1001W, AutoCompletePlant>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.WERKS));

            Mapper.CreateMap<Plant, T1001W>().IgnoreAllNonExisting();
            Mapper.CreateMap<T1001W, Plant>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NPPBKC_NO, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC != null ? src.ZAIDM_EX_NPPBKC.NPPBKC_NO : string.Empty))
                .ForMember(dest => dest.KPPBC_NO, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC != null && src.ZAIDM_EX_NPPBKC.ZAIDM_EX_KPPBC != null ? src.ZAIDM_EX_NPPBKC.ZAIDM_EX_KPPBC.KPPBC_NUMBER : string.Empty))
                ;

            Mapper.CreateMap<PBCK1_PROD_CONVERTER, Pbck1ProdConverter>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck1ProdConvId, opt => opt.MapFrom(src => src.PBCK1_PROD_COV_ID))
                .ForMember(dest => dest.Pbck1Id, opt => opt.MapFrom(src => src.PBCK1_ID))
                .ForMember(dest => dest.ProdTypeId, opt => opt.MapFrom(src => src.PROD_TYPE_ID))
                .ForMember(dest => dest.ConverterOutput, opt => opt.MapFrom(src => src.CONVERTER_OUTPUT))
                .ForMember(dest => dest.ConverterOutputUomId, opt => opt.MapFrom(src => src.CONVERTER_UOM_ID))
                ;

            Mapper.CreateMap<PBCK1_PROD_PLAN, Pbck1ProdPlan>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck1ProdPlanId, opt => opt.MapFrom(src => src.PBCK1_PROD_PLAN_ID))
                .ForMember(dest => dest.Pbck1Id, opt => opt.MapFrom(src => src.PBCK1_ID))
                .ForMember(dest => dest.ProdTypeId, opt => opt.MapFrom(src => src.PROD_TYPE_ID))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.AMOUNT))
                .ForMember(dest => dest.BkcRequired, opt => opt.MapFrom(src => src.BKC_REQUIRED))
                .ForMember(dest => dest.MonthId, opt => opt.MapFrom(src => src.MONTH))
                ;

            Mapper.CreateMap<PBCK1, Pbck1>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck1Id, opt => opt.MapFrom(src => src.PBCK1_ID))
                .ForMember(dest => dest.Pbck1Number, opt => opt.MapFrom(src => src.NUMBER))
                .ForMember(dest => dest.Pbck1Reference, opt => opt.MapFrom(src => src.PBCK1_REF))
                .ForMember(dest => dest.Pbck1Type, opt => opt.MapFrom(src => src.PBCK1_TYPE))
                .ForMember(dest => dest.PeriodFrom, opt => opt.MapFrom(src => src.PERIOD_FROM.Value))
                .ForMember(dest => dest.PeriodTo, opt => opt.MapFrom(src => src.PERIOD_TO))
                .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.REPORTED_ON))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.GoodTypeId, opt => opt.MapFrom(src => src.GOODTYPE_ID))
                .ForMember(dest => dest.SupplierPlant, opt => opt.MapFrom(src => src.SUPPLIER_PLANT))
                .ForMember(dest => dest.SupplierPortId, opt => opt.MapFrom(src => src.SUPPLIER_PORT_ID))
                .ForMember(dest => dest.SupplierAddress, opt => opt.MapFrom(src => src.SUPPLIER_ADDRESS))
                .ForMember(dest => dest.SupplierPhone, opt => opt.MapFrom(src => src.SUPPLIER_PHONE))
                .ForMember(dest => dest.PlanProdFrom, opt => opt.MapFrom(src => src.PLAN_PROD_FROM))
                .ForMember(dest => dest.PlanProdTo, opt => opt.MapFrom(src => src.PLAN_PROD_TO))
                .ForMember(dest => dest.RequestQty, opt => opt.MapFrom(src => src.REQUEST_QTY))
                .ForMember(dest => dest.RequestQtyUomId, opt => opt.MapFrom(src => src.REQUEST_QTY_UOM))
                .ForMember(dest => dest.Lack1FromMonthId, opt => opt.MapFrom(src => src.LACK1_FROM_MONTH))
                .ForMember(dest => dest.Lack1FormYear, opt => opt.MapFrom(src => src.LACK1_FROM_YEAR))
                .ForMember(dest => dest.Lack1ToMonthId, opt => opt.MapFrom(src => src.LACK1_TO_MONTH))
                .ForMember(dest => dest.Lack1ToYear, opt => opt.MapFrom(src => src.LACK1_TO_YEAR))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS))
                .ForMember(dest => dest.StatusGov, opt => opt.MapFrom(src => src.STATUS_GOV))
                .ForMember(dest => dest.QtyApproved, opt => opt.MapFrom(src => src.QTY_APPROVED))
                .ForMember(dest => dest.DecreeDate, opt => opt.MapFrom(src => src.DECREE_DATE))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.ApprovedById, opt => opt.MapFrom(src => src.APPROVED_BY))
                .ForMember(dest => dest.ApprovedDate, opt => opt.MapFrom(src => src.APPROVED_DATE))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.LatestSaldo, opt => opt.MapFrom(src => src.LATEST_SALDO))
                .ForMember(dest => dest.LatestSaldoUomId, opt => opt.MapFrom(src => src.LATEST_SALDO_UOM))
                .ForMember(dest => dest.Pbck1Childs, opt => opt.MapFrom(src => Mapper.Map<List<Pbck1>>(src.PBCK11)))
                .ForMember(dest => dest.Pbck1Parent, opt => opt.MapFrom(src => Mapper.Map<Pbck1>(src.PBCK12)))
                .ForMember(dest => dest.Pbck1ProdConverter, opt => opt.MapFrom(src => Mapper.Map<List<Pbck1ProdConverter>>(src.PBCK1_PROD_CONVERTER)))
                .ForMember(dest => dest.Pbck1ProdPlan, opt => opt.MapFrom(src => Mapper.Map<List<Pbck1ProdPlan>>(src.PBCK1_PROD_PLAN)))
                ;

        }
    }
}
