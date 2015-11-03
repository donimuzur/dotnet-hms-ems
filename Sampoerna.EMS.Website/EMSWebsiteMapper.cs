using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Website.Models.BrandRegistration;
using Sampoerna.EMS.Website.Models.ChangesHistory;
using Sampoerna.EMS.Website.Models.GOODSTYPE;
using Sampoerna.EMS.Website.Models.HeaderFooter;
using Sampoerna.EMS.Website.Models.LACK1;
using Sampoerna.EMS.Website.Models.NPPBKC;
using Sampoerna.EMS.Website.Models.PLANT;
using Sampoerna.EMS.Website.Models.PlantReceiveMaterial;
using Sampoerna.EMS.Website.Models.POA;
using Sampoerna.EMS.Website.Models.PrintHistory;
using Sampoerna.EMS.Website.Models.PRODUCTION;
using Sampoerna.EMS.Website.Models.UOM;
using Sampoerna.EMS.Website.Models.UserAuthorization;
using Sampoerna.EMS.Website.Models.UserPlantMap;
using Sampoerna.EMS.Website.Models.VirtualMappingPlant;
using Sampoerna.EMS.Website.Models.Material;
using Sampoerna.EMS.Website.Models.Waste;
using Sampoerna.EMS.Website.Models.WorkflowHistory;
using Sampoerna.EMS.Website.Models.Settings;
using Sampoerna.EMS.Website.Models.WorkflowSetting;
using Sampoerna.EMS.Website.Models.EmailTemplate;
using Sampoerna.EMS.Website.Models.LACK2;

namespace Sampoerna.EMS.Website
{
    public partial class EMSWebsiteMapper
    {
        public static void Initialize()
        {
            InitializeCK5();
            InitializePBCK1();
            InitializePbck7AndPbck3();
            InitializeLACK1();
            InitializeCk4C();
            InitializePBCK4();
            InitializeLACK2();
            //AutoMapper
            Mapper.CreateMap<USER, Login>().IgnoreAllNonExisting()
                .ForMember(dest => dest.USER_ID, opt => opt.MapFrom(src => src.USER_ID))
                .ForMember(dest => dest.FIRST_NAME, opt => opt.MapFrom(src => src.FIRST_NAME));

            ;

            Mapper.CreateMap<UserTree, UserItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IS_ACTIVE.HasValue && src.IS_ACTIVE.Value))
                ;

            #region Company
            Mapper.CreateMap<T001, CompanyDetail>().IgnoreAllNonExisting()
            .ForMember(dest => dest.DocumentBukrs, opt => opt.MapFrom(src => src.BUKRS))
            .ForMember(dest => dest.DocumentBukrstxt, opt => opt.MapFrom(src => src.BUTXT))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE));
            #endregion

            Mapper.CreateMap<BrandRegistrationOutput, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.FaCode))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.FaCode));

            Mapper.CreateMap<ZAIDM_EX_MATERIAL, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.STICKER_CODE))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.STICKER_CODE + " - " + src.MATERIAL_DESC));


            Mapper.CreateMap<POA, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.POA_ID))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.PRINTED_NAME));

            Mapper.CreateMap<POADto, SelectItemModel>().IgnoreAllNonExisting()
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

            Mapper.CreateMap<T001W, SelectItemModel>().IgnoreAllNonExisting()
                  .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.WERKS + "-" + src.NAME1))
                  .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.WERKS));


            //Mapper.CreateMap<Plant, SelectItemModel>().IgnoreAllNonExisting()
            //    .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.WERKS + "-" + src.NAME1))
            //    .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.WERKS));
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
                .ForMember(dest => dest.NPPBKC_IMPORT_ID, opt => opt.MapFrom(src => src.NPPBKC_IMPORT_ID))
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
                .ForMember(dest => dest.NPPBKC_IMPORT_ID, opt => opt.MapFrom(src => src.NPPBKC_IMPORT_ID))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.PLANT_RECEIVE_MATERIAL,
                    opt => opt.MapFrom(src => Mapper.Map<List<PLANT_RECEIVE_MATERIAL>>(src.ReceiveMaterials)));



            Mapper.CreateMap<T001W, DetailPlantT1001W>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.Werks, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.PlantDescription, opt => opt.MapFrom(src => src.NAME1))
                .ForMember(dest => dest.IsMainPlant, opt => opt.MapFrom(src => src.IS_MAIN_PLANT.HasValue && src.IS_MAIN_PLANT.Value))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.ADDRESS))
                .ForMember(dest => dest.Name1, opt => opt.MapFrom(src => src.NAME1))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PHONE))
                .ForMember(dest => dest.Ort01, opt => opt.MapFrom(src => src.ORT01))
                .ForMember(dest => dest.NPPBKC_IMPORT_ID, opt => opt.MapFrom(src => src.NPPBKC_IMPORT_ID))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IS_DELETED));


            Mapper.CreateMap<DetailPlantT1001W, T001W>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.NPPBKC_IMPORT_ID, opt => opt.MapFrom(src => src.NPPBKC_IMPORT_ID))
                .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.Werks))
                .ForMember(dest => dest.NAME1, opt => opt.MapFrom(src => src.Name1))
                .ForMember(dest => dest.IS_MAIN_PLANT, opt => opt.MapFrom(src => src.IsMainPlant))
                .ForMember(dest => dest.ADDRESS, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.SKEPTIS, opt => opt.MapFrom(src => src.Skeptis))
                .ForMember(dest => dest.PHONE, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.ORT01, opt => opt.MapFrom(src => src.Ort01))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.IS_DELETED, opt => opt.MapFrom(src => src.IsDeleted));

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
                .ForMember(dest => dest.POA_ID, opt => opt.MapFrom(src => src.UserId))
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

            Mapper.CreateMap<ExGoodTyp, DetailsGoodsTypGroup>().IgnoreAllNonExisting()
                .ForMember(dest => dest.GoodsTypeId, opt => opt.MapFrom(src => src.EX_GROUP_TYPE_ID))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GROUP_NAME))
                .ForMember(dest => dest.Inactive, opt => opt.MapFrom(src => src.Inactive ? "Yes" : "No"));



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

            Mapper.CreateMap<T001W, SelectItemModelVirtualPlant>().IgnoreAllNonExisting()
             .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.WERKS + "-" + src.NAME1))
             .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.WERKS));

            Mapper.CreateMap<VirtualMappingPlantCreateViewModel, VIRTUAL_PLANT_MAP>().IgnoreAllNonExisting()
                .ForMember(dest => dest.COMPANY_ID, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.IMPORT_PLANT_ID, opt => opt.MapFrom(src => src.ImportPlantId))
                .ForMember(dest => dest.EXPORT_PLANT_ID, opt => opt.MapFrom(src => src.ExportPlantId));

            Mapper.CreateMap<VIRTUAL_PLANT_MAP, VirtualMappingPlantDetailsViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IS_DELETED))
                .ForMember(dest => dest.ImportPlanName, opt => opt.MapFrom(src => src.T001W1.WERKS + "-" + src.T001W.NAME1))
                .ForMember(dest => dest.ExportPlanName, opt => opt.MapFrom(src => src.T001W.WERKS + "-" + src.T001W.NAME1));

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
                .ForMember(dest => dest.IsDeleted, opt => opt.ResolveUsing<NullableBooleanToStringDeletedResolver>().FromMember(src => src.IS_DELETED))
                .ForMember(dest => dest.IsActive, opt => opt.ResolveUsing<NullableBooleanToStringDeletedResolver>().FromMember(src => src.STATUS))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE));


            Mapper.CreateMap<ZAIDM_EX_BRAND, BrandRegistrationDetailsViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.StickerCode, opt => opt.MapFrom(src => src.STICKER_CODE))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FA_CODE))
                .ForMember(dest => dest.PersonalizationCode, opt => opt.MapFrom(src => src.PER_CODE))
                .ForMember(dest => dest.PersonalizationCodeDescription,
                    opt => opt.MapFrom(src => src.PER_CODE_DESC))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.BRAND_CE))
                .ForMember(dest => dest.SkepNo, opt => opt.MapFrom(src => src.SKEP_NO))
                .ForMember(dest => dest.SkepDate, opt => opt.MapFrom(src => src.SKEP_DATE))
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.PROD_CODE))
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
                .ForMember(dest => dest.IsFromSap, opt => opt.MapFrom(src => src.IS_FROM_SAP))
                .ForMember(dest => dest.BoolIsDeleted, opt => opt.MapFrom(src => src.IS_DELETED))
                .ForMember(dest => dest.Conversion, opt => opt.MapFrom(src => src.CONVERSION))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.BRAND_CONTENT))
                .ForMember(dest => dest.HjeValueStr, opt => opt.MapFrom(src => src.HJE_IDR))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.STATUS));


            Mapper.CreateMap<ZAIDM_EX_BRAND, BrandRegistrationEditViewModel>().IgnoreAllNonExisting()
            .ForMember(dest => dest.StickerCode, opt => opt.MapFrom(src => src.STICKER_CODE))
            .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.WERKS))
            .ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FA_CODE))
            .ForMember(dest => dest.PersonalizationCode, opt => opt.MapFrom(src => src.PER_CODE))
            .ForMember(dest => dest.PersonalizationCodeDescription, opt => opt.MapFrom(src => src.PER_CODE_DESC))
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
            .ForMember(dest => dest.CutFillerCode, opt => opt.MapFrom(src => src.CUT_FILLER_CODE))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS == true ? "Active" : "Inactive"))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.STATUS))
            .ForMember(dest => dest.IsFromSAP, opt => opt.MapFrom(src => src.IS_FROM_SAP))
            .ForMember(dest => dest.Conversion, opt => opt.MapFrom(src => src.CONVERSION))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.BRAND_CONTENT))
            .ForMember(dest => dest.BoolIsDeleted, opt => opt.MapFrom(src => src.IS_DELETED))
            .ForMember(dest => dest.IsDeleted, opt => opt.ResolveUsing<NullableBooleanToStringDeletedResolver>().FromMember(src => src.IS_DELETED))
            ;

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
                .ForMember(dest => dest.CUT_FILLER_CODE, opt => opt.MapFrom(src => src.CutFillerCode))
                .ForMember(dest => dest.CONVERSION, opt => opt.MapFrom(src => src.Conversion))
                .ForMember(dest => dest.BRAND_CONTENT, opt => opt.MapFrom(src => src.Content));

            Mapper.CreateMap<BrandRegistrationEditViewModel, ZAIDM_EX_BRAND>().IgnoreAllUnmapped()
                .ForMember(dest => dest.STICKER_CODE, opt => opt.MapFrom(src => src.StickerCode))
                .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.FA_CODE, opt => opt.MapFrom(src => src.FaCode))
                .ForMember(dest => dest.PER_CODE, opt => opt.MapFrom(src => src.PersonalizationCode))
                .ForMember(dest => dest.PER_CODE_DESC, opt => opt.MapFrom(src => src.PersonalizationCodeDescription))
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
                .ForMember(dest => dest.CUT_FILLER_CODE, opt => opt.MapFrom(src => src.CutFillerCode))
                .ForMember(dest => dest.CONVERSION, opt => opt.MapFrom(src => src.Conversion))
                .ForMember(dest => dest.BRAND_CONTENT, opt => opt.MapFrom(src => src.Content));
            #endregion

            Mapper.CreateMap<CHANGES_HISTORY, ChangesHistoryItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.USERNAME,
                    opt => opt.MapFrom(src => src.USER != null ? src.USER.USER_ID : string.Empty))
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
                .ForMember(dest => dest.Is_Deleted,opt => opt.MapFrom(src => src.IS_DELETED.HasValue && src.IS_DELETED.Value ? "Yes" : "No"))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.END_DATE))
                .ForMember(dest => dest.FlagForLack1, opt => opt.MapFrom(src => src.FLAG_FOR_LACK1.HasValue));

            Mapper.CreateMap<VirtualNppbckDetails, ZAIDM_EX_NPPBKC>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.VirtualNppbckId))
                .ForMember(dest => dest.KPPBC_ID, opt => opt.MapFrom(src => src.KppbcId))
                .ForMember(dest => dest.REGION_DGCE, opt => opt.MapFrom(src => src.RegionOfficeOfDGCE))
                .ForMember(dest => dest.REGION, opt => opt.MapFrom(src => src.Region))
                .ForMember(dest => dest.TEXT_TO, opt => opt.MapFrom(src => src.TextTo))
                .ForMember(dest => dest.CITY_ALIAS, opt => opt.MapFrom(src => src.CityAlias));
                


            #endregion


            #region Material
            Mapper.CreateMap<MaterialDto, MaterialDetails>().IgnoreAllNonExisting()
                .ForMember(dest => dest.BaseUom, opt => opt.MapFrom(src => src.BASE_UOM_ID))
                .ForMember(dest => dest.UomName, opt => opt.MapFrom(src => src.UOM.UOM_DESC))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.T001W.NAME1))
                .ForMember(dest => dest.GoodtypId, opt => opt.MapFrom(src => src.EXC_GOOD_TYP))
                .ForMember(dest => dest.GoodTypeName, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC))
                .ForMember(dest => dest.MaterialNumber, opt => opt.MapFrom(src => src.STICKER_CODE))
                .ForMember(dest => dest.MaterialDesc, opt => opt.MapFrom(src => src.MATERIAL_DESC))
                .ForMember(dest => dest.PlantDeletion, opt => opt.MapFrom(src => src.PLANT_DELETION.HasValue && src.PLANT_DELETION.Value ? "yes" : "No"))
                .ForMember(dest => dest.ClientDeletion, opt => opt.MapFrom(src => src.CLIENT_DELETION.HasValue && src.CLIENT_DELETION.Value ? "yes" : "No")); ;

            Mapper.CreateMap<MaterialDto, MaterialCreateViewModel>().IgnoreAllNonExisting()
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
                .ForMember(dest => dest.Tariff, opt => opt.MapFrom(src => src.TARIFF))
                .ForMember(dest => dest.Tariff_Curr, opt => opt.MapFrom(src => src.TARIFF_CURR))
                .ForMember(dest => dest.Hje, opt => opt.MapFrom(src => src.HJE))
                .ForMember(dest => dest.Hje_Curr, opt => opt.MapFrom(src => src.HJE_CURR))
                .ForMember(dest => dest.IssueStorageLoc, opt => opt.MapFrom(src => src.ISSUE_STORANGE_LOC))
                .ForMember(dest => dest.IsPlantDelete, opt => opt.MapFrom(src => src.PLANT_DELETION))
                .ForMember(dest => dest.IsClientDelete, opt => opt.MapFrom(src => src.CLIENT_DELETION))
                .ForMember(dest => dest.IsFromSap, opt => opt.MapFrom(src => false));

            Mapper.CreateMap<MaterialDto, MaterialEditViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UomId, opt => opt.MapFrom(src => src.BASE_UOM_ID))
                .ForMember(dest => dest.UomName, opt => opt.MapFrom(src => src.UOM.UOM_DESC))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.T001W.NAME1))
                .ForMember(dest => dest.GoodTypeId, opt => opt.MapFrom(src => src.EXC_GOOD_TYP))
                .ForMember(dest => dest.GoodTypeName, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC))
                .ForMember(dest => dest.MaterialNumber, opt => opt.MapFrom(src => src.STICKER_CODE))
                .ForMember(dest => dest.MaterialDesc, opt => opt.MapFrom(src => src.MATERIAL_DESC))
                .ForMember(dest => dest.MaterialGroup, opt => opt.MapFrom(src => src.MATERIAL_GROUP))
                .ForMember(dest => dest.PurchasingGroup, opt => opt.MapFrom(src => src.PURCHASING_GROUP))
                .ForMember(dest => dest.IssueStorageLoc, opt => opt.MapFrom(src => src.ISSUE_STORANGE_LOC))
                .ForMember(dest => dest.Tariff, opt => opt.MapFrom(src => src.TARIFF))
                .ForMember(dest => dest.Tariff_Curr, opt => opt.MapFrom(src => src.TARIFF_CURR))
                .ForMember(dest => dest.Hje, opt => opt.MapFrom(src => src.HJE))
                .ForMember(dest => dest.Hje_Curr, opt => opt.MapFrom(src => src.HJE_CURR))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.IsFromSap, opt => opt.MapFrom(src => src.IS_FROM_SAP))
                .ForMember(dest => dest.IsPlantDelete, opt => opt.MapFrom(src => src.PLANT_DELETION))
                .ForMember(dest => dest.IsClientDelete, opt => opt.MapFrom(src => src.CLIENT_DELETION))

             .ForMember(dest => dest.MaterialUom, opt => opt.MapFrom(src => src.MATERIAL_UOM));

            Mapper.CreateMap<MaterialEditViewModel, MaterialDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.BASE_UOM_ID, opt => opt.MapFrom(src => src.UomId))

                .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.PlantId))

                .ForMember(dest => dest.EXC_GOOD_TYP, opt => opt.MapFrom(src => src.GoodTypeId)) //

                .ForMember(dest => dest.STICKER_CODE, opt => opt.MapFrom(src => src.MaterialNumber))
                .ForMember(dest => dest.MATERIAL_DESC, opt => opt.MapFrom(src => src.MaterialDesc))
                .ForMember(dest => dest.MATERIAL_GROUP, opt => opt.MapFrom(src => src.MaterialGroup))
                .ForMember(dest => dest.PURCHASING_GROUP, opt => opt.MapFrom(src => src.PurchasingGroup))
                .ForMember(dest => dest.ISSUE_STORANGE_LOC, opt => opt.MapFrom(src => src.IssueStorageLoc))
                .ForMember(dest => dest.TARIFF_CURR, opt => opt.MapFrom(src => src.Tariff_Curr))
                .ForMember(dest => dest.HJE_CURR, opt => opt.MapFrom(src => src.Hje_Curr))
                .ForMember(dest => dest.TARIFF, opt => opt.MapFrom(src => src.Tariff))
                .ForMember(dest => dest.HJE, opt => opt.MapFrom(src => src.Hje))
                .ForMember(dest => dest.MATERIAL_UOM, opt => opt.MapFrom(src => src.MaterialUom))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedById))
                .ForMember(dest => dest.IS_FROM_SAP, opt => opt.MapFrom(src => src.IsFromSap))
                .ForMember(dest => dest.PLANT_DELETION, opt => opt.MapFrom(src => src.IsPlantDelete))
                .ForMember(dest => dest.CLIENT_DELETION, opt => opt.MapFrom(src => src.IsClientDelete));



            Mapper.CreateMap<MaterialCreateViewModel, MaterialDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.BASE_UOM_ID, opt => opt.MapFrom(src => src.UomId))

                .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.PlantId))

                .ForMember(dest => dest.EXC_GOOD_TYP, opt => opt.MapFrom(src => src.GoodTypeId))

                .ForMember(dest => dest.STICKER_CODE, opt => opt.MapFrom(src => src.MaterialNumber))
                .ForMember(dest => dest.MATERIAL_DESC, opt => opt.MapFrom(src => src.MaterialDesc))
                .ForMember(dest => dest.MATERIAL_GROUP, opt => opt.MapFrom(src => src.MaterialGroup))
                .ForMember(dest => dest.PURCHASING_GROUP, opt => opt.MapFrom(src => src.PurchasingGroup))
                .ForMember(dest => dest.ISSUE_STORANGE_LOC, opt => opt.MapFrom(src => src.IssueStorageLoc))
                .ForMember(dest => dest.TARIFF_CURR, opt => opt.MapFrom(src => src.Tariff_Curr))
                .ForMember(dest => dest.HJE_CURR, opt => opt.MapFrom(src => src.Hje_Curr))
                .ForMember(dest => dest.TARIFF, opt => opt.MapFrom(src => src.Tariff))
                .ForMember(dest => dest.HJE, opt => opt.MapFrom(src => src.Hje))
                .ForMember(dest => dest.IS_FROM_SAP, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.MATERIAL_UOM, opt => opt.MapFrom(src => src.MaterialUom));




            Mapper.CreateMap<MaterialDto, MaterialDetailViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UomName, opt => opt.MapFrom(src => src.BASE_UOM_ID))
                  .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.T001W.WERKS))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.T001W.WERKS + "-" + src.T001W.NAME1))
                .ForMember(dest => dest.GoodTypeName, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC))
                .ForMember(dest => dest.MaterialGroup, opt => opt.MapFrom(src => src.MATERIAL_GROUP))
                .ForMember(dest => dest.PurchasingGroup, opt => opt.MapFrom(src => src.PURCHASING_GROUP))
                .ForMember(dest => dest.IssueStorageLoc, opt => opt.MapFrom(src => src.ISSUE_STORANGE_LOC))
                .ForMember(dest => dest.Tariff, opt => opt.MapFrom(src => src.TARIFF))
                .ForMember(dest => dest.Tariff_Curr, opt => opt.MapFrom(src => src.TARIFF_CURR))
                .ForMember(dest => dest.Hje, opt => opt.MapFrom(src => src.HJE))
                .ForMember(dest => dest.Hje_Curr, opt => opt.MapFrom(src => src.HJE_CURR))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreatedBy,
                    opt => opt.MapFrom(src => src.CREATED_BY))
                //.ForMember(dest => dest.ChangedDate, opt => opt.MapFrom(src => src.MOD))
                .ForMember(dest => dest.MaterialNumber, opt => opt.MapFrom(src => src.STICKER_CODE))
                .ForMember(dest => dest.MaterialDesc, opt => opt.MapFrom(src => src.MATERIAL_DESC))
                .ForMember(dest => dest.IsFromSap, opt => opt.MapFrom(src => src.IS_FROM_SAP))
                .ForMember(dest => dest.MaterialUom, opt => opt.MapFrom(src => src.MATERIAL_UOM))
                .ForMember(dest => dest.IsPlantDelete, opt => opt.MapFrom(src => src.PLANT_DELETION))
                .ForMember(dest => dest.IsClientDelete, opt => opt.MapFrom(src => src.CLIENT_DELETION));


            Mapper.CreateMap<MATERIAL_UOM, MaterialUomDetails>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.MATERIAL_UOM_ID))
                .ForMember(dest => dest.MaterialNumber, opt => opt.MapFrom(src => src.STICKER_CODE))
                .ForMember(dest => dest.Plant, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.Meinh, opt => opt.MapFrom(src => src.MEINH))
                .ForMember(dest => dest.UmrenStr, opt => opt.MapFrom(src => src.UMREN))
                .ForMember(dest => dest.Umren, opt => opt.MapFrom(src => src.UMREN))
                .ForMember(dest => dest.Umrez, opt => opt.MapFrom(src => src.UMREZ));
            Mapper.CreateMap<MaterialUomDetails, MATERIAL_UOM>().IgnoreAllNonExisting()
               .ForMember(dest => dest.MATERIAL_UOM_ID, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.STICKER_CODE, opt => opt.MapFrom(src => src.MaterialNumber))
               .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.Plant))
               .ForMember(dest => dest.MEINH, opt => opt.MapFrom(src => src.Meinh))
               .ForMember(dest => dest.UMREN, opt => opt.MapFrom(src => src.Umren))
               .ForMember(dest => dest.UMREZ, opt => opt.MapFrom(src => src.Umrez));

            #endregion

            #region UOM
            Mapper.CreateMap<UOM, UomDetailViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UomId, opt => opt.MapFrom(src => src.UOM_ID))
                .ForMember(dest => dest.UomName, opt => opt.MapFrom(src => src.UOM_DESC))
                .ForMember(dest => dest.IsEms, opt => opt.MapFrom(src => src.IS_EMS));

            Mapper.CreateMap<UOM, UomDetails>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UomId, opt => opt.MapFrom(src => src.UOM_ID))
                .ForMember(dest => dest.UomName, opt => opt.MapFrom(src => src.UOM_DESC))
                .ForMember(dest => dest.IsEmsString, opt => opt.MapFrom(src => src.IS_EMS.HasValue ? src.IS_EMS.Value ? "Yes" : "No" : "No"));

            Mapper.CreateMap<UomDetailViewModel, UOM>().IgnoreAllNonExisting()
               .ForMember(dest => dest.UOM_ID, opt => opt.MapFrom(src => src.UomId))
               .ForMember(dest => dest.UOM_DESC, opt => opt.MapFrom(src => src.UomName))
               .ForMember(dest => dest.IS_EMS, opt => opt.MapFrom(src => src.IsEms));




            #endregion



            Mapper.CreateMap<DOC_NUMBER_SEQ, DocumentSequenceModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.LastSequence, opt => opt.MapFrom(src => src.DOC_NUMBER_SEQ_LAST))
                .ForMember(dest => dest.MonthInt, opt => opt.MapFrom(src => src.MONTH))
                .ForMember(dest => dest.MonthName_Eng, opt => opt.MapFrom(src => src.MONTH1.MONTH_NAME_ENG))
                .ForMember(dest => dest.MonthName_Ind, opt => opt.MapFrom(src => src.MONTH1.MONTH_NAME_IND))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.YEAR));


            #region Workflow History
            Mapper.CreateMap<PAGE, WorkflowDetails>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Form_Id, opt => opt.MapFrom(src => src.PAGE_ID))
                .ForMember(dest => dest.Modul, opt => opt.MapFrom(src => src.MENU_NAME));

            Mapper.CreateMap<WORKFLOW_STATE, WorkflowMappingDetails>().IgnoreAllNonExisting()
                //.ForMember(dest => dest.StateMappingId, opt => opt.MapFrom(src => src.ACTION_ID))
                //.ForMember(dest => dest.State, opt => opt.MapFrom(src => src.ACTION_NAME))
                .ForMember(dest => dest.EmailTemplateId, opt => opt.MapFrom(src => src.EMAIL_TEMPLATE_ID))
                .ForMember(dest => dest.EmailTemplateName, opt => opt.MapFrom(src => src.EMAIL_TEMPLATE.TEMPLATE_NAME))
                .ForMember(dest => dest.ListUser, opt => opt.MapFrom(src => src.WORKFLOW_STATE_USERS));

            Mapper.CreateMap<WorkflowMappingDetails, WORKFLOW_STATE>().IgnoreAllNonExisting()
                //.ForMember(dest => dest.ACTION_ID, opt => opt.MapFrom(src => src.StateMappingId))
                //.ForMember(dest => dest.ACTION_NAME, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.EMAIL_TEMPLATE_ID, opt => opt.MapFrom(src => src.EmailTemplateId));
            //.ForMember(dest => dest.ListUser, opt => opt.MapFrom(src => src.USER));

            Mapper.CreateMap<USER, WorkflowUsers>().IgnoreAllNonExisting()
                .ForMember(dest => dest.User_Id, opt => opt.MapFrom(src => src.USER_ID))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EMAIL));

            Mapper.CreateMap<WorkflowHistoryDto, WorkflowHistoryViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ACTION, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.ACTION)))
                .ForMember(dest => dest.USERNAME, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.USER_FIRST_NAME, opt => opt.MapFrom(src => src.UserFirstName))
                .ForMember(dest => dest.USER_LAST_NAME, opt => opt.MapFrom(src => src.UserLastName));

            #endregion


            Mapper.CreateMap<USER, UserItem>().IgnoreAllNonExisting();

            Mapper.CreateMap<T001WDto, T001WModel>().IgnoreAllNonExisting();

            #region Email Template
            Mapper.CreateMap<EMAIL_TEMPLATE, EmailTemplateModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.EmailTemplateId,
                    opt => opt.MapFrom(src => src.EMAIL_TEMPLATE_ID))
                .ForMember(dest => dest.EmailTemplateName,
                    opt => opt.MapFrom(src => src.TEMPLATE_NAME))
                .ForMember(dest => dest.EmailTemplateSubject,
                    opt => opt.MapFrom(src => src.SUBJECT))
                .ForMember(dest => dest.EmailTemplateBody,
                    opt => opt.MapFrom(src => src.BODY));
            //.ForMember(dest => dest.;

            Mapper.CreateMap<EmailTemplateModel, EMAIL_TEMPLATE>().IgnoreAllNonExisting()
                .ForMember(dest => dest.EMAIL_TEMPLATE_ID,
                    opt => opt.MapFrom(src => src.EmailTemplateId))
                .ForMember(dest => dest.TEMPLATE_NAME,
                    opt => opt.MapFrom(src => src.EmailTemplateName))
                .ForMember(dest => dest.SUBJECT,
                    opt => opt.MapFrom(src => src.EmailTemplateSubject))
                .ForMember(dest => dest.BODY,
                    opt => opt.MapFrom(src => src.EmailTemplateBody));
            #endregion

            #region User Authorization

            Mapper.CreateMap<BRoleDto, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.Id + Constans.DelimeterSelectItem + src.Description))
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.Id));


            Mapper.CreateMap<UserAuthorizationDto, DetailIndexUserAuthorization>().IgnoreAllNonExisting();

            Mapper.CreateMap<UserAuthorizationDto, EditUserAuthorizationViewModel>().IgnoreAllNonExisting();



            #endregion



            #region Lack2

            Mapper.CreateMap<Lack2Dto, LACK2NppbkcData>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Butxt))
                .ForMember(dest => dest.TobaccoGoodType, opt => opt.MapFrom(src => src.ExGoodTyp + "-" + src.ExTypDesc))
                .ForMember(dest => dest.Supplier, opt => opt.MapFrom(src => src.LevelPlantName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
                .ForMember(dest => dest.Nppbkc, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.Period, opt => opt.MapFrom(src => src.PeriodYear + "-" + src.PeriodMonth.ToString("00")));

            Mapper.CreateMap<Lack2Dto, LACK2PlantData>().IgnoreAllNonExisting()
               .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Butxt))
               .ForMember(dest => dest.TobaccoGoodType, opt => opt.MapFrom(src => src.ExGoodTyp + "-" + src.ExTypDesc))
               .ForMember(dest => dest.Supplier, opt => opt.MapFrom(src => src.LevelPlantName))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
               .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.LevelPlantId))
               .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Status)))
               .ForMember(dest => dest.Period, opt => opt.MapFrom(src => src.PeriodYear + "-" + src.PeriodMonth.ToString("00")))
               ;

            Mapper.CreateMap<LACK2Model, Lack2Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Lack2Number, opt => opt.MapFrom(src => src.Lack2Number))
                .ForMember(dest => dest.GovStatus, opt => opt.MapFrom(src => src.StatusGov))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment));


            Mapper.CreateMap<Lack2Dto, LACK2Model>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Lack2Number, opt => opt.MapFrom(src => src.Lack2Number))
                 .ForMember(dest => dest.StatusGov, opt => opt.MapFrom(src => src.GovStatus))
                .ForMember(dest => dest.ExGoodDesc, opt => opt.MapFrom(src => src.ExGoodTyp + "-" + src.ExTypDesc));


            Mapper.CreateMap<LACK2FilterViewModel, Lack2GetByParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NppbKcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.Poa, opt => opt.MapFrom(src => src.Poa))
                .ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(src => src.ReportedOn))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator));

            Mapper.CreateMap<Lack2GetByParamInput, LACK2FilterViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbKcId))
                .ForMember(dest => dest.Poa, opt => opt.MapFrom(src => src.Poa))
                .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.SubmissionDate))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator));

            #endregion

            Mapper.CreateMap<PrintHistoryDto, PrintHistoryItemModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<ZAIDM_EX_NPPBKCDto, NppbkcItemModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<PlantDto, SelectItemModel>().IgnoreAllNonExisting()
              .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.WERKS))
              .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.NAME1));

            Mapper.CreateMap<T001WDto, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.WERKS + "-" + src.NAME1));

            //Mapper.CreateMap<Lack2, Lack2ItemDto>().IgnoreAllNonExisting()
            //.ForMember(dest => dest.Ck5Id, opt => opt.MapFrom(src => src.Ck5Id));

            #region Production For Ck4c

            Mapper.CreateMap<ProductionDto, ProductionDetail>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionDate,
                    opt => opt.MapFrom(src => src.ProductionDate.ToString("dd MMM yyyy")))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.PlantWerks + " - " + src.PlantName))
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.Qty));
            //.ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FaCode));


            Mapper.CreateMap<ProductionViewModel, ProductionGetByParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Plant, opt => opt.MapFrom(src => src.PlantWerks))
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.CompanyCode))
                .ForMember(dest => dest.ProoductionDate, opt => opt.MapFrom(src => src.ProductionDate));


            Mapper.CreateMap<ProductionDetail, ProductionDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<ProductionDto, ProductionUploadViewModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<ProductionUploadViewModel, ProductionDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<ProductionUploadItemsInput, ProductionUploadItems>().IgnoreAllNonExisting();
            //.ForMember(dest => dest.QtyPacked, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.QtyPacked))
            //.ForMember(dest => dest.Qty, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.Qty));

            Mapper.CreateMap<ProductionUploadItems, ProductionUploadItemsInput>().IgnoreAllNonExisting();

            Mapper.CreateMap<ProductionUploadItemsOutput, ProductionUploadItemsInput>().IgnoreAllNonExisting();
            Mapper.CreateMap<ProductionUploadItemsInput, ProductionUploadItemsOutput>().IgnoreAllNonExisting();

            Mapper.CreateMap<ProductionUploadItemsOutput, ProductionUploadItems>().IgnoreAllNonExisting();
            //Mapper.CreateMap<ProductionUploadViewModel, ProductionUploadItemsOutput>().IgnoreAllNonExisting();

            #endregion

            Mapper.CreateMap<ZAIDM_EX_BRAND, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.FA_CODE))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.FA_CODE));

            #region Waste For Ck4c

            Mapper.CreateMap<WasteDto, WasteDetail>().IgnoreAllNonExisting()
                .ForMember(dest => dest.RejectCigaretteStick,
                    opt => opt.MapFrom(src => src.PackerRejectStickQty))
                .ForMember(dest => dest.WasteQtyGram,
                    opt => opt.MapFrom(src => src.DustWasteGramQty + src.FloorWasteGramQty))
                .ForMember(dest => dest.WasteQtyStick,
                    opt => opt.MapFrom(src => src.DustWasteStickQty + src.FloorWasteStickQty))
                .ForMember(dest => dest.WasteProductionDate,
                    opt => opt.MapFrom(src => src.WasteProductionDate.ToString("dd MMM yyyy")))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.PlantWerks + " - " + src.PlantName));



            Mapper.CreateMap<WasteViewModel, WasteGetByParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Plant, opt => opt.MapFrom(src => src.PlantWerks))
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.CompanyCode))
                .ForMember(dest => dest.WasteProductionDate, opt => opt.MapFrom(src => src.WasteProductionDate));

            Mapper.CreateMap<WasteDetail, WasteDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<WasteUploadViewModel, WasteDto>().IgnoreAllNonExisting()
            .ForMember(dest => dest.UploadItems, opt => opt.MapFrom(src => src.UploadItems));

            Mapper.CreateMap<WasteUploadItems, WasteUploadItemsInput>().IgnoreAllNonExisting();

            Mapper.CreateMap<WasteUploadItemsInput, WasteUploadItemsOuput>().IgnoreAllNonExisting();

            Mapper.CreateMap<WasteUploadItemsOuput, WasteUploadItems>().IgnoreAllNonExisting();

            #endregion


            Mapper.CreateMap<COUNTRY, SelectItemModel>().IgnoreAllNonExisting()
             .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.COUNTRY_CODE))
             .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.COUNTRY_CODE + "-" + src.COUNTRY_NAME));

            Mapper.CreateMap<CK5ExternalSupplierDto, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.SUPPLIER_PLANT))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.SUPPLIER_PLANT));

            #region User Plant Map
            Mapper.CreateMap<UserPlantMapDetail, UserPlantMapDto>().IgnoreAllNonExisting();
            Mapper.CreateMap<UserPlantMapDto, UserPlantMapDetail>().IgnoreAllNonExisting();

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