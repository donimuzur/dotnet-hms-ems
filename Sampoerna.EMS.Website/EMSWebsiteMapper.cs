using System;
using System.Collections.Generic;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
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
               .ForMember(dest => dest.Is_Deleted, opt => opt.MapFrom(src => src.IS_DELETED == null  ? "No" : "Yes"))
               .ForMember(dest => dest.Manager, opt => opt.MapFrom(src => src.USER1))
               .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.USER))
               .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.USER_ID))
                .ForMember(dest => dest.ManagerId, opt => opt.MapFrom(src => src.MANAGER_ID))
               .ForMember(dest => dest.IsFromSAP, opt => opt.MapFrom(src => src.IS_FROM_SAP.HasValue && src.IS_FROM_SAP.Value));

            Mapper.CreateMap<POAViewDetailModel, ZAIDM_EX_POA>().IgnoreAllUnmapped()
               .ForMember(dest => dest.POA_ID_CARD, opt => opt.MapFrom(src => src.PoaIdCard))
               .ForMember(dest => dest.POA_CODE, opt => opt.MapFrom(src => src.PoaCode))
               .ForMember(dest => dest.POA_PRINTED_NAME, opt => opt.MapFrom(src => src.PoaPrintedName))
               .ForMember(dest => dest.POA_PHONE, opt => opt.MapFrom(src => src.PoaPhone))
               .ForMember(dest => dest.POA_ADDRESS, opt => opt.MapFrom(src => src.PoaAddress))
               .ForMember(dest => dest.POA_ID, opt => opt.MapFrom(src => src.PoaId))
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
        }
    }
}