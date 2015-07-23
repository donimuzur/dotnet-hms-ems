using System;
using System.Collections.Generic;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.BrandRegistration;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.CK5;
using Sampoerna.EMS.Website.Models.GOODSTYPE;
using Sampoerna.EMS.Website.Models.HeaderFooter;
using Sampoerna.EMS.Website.Models.NPPBKC;
using Sampoerna.EMS.Website.Models.PBCK1;
using Sampoerna.EMS.Website.Models.PLANT;
using Sampoerna.EMS.Website.Models.PlantReceiveMaterial;
using Sampoerna.EMS.Website.Models.POA;
using Sampoerna.EMS.Website.Models.UOM;
using Sampoerna.EMS.Website.Models.VirtualMappingPlant;
using Sampoerna.EMS.Website.Models.Material;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Models.Settings;

namespace Sampoerna.EMS.Website
{
    public partial class EMSWebsiteMapper
    {
        public static void Initialize()
        {
            InitializeCK5();

            //AutoMapper
            Mapper.CreateMap<USER, Login>().IgnoreAllNonExisting()
                .ForMember(dest => dest.USERNAME, opt => opt.MapFrom(src => src.USERNAME))
                .ForMember(dest => dest.FIRST_NAME, opt => opt.MapFrom(src => src.FIRST_NAME));

            ;
            Mapper.CreateMap<USER, UserItem>().IgnoreAllNonExisting()
               .ForMember(dest => dest.USERNAME, opt => opt.MapFrom(src => src.USERNAME))
               .ForMember(dest => dest.FIRST_NAME, opt => opt.MapFrom(src => src.FIRST_NAME));

            Mapper.CreateMap<UserTree, UserItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IS_ACTIVE.HasValue && src.IS_ACTIVE.Value))
                ;

            #region Company
            Mapper.CreateMap<T001, CompanyDetail>().IgnoreAllNonExisting()
            .ForMember(dest => dest.DocumentBukrs, opt => opt.MapFrom(src => src.BUKRS))
            .ForMember(dest => dest.DocumentBukrstxt, opt => opt.MapFrom(src => src.BUTXT))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE));
            #endregion

            #region Pbck1

            Mapper.CreateMap<Pbck1Dto, Pbck1Item>().IgnoreAllNonExisting()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
                .ForMember(dest => dest.StatusGovName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.StatusGov)))
                .ForMember(dest => dest.PbckTypeName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Pbck1Type)))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.PeriodFrom.Year))
                .ForMember(dest => dest.Pbck1ReferenceNumber, opt => opt.MapFrom(src => src.Pbck1Reference != null && src.Pbck1Parent != null ? src.Pbck1Parent.Pbck1Number : string.Empty))
                .ForMember(dest => dest.Pbck1ProdConverter,
                    opt => opt.MapFrom(src => Mapper.Map<List<Pbck1ProdConvModel>>(src.Pbck1ProdConverter)))
                .ForMember(dest => dest.Pbck1ProdPlan,
                    opt => opt.MapFrom(src => Mapper.Map<List<Pbck1ProdPlanModel>>(src.Pbck1ProdPlan)))
             ;

            Mapper.CreateMap<Pbck1FilterViewModel, Pbck1GetOpenDocumentByParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Poa, opt => opt.ResolveUsing<StringToNullableIntegerResolver>().FromMember(src => src.Poa))
                .ForMember(dest => dest.Creator, opt => opt.ResolveUsing<StringToNullableIntegerResolver>().FromMember(src => src.Creator))
                ;

            Mapper.CreateMap<Pbck1FilterViewModel, Pbck1GetCompletedDocumentByParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Poa, opt => opt.ResolveUsing<StringToNullableIntegerResolver>().FromMember(src => src.Poa))
                .ForMember(dest => dest.Creator, opt => opt.ResolveUsing<StringToNullableIntegerResolver>().FromMember(src => src.Creator))
                ;

            //Mapper.CreateMap<Pbck1FilterViewModel, Pbck1GetByDocumentStatusParam>().IgnoreAllNonExisting()
            //    .ForMember(dest => dest.Poa, opt => opt.ResolveUsing<StringToNullableIntegerResolver>().FromMember(src => src.Poa))
            //    .ForMember(dest => dest.Creator, opt => opt.ResolveUsing<StringToNullableIntegerResolver>().FromMember(src => src.Creator))
            //    ;

            Mapper.CreateMap<Pbck1ProdConvModel, Pbck1ProdConverterDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProdTypeCode, opt => opt.MapFrom(src => src.ProductCode))
                .ForMember(dest => dest.ProdTypeName, opt => opt.MapFrom(src => src.ProdTypeName))
                .ForMember(dest => dest.ProdAlias, opt => opt.MapFrom(src => src.ProdTypeAlias))
                .ForMember(dest => dest.ConverterOutput, opt => opt.MapFrom(src => src.ConverterOutput))
                .ForMember(dest => dest.ConverterOutputUomId, opt => opt.MapFrom(src => src.ConverterUomId))
                ;
            Mapper.CreateMap<Pbck1ProdConverterDto, Pbck1ProdConvModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.ProdTypeCode))
                .ForMember(dest => dest.ProdTypeName, opt => opt.MapFrom(src => src.ProdTypeName))
                .ForMember(dest => dest.ProdTypeAlias, opt => opt.MapFrom(src => src.ProdAlias))
                .ForMember(dest => dest.ConverterOutput, opt => opt.MapFrom(src => src.ConverterOutput))
                .ForMember(dest => dest.ConverterUomId, opt => opt.MapFrom(src => src.ConverterOutputUomId))
                .ForMember(dest => dest.ConverterUom, opt => opt.MapFrom(src => src.ConverterOutputUomName))
                ;

            Mapper.CreateMap<Pbck1ProdPlanModel, Pbck1ProdPlanDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MonthId, opt => opt.ResolveUsing<StringToNullableIntegerResolver>().FromMember(src => src.Month))
                .ForMember(dest => dest.ProdTypeCode, opt => opt.MapFrom(src => src.ProductCode))
                .ForMember(dest => dest.ProdTypeName, opt => opt.MapFrom(src => src.ProdTypeName))
                .ForMember(dest => dest.ProdAlias, opt => opt.MapFrom(src => src.ProdTypeAlias))
                .ForMember(dest => dest.Amount, opt => opt.ResolveUsing<StringToNullableDecimalResolver>().FromMember(src => src.Amount))
                .ForMember(dest => dest.BkcRequired, opt => opt.MapFrom(src => src.BkcRequired))
                ;

            Mapper.CreateMap<Pbck1ProdPlanDto, Pbck1ProdPlanModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.ProdTypeCode))
                .ForMember(dest => dest.Month, opt => opt.MapFrom(src => src.MonthId))
                .ForMember(dest => dest.ProdTypeAlias, opt => opt.MapFrom(src => src.ProdAlias))
                ;

            Mapper.CreateMap<Pbck1Item, Pbck1Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck1ProdConverter,
                    opt => opt.MapFrom(src => Mapper.Map<List<Pbck1ProdConverterDto>>(src.Pbck1ProdConverter)))
                .ForMember(dest => dest.Pbck1ProdPlan,
                    opt => opt.MapFrom(src => Mapper.Map<List<Pbck1ProdPlanModel>>(src.Pbck1ProdPlan)))
                    ;

            #endregion

            Mapper.CreateMap<BrandRegistrationOutput, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.BrandIdZaidmExBrand))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.BrandCe));

            Mapper.CreateMap<ZAIDM_EX_MATERIAL, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.STICKER_CODE))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.STICKER_CODE + " - " + src.MATERIAL_DESC));


            Mapper.CreateMap<POA, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.POA_ID))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.PRINTED_NAME));

            Mapper.CreateMap<ZAIDM_EX_NPPBKC, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.NPPBKC_ID));

            Mapper.CreateMap<USER, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.USER_ID))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => (src.FIRST_NAME + ' ' + src.LAST_NAME)));


            Mapper.CreateMap<T001W, SelectItemModel>().IgnoreAllNonExisting()
            .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.WERKS))
            .ForMember(dest => dest.TextField, opt => opt.ResolveUsing<SourcePlantTextResolver>().FromMember(src => src));
         

            Mapper.CreateMap<Plant, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.WERKS + "-" + src.NAME1))
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.WERKS));
            Mapper.CreateMap<ZAIDM_EX_GOODTYP, SelectItemModel>().IgnoreAllNonExisting()
               .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.EXC_GOOD_TYP + "-" + src.EXT_TYP_DESC))
               .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.EXC_GOOD_TYP));


            #region NPPBKC

            //Mapper.CreateMap<ZAIDM_EX_NPPBKC, DetailsNppbck>().IgnoreAllNonExisting()
            //    .ForMember(dest => dest.NppbckId, opt => opt.MapFrom(src => src.NPPBKC_ID))
            //    .ForMember(dest => dest.Address1, opt => opt.MapFrom(src => src.ADDR1))
            //    .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.CITY))
            //    .ForMember(dest => dest.RegionOfficeIdNppbkc, opt => opt.MapFrom(src => src.REGION_OFFICE))
            //    .ForMember(dest => dest.TextTo, opt => opt.MapFrom(src => src.TEXT_TO))
            //    .ForMember(dest => dest.NppbckNo, opt => opt.MapFrom(src => src.NPPBKC_NO));

            #endregion

            #region GoodsTypeGroup

            //Mapper.CreateMap<ZAIDM_EX_GOODTYP, DetailsGoodsTypGroup>().IgnoreAllNonExisting()
            //    .ForMember(dest => dest.GoodsTypeId, opt => opt.MapFrom(src => src.GOODTYPE_ID))
            //    .ForMember(dest => dest.ExcisableGoodsType, opt => opt.MapFrom(src => src.EXC_GOOD_TYP))
            //    .ForMember(dest => dest.ExtTypDescending, opt => opt.MapFrom(src => src.EXT_TYP_DESC))
            //    .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CREATED_DATE));

            #endregion

            #region Plant

            Mapper.CreateMap<PLANT_RECEIVE_MATERIAL, PlantReceiveMaterialItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.EXC_GOOD_TYP,
                    opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP != null ? src.ZAIDM_EX_GOODTYP.EXC_GOOD_TYP : null))
                    .ForMember(dest => dest.EXT_TYP_DESC, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP != null ? src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC : string.Empty))
                    ;

            Mapper.CreateMap<PlantReceiveMaterialItemModel, PLANT_RECEIVE_MATERIAL>().IgnoreAllNonExisting();

            Mapper.CreateMap<Plant, DetailPlantT1001W>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.Werks, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.PlantDescription, opt => opt.MapFrom(src => src.NAME1))
                .ForMember(dest => dest.IsMainPlant, opt => opt.MapFrom(src => src.IS_MAIN_PLANT.HasValue && src.IS_MAIN_PLANT.Value))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.ADDRESS))
                .ForMember(dest => dest.Name1, opt => opt.MapFrom(src => src.NAME1))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PHONE))
                .ForMember(dest => dest.Ort01, opt => opt.MapFrom(src => src.ORT01))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ReceiveMaterials, opt => opt.MapFrom(src => Mapper.Map<List<PlantReceiveMaterialItemModel>>(src.PLANT_RECEIVE_MATERIAL)))
                ;

            Mapper.CreateMap<DetailPlantT1001W, Plant>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.Werks))
                .ForMember(dest => dest.NAME1, opt => opt.MapFrom(src => src.Name1))
                .ForMember(dest => dest.IS_MAIN_PLANT, opt => opt.MapFrom(src => src.IsMainPlant))
                .ForMember(dest => dest.ADDRESS, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.SKEPTIS, opt => opt.MapFrom(src => src.Skeptis))
                .ForMember(dest => dest.PHONE, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.ORT01, opt => opt.MapFrom(src => src.Ort01))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.PLANT_RECEIVE_MATERIAL,
                    opt => opt.MapFrom(src => Mapper.Map<List<PLANT_RECEIVE_MATERIAL>>(src.ReceiveMaterials)));

            #endregion

            #region POA



            Mapper.CreateMap<POA, POAViewDetailModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PoaIdCard, opt => opt.MapFrom(src => src.ID_CARD))
                .ForMember(dest => dest.PoaId, opt => opt.MapFrom(src => src.POA_ID))
                .ForMember(dest => dest.PoaPrintedName, opt => opt.MapFrom(src => src.PRINTED_NAME))
                .ForMember(dest => dest.PoaPhone, opt => opt.MapFrom(src => src.POA_PHONE))
                .ForMember(dest => dest.PoaAddress, opt => opt.MapFrom(src => src.POA_ADDRESS))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.POA_EMAIL))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.TITLE))
                .ForMember(dest => dest.Is_Active, opt => opt.MapFrom(src => src.IS_ACTIVE == true ? "Yes" : "No"))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.LOGIN_AS))
                .ForMember(dest => dest.ManagerId, opt => opt.MapFrom(src => src.MANAGER_ID))
                .ForMember(dest => dest.PoaSk, opt => opt.MapFrom(src => src.POA_SK));

            Mapper.CreateMap<POAViewDetailModel, POA>().IgnoreAllUnmapped()
                .ForMember(dest => dest.ID_CARD, opt => opt.MapFrom(src => src.PoaIdCard))
                .ForMember(dest => dest.PRINTED_NAME, opt => opt.MapFrom(src => src.PoaPrintedName))
                .ForMember(dest => dest.POA_PHONE, opt => opt.MapFrom(src => src.PoaPhone))
                .ForMember(dest => dest.POA_ADDRESS, opt => opt.MapFrom(src => src.PoaAddress))
                .ForMember(dest => dest.POA_EMAIL, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.TITLE, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.MANAGER_ID, opt => opt.MapFrom(src => src.ManagerId))
                .ForMember(dest => dest.LOGIN_AS, opt => opt.MapFrom(src => src.UserId));


            #endregion

            Mapper.CreateMap<HeaderFooter, HeaderFooterItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.HEADER_IMAGE_PATH_BEFOREEDIT, opt => opt.MapFrom(src => src.HEADER_IMAGE_PATH))
                .ForMember(dest => dest.FOOTER_CONTENT,
                    opt => opt.MapFrom(src => src.FOOTER_CONTENT.Replace("<br />", Environment.NewLine)))
                    .ForMember(dest => dest.IS_DELETED, opt => opt.MapFrom(src => src.IS_DELETED.HasValue && src.IS_DELETED.Value))
                    .ForMember(dest => dest.IsDeletedDesc, opt => opt.MapFrom(src => src.IS_DELETED.HasValue && src.IS_DELETED.Value ? "Yes" : "No"))
                    ;

            Mapper.CreateMap<HeaderFooterMap, HeaderFooterMapItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.IS_HEADER_SET, opt => opt.MapFrom(src => src.IS_HEADER_SET.HasValue && src.IS_HEADER_SET.Value))
                .ForMember(dest => dest.IS_FOOTER_SET, opt => opt.MapFrom(src => src.IS_FOOTER_SET.HasValue && src.IS_FOOTER_SET.Value))
                .ForMember(dest => dest.FORM_TYPE_DESC, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.FORM_TYPE_ID)))
                ;

            Mapper.CreateMap<HeaderFooterDetails, HeaderFooterDetailItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.HEADER_IMAGE_PATH_BEFOREEDIT, opt => opt.MapFrom(src => src.HEADER_IMAGE_PATH))
                .ForMember(dest => dest.FOOTER_CONTENT, opt => opt.MapFrom(src => src.FOOTER_CONTENT.Replace("<br />", Environment.NewLine)))
                .ForMember(dest => dest.HeaderFooterMapList,
                    opt => opt.MapFrom(src => Mapper.Map<List<HeaderFooterMapItem>>(src.HeaderFooterMapList)))
                    .ForMember(dest => dest.IS_DELETED, opt => opt.MapFrom(src => src.IS_DELETED.HasValue && src.IS_DELETED.Value))
                    .ForMember(dest => dest.IsDeletedDesc, opt => opt.MapFrom(src => src.IS_DELETED.HasValue && src.IS_DELETED.Value ? "Yes" : "No"));

            Mapper.CreateMap<HeaderFooterMapItem, HeaderFooterMap>().IgnoreAllNonExisting();

            Mapper.CreateMap<HeaderFooterDetailItem, HeaderFooterDetails>().IgnoreAllNonExisting()
                .ForMember(dest => dest.HeaderFooterMapList,
                    opt => opt.MapFrom(src => Mapper.Map<List<HeaderFooterMap>>(src.HeaderFooterMapList)))
                    .ForMember(dest => dest.IS_DELETED, opt => opt.MapFrom(src => src.IS_DELETED));





            Mapper.CreateMap<ZAIDM_EX_GOODTYP, GoodsTypeDetails>().IgnoreAllNonExisting()
                .ForMember(dest => dest.GoodTypeId, opt => opt.MapFrom(src => src.EXC_GOOD_TYP))
               .ForMember(dest => dest.GoodTypeName, opt => opt.MapFrom(src => src.EXT_TYP_DESC));
            ;


            Mapper.CreateMap<EX_GROUP_TYPE, DetailsGoodsTypGroup>().IgnoreAllNonExisting()
                .ForMember(dest => dest.GoodsTypeId, opt => opt.MapFrom(src => src.EX_GROUP_TYPE_ID))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GROUP_NAME));



            #region VirtualMappingPlant
            //Virtual Mapping Plant
            Mapper.CreateMap<VIRTUAL_PLANT_MAP, VirtualMappingPlantDetail>().IgnoreAllNonExisting()
                .ForMember(dest => dest.VirtualPlantMapId, opt => opt.MapFrom(src => src.VIRTUAL_PLANT_MAP_ID))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.COMPANY_ID))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.T001.BUTXT))
                .ForMember(dest => dest.ImportPlantId, opt => opt.MapFrom(src => src.IMPORT_PLANT_ID))
                .ForMember(dest => dest.ImportPlanName, opt => opt.MapFrom(src => src.T001W1.WERKS))
                .ForMember(dest => dest.ExportPlantId, opt => opt.MapFrom(src => src.EXPORT_PLANT_ID))
                .ForMember(dest => dest.ExportPlanName, opt => opt.MapFrom(src => src.T001W.WERKS))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IS_DELETED));

            Mapper.CreateMap<Plant, SelectItemModelVirtualPlant>().IgnoreAllNonExisting()
              .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.WERKS + "-" + src.NAME1))
              .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.WERKS));

            Mapper.CreateMap<VirtualMappingPlantCreateViewModel, VIRTUAL_PLANT_MAP>().IgnoreAllNonExisting()
                .ForMember(dest => dest.COMPANY_ID, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.IMPORT_PLANT_ID, opt => opt.MapFrom(src => src.ImportPlantId))
                .ForMember(dest => dest.EXPORT_PLANT_ID, opt => opt.MapFrom(src => src.ExportPlantId));

            #endregion

            #region BrandRegistration

            Mapper.CreateMap<ZAIDM_EX_BRAND, BrandRegistrationDetail>().IgnoreAllNonExisting()
                .ForMember(dest => dest.StickerCode, opt => opt.MapFrom(src => src.STICKER_CODE))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.T001W.WERKS))
                .ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FA_CODE))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.BRAND_CE))
                .ForMember(dest => dest.SeriesValue, opt => opt.MapFrom(src => src.ZAIDM_EX_SERIES.SERIES_VALUE))
                .ForMember(dest => dest.Conversion, opt => opt.MapFrom(src => src.CONVERSION))
                .ForMember(dest => dest.PrintingPrice, opt => opt.MapFrom(src => src.PRINTING_PRICE))
                .ForMember(dest => dest.CutFilterCode, opt => opt.MapFrom(src => src.CUT_FILLER_CODE))
                .ForMember(dest => dest.IsDeleted, opt => opt.ResolveUsing<NullableBooleanToStringDeletedResolver>().FromMember(src => src.IS_DELETED));


            Mapper.CreateMap<ZAIDM_EX_BRAND, BrandRegistrationDetailsViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.StickerCode, opt => opt.MapFrom(src => src.STICKER_CODE))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.T001W.WERKS))
                .ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FA_CODE))
                .ForMember(dest => dest.PersonalizationCode, opt => opt.MapFrom(src => src.ZAIDM_EX_PCODE.PER_CODE))
                .ForMember(dest => dest.PersonalizationCodeDescription,
                    opt => opt.MapFrom(src => src.ZAIDM_EX_PCODE.PER_DESC))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.BRAND_CE))
                .ForMember(dest => dest.SkepNo, opt => opt.MapFrom(src => src.SKEP_NO))
                .ForMember(dest => dest.SkepDate, opt => opt.MapFrom(src => src.SKEP_DATE))
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.ZAIDM_EX_PRODTYP.PROD_CODE))
                .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => src.ZAIDM_EX_PRODTYP.PRODUCT_TYPE))
                .ForMember(dest => dest.ProductAlias, opt => opt.MapFrom(src => src.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS))
                .ForMember(dest => dest.SeriesCode, opt => opt.MapFrom(src => src.ZAIDM_EX_SERIES.SERIES_CODE))
                .ForMember(dest => dest.SeriesValue, opt => opt.MapFrom(src => src.ZAIDM_EX_SERIES.SERIES_VALUE))
                .ForMember(dest => dest.MarketCode, opt => opt.MapFrom(src => src.ZAIDM_EX_MARKET.MARKET_ID))
                .ForMember(dest => dest.MarketDescription, opt => opt.MapFrom(src => src.ZAIDM_EX_MARKET.MARKET_DESC))
                .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.COUNTRY))
                .ForMember(dest => dest.HjeValue, opt => opt.MapFrom(src => src.HJE_IDR))
                .ForMember(dest => dest.HjeCurrency, opt => opt.MapFrom(src => src.HJE_CURR))
                .ForMember(dest => dest.Tariff, opt => opt.MapFrom(src => src.TARIFF))
                .ForMember(dest => dest.TariffCurrency, opt => opt.MapFrom(src => src.TARIF_CURR))
                //todo check which one correct
                .ForMember(dest => dest.ColourName, opt => opt.MapFrom(src => src.COLOUR))
                .ForMember(dest => dest.GoodType, opt => opt.MapFrom(src => src.EXC_GOOD_TYP))
                .ForMember(dest => dest.GoodTypeDescription,
                    opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.START_DATE))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.END_DATE))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS == true ? "Active" : "Inactive"))
                .ForMember(dest => dest.PrintingPrice, opt => opt.MapFrom(src => src.PRINTING_PRICE))
                .ForMember(dest => dest.CutFilterCode, opt => opt.MapFrom(src => src.CUT_FILLER_CODE))
                .ForMember(dest => dest.IsDeleted, opt => opt.ResolveUsing<NullableBooleanToStringDeletedResolver>().FromMember(src => src.IS_DELETED))
                .ForMember(dest => dest.Conversion, opt => opt.MapFrom(src => src.CONVERSION))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.BRAND_CONTENT));


            Mapper.CreateMap<ZAIDM_EX_BRAND, BrandRegistrationEditViewModel>().IgnoreAllNonExisting()
            .ForMember(dest => dest.StickerCode, opt => opt.MapFrom(src => src.STICKER_CODE))
            .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.WERKS))
            .ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FA_CODE))
            .ForMember(dest => dest.PersonalizationCode, opt => opt.MapFrom(src => src.PER_CODE))
            .ForMember(dest => dest.PersonalizationCodeDescription, opt => opt.MapFrom(src => src.ZAIDM_EX_PCODE.PER_DESC))
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.BRAND_CE))
            .ForMember(dest => dest.SkepNo, opt => opt.MapFrom(src => src.SKEP_NO))
            .ForMember(dest => dest.SkepDate, opt => opt.MapFrom(src => src.SKEP_DATE))
            .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.PROD_CODE))
            .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => src.ZAIDM_EX_PRODTYP.PRODUCT_TYPE))
            .ForMember(dest => dest.ProductAlias, opt => opt.MapFrom(src => src.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS))
            .ForMember(dest => dest.SeriesId, opt => opt.MapFrom(src => src.SERIES_CODE))
            .ForMember(dest => dest.SeriesValue, opt => opt.MapFrom(src => src.ZAIDM_EX_SERIES.SERIES_VALUE))
            .ForMember(dest => dest.MarketId, opt => opt.MapFrom(src => src.MARKET_ID))
            .ForMember(dest => dest.MarketDescription, opt => opt.MapFrom(src => src.ZAIDM_EX_MARKET.MARKET_DESC))
            .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.COUNTRY))
            .ForMember(dest => dest.HjeValue, opt => opt.MapFrom(src => src.HJE_IDR))
            .ForMember(dest => dest.HjeCurrency, opt => opt.MapFrom(src => src.HJE_CURR)) //todo check which one correct
            .ForMember(dest => dest.Tariff, opt => opt.MapFrom(src => src.TARIFF))
            .ForMember(dest => dest.TariffCurrency, opt => opt.MapFrom(src => src.TARIF_CURR)) //todo check which one correct
            .ForMember(dest => dest.ColourName, opt => opt.MapFrom(src => src.COLOUR))
            .ForMember(dest => dest.GoodType, opt => opt.MapFrom(src => src.EXC_GOOD_TYP))
            .ForMember(dest => dest.GoodTypeDescription, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.START_DATE))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.END_DATE))
            .ForMember(dest => dest.PrintingPrice, opt => opt.MapFrom(src => src.PRINTING_PRICE))
            .ForMember(dest => dest.CutFilterCode, opt => opt.MapFrom(src => src.CUT_FILLER_CODE))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS == true ? "Active" : "Inactive"))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.STATUS))
            .ForMember(dest => dest.IsFromSAP, opt => opt.MapFrom(src => src.IS_FROM_SAP))
            .ForMember(dest => dest.Conversion, opt => opt.MapFrom(src => src.CONVERSION))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.BRAND_CONTENT));

            Mapper.CreateMap<BrandRegistrationCreateViewModel, ZAIDM_EX_BRAND>().IgnoreAllNonExisting()
                .ForMember(dest => dest.STICKER_CODE, opt => opt.MapFrom(src => src.StickerCode))
                .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.FA_CODE, opt => opt.MapFrom(src => src.FaCode))
                .ForMember(dest => dest.PER_CODE, opt => opt.MapFrom(src => src.PersonalizationCode))
                .ForMember(dest => dest.BRAND_CE, opt => opt.MapFrom(src => src.BrandName))
                .ForMember(dest => dest.SKEP_NO, opt => opt.MapFrom(src => src.SkepNo))
                .ForMember(dest => dest.SKEP_DATE, opt => opt.MapFrom(src => src.SkepDate))
                .ForMember(dest => dest.PROD_CODE, opt => opt.MapFrom(src => src.ProductCode))
                .ForMember(dest => dest.SERIES_CODE, opt => opt.MapFrom(src => src.SeriesId))
                .ForMember(dest => dest.MARKET_ID, opt => opt.MapFrom(src => src.MarketId))
                .ForMember(dest => dest.COUNTRY, opt => opt.MapFrom(src => src.CountryId))
                .ForMember(dest => dest.HJE_IDR, opt => opt.MapFrom(src => src.HjeValue))
                .ForMember(dest => dest.HJE_CURR, opt => opt.MapFrom(src => src.HjeCurrency))
                .ForMember(dest => dest.TARIFF, opt => opt.MapFrom(src => src.Tariff))
                .ForMember(dest => dest.TARIF_CURR, opt => opt.MapFrom(src => src.TariffCurrency))
                .ForMember(dest => dest.COLOUR, opt => opt.MapFrom(src => src.ColourName))
                .ForMember(dest => dest.EXC_GOOD_TYP, opt => opt.MapFrom(src => src.GoodType))
                .ForMember(dest => dest.START_DATE, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.END_DATE, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.STATUS, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.PRINTING_PRICE, opt => opt.MapFrom(src => src.PrintingPrice))
                .ForMember(dest => dest.CUT_FILLER_CODE, opt => opt.MapFrom(src => src.CutFilterCode))
                .ForMember(dest => dest.CONVERSION, opt => opt.MapFrom(src => src.Conversion))
                .ForMember(dest => dest.BRAND_CONTENT, opt => opt.MapFrom(src => src.Content));

            Mapper.CreateMap<BrandRegistrationEditViewModel, ZAIDM_EX_BRAND>().IgnoreAllUnmapped()
                .ForMember(dest => dest.STICKER_CODE, opt => opt.MapFrom(src => src.StickerCode))
                .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.FA_CODE, opt => opt.MapFrom(src => src.FaCode))
                .ForMember(dest => dest.PER_CODE, opt => opt.MapFrom(src => src.PersonalizationCode))
                .ForMember(dest => dest.BRAND_CE, opt => opt.MapFrom(src => src.BrandName))
                .ForMember(dest => dest.SKEP_NO, opt => opt.MapFrom(src => src.SkepNo))
                .ForMember(dest => dest.SKEP_DATE, opt => opt.MapFrom(src => src.SkepDate))
                .ForMember(dest => dest.PROD_CODE, opt => opt.MapFrom(src => src.ProductCode))
                .ForMember(dest => dest.SERIES_CODE, opt => opt.MapFrom(src => src.SeriesId))
                .ForMember(dest => dest.MARKET_ID, opt => opt.MapFrom(src => src.MarketId))
                .ForMember(dest => dest.COUNTRY, opt => opt.MapFrom(src => src.CountryId))
                .ForMember(dest => dest.HJE_IDR, opt => opt.MapFrom(src => src.HjeValue))
                .ForMember(dest => dest.HJE_CURR, opt => opt.MapFrom(src => src.HjeCurrency))
                .ForMember(dest => dest.TARIFF, opt => opt.MapFrom(src => src.Tariff))
                .ForMember(dest => dest.TARIF_CURR, opt => opt.MapFrom(src => src.TariffCurrency))
                .ForMember(dest => dest.COLOUR, opt => opt.MapFrom(src => src.ColourName))
                .ForMember(dest => dest.EXC_GOOD_TYP, opt => opt.MapFrom(src => src.GoodType))
                .ForMember(dest => dest.START_DATE, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.END_DATE, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.STATUS, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.PRINTING_PRICE, opt => opt.MapFrom(src => src.PrintingPrice))
                .ForMember(dest => dest.CUT_FILLER_CODE, opt => opt.MapFrom(src => src.CutFilterCode))
                .ForMember(dest => dest.CONVERSION, opt => opt.MapFrom(src => src.Conversion))
                .ForMember(dest => dest.BRAND_CONTENT, opt => opt.MapFrom(src => src.Content));
            #endregion

            Mapper.CreateMap<CHANGES_HISTORY, ChangesHistoryItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.USERNAME,
                    opt => opt.MapFrom(src => src.USER != null ? src.USER.USERNAME : string.Empty))
                .ForMember(dest => dest.USER_FIRST_NAME,
                    opt => opt.MapFrom(src => src.USER != null ? src.USER.FIRST_NAME : string.Empty))
                .ForMember(dest => dest.USER_LAST_NAME,
                    opt => opt.MapFrom(src => src.USER != null ? src.USER.LAST_NAME : string.Empty))
                    .ForMember(dest => dest.FORM_TYPE_DESC, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.FORM_TYPE_ID)));

            #region NPPBKC

            Mapper.CreateMap<ZAIDM_EX_NPPBKC, VirtualNppbckDetails>().IgnoreAllNonExisting()
                .ForMember(dest => dest.VirtualNppbckId, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.CITY))
                .ForMember(dest => dest.Address1, opt => opt.MapFrom(src => src.ADDR1))
                .ForMember(dest => dest.RegionOfficeOfDGCE, opt => opt.MapFrom(src => src.REGION_DGCE))
                .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.REGION))
                .ForMember(dest => dest.Address2, opt => opt.MapFrom(src => src.ADDR2))
                .ForMember(dest => dest.TextTo, opt => opt.MapFrom(src => src.TEXT_TO))
                .ForMember(dest => dest.KppbcId, opt => opt.MapFrom(src => src.KPPBC_ID))
                .ForMember(dest => dest.CityAlias, opt => opt.MapFrom(src => src.CITY_ALIAS))
                .ForMember(dest => dest.AcountNumber, opt => opt.MapFrom(src => src.LFA1.LIFNR))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.START_DATE))
                .ForMember(dest => dest.Is_Deleted, opt => opt.MapFrom(src => src.IS_DELETED.HasValue && src.IS_DELETED.Value ? "Yes" : "No"))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.END_DATE));

            Mapper.CreateMap<VirtualNppbckDetails, ZAIDM_EX_NPPBKC>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.VirtualNppbckId))
                .ForMember(dest => dest.KPPBC_ID, opt => opt.MapFrom(src => src.KppbcId))
                .ForMember(dest => dest.REGION_DGCE, opt => opt.MapFrom(src => src.RegionOfficeOfDGCE))
                .ForMember(dest => dest.REGION, opt => opt.MapFrom(src => src.Region))
                .ForMember(dest => dest.TEXT_TO, opt => opt.MapFrom(src => src.TextTo))
                .ForMember(dest => dest.CITY_ALIAS, opt => opt.MapFrom(src => src.CityAlias));


            #endregion


            #region Material
            Mapper.CreateMap<ZAIDM_EX_MATERIAL, MaterialDetails>().IgnoreAllNonExisting()
                .ForMember(dest => dest.BaseUom, opt => opt.MapFrom(src => src.BASE_UOM_ID))
                .ForMember(dest => dest.UomName, opt => opt.MapFrom(src => src.UOM.UOM_DESC))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.T001W.NAME1))
                .ForMember(dest => dest.GoodtypId, opt => opt.MapFrom(src => src.EXC_GOOD_TYP))
                .ForMember(dest => dest.GoodTypeName, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC))
                .ForMember(dest => dest.MaterialNumber, opt => opt.MapFrom(src => src.STICKER_CODE))
                .ForMember(dest => dest.MaterialDesc, opt => opt.MapFrom(src => src.MATERIAL_DESC))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IS_DELETED.HasValue && src.IS_DELETED.Value ? "yes" : "No"));

            Mapper.CreateMap<ZAIDM_EX_MATERIAL, MaterialCreateViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UomId, opt => opt.MapFrom(src => src.BASE_UOM_ID))
                .ForMember(dest => dest.UomName, opt => opt.MapFrom(src => src.UOM.UOM_DESC))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.T001W.WERKS))
                .ForMember(dest => dest.GoodTypeId, opt => opt.MapFrom(src => src.EXC_GOOD_TYP))
                .ForMember(dest => dest.GoodTypeName, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC))
                .ForMember(dest => dest.MaterialNumber, opt => opt.MapFrom(src => src.STICKER_CODE))
                .ForMember(dest => dest.MaterialDesc, opt => opt.MapFrom(src => src.MATERIAL_DESC))
                .ForMember(dest => dest.MaterialGroup, opt => opt.MapFrom(src => src.MATERIAL_GROUP))
                .ForMember(dest => dest.PurchasingGroup, opt => opt.MapFrom(src => src.PURCHASING_GROUP))
                .ForMember(dest => dest.IssueStorageLoc, opt => opt.MapFrom(src => src.ISSUE_STORANGE_LOC))
                .ForMember(dest => dest.Conversion, opt => opt.MapFrom(src => src.CONVERSION))
                .ForMember(dest => dest.IsFromSap, opt => opt.MapFrom(src => false));

            Mapper.CreateMap<ZAIDM_EX_MATERIAL, MaterialEditViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UomId, opt => opt.MapFrom(src => src.BASE_UOM_ID))
                .ForMember(dest => dest.UomName, opt => opt.MapFrom(src => src.UOM.UOM_DESC))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.T001W.WERKS))
                .ForMember(dest => dest.GoodTypeId, opt => opt.MapFrom(src => src.EXC_GOOD_TYP))
                .ForMember(dest => dest.GoodTypeName, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC))
                .ForMember(dest => dest.MaterialNumber, opt => opt.MapFrom(src => src.STICKER_CODE))
                .ForMember(dest => dest.MaterialDesc, opt => opt.MapFrom(src => src.MATERIAL_DESC))
                .ForMember(dest => dest.MaterialGroup, opt => opt.MapFrom(src => src.MATERIAL_GROUP))
                .ForMember(dest => dest.PurchasingGroup, opt => opt.MapFrom(src => src.PURCHASING_GROUP))
                .ForMember(dest => dest.IssueStorageLoc, opt => opt.MapFrom(src => src.ISSUE_STORANGE_LOC))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.USER.FIRST_NAME + " " + src.USER.LAST_NAME))
                .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.Conversion, opt => opt.MapFrom(src => src.CONVERSION))
                .ForMember(dest => dest.IsFromSap, opt => opt.MapFrom(src => src.IS_FROM_SAP));
            Mapper.CreateMap<MaterialEditViewModel, ZAIDM_EX_MATERIAL>().IgnoreAllNonExisting()
              .ForMember(dest => dest.BASE_UOM_ID, opt => opt.MapFrom(src => src.UomId))

                .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.PlantId))

                .ForMember(dest => dest.EXC_GOOD_TYP, opt => opt.MapFrom(src => src.GoodTypeId))

                .ForMember(dest => dest.STICKER_CODE, opt => opt.MapFrom(src => src.MaterialNumber))
                .ForMember(dest => dest.MATERIAL_DESC, opt => opt.MapFrom(src => src.MaterialDesc))
                .ForMember(dest => dest.MATERIAL_GROUP, opt => opt.MapFrom(src => src.MaterialGroup))
                .ForMember(dest => dest.PURCHASING_GROUP, opt => opt.MapFrom(src => src.PurchasingGroup))
                .ForMember(dest => dest.ISSUE_STORANGE_LOC, opt => opt.MapFrom(src => src.IssueStorageLoc))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedById))
                .ForMember(dest => dest.CONVERSION, opt => opt.MapFrom(src => src.Conversion))
                .ForMember(dest => dest.IS_FROM_SAP, opt => opt.MapFrom(src => src.IsFromSap));

            Mapper.CreateMap<MaterialCreateViewModel, ZAIDM_EX_MATERIAL>().IgnoreAllNonExisting()
                .ForMember(dest => dest.BASE_UOM_ID, opt => opt.MapFrom(src => src.UomId))

                .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.PlantId))

                .ForMember(dest => dest.EXC_GOOD_TYP, opt => opt.MapFrom(src => src.GoodTypeId))

                .ForMember(dest => dest.STICKER_CODE, opt => opt.MapFrom(src => src.MaterialNumber))
                .ForMember(dest => dest.MATERIAL_DESC, opt => opt.MapFrom(src => src.MaterialDesc))
                .ForMember(dest => dest.MATERIAL_GROUP, opt => opt.MapFrom(src => src.MaterialGroup))
                .ForMember(dest => dest.PURCHASING_GROUP, opt => opt.MapFrom(src => src.PurchasingGroup))
                .ForMember(dest => dest.ISSUE_STORANGE_LOC, opt => opt.MapFrom(src => src.IssueStorageLoc))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedById))
                .ForMember(dest => dest.CONVERSION, opt => opt.MapFrom(src => src.Conversion))
                .ForMember(dest => dest.IS_FROM_SAP, opt => opt.MapFrom(src => false));

            Mapper.CreateMap<MaterialEditViewModel, ZAIDM_EX_MATERIAL>().IgnoreAllNonExisting()
                .ForMember(dest => dest.BASE_UOM_ID, opt => opt.MapFrom(src => src.UomId))

                .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.PlantId))

                .ForMember(dest => dest.EXC_GOOD_TYP, opt => opt.MapFrom(src => src.GoodTypeId))

                .ForMember(dest => dest.STICKER_CODE, opt => opt.MapFrom(src => src.MaterialNumber))
                .ForMember(dest => dest.MATERIAL_DESC, opt => opt.MapFrom(src => src.MaterialDesc))
                .ForMember(dest => dest.MATERIAL_GROUP, opt => opt.MapFrom(src => src.MaterialGroup))
                .ForMember(dest => dest.PURCHASING_GROUP, opt => opt.MapFrom(src => src.PurchasingGroup))
                .ForMember(dest => dest.ISSUE_STORANGE_LOC, opt => opt.MapFrom(src => src.IssueStorageLoc))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedById))
                .ForMember(dest => dest.CONVERSION, opt => opt.MapFrom(src => src.Conversion))
                .ForMember(dest => dest.IS_FROM_SAP, opt => opt.MapFrom(src => src.IsFromSap));

            Mapper.CreateMap<ZAIDM_EX_MATERIAL, MaterialDetailViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UomName, opt => opt.MapFrom(src => src.UOM.UOM_DESC))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.T001W.WERKS))
                .ForMember(dest => dest.GoodTypeName, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC))
                .ForMember(dest => dest.MaterialGroup, opt => opt.MapFrom(src => src.MATERIAL_GROUP))
                .ForMember(dest => dest.PurchasingGroup, opt => opt.MapFrom(src => src.PURCHASING_GROUP))
                .ForMember(dest => dest.IssueStorageLoc, opt => opt.MapFrom(src => src.ISSUE_STORANGE_LOC))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.USER.FIRST_NAME + " " + src.USER.LAST_NAME))
                //.ForMember(dest => dest.ChangedDate, opt => opt.MapFrom(src => src.MOD))
                .ForMember(dest => dest.MaterialNumber, opt => opt.MapFrom(src => src.STICKER_CODE))
                .ForMember(dest => dest.MaterialDesc, opt => opt.MapFrom(src => src.MATERIAL_DESC))
                .ForMember(dest => dest.Convertion, opt => opt.MapFrom(src => src.CONVERSION))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IS_DELETED));
            //.ForMember(dest => dest.isPlantDeleteTemp, opt => opt.MapFrom(src => src.));



            #endregion

            #region UOM
            Mapper.CreateMap<UOM, UomDetailViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UomId, opt => opt.MapFrom(src => src.UOM_ID))
                .ForMember(dest => dest.UomName, opt => opt.MapFrom(src => src.UOM_DESC));

            Mapper.CreateMap<UOM, UomDetails>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UomId, opt => opt.MapFrom(src => src.UOM_ID))
                .ForMember(dest => dest.UomName, opt => opt.MapFrom(src => src.UOM_DESC));

            Mapper.CreateMap<UomDetailViewModel, UOM>().IgnoreAllNonExisting()
               .ForMember(dest => dest.UOM_ID, opt => opt.MapFrom(src => src.UomId))
               .ForMember(dest => dest.UOM_DESC, opt => opt.MapFrom(src => src.UomName));




            #endregion


         
            Mapper.CreateMap<DOC_NUMBER_SEQ, DocumentSequenceModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.LastSequence, opt => opt.MapFrom(src => src.DOC_NUMBER_SEQ_LAST))
                .ForMember(dest => dest.MonthInt, opt => opt.MapFrom(src => src.MONTH))
                .ForMember(dest => dest.MonthName_Eng, opt => opt.MapFrom(src => src.MONTH1.MONTH_NAME_ENG))
                .ForMember(dest => dest.MonthName_Ind, opt => opt.MapFrom(src => src.MONTH1.MONTH_NAME_IND))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.YEAR));
            #region Workflow History

            Mapper.CreateMap<WorkflowHistoryDto, WorkflowHistoryViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ACTION, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.ACTION)))
                .ForMember(dest => dest.USERNAME, opt => opt.MapFrom(src => src.USER.USERNAME))
                .ForMember(dest => dest.USER_FIRST_NAME, opt => opt.MapFrom(src => src.USER.FIRST_NAME))
                .ForMember(dest => dest.USER_LAST_NAME, opt => opt.MapFrom(src => src.USER.LAST_NAME));

            #endregion

            Mapper.CreateMap<Pbck1ProdPlanModel, Pbck1ProdPlanInput>().IgnoreAllNonExisting();
            Mapper.CreateMap<Pbck1ProdPlanOutput, Pbck1ProdPlanModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<Pbck1ProdConvModel, Pbck1ProdConverterInput>().IgnoreAllNonExisting();
            Mapper.CreateMap<Pbck1ProdConverterOutput, Pbck1ProdConvModel>().IgnoreAllNonExisting();

        }
    }

    public static class MappingExpressionExtensions
    {
        public static string BoolToString(bool? param)
        {
            if (param == true)
                return "Yes";

            return "No";
        }
    }
}