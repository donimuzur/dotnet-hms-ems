using System;
using System.Collections.Generic;
using System.Linq;
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
using Sampoerna.EMS.Website.Models.VirtualMappingPlant;
using Sampoerna.EMS.Website.Models.Material;
using Sampoerna.EMS.Website.Models.WorkflowHistory;

namespace Sampoerna.EMS.Website
{
    public class EMSWebsiteMapper
    {
        public static void Initialize()
        {
            //AutoMapper
            Mapper.CreateMap<USER, UserItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Manager, opt => opt.MapFrom(src => src.USER2))
                .ForMember(dest => dest.Employees, opt => opt.MapFrom(src => src.USER1));

            Mapper.CreateMap<UserTree, UserItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IS_ACTIVE.HasValue && src.IS_ACTIVE.Value))
                ;

            #region Company
            Mapper.CreateMap<T1001, CompanyDetail>().IgnoreAllNonExisting()
            .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.COMPANY_ID))
            .ForMember(dest => dest.DocumentBukrs, opt => opt.MapFrom(src => src.BUKRS))
            .ForMember(dest => dest.DocumentBukrstxt, opt => opt.MapFrom(src => src.BUKRSTXT))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE));
            #endregion

            Mapper.CreateMap<Pbck1, Pbck1Item>().IgnoreAllNonExisting()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
                .ForMember(dest => dest.StatusGovName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.StatusGov)))
                .ForMember(dest => dest.PbckTypeName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Pbck1Type)))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.PeriodFrom.Year))
                .ForMember(dest => dest.Pbck1ReferenceNumber, opt => opt.MapFrom(src => src.Pbck1Reference != null && src.Pbck1Parent != null ? src.Pbck1Parent.Pbck1Number : string.Empty))
            ;

            Mapper.CreateMap<Pbck1FilterViewModel, Pbck1GetByParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Poa, opt => opt.ResolveUsing<StringToNullableIntegerResolver>().FromMember(src => src.Poa))
                .ForMember(dest => dest.Creator, opt => opt.ResolveUsing<StringToNullableIntegerResolver>().FromMember(src => src.Creator))
                ;

            Mapper.CreateMap<Pbck1ProdConvModel, Pbck1ProdConverter>().IgnoreAllNonExisting();

            Mapper.CreateMap<Pbck1ProdPlanModel, Pbck1ProdPlan>().IgnoreAllNonExisting();

            Mapper.CreateMap<Pbck1Item, Pbck1>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck1ProdConverter,
                    opt => opt.MapFrom(src => Mapper.Map<List<Pbck1ProdConverter>>(src.Pbck1ProdConverter)))
                .ForMember(dest => dest.Pbck1ProdPlan,
                    opt => opt.MapFrom(src => Mapper.Map<List<Pbck1ProdPlan>>(src.Pbck1ProdPlan)))
                    ;

            Mapper.CreateMap<BrandRegistrationOutput, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.BrandIdZaidmExBrand))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.BrandCe));

            Mapper.CreateMap<ZAIDM_EX_MATERIAL, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.MATERIAL_ID))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.MATERIAL_NUMBER + " - " + src.MATERIAL_DESC));


            Mapper.CreateMap<ZAIDM_EX_POA, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.POA_ID))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.POA_CODE));

            Mapper.CreateMap<ZAIDM_EX_NPPBKC, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.NPPBKC_NO));

            Mapper.CreateMap<USER, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.USER_ID))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => (src.FIRST_NAME + ' ' + src.LAST_NAME)));

       
            Mapper.CreateMap<T1001W, SelectItemModel>().IgnoreAllNonExisting()
            .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.PLANT_ID))
            .ForMember(dest => dest.TextField, opt => opt.ResolveUsing<SourcePlantTextResolver>().FromMember(src => src));
            Mapper.CreateMap<SUPPLIER_PORT, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.SUPPLIER_PORT_ID + "-" + src.PORT_NAME))
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.SUPPLIER_PORT_ID))
                ;

            Mapper.CreateMap<Plant, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.PLANT_ID + "-" + src.NAME1))
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.PLANT_ID));
            Mapper.CreateMap<ZAIDM_EX_GOODTYP, SelectItemModel>().IgnoreAllNonExisting()
               .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.EXC_GOOD_TYP + "-" + src.EXT_TYP_DESC))
               .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.GOODTYPE_ID));


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
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.Werks, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.PlantDescription, opt => opt.MapFrom(src => src.NAME1))
                .ForMember(dest => dest.IsMainPlant, opt => opt.MapFrom(src => src.IS_MAIN_PLANT.HasValue && src.IS_MAIN_PLANT.Value))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.ADDRESS))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.CITY))
                .ForMember(dest => dest.Name1, opt => opt.MapFrom(src => src.NAME1))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PHONE))
                .ForMember(dest => dest.Ort01, opt => opt.MapFrom(src => src.ORT01))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ReceiveMaterials, opt => opt.MapFrom(src => Mapper.Map<List<PlantReceiveMaterialItemModel>>(src.PLANT_RECEIVE_MATERIAL)))
                ;

            Mapper.CreateMap<DetailPlantT1001W, Plant>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PLANT_ID, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.Werks))
                .ForMember(dest => dest.NAME1, opt => opt.MapFrom(src => src.Name1))
                .ForMember(dest => dest.IS_MAIN_PLANT, opt => opt.MapFrom(src => src.IsMainPlant))
                .ForMember(dest => dest.ADDRESS, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.CITY, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.SKEPTIS, opt => opt.MapFrom(src => src.Skeptis))
                .ForMember(dest => dest.PHONE, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.ORT01, opt => opt.MapFrom(src => src.Ort01))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.PLANT_RECEIVE_MATERIAL,
                    opt => opt.MapFrom(src => Mapper.Map<List<PLANT_RECEIVE_MATERIAL>>(src.ReceiveMaterials)));

            #endregion

            #region POA



            Mapper.CreateMap<ZAIDM_EX_POA, POAViewDetailModel>().IgnoreAllNonExisting()
               .ForMember(dest => dest.PoaIdCard, opt => opt.MapFrom(src => src.POA_ID_CARD))
               .ForMember(dest => dest.PoaId, opt => opt.MapFrom(src => src.POA_ID))
               .ForMember(dest => dest.PoaCode, opt => opt.MapFrom(src => src.POA_CODE))
               .ForMember(dest => dest.PoaPrintedName, opt => opt.MapFrom(src => src.POA_PRINTED_NAME))
               .ForMember(dest => dest.PoaPhone, opt => opt.MapFrom(src => src.POA_PHONE))
               .ForMember(dest => dest.PoaAddress, opt => opt.MapFrom(src => src.POA_ADDRESS))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EMAIL))
               .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.TITLE))
               .ForMember(dest => dest.Is_Deleted, opt => opt.MapFrom(src => src.IS_DELETED == null ? "No" : MappingExpressionExtensions.BoolToString(src.IS_DELETED)))
               .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.USER_ID))
               .ForMember(dest => dest.ManagerId, opt => opt.MapFrom(src => src.MANAGER_ID))
               .ForMember(dest => dest.IsFromSAP, opt => opt.MapFrom(src => src.IS_FROM_SAP.HasValue && src.IS_FROM_SAP.Value));

            Mapper.CreateMap<POAViewDetailModel, ZAIDM_EX_POA>().IgnoreAllUnmapped()
               .ForMember(dest => dest.POA_ID_CARD, opt => opt.MapFrom(src => src.PoaIdCard))
               .ForMember(dest => dest.POA_CODE, opt => opt.MapFrom(src => src.PoaCode))
               .ForMember(dest => dest.POA_PRINTED_NAME, opt => opt.MapFrom(src => src.PoaPrintedName))
               .ForMember(dest => dest.POA_PHONE, opt => opt.MapFrom(src => src.PoaPhone))
               .ForMember(dest => dest.POA_ADDRESS, opt => opt.MapFrom(src => src.PoaAddress))
               .ForMember(dest => dest.EMAIL, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.TITLE, opt => opt.MapFrom(src => src.Title))
               .ForMember(dest => dest.MANAGER_ID, opt => opt.MapFrom(src => src.ManagerId))
               .ForMember(dest => dest.USER_ID, opt => opt.MapFrom(src => src.UserId));

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
               .ForMember(dest => dest.GoodTypeId, opt => opt.MapFrom(src => src.GOODTYPE_ID))
               .ForMember(dest => dest.GoodTypeName, opt => opt.MapFrom(src => src.EXT_TYP_DESC));

            Mapper.CreateMap<EX_GROUP_TYPE, DetailsGoodsTypGroup>().IgnoreAllNonExisting()
             .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GROUP_NAME))
             .ForMember(dest => dest.GroupTypeName, opt => opt.MapFrom(src => src.GROUP_NAME));

            Mapper.CreateMap<EX_GROUP_TYPE, GoodsTypeDetails>().IgnoreAllNonExisting()
              .ForMember(dest => dest.GoodTypeId, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.GOODTYPE_ID))
              .ForMember(dest => dest.GoodTypeName, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC));

            #region VirtualMappingPlant
            //Virtual Mapping Plant
            Mapper.CreateMap<VIRTUAL_PLANT_MAP, VirtualMappingPlantDetail>().IgnoreAllNonExisting()
                .ForMember(dest => dest.VirtualPlantMapId, opt => opt.MapFrom(src => src.VIRTUAL_PLANT_MAP_ID))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.COMPANY_ID))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.T1001.BUKRSTXT))
                .ForMember(dest => dest.ImportPlantId, opt => opt.MapFrom(src => src.IMPORT_PLANT_ID))
                .ForMember(dest => dest.ImportPlanName, opt => opt.MapFrom(src => src.T1001W.WERKS))
                .ForMember(dest => dest.ExportPlantId, opt => opt.MapFrom(src => src.EXPORT_PLANT_ID))
                .ForMember(dest => dest.ExportPlanName, opt => opt.MapFrom(src => src.T1001W1.WERKS))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IS_DELETED));

            Mapper.CreateMap<Plant, SelectItemModelVirtualPlant>().IgnoreAllNonExisting()
              .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.PLANT_ID + "-" + src.WERKS))
              .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.PLANT_ID));

            Mapper.CreateMap<VirtualMappingPlantCreateViewModel, VIRTUAL_PLANT_MAP>().IgnoreAllNonExisting()
                .ForMember(dest => dest.COMPANY_ID, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.IMPORT_PLANT_ID, opt => opt.MapFrom(src => src.ImportPlantId))
                .ForMember(dest => dest.EXPORT_PLANT_ID, opt => opt.MapFrom(src => src.ExportPlantId));

            #endregion

            #region BrandRegistration

            Mapper.CreateMap<ZAIDM_EX_BRAND, BrandRegistrationDetail>().IgnoreAllNonExisting()
                .ForMember(dest => dest.BrandId, opt => opt.MapFrom(src => src.BRAND_ID))
                .ForMember(dest => dest.StickerCode, opt => opt.MapFrom(src => src.STICKER_CODE))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.T1001W.WERKS))
                .ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FA_CODE))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.BRAND_CE))
                .ForMember(dest => dest.SeriesValue, opt => opt.MapFrom(src => src.ZAIDM_EX_SERIES.SERIES_VALUE))
                .ForMember(dest => dest.Conversion, opt => opt.MapFrom(src => src.CONVERSION))
                .ForMember(dest => dest.PrintingPrice, opt => opt.MapFrom(src => src.PRINTING_PRICE))
                .ForMember(dest => dest.CutFilterCode, opt => opt.MapFrom(src => src.CUT_FILLER_CODE))
                .ForMember(dest => dest.IsDeleted, opt => opt.ResolveUsing<NullableBooleanToStringDeletedResolver>().FromMember(src => src.IS_DELETED));


            Mapper.CreateMap<ZAIDM_EX_BRAND, BrandRegistrationDetailsViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.BrandId, opt => opt.MapFrom(src => src.BRAND_ID))
                .ForMember(dest => dest.StickerCode, opt => opt.MapFrom(src => src.STICKER_CODE))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.T1001W.WERKS))
                .ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FA_CODE))
                .ForMember(dest => dest.PersonalizationCode, opt => opt.MapFrom(src => src.ZAIDM_EX_PCODE.PER_CODE))
                .ForMember(dest => dest.PersonalizationCodeDescription,
                    opt => opt.MapFrom(src => src.ZAIDM_EX_PCODE.PER_DESC))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.BRAND_CE))
                .ForMember(dest => dest.SkepNo, opt => opt.MapFrom(src => src.SKEP_NP))
                .ForMember(dest => dest.SkepDate, opt => opt.MapFrom(src => src.SKEP_DATE))
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.ZAIDM_EX_PRODTYP.PRODUCT_CODE))
                .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => src.ZAIDM_EX_PRODTYP.PRODUCT_TYPE))
                .ForMember(dest => dest.ProductAlias, opt => opt.MapFrom(src => src.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS))
                .ForMember(dest => dest.SeriesCode, opt => opt.MapFrom(src => src.ZAIDM_EX_SERIES.SERIES_CODE))
                .ForMember(dest => dest.SeriesValue, opt => opt.MapFrom(src => src.ZAIDM_EX_SERIES.SERIES_VALUE))
                .ForMember(dest => dest.MarketCode, opt => opt.MapFrom(src => src.ZAIDM_EX_MARKET.MARKET_CODE))
                .ForMember(dest => dest.MarketDescription, opt => opt.MapFrom(src => src.ZAIDM_EX_MARKET.MARKET_DESC))
                .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.COUNTRY.COUNTRY_CODE))
                .ForMember(dest => dest.HjeValue, opt => opt.MapFrom(src => src.HJE_IDR))
                .ForMember(dest => dest.HjeCurrency, opt => opt.MapFrom(src => src.CURRENCY.CURRENCY_CODE))
                //todo check which one correct
                .ForMember(dest => dest.Tariff, opt => opt.MapFrom(src => src.TARIFF))
                .ForMember(dest => dest.TariffCurrency, opt => opt.MapFrom(src => src.CURRENCY1.CURRENCY_CODE))
                //todo check which one correct
                .ForMember(dest => dest.ColourName, opt => opt.MapFrom(src => src.COLOUR))
                .ForMember(dest => dest.GoodType, opt => opt.MapFrom(src => src.GOODTYP_ID))
                .ForMember(dest => dest.GoodTypeDescription,
                    opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.START_DATE))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.END_DATE))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IS_ACTIVE == true ? "Active" : "Inactive"))
                .ForMember(dest => dest.PrintingPrice, opt => opt.MapFrom(src => src.PRINTING_PRICE))
                .ForMember(dest => dest.CutFilterCode, opt => opt.MapFrom(src => src.CUT_FILLER_CODE))
                .ForMember(dest => dest.IsDeleted, opt => opt.ResolveUsing<NullableBooleanToStringDeletedResolver>().FromMember(src => src.IS_DELETED))
                .ForMember(dest => dest.Conversion, opt => opt.MapFrom(src => src.CONVERSION))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.BRAND_CONTENT));


            Mapper.CreateMap<ZAIDM_EX_BRAND, BrandRegistrationEditViewModel>().IgnoreAllNonExisting()
            .ForMember(dest => dest.BrandId, opt => opt.MapFrom(src => src.BRAND_ID))
            .ForMember(dest => dest.StickerCode, opt => opt.MapFrom(src => src.STICKER_CODE))
            .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PLANT_ID))
            .ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FA_CODE))
            .ForMember(dest => dest.PersonalizationCode, opt => opt.MapFrom(src => src.PER_ID))
            .ForMember(dest => dest.PersonalizationCodeDescription, opt => opt.MapFrom(src => src.ZAIDM_EX_PCODE.PER_DESC))
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.BRAND_CE))
            .ForMember(dest => dest.SkepNo, opt => opt.MapFrom(src => src.SKEP_NP))
            .ForMember(dest => dest.SkepDate, opt => opt.MapFrom(src => src.SKEP_DATE))
            .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.PRODUCT_ID))
            .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => src.ZAIDM_EX_PRODTYP.PRODUCT_TYPE))
            .ForMember(dest => dest.ProductAlias, opt => opt.MapFrom(src => src.ZAIDM_EX_PRODTYP.PRODUCT_ALIAS))
            .ForMember(dest => dest.SeriesId, opt => opt.MapFrom(src => src.SERIES_ID))
            .ForMember(dest => dest.SeriesValue, opt => opt.MapFrom(src => src.ZAIDM_EX_SERIES.SERIES_VALUE))
            .ForMember(dest => dest.MarketId, opt => opt.MapFrom(src => src.MARKET_ID))
            .ForMember(dest => dest.MarketDescription, opt => opt.MapFrom(src => src.ZAIDM_EX_MARKET.MARKET_DESC))
            .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.COUNTRY_ID))
            .ForMember(dest => dest.HjeValue, opt => opt.MapFrom(src => src.HJE_IDR))
            .ForMember(dest => dest.HjeCurrency, opt => opt.MapFrom(src => src.HJE_CURR)) //todo check which one correct
            .ForMember(dest => dest.Tariff, opt => opt.MapFrom(src => src.TARIFF))
            .ForMember(dest => dest.TariffCurrency, opt => opt.MapFrom(src => src.TARIFF_CURR)) //todo check which one correct
            .ForMember(dest => dest.ColourName, opt => opt.MapFrom(src => src.COLOUR))
            .ForMember(dest => dest.GoodType, opt => opt.MapFrom(src => src.GOODTYP_ID))
            .ForMember(dest => dest.GoodTypeDescription, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.START_DATE))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.END_DATE))
            .ForMember(dest => dest.PrintingPrice, opt => opt.MapFrom(src => src.PRINTING_PRICE))
            .ForMember(dest => dest.CutFilterCode, opt => opt.MapFrom(src => src.CUT_FILLER_CODE))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IS_ACTIVE == true ? "Active" : "Inactive"))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE))
            .ForMember(dest => dest.IsFromSAP, opt => opt.MapFrom(src => src.IS_FROM_SAP))
            .ForMember(dest => dest.Conversion, opt => opt.MapFrom(src => src.CONVERSION))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.BRAND_CONTENT));

            Mapper.CreateMap<BrandRegistrationCreateViewModel, ZAIDM_EX_BRAND>().IgnoreAllNonExisting()
                .ForMember(dest => dest.STICKER_CODE, opt => opt.MapFrom(src => src.StickerCode))
                .ForMember(dest => dest.PLANT_ID, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.FA_CODE, opt => opt.MapFrom(src => src.FaCode))
                .ForMember(dest => dest.PER_ID, opt => opt.MapFrom(src => src.PersonalizationCode))
                .ForMember(dest => dest.BRAND_CE, opt => opt.MapFrom(src => src.BrandName))
                .ForMember(dest => dest.SKEP_NP, opt => opt.MapFrom(src => src.SkepNo))
                .ForMember(dest => dest.SKEP_DATE, opt => opt.MapFrom(src => src.SkepDate))
                .ForMember(dest => dest.PRODUCT_ID, opt => opt.MapFrom(src => src.ProductCode))
                .ForMember(dest => dest.SERIES_ID, opt => opt.MapFrom(src => src.SeriesId))
                .ForMember(dest => dest.MARKET_ID, opt => opt.MapFrom(src => src.MarketId))
                .ForMember(dest => dest.COUNTRY_ID, opt => opt.MapFrom(src => src.CountryId))
                .ForMember(dest => dest.HJE_IDR, opt => opt.MapFrom(src => src.HjeValue))
                .ForMember(dest => dest.HJE_CURR, opt => opt.MapFrom(src => src.HjeCurrency))
                .ForMember(dest => dest.TARIFF, opt => opt.MapFrom(src => src.Tariff))
                .ForMember(dest => dest.TARIFF_CURR, opt => opt.MapFrom(src => src.TariffCurrency))
                .ForMember(dest => dest.COLOUR, opt => opt.MapFrom(src => src.ColourName))
                .ForMember(dest => dest.GOODTYP_ID, opt => opt.MapFrom(src => src.GoodType))
                .ForMember(dest => dest.START_DATE, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.END_DATE, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.PRINTING_PRICE, opt => opt.MapFrom(src => src.PrintingPrice))
                .ForMember(dest => dest.CUT_FILLER_CODE, opt => opt.MapFrom(src => src.CutFilterCode))
                .ForMember(dest => dest.CONVERSION, opt => opt.MapFrom(src => src.Conversion))
                .ForMember(dest => dest.BRAND_CONTENT, opt => opt.MapFrom(src => src.Content));

            Mapper.CreateMap<BrandRegistrationEditViewModel, ZAIDM_EX_BRAND>().IgnoreAllUnmapped()
                .ForMember(dest => dest.STICKER_CODE, opt => opt.MapFrom(src => src.StickerCode))
                .ForMember(dest => dest.PLANT_ID, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.FA_CODE, opt => opt.MapFrom(src => src.FaCode))
                .ForMember(dest => dest.PER_ID, opt => opt.MapFrom(src => src.PersonalizationCode))
                .ForMember(dest => dest.BRAND_CE, opt => opt.MapFrom(src => src.BrandName))
                .ForMember(dest => dest.SKEP_NP, opt => opt.MapFrom(src => src.SkepNo))
                .ForMember(dest => dest.SKEP_DATE, opt => opt.MapFrom(src => src.SkepDate))
                .ForMember(dest => dest.PRODUCT_ID, opt => opt.MapFrom(src => src.ProductCode))
                .ForMember(dest => dest.SERIES_ID, opt => opt.MapFrom(src => src.SeriesId))
                .ForMember(dest => dest.MARKET_ID, opt => opt.MapFrom(src => src.MarketId))
                .ForMember(dest => dest.COUNTRY_ID, opt => opt.MapFrom(src => src.CountryId))
                .ForMember(dest => dest.HJE_IDR, opt => opt.MapFrom(src => src.HjeValue))
                .ForMember(dest => dest.HJE_CURR, opt => opt.MapFrom(src => src.HjeCurrency))
                .ForMember(dest => dest.TARIFF, opt => opt.MapFrom(src => src.Tariff))
                .ForMember(dest => dest.TARIFF_CURR, opt => opt.MapFrom(src => src.TariffCurrency))
                .ForMember(dest => dest.COLOUR, opt => opt.MapFrom(src => src.ColourName))
                .ForMember(dest => dest.GOODTYP_ID, opt => opt.MapFrom(src => src.GoodType))
                .ForMember(dest => dest.START_DATE, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.END_DATE, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive))
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
                .ForMember(dest => dest.NppbckNo, opt => opt.MapFrom(src => src.NPPBKC_NO))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.CITY))
                .ForMember(dest => dest.Address1, opt => opt.MapFrom(src => src.ADDR1))
                .ForMember(dest => dest.RegionOfficeOfDGCE, opt => opt.MapFrom(src => src.REGION_OFFICE_DGCE))
                .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.REGION_OFFICE))
                .ForMember(dest => dest.Address2, opt => opt.MapFrom(src => src.ADDR2))
                .ForMember(dest => dest.TextTo, opt => opt.MapFrom(src => src.TEXT_TO))
                .ForMember(dest => dest.KppbcId, opt => opt.MapFrom(src => src.ZAIDM_EX_KPPBC.KPPBC_NUMBER))
                .ForMember(dest => dest.CityAlias, opt => opt.MapFrom(src => src.CITY_ALIAS))
                .ForMember(dest => dest.AcountNumber, opt => opt.MapFrom(src => src.C1LFA1.LIFNR))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.START_DATE))
                .ForMember(dest => dest.Is_Deleted, opt => opt.MapFrom(src => src.IS_DELETED.HasValue && src.IS_DELETED.Value ? "Yes" : "No"))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.END_DATE));

            Mapper.CreateMap<VirtualNppbckDetails, ZAIDM_EX_NPPBKC>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.VirtualNppbckId))
                .ForMember(dest => dest.REGION_OFFICE_DGCE, opt => opt.MapFrom(src => src.RegionOfficeOfDGCE))
                .ForMember(dest => dest.TEXT_TO, opt => opt.MapFrom(src => src.TextTo))
                .ForMember(dest => dest.CITY_ALIAS, opt => opt.MapFrom(src => src.CityAlias));


            #endregion


            #region Material
            Mapper.CreateMap<ZAIDM_EX_MATERIAL, MaterialDetails>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MaterialId, opt => opt.MapFrom(src => src.MATERIAL_ID))
                .ForMember(dest => dest.BaseUom, opt => opt.MapFrom(src => src.BASE_UOM))
                .ForMember(dest => dest.UomName, opt => opt.MapFrom(src => src.UOM.UOM_NAME))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.T1001W.WERKS))
                .ForMember(dest => dest.GoodtypId, opt => opt.MapFrom(src => src.EX_GOODTYP))
                .ForMember(dest => dest.GoodTypeName, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC))
                .ForMember(dest => dest.MaterialNumber, opt => opt.MapFrom(src => src.MATERIAL_NUMBER))
                .ForMember(dest => dest.MaterialDesc, opt => opt.MapFrom(src => src.MATERIAL_DESC));

            Mapper.CreateMap<ZAIDM_EX_MATERIAL, MaterialCreateViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MaterialId, opt => opt.MapFrom(src => src.MATERIAL_ID))
                .ForMember(dest => dest.UomId, opt => opt.MapFrom(src => src.BASE_UOM))
                .ForMember(dest => dest.UomName, opt => opt.MapFrom(src => src.UOM.UOM_NAME))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.T1001W.WERKS))
                .ForMember(dest => dest.GoodTypeId, opt => opt.MapFrom(src => src.EX_GOODTYP))
                .ForMember(dest => dest.GoodTypeName, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC))
                .ForMember(dest => dest.MaterialNumber, opt => opt.MapFrom(src => src.MATERIAL_NUMBER))
                .ForMember(dest => dest.MaterialDesc, opt => opt.MapFrom(src => src.MATERIAL_DESC))
                .ForMember(dest => dest.MaterialGroup, opt => opt.MapFrom(src => src.MATERIAL_GROUP))
                .ForMember(dest => dest.PurchasingGroup, opt => opt.MapFrom(src => src.PURCHASING_GROUP))
                .ForMember(dest => dest.IssueStorageLoc, opt => opt.MapFrom(src => src.ISSUE_STORANGE_LOC))
                .ForMember(dest => dest.Convertion, opt => opt.MapFrom(src => src.CONVERSION))
                .ForMember(dest => dest.IsFromSap, opt => opt.MapFrom(src => false));

            Mapper.CreateMap<ZAIDM_EX_MATERIAL, MaterialEditViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MaterialId, opt => opt.MapFrom(src => src.MATERIAL_ID))
                .ForMember(dest => dest.UomId, opt => opt.MapFrom(src => src.BASE_UOM))
                .ForMember(dest => dest.UomName, opt => opt.MapFrom(src => src.UOM.UOM_NAME))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.T1001W.WERKS))
                .ForMember(dest => dest.GoodTypeId, opt => opt.MapFrom(src => src.EX_GOODTYP))
                .ForMember(dest => dest.GoodTypeName, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC))
                .ForMember(dest => dest.MaterialNumber, opt => opt.MapFrom(src => src.MATERIAL_NUMBER))
                .ForMember(dest => dest.MaterialDesc, opt => opt.MapFrom(src => src.MATERIAL_DESC))
                .ForMember(dest => dest.MaterialGroup, opt => opt.MapFrom(src => src.MATERIAL_GROUP))
                .ForMember(dest => dest.PurchasingGroup, opt => opt.MapFrom(src => src.PURCHASING_GROUP))
                .ForMember(dest => dest.IssueStorageLoc, opt => opt.MapFrom(src => src.ISSUE_STORANGE_LOC))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.USER.FIRST_NAME + " " + src.USER.LAST_NAME))
                .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.Convertion, opt => opt.MapFrom(src => src.CONVERSION))
                .ForMember(dest => dest.IsFromSap, opt => opt.MapFrom(src => src.IS_FROM_SAP));

            Mapper.CreateMap<MaterialCreateViewModel, ZAIDM_EX_MATERIAL>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MATERIAL_ID, opt => opt.MapFrom(src => src.MaterialId))
                .ForMember(dest => dest.BASE_UOM, opt => opt.MapFrom(src => src.UomId))

                .ForMember(dest => dest.PLANT_ID, opt => opt.MapFrom(src => src.PlantId))

                .ForMember(dest => dest.EX_GOODTYP, opt => opt.MapFrom(src => src.GoodTypeId))

                .ForMember(dest => dest.MATERIAL_NUMBER, opt => opt.MapFrom(src => src.MaterialNumber))
                .ForMember(dest => dest.MATERIAL_DESC, opt => opt.MapFrom(src => src.MaterialDesc))
                .ForMember(dest => dest.MATERIAL_GROUP, opt => opt.MapFrom(src => src.MaterialGroup))
                .ForMember(dest => dest.PURCHASING_GROUP, opt => opt.MapFrom(src => src.PurchasingGroup))
                .ForMember(dest => dest.ISSUE_STORANGE_LOC, opt => opt.MapFrom(src => src.IssueStorageLoc))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedById))
                .ForMember(dest => dest.CONVERSION, opt => opt.MapFrom(src => src.Convertion))
                .ForMember(dest => dest.IS_FROM_SAP, opt => opt.MapFrom(src => false));

            Mapper.CreateMap<MaterialEditViewModel, ZAIDM_EX_MATERIAL>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MATERIAL_ID, opt => opt.MapFrom(src => src.MaterialId))
                .ForMember(dest => dest.BASE_UOM, opt => opt.MapFrom(src => src.UomId))

                .ForMember(dest => dest.PLANT_ID, opt => opt.MapFrom(src => src.PlantId))

                .ForMember(dest => dest.EX_GOODTYP, opt => opt.MapFrom(src => src.GoodTypeId))

                .ForMember(dest => dest.MATERIAL_NUMBER, opt => opt.MapFrom(src => src.MaterialNumber))
                .ForMember(dest => dest.MATERIAL_DESC, opt => opt.MapFrom(src => src.MaterialDesc))
                .ForMember(dest => dest.MATERIAL_GROUP, opt => opt.MapFrom(src => src.MaterialGroup))
                .ForMember(dest => dest.PURCHASING_GROUP, opt => opt.MapFrom(src => src.PurchasingGroup))
                .ForMember(dest => dest.ISSUE_STORANGE_LOC, opt => opt.MapFrom(src => src.IssueStorageLoc))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedById))
                .ForMember(dest => dest.CONVERSION, opt => opt.MapFrom(src => src.Convertion))
                .ForMember(dest => dest.IS_FROM_SAP, opt => opt.MapFrom(src => src.IsFromSap));

            Mapper.CreateMap<ZAIDM_EX_MATERIAL, MaterialDetailViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MaterialId, opt => opt.MapFrom(src => src.MATERIAL_ID))
                .ForMember(dest => dest.UomName, opt => opt.MapFrom(src => src.UOM.UOM_NAME))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.T1001W.WERKS))
                .ForMember(dest => dest.GoodTypeName, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC))
                .ForMember(dest => dest.MaterialGroup, opt => opt.MapFrom(src => src.MATERIAL_GROUP))
                .ForMember(dest => dest.PurchasingGroup, opt => opt.MapFrom(src => src.PURCHASING_GROUP))
                .ForMember(dest => dest.IssueStorageLoc, opt => opt.MapFrom(src => src.ISSUE_STORANGE_LOC))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.USER.FIRST_NAME + " " + src.USER.LAST_NAME))
                //.ForMember(dest => dest.ChangedDate, opt => opt.MapFrom(src => src.MOD))
                .ForMember(dest => dest.MaterialNumber, opt => opt.MapFrom(src => src.MATERIAL_NUMBER))
                .ForMember(dest => dest.MaterialDesc, opt => opt.MapFrom(src => src.MATERIAL_DESC))
                .ForMember(dest => dest.Convertion, opt => opt.MapFrom(src => src.CONVERSION))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IS_DELETED));
            //.ForMember(dest => dest.isPlantDeleteTemp, opt => opt.MapFrom(src => src.));



            #endregion
            
            
            #region CK5

            Mapper.CreateMap<CK5UploadViewModel, CK5MaterialInput>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5MaterialOutput, CK5UploadViewModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5Dto, CK5Item>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Ck5Id, opt => opt.MapFrom(src => src.CK5_ID))
                .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.SUBMISSION_NUMBER))
                .ForMember(dest => dest.Qty, opt => opt.ResolveUsing<CK5ListIndexQtyResolver>().FromMember(src => src))
                .ForMember(dest => dest.POA, opt => opt.MapFrom(src => src.APPROVED_BY))
                .ForMember(dest => dest.SourcePlant,opt => opt.MapFrom(src => src.SourcePlantWerks + " - " + src.SourcePlantName))
                .ForMember(dest => dest.DestinationPlant,opt => opt.MapFrom(src => src.DestPlantWerks + " - " + src.DestPlantName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.STATUS_ID)));

            Mapper.CreateMap<CK5SearchViewModel, CK5GetByParamInput>().IgnoreAllNonExisting();


            Mapper.CreateMap<T1001W, CK5PlantModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.PlantNpwp, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC.T1001.NPWP))
                .ForMember(dest => dest.NPPBCK_ID, opt => opt.MapFrom(src => src.NPPBCK_ID))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC.T1001.BUKRSTXT))
                .ForMember(dest => dest.CompanyAddress, opt => opt.MapFrom(src => src.ADDRESS))
                .ForMember(dest => dest.KppBcName, opt => opt.MapFrom(src => src.NAME1));

            Mapper.CreateMap<CK5FormViewModel, CK5Dto>().IgnoreAllNonExisting()
              .ForMember(dest => dest.CK5_TYPE, opt => opt.MapFrom(src => src.Ck5Type))
              .ForMember(dest => dest.KPPBC_CITY, opt => opt.MapFrom(src => src.KppBcCityId))
              .ForMember(dest => dest.SUBMISSION_NUMBER, opt => opt.MapFrom(src => src.SubmissionNumber))
              .ForMember(dest => dest.REGISTRATION_NUMBER, opt => opt.MapFrom(src => src.RegistrationNumber))
              .ForMember(dest => dest.EX_GOODS_TYPE_ID, opt => opt.MapFrom(src => src.GoodTypeId))
              .ForMember(dest => dest.EX_SETTLEMENT_ID, opt => opt.MapFrom(src => src.ExciseSettlementId))
              .ForMember(dest => dest.EX_STATUS_ID, opt => opt.MapFrom(src => src.ExciseStatusId))
              .ForMember(dest => dest.REQUEST_TYPE_ID, opt => opt.MapFrom(src => src.RequestTypeId))
              .ForMember(dest => dest.SUBMISSION_DATE, opt => opt.MapFrom(src => src.SubmissionDate))
              .ForMember(dest => dest.SOURCE_PLANT_ID, opt => opt.MapFrom(src => src.SourcePlantId))
              .ForMember(dest => dest.DEST_PLANT_ID, opt => opt.MapFrom(src => src.DestPlantId))
              .ForMember(dest => dest.INVOICE_NUMBER, opt => opt.MapFrom(src => src.InvoiceNumber))
              .ForMember(dest => dest.PBCK1_DECREE_ID, opt => opt.MapFrom(src => src.PbckDecreeId))
              .ForMember(dest => dest.CARRIAGE_METHOD_ID, opt => opt.MapFrom(src => src.CarriageMethodId))
              .ForMember(dest => dest.GRAND_TOTAL_EX, opt => opt.MapFrom(src => src.GrandTotalEx))
              .ForMember(dest => dest.INVOICE_DATE, opt => opt.MapFrom(src => src.InvoiceDate))
              .ForMember(dest => dest.PACKAGE_UOM_ID, opt => opt.MapFrom(src => src.PackageUomId));


            Mapper.CreateMap<CK5Dto, CK5FormViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Ck5Id, opt => opt.MapFrom(src => src.CK5_ID))
                .ForMember(dest => dest.DocumentStatus, opt => opt.MapFrom(src => src.STATUS_ID))
                .ForMember(dest => dest.DocumentStatusDescription, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.STATUS_ID)))
                .ForMember(dest => dest.Ck5Type, opt => opt.MapFrom(src => src.CK5_TYPE))
                .ForMember(dest => dest.KppBcCityId, opt => opt.MapFrom(src => src.KPPBC_CITY))
                .ForMember(dest => dest.KppBcCityName, opt => opt.MapFrom(src => src.KppbcCityName))
                .ForMember(dest => dest.CeOfficeCode, opt => opt.MapFrom(src => src.CeOfficeCode))
                .ForMember(dest => dest.SubmissionNumber, opt => opt.MapFrom(src => src.SUBMISSION_NUMBER))
                .ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(src => src.SUBMISSION_DATE))
                .ForMember(dest => dest.RegistrationNumber, opt => opt.MapFrom(src => src.REGISTRATION_NUMBER))
                .ForMember(dest => dest.GoodTypeId, opt => opt.MapFrom(src => src.EX_GOODS_TYPE_ID))
                .ForMember(dest => dest.GoodTypeName, opt => opt.MapFrom(src => src.GoodTypeDesc))
                .ForMember(dest => dest.ExciseSettlementId, opt => opt.MapFrom(src => src.EX_SETTLEMENT_ID))
                .ForMember(dest => dest.ExciseSettlementName, opt => opt.MapFrom(src => src.ExSettlementName))
                .ForMember(dest => dest.ExciseStatusId, opt => opt.MapFrom(src => src.EX_STATUS_ID))
                .ForMember(dest => dest.ExciseStatusName, opt => opt.MapFrom(src => src.ExStatusName))
                //.ForMember(dest => dest.RequestTypeId, opt => opt.MapFrom(src => src.REQUEST_TYPE_ID))
                //.ForMember(dest => dest.RequestTypeName, opt => opt.MapFrom(src => src.RequestTypeName))
                .ForMember(dest => dest.SourcePlantId, opt => opt.MapFrom(src => src.SOURCE_PLANT_ID))
                .ForMember(dest => dest.SourcePlantName, opt => opt.MapFrom(src => src.SourcePlantWerks))
                .ForMember(dest => dest.DestPlantId, opt => opt.MapFrom(src => src.DEST_PLANT_ID))
                .ForMember(dest => dest.DestPlantName, opt => opt.MapFrom(src => src.DestPlantWerks))
                .ForMember(dest => dest.InvoiceNumber, opt => opt.MapFrom(src => src.INVOICE_NUMBER))
                .ForMember(dest => dest.PbckDecreeId, opt => opt.MapFrom(src => src.PBCK1_DECREE_ID))
                .ForMember(dest => dest.PbckDecreeNumber, opt => opt.MapFrom(src => src.PbckNumber))
                .ForMember(dest => dest.PbckDecreeDate, opt => opt.MapFrom(src => src.PbckDecreeDate))
                .ForMember(dest => dest.CarriageMethodId, opt => opt.MapFrom(src => src.CARRIAGE_METHOD_ID))
                .ForMember(dest => dest.CarriageMethodName, opt => opt.MapFrom(src => src.CarriageMethodName))
                .ForMember(dest => dest.GrandTotalEx, opt => opt.MapFrom(src => src.GRAND_TOTAL_EX))
                .ForMember(dest => dest.PackageUomId, opt => opt.MapFrom(src => src.PACKAGE_UOM_ID))
                .ForMember(dest => dest.PackageUomName, opt => opt.MapFrom(src => src.PackageUomName))
                .ForMember(dest => dest.DnNumber, opt => opt.MapFrom(src => src.DN_NUMBER))
                .ForMember(dest => dest.StoSenderNumber, opt => opt.MapFrom(src => src.STO_SENDER_NUMBER))
                .ForMember(dest => dest.StoReceiverNumber, opt => opt.MapFrom(src => src.STO_RECEIVER_NUMBER))
                .ForMember(dest => dest.StobNumber, opt => opt.MapFrom(src => src.STOB_NUMBER))
                .ForMember(dest => dest.GiDate, opt => opt.MapFrom(src => src.GI_DATE))
                .ForMember(dest => dest.SealingNotifNumber, opt => opt.MapFrom(src => src.SEALING_NOTIF_NUMBER))
                .ForMember(dest => dest.SealingNotifDate, opt => opt.MapFrom(src => src.SEALING_NOTIF_DATE))
                .ForMember(dest => dest.UnSealingNotifNumber, opt => opt.MapFrom(src => src.UNSEALING_NOTIF_NUMBER))
                .ForMember(dest => dest.UnsealingNotifDate, opt => opt.MapFrom(src => src.UNSEALING_NOTIF_DATE));
            ////todo attachment
            ////todo Requisitioner

            Mapper.CreateMap<CK5UploadViewModel, CK5MaterialDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.QTY,opt => opt.ResolveUsing<StringToDecimalResolver>().FromMember(src => src.Qty))
                .ForMember(dest => dest.CONVERTED_QTY, opt => opt.ResolveUsing<StringToDecimalResolver>().FromMember(src => src.ConvertedQty))
                .ForMember(dest => dest.EXCISE_VALUE, opt => opt.ResolveUsing<StringToDecimalResolver>().FromMember(src => src.ExciseValue))
                .ForMember(dest => dest.CONVERTION, opt => opt.ResolveUsing<StringToDecimalResolver>().FromMember(src => src.Convertion))
                .ForMember(dest => dest.USD_VALUE, opt => opt.ResolveUsing<StringToDecimalResolver>().FromMember(src => src.UsdValue))
                .ForMember(dest => dest.CONVERTED_UOM, opt => opt.MapFrom(src => src.ConvertedUom));

            Mapper.CreateMap<CK5MaterialDto, CK5UploadViewModel>().IgnoreAllNonExisting()
               .ForMember(dest => dest.Qty, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.QTY))
               .ForMember(dest => dest.ConvertedQty, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.CONVERTED_QTY))
               .ForMember(dest => dest.Convertion, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.CONVERTION))
               .ForMember(dest => dest.ExciseValue, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.EXCISE_VALUE))
               .ForMember(dest => dest.UsdValue, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.USD_VALUE))
               .ForMember(dest => dest.ConvertedUom, opt => opt.MapFrom(src => src.CONVERTED_UOM));
                

            #endregion

            #region Workflow History

            Mapper.CreateMap<WORKFLOW_HISTORY, WorkflowHistoryViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ACTION, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.ACTION)))
                .ForMember(dest => dest.USERNAME, opt => opt.MapFrom(src => src.USER.USERNAME))
                .ForMember(dest => dest.USER_FIRST_NAME, opt => opt.MapFrom(src => src.USER.FIRST_NAME))
                .ForMember(dest => dest.USER_LAST_NAME, opt => opt.MapFrom(src => src.USER.LAST_NAME));

            Mapper.CreateMap<WorkflowHistoryDto, WorkflowHistoryViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ACTION, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.ACTION)))
                .ForMember(dest => dest.USERNAME, opt => opt.MapFrom(src => src.USER.USERNAME))
                .ForMember(dest => dest.USER_FIRST_NAME, opt => opt.MapFrom(src => src.USER.FIRST_NAME))
                .ForMember(dest => dest.USER_LAST_NAME, opt => opt.MapFrom(src => src.USER.LAST_NAME));


            #endregion
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