﻿using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.BrandRegistration;
using Sampoerna.EMS.Website.Models.CK5;
using Sampoerna.EMS.Website.Models.GOODSTYPE;
using Sampoerna.EMS.Website.Models.NPPBKC;
using Sampoerna.EMS.Website.Models.PBCK1;
using Sampoerna.EMS.Website.Models.PLANT;
using Sampoerna.EMS.Website.Models.POA;

namespace Sampoerna.EMS.Website
{
    public class EMSWebsiteMapper
    {
        public static void Initialize()
        {
            //AutoMapper
            Mapper.CreateMap<USER, UserViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Manager, opt => opt.MapFrom(src => src.USER2))
                .ForMember(dest => dest.Employees, opt => opt.MapFrom(src => src.USER1));

            Mapper.CreateMap<UserTree, UserViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IS_ACTIVE.HasValue && src.IS_ACTIVE.Value))
                ;

            #region Company
            Mapper.CreateMap<T1001, CompanyDetail>().IgnoreAllNonExisting()
            .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.COMPANY_ID))
            .ForMember(dest => dest.DocumentBukrs, opt => opt.MapFrom(src => src.BUKRS))
            .ForMember(dest => dest.DocumentBukrstxt, opt => opt.MapFrom(src => src.BUKRSTXT))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE));
            #endregion

            Mapper.CreateMap<PBCK1, PBCK1Item>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PBCK1_TYPEText, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.PBCK1_TYPE)))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.PERIOD_FROM.HasValue ? src.PERIOD_FROM.Value.Year.ToString() : string.Empty))
                .ForMember(dest => dest.NPPBKC_NO, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC != null ? src.ZAIDM_EX_NPPBKC.NPPBKC_NO : string.Empty))
                .ForMember(dest => dest.GOODTYPE_DESC, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC))
                .ForMember(dest => dest.REQUEST_QTY_UOM_NAME, opt => opt.MapFrom(src => src.UOM != null ? src.UOM.UOM_NAME : string.Empty))
                .ForMember(dest => dest.LACK1_FROM_MONTH_ID, opt => opt.MapFrom(src => src.LACK1_FROM_MONTH))
                .ForMember(dest => dest.LACK1_FROM_MONTH_NAME, opt => opt.MapFrom(src => src.MONTH.MONTH_NAME_ENG))
                .ForMember(dest => dest.LACK1_TO_MONTH_ID, opt => opt.MapFrom(src => src.LACK1_TO_MONTH))
                .ForMember(dest => dest.LACK1_TO_MONTH_NAME, opt => opt.MapFrom(src => src.MONTH1.MONTH_NAME_ENG))
                .ForMember(dest => dest.STATUS_NAME, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.STATUS)))
                .ForMember(dest => dest.STATUS_GOV_NAME, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.STATUS_GOV)))
                .ForMember(dest => dest.CREATED_USERNAME, opt => opt.MapFrom(src => src.USER != null ? src.USER.USERNAME : string.Empty))
                .ForMember(dest => dest.APPROVED_USERNAME, opt => opt.MapFrom(src => src.ZAIDM_EX_POA != null && src.ZAIDM_EX_POA.USER != null ? src.ZAIDM_EX_POA.USER.USERNAME : string.Empty))
                .ForMember(dest => dest.LATEST_SALDO_UOM_NAME, opt => opt.MapFrom(src => src.UOM1 != null ? src.UOM1.UOM_NAME : string.Empty))
                .ForMember(dest => dest.SUPPLIER_PORT_NAME, opt => opt.MapFrom(src => src.SUPPLIER_PORT != null ? src.SUPPLIER_PORT.PORT_NAME : string.Empty))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC != null ? src.ZAIDM_EX_NPPBKC.T1001.BUKRSTXT : string.Empty))
            ;

            Mapper.CreateMap<PBCK1FilterViewModel, PBCK1Input>().IgnoreAllNonExisting()
                .ForMember(dest => dest.POA, opt => opt.ResolveUsing<StringToNullableIntegerResolver>().FromMember(src => src.POA));

            //Virtual Mapping Plant
            Mapper.CreateMap<VIRTUAL_PLANT_MAP, VirtualMappingPlantDetail>().IgnoreAllNonExisting()
                .ForMember(dest => dest.VirtualPlantMapId, opt => opt.MapFrom(src => src.VIRTUAL_PLANT_MAP_ID))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.COMPANY_ID))
                .ForMember(dest => dest.ImportPlantId, opt => opt.MapFrom(src => src.IMPORT_PLANT_ID))
                .ForMember(dest => dest.ExportPlantId, opt => opt.MapFrom(src => src.EXPORT_PLANT_ID));

            Mapper.CreateMap<SaveVirtualMappingPlantOutput, VirtualMappingPlantDetail>().IgnoreAllNonExisting()
               .ForMember(dest => dest.VirtualPlantMapId, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company))
               .ForMember(dest => dest.ImportPlantId, opt => opt.MapFrom(src => src.ImportVitualPlant))
               .ForMember(dest => dest.ExportPlantId, opt => opt.MapFrom(src => src.ExportVirtualPlant));

            Mapper.CreateMap<ZAIDM_EX_POA, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.POA_ID))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.POA_CODE));

            Mapper.CreateMap<ZAIDM_EX_NPPBKC, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.NPPBKC_NO));

            Mapper.CreateMap<USER, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.USER_ID))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => (src.USER_ID + "-" + src.USERNAME)));

            Mapper.CreateMap<CK5, CK5Item>().IgnoreAllNonExisting()
                .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.SUBMISSION_NUMBER))
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.GRAND_TOTAL_EX)) //todo ask
                .ForMember(dest => dest.UOM, opt => opt.MapFrom(src => src.UOM.UOM_NAME))
                .ForMember(dest => dest.POA, opt => opt.MapFrom(src => src.CK5_TYPE));
            //.ForMember(dest => dest.POA, opt => opt.MapFrom(src => src.po)) //todo ask
            //.ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.T1001W.ZAIDM_EX_NPPBKC.NPPBKC_NO))//todo ask
            // .ForMember(dest => dest.SourcePlant, opt => opt.MapFrom(src => src.T1001W.CITY)) //todo ask
            //.ForMember(dest => dest.SourcePlant, opt => opt.MapFrom(src => src.)) //todo ask
            // .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS.STATUS_NAME));

            Mapper.CreateMap<CK5SearchViewModel, CK5Input>().IgnoreAllNonExisting();
            Mapper.CreateMap<SUPPLIER_PORT, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.SUPPLIER_PORT_ID + "-" + src.PORT_NAME))
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.SUPPLIER_PORT_ID))
                ;

            Mapper.CreateMap<T1001W, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.PLANT_ID + "-" + src.NAME1))
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.PLANT_ID));
            Mapper.CreateMap<ZAIDM_EX_GOODTYP, SelectItemModel>().IgnoreAllNonExisting()
               .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.EXC_GOOD_TYP + "-" + src.EXT_TYP_DESC))
               .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.GOODTYPE_ID));


            #region NPPBKC

            Mapper.CreateMap<ZAIDM_EX_NPPBKC, DetailNppbck>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NppbckId, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.Addr1, opt => opt.MapFrom(src => src.ADDR1))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.CITY))
                .ForMember(dest => dest.RegionOfficeIdNppbkc, opt => opt.MapFrom(src => src.REGION_OFFICE))
                .ForMember(dest => dest.TextTo, opt => opt.MapFrom(src => src.TEXT_TO))
                ;

            #endregion
            
            #region GoodsTypeGroup

            //Mapper.CreateMap<ZAIDM_EX_GOODTYP, DetailsGoodsTypGroup>().IgnoreAllNonExisting()
            //    .ForMember(dest => dest.GoodsTypeId, opt => opt.MapFrom(src => src.GOODTYPE_ID))
            //    .ForMember(dest => dest.ExcisableGoodsType, opt => opt.MapFrom(src => src.EXC_GOOD_TYP))
            //    .ForMember(dest => dest.ExtTypDescending, opt => opt.MapFrom(src => src.EXT_TYP_DESC))
            //    .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CREATED_DATE));

            #endregion

            #region Plant

            Mapper.CreateMap<T1001W, DetailPlantT1001W>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.Werks, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.PlantDescription, opt => opt.MapFrom(src => src.NAME1))
                .ForMember(dest => dest.IsMainPlant, opt => opt.MapFrom(src => src.IS_MAIN_PLANT))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.ADDRESS))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.CITY))
                .ForMember(dest => dest.NPPBKC_NO, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC != null ? src.ZAIDM_EX_NPPBKC.NPPBKC_NO : string.Empty))
                .ForMember(dest => dest.KPPBC_NO, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC != null && src.ZAIDM_EX_NPPBKC.ZAIDM_EX_KPPBC != null ? src.ZAIDM_EX_NPPBKC.ZAIDM_EX_KPPBC.KPPBC_NUMBER : string.Empty))
                ;

            #endregion

            #region BrandRegistration

            Mapper.CreateMap<BrandRegistrationOutput, DetailBrandRegistration>().IgnoreAllNonExisting();


            #endregion

            #region POA

            Mapper.CreateMap<POAViewModel, ZAIDM_EX_POA>().IgnoreAllNonExisting()
            .ForMember(dest => dest.POA_ID_CARD, opt => opt.MapFrom(src => src.PoaIdCard))
                .ForMember(dest => dest.POA_CODE, opt => opt.MapFrom(src => src.PoaCode))
                .ForMember(dest => dest.POA_PRINTED_NAME, opt => opt.MapFrom(src => src.PoaPrintedName))
                .ForMember(dest => dest.POA_PHONE, opt => opt.MapFrom(src => src.PoaPhone))
                .ForMember(dest => dest.POA_ADDRESS, opt => opt.MapFrom(src => src.PoaAddress))
                .ForMember(dest => dest.TITLE, opt => opt.MapFrom(src => src.Title));
            

            Mapper.CreateMap<ZAIDM_EX_POA, POAViewDetailModel>().IgnoreAllNonExisting()
               .ForMember(dest => dest.PoaIdCard, opt => opt.MapFrom(src => src.POA_ID_CARD))
               .ForMember(dest => dest.PoaId, opt => opt.MapFrom(src => src.POA_ID))
               .ForMember(dest => dest.PoaCode, opt => opt.MapFrom(src => src.POA_CODE))
               .ForMember(dest => dest.PoaPrintedName, opt => opt.MapFrom(src => src.POA_PRINTED_NAME))
               .ForMember(dest => dest.PoaPhone, opt => opt.MapFrom(src => src.POA_PHONE))
               .ForMember(dest => dest.PoaAddress, opt => opt.MapFrom(src => src.POA_ADDRESS))
               .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.TITLE))
               .ForMember(dest => dest.isNewData, opt => opt.MapFrom(src => src.IS_FROM_SAP));

            Mapper.CreateMap<POAViewDetailModel, ZAIDM_EX_POA>().IgnoreAllUnmapped()
               .ForMember(dest => dest.POA_ID_CARD, opt => opt.MapFrom(src => src.PoaIdCard))
               .ForMember(dest => dest.POA_CODE, opt => opt.MapFrom(src => src.PoaCode))
               .ForMember(dest => dest.POA_PRINTED_NAME, opt => opt.MapFrom(src => src.PoaPrintedName))
               .ForMember(dest => dest.POA_PHONE, opt => opt.MapFrom(src => src.PoaPhone))
               .ForMember(dest => dest.POA_ADDRESS, opt => opt.MapFrom(src => src.PoaAddress))
               .ForMember(dest => dest.TITLE, opt => opt.MapFrom(src => src.Title))
               .ForMember(dest => dest.USER_ID, opt => opt.MapFrom(src => src.User.USER_ID));

           #endregion

            #region PlantDetail

            Mapper.CreateMap<T1001W, DetailPlantT1001W>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PlantDescription, opt => opt.MapFrom(src => src.NAME1 + "-" + src.CITY))
                .ForMember(dest => dest.IsMainPlant, opt => opt.MapFrom(src => src.IS_MAIN_PLANT))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.ADDRESS ))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.CITY))
                .ForMember(dest => dest.Skeptis, opt => opt.MapFrom(src => src.SKEPTIS))
                .ForMember(dest => dest.RecievedMaterialTypeId, opt => opt.MapFrom(src => src.RECEIVED_MATERIAL_TYPE_ID));

            Mapper.CreateMap<DetailPlantT1001W, T1001W>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NAME1, opt => opt.MapFrom(src => src.PlantDescription + "-" + src.City))
                .ForMember(dest => dest.IS_MAIN_PLANT, opt => opt.MapFrom(src => src.IsMainPlant))
                .ForMember(dest => dest.ADDRESS, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.CITY, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.SKEPTIS, opt => opt.MapFrom(src => src.Skeptis))
                .ForMember(dest => dest.RECEIVED_MATERIAL_TYPE_ID, opt => opt.MapFrom(src => src.GOODTYP.EXT_TYP_DESC))
                .ForMember(dest => dest.NPPBCK_ID, opt => opt.MapFrom(src => src.NPPBKC.NPPBKC_ID));



            #endregion


            Mapper.CreateMap<ZAIDM_EX_GOODTYP, GoodsTypeDetails>().IgnoreAllNonExisting()
               .ForMember(dest => dest.GoodTypeId, opt => opt.MapFrom(src => src.GOODTYPE_ID))
               .ForMember(dest => dest.GoodTypeName, opt => opt.MapFrom(src => src.EXT_TYP_DESC));

            Mapper.CreateMap<EX_GROUP_TYPE, DetailsGoodsTypGroup>().IgnoreAllNonExisting()
             .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GROUP_NAME))
             .ForMember(dest => dest.GroupTypeName, opt => opt.MapFrom(src => src.GROUP_NAME));

            Mapper.CreateMap<EX_GROUP_TYPE, GoodsTypeDetails>().IgnoreAllNonExisting()
              .ForMember(dest => dest.GoodTypeId, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.GOODTYPE_ID))
              .ForMember(dest => dest.GoodTypeName, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC));

        }
    }
}