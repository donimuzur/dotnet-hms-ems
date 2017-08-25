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
using Sampoerna.EMS.Website.Models.WasteRole;
using Sampoerna.EMS.Website.Models.WasteStock;
using Sampoerna.EMS.Website.Models.XmlFileManagement;
using Sampoerna.EMS.Website.Models.XmlLog;
using Sampoerna.EMS.Website.Models.PoaDelegation;
using Sampoerna.EMS.Website.Models.SchedulerSetting;
using Sampoerna.EMS.Website.Models.Reversal;
using Sampoerna.EMS.Website.Models.ProductType;
using Sampoerna.EMS.Website.Models.Shared;
//using Sampoerna.EMS.Website.Models.FinanceRatio;
using Sampoerna.EMS.Website.Models.Configuration;
using Sampoerna.EMS.Website.Models.FinanceRatio;
using Sampoerna.EMS.Website.Models.Tariff;
using Sampoerna.EMS.Website.Models.SupportDoc;
using Sampoerna.EMS.Website.Models.BrandRegistrationTransaction;
using Sampoerna.EMS.Website.Models.FileUpload;
using Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.ProductDevelopment;
using Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.ProductDevelopment;
using Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.BrandRegistration;
using Sampoerna.EMS.Website.Models.BrandRegistrationTransaction.MapSKEP;
using Sampoerna.EMS.Website.Models.ProductDevUpload;
using Sampoerna.EMS.Website.Models.Market;
using Sampoerna.EMS.Website.Models.Country;
//using Sampoerna.EMS.Website.Models.POAExciser;
using Sampoerna.EMS.Website.Models.MonthClosing;
using Sampoerna.EMS.Website.Models.MasterDataApproval;
using Sampoerna.EMS.Website.Models.MasterDataApprovalSetting;

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
            InitializeLACK10();
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

            Mapper.CreateMap<ZAIDM_EX_KPPBCDto, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.KPPBC_ID))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.KPPBC_ID));

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

            Mapper.CreateMap<ZAAP_SHIFT_RPT, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.ZAAP_SHIFT_RPT_ID))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => string.Format("{0} - {1} - {2} - {3}", src.POSTING_DATE.Value.ToString("dd MMM yyyy"), (src.QTY.Value * -1), src.ORDR, src.MATDOC)));

            Mapper.CreateMap<INVENTORY_MOVEMENT, SelectItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.INVENTORY_MOVEMENT_ID))
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => string.Format("{0} - {1} - {2}", src.POSTING_DATE.Value.ToString("dd MMM yyyy"), (src.QTY.Value * -1), src.ORDR)));


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
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IS_DELETED))
                ;


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
                .ForMember(dest => dest.TariffValueStr, opt => opt.MapFrom(src => src.TARIFF))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.STATUS))
                .ForMember(dest => dest.BahanKemasan, opt => opt.MapFrom(src => src.BAHAN_KEMASAN))
                .ForMember(dest => dest.IsPackedAdjusted, opt => opt.MapFrom(src => src.PACKED_ADJUSTED));


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
            .ForMember(dest => dest.BahanKemasan, opt => opt.MapFrom(src => src.BAHAN_KEMASAN))
            .ForMember(dest => dest.IsPackedAdjusted, opt => opt.MapFrom(src => src.PACKED_ADJUSTED));

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
                .ForMember(dest => dest.BRAND_CONTENT, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.IS_FROM_SAP, opt => opt.MapFrom(src => src.IsFromSAP));

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
                .ForMember(dest => dest.BRAND_CONTENT, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.BAHAN_KEMASAN, opt => opt.MapFrom(src => src.BahanKemasan))
                .ForMember(dest => dest.PACKED_ADJUSTED, opt => opt.MapFrom(src => src.IsPackedAdjusted))
                .ForMember(dest => dest.IS_FROM_SAP, opt => opt.MapFrom(src => src.IsFromSAP));
            Mapper.CreateMap<BrandRegistrationEditViewModel, BrandRegistrationCreateViewModel>().IgnoreAllNonExisting();
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
                .ForMember(dest => dest.Is_Deleted, opt => opt.MapFrom(src => src.IS_DELETED.HasValue && src.IS_DELETED.Value ? "Yes" : "No"))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.END_DATE))
                .ForMember(dest => dest.FlagForLack1, opt => opt.MapFrom(src => src.FLAG_FOR_LACK1.HasValue && src.FLAG_FOR_LACK1.Value));

            Mapper.CreateMap<VirtualNppbckDetails, ZAIDM_EX_NPPBKC>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.VirtualNppbckId))
                .ForMember(dest => dest.KPPBC_ID, opt => opt.MapFrom(src => src.KppbcId))
                .ForMember(dest => dest.REGION_DGCE, opt => opt.MapFrom(src => src.RegionOfficeOfDGCE))
                .ForMember(dest => dest.REGION, opt => opt.MapFrom(src => src.Region))
                .ForMember(dest => dest.TEXT_TO, opt => opt.MapFrom(src => src.TextTo))
                .ForMember(dest => dest.CITY_ALIAS, opt => opt.MapFrom(src => src.CityAlias))
                .ForMember(dest => dest.FLAG_FOR_LACK1, opt => opt.MapFrom(src => src.FlagForLack1));



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
                .ForMember(dest => dest.ClientDeletion, opt => opt.MapFrom(src => src.CLIENT_DELETION.HasValue && src.CLIENT_DELETION.Value ? "yes" : "No"));

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

            Mapper.CreateMap<ZAIDM_EX_MATERIAL, MaterialDetails>().IgnoreAllNonExisting()
                .ForMember(dest => dest.BaseUom, opt => opt.MapFrom(src => src.BASE_UOM_ID))
                .ForMember(dest => dest.UomName, opt => opt.MapFrom(src => src.UOM.UOM_DESC))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.T001W.NAME1))
                .ForMember(dest => dest.GoodtypId, opt => opt.MapFrom(src => src.EXC_GOOD_TYP))
                .ForMember(dest => dest.GoodTypeName, opt => opt.MapFrom(src => src.ZAIDM_EX_GOODTYP.EXT_TYP_DESC))
                .ForMember(dest => dest.MaterialNumber, opt => opt.MapFrom(src => src.STICKER_CODE))
                .ForMember(dest => dest.MaterialDesc, opt => opt.MapFrom(src => src.MATERIAL_DESC))
                .ForMember(dest => dest.PlantDeletion, opt => opt.MapFrom(src => src.PLANT_DELETION.HasValue && src.PLANT_DELETION.Value ? "yes" : "No"))
                .ForMember(dest => dest.ClientDeletion, opt => opt.MapFrom(src => src.CLIENT_DELETION.HasValue && src.CLIENT_DELETION.Value ? "yes" : "No"));

            Mapper.CreateMap<MaterialSearchView, MaterialInput>().IgnoreAllNonExisting();
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


            Mapper.CreateMap<USER, UserItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE.Value == 1 ? "Yes" : "No"))
                .ForMember(dest => dest.IsMasterApprover, opt => opt.MapFrom(src => src.IS_MASTER_DATA_APPROVER.HasValue && src.IS_MASTER_DATA_APPROVER.Value ? "Yes" : "No"));
            Mapper.CreateMap<UserItem, USER>().IgnoreAllNonExisting().
                ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IS_ACTIVE ? 1 : 0)); ;

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
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.Qty))
                .ForMember(dest => dest.Zb, opt => opt.MapFrom(src => src.Zb))
                .ForMember(dest => dest.PackedAdjusted, opt => opt.MapFrom(src => src.PackedAdjusted))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark));
            //.ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FaCode));


            Mapper.CreateMap<ProductionViewModel, ProductionGetByParamInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Plant, opt => opt.MapFrom(src => src.PlantWerks))
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.CompanyCode))
                .ForMember(dest => dest.ProoductionDate, opt => opt.MapFrom(src => src.ProductionDate))
                .ForMember(dest => dest.Month, opt => opt.MapFrom(src => Convert.ToInt16(src.Month)))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => Convert.ToInt16(src.Year)));


            Mapper.CreateMap<ProductionDetail, ProductionDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Zb, opt => opt.MapFrom(src => src.Zb))
                .ForMember(dest => dest.PackedAdjusted, opt => opt.MapFrom(src => src.PackedAdjusted))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark));

            Mapper.CreateMap<ProductionDto, ProductionUploadViewModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<ProductionUploadViewModel, ProductionDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<ProductionDto, PRODUCTION>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ZB, opt => opt.MapFrom(src => src.Zb))
                .ForMember(dest => dest.PACKED_ADJUSTED, opt => opt.MapFrom(src => src.PackedAdjusted))
                .ForMember(dest => dest.REMARK, opt => opt.MapFrom(src => src.Remark));

            Mapper.CreateMap<PRODUCTION, ProductionDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Zb, opt => opt.MapFrom(src => src.ZB))
                .ForMember(dest => dest.PackedAdjusted, opt => opt.MapFrom(src => src.PACKED_ADJUSTED))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.REMARK));

            Mapper.CreateMap<ProductionUploadItemsInput, ProductionUploadItems>().IgnoreAllNonExisting();
            //.ForMember(dest => dest.QtyPacked, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.QtyPacked))
            //.ForMember(dest => dest.Qty, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.Qty));

            Mapper.CreateMap<ProductionUploadItems, ProductionUploadItemsInput>().IgnoreAllNonExisting();

            Mapper.CreateMap<ProductionUploadItemsOutput, ProductionUploadItemsInput>().IgnoreAllNonExisting();
            Mapper.CreateMap<ProductionUploadItemsInput, ProductionUploadItemsOutput>().IgnoreAllNonExisting();

            Mapper.CreateMap<ProductionUploadItemsOutput, ProductionUploadItems>().IgnoreAllNonExisting();
            //Mapper.CreateMap<ProductionUploadViewModel, ProductionUploadItemsOutput>().IgnoreAllNonExisting();

            Mapper.CreateMap<PRODUCTION, ProductionDetail>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionDate,
                    opt => opt.MapFrom(src => src.PRODUCTION_DATE.ToString("dd MMM yyyy")))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.WERKS + " - " + src.PLANT_NAME))
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.QTY))
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.COMPANY_CODE))
                .ForMember(dest => dest.PlantWerks, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FA_CODE))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.COMPANY_NAME))
                .ForMember(dest => dest.BrandDescription, opt => opt.MapFrom(src => src.BRAND_DESC))
                .ForMember(dest => dest.QtyPacked, opt => opt.MapFrom(src => src.QTY_PACKED))
                .ForMember(dest => dest.Uom, opt => opt.MapFrom(src => src.UOM))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.Zb, opt => opt.MapFrom(src => src.ZB))
                .ForMember(dest => dest.PackedAdjusted, opt => opt.MapFrom(src => src.PACKED_ADJUSTED))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.REMARK));

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
                .ForMember(dest => dest.WasteProductionDate, opt => opt.MapFrom(src => src.WasteProductionDate))
                .ForMember(dest => dest.Month, opt => opt.MapFrom(src => Convert.ToInt16(src.Month)))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => Convert.ToInt16(src.Year)));

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
                .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.SUPPLIER_PLANT + " - " + src.SUPPLIER_COMPANY));

            #region User Plant Map
            Mapper.CreateMap<UserPlantMapDetail, UserPlantMapDto>().IgnoreAllNonExisting();
            Mapper.CreateMap<UserPlantMapDto, UserPlantMapDetail>().IgnoreAllNonExisting();

            #endregion


            #region WasteRole

            Mapper.CreateMap<USER, WasteRoleFormViewModel>().IgnoreAllNonExisting()
              .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FIRST_NAME))
              .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LAST_NAME))
              .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EMAIL))
              .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PHONE));

            Mapper.CreateMap<WasteRoleFormViewModel, WasteRoleDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.WASTE_ROLE_ID, opt => opt.MapFrom(src => src.WasteRoleId))
                .ForMember(dest => dest.USER_ID, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.GROUP_ROLE, opt => opt.MapFrom(src => src.WasteGroup))
                .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => Mapper.Map<List<WasteRoleDetailsDto>>(src.Details)))
                ;

            Mapper.CreateMap<WasteRoleDto, WasteRoleFormViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.WasteRoleId, opt => opt.MapFrom(src => src.WASTE_ROLE_ID))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.USER_ID))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.WasteGroup, opt => opt.MapFrom(src => src.GROUP_ROLE))
                .ForMember(dest => dest.WasteGroupDescription, opt => opt.MapFrom(src => src.WasteGroupDescription))
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))

                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.PlantDescription, opt => opt.MapFrom(src => src.WERKS + "-" + src.PlantDescription))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => Mapper.Map<List<WasteRoleFormDetails>>(src.Details)))
                ;

            Mapper.CreateMap<WasteRoleDetailsDto, WasteRoleFormDetails>().IgnoreAllNonExisting()
                .ForMember(dest => dest.WasteRoleId, opt => opt.MapFrom(src => src.WASTE_ROLE_ID));

            Mapper.CreateMap<WasteRoleFormDetails, WasteRoleDetailsDto>().IgnoreAllNonExisting()
              .ForMember(dest => dest.WASTE_ROLE_ID, opt => opt.MapFrom(src => src.WasteRoleId))
              ;

            #endregion

            #region Waste Stock

            Mapper.CreateMap<WasteStockFormViewModel, WasteStockDto>().IgnoreAllNonExisting()
              .ForMember(dest => dest.WASTE_STOCK_ID, opt => opt.MapFrom(src => src.WasteStockId))
              .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.PlantId))
              .ForMember(dest => dest.MATERIAL_NUMBER, opt => opt.MapFrom(src => src.MaterialNumber))
              .ForMember(dest => dest.STOCK, opt => opt.MapFrom(src => ConvertHelper.ConvertToDecimalOrZero(src.StockDisplay.Replace(",", ""))))

              .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
              .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
              .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
              .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
              ;

            Mapper.CreateMap<WasteStockDto, WasteStockFormViewModel>().IgnoreAllNonExisting()
              .ForMember(dest => dest.WasteStockId, opt => opt.MapFrom(src => src.WASTE_STOCK_ID))
              .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.WERKS))
              .ForMember(dest => dest.MaterialNumber, opt => opt.MapFrom(src => src.MATERIAL_NUMBER))
              .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.STOCK))
               .ForMember(dest => dest.StockDisplay, opt => opt.MapFrom(src => ConvertHelper.ConvertDecimalToStringMoneyFormat(src.STOCK)))
              .ForMember(dest => dest.Uom, opt => opt.MapFrom(src => src.Uom))
              .ForMember(dest => dest.UomDescription, opt => opt.MapFrom(src => src.Uom))
                .ForMember(dest => dest.PlantDescription, opt => opt.MapFrom(src => src.WERKS + "-" + src.PlantDescription))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                 .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                ;

            #endregion

            #region Nlog

            Mapper.CreateMap<NlogDto, XmlLogFormViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.XmlLogId, opt => opt.MapFrom(src => src.Nlog_Id))
                .ForMember(dest => dest.TimeStampDisplay, opt => opt.MapFrom(src => src.Timestamp.HasValue ? src.Timestamp.Value.ToString("dd MMM yyyy hh:mm:ss tt") : string.Empty))
                //.ForMember(dest => dest.Logger, opt => opt.ResolveUsing<ConcatStringResolver>().FromMember(src => src.Logger))
                //.ForMember(dest => dest.Message, opt => opt.ResolveUsing<ConcatStringResolver>().FromMember(src => src.Message))
                ;

            #endregion

            #region xml file log

            Mapper.CreateMap<XML_LOGSDto, XmlFileManagementFormViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.XmlLogId, opt => opt.MapFrom(src => src.XML_LOGS_ID))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.XML_FILENAME))
                .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.LAST_ERROR_TIME))
                .ForMember(dest => dest.TimeStampDisplay, opt => opt.MapFrom(src => src.LAST_ERROR_TIME.ToString("dd MMM yyyy HH:mm:ss")))
                .ForMember(dest => dest.XmlLogStatus, opt => opt.MapFrom(src => src.STATUS))
                .ForMember(dest => dest.XmlLogStatusDescription, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.STATUS)))
                .ForMember(dest => dest.DetailListLogs, opt => opt.MapFrom(src => Mapper.Map<List<XmlFileManagementDetailsViewModel>>(src.DetailList)))
                ;

            Mapper.CreateMap<XML_LOGS_DETAILSDto, XmlFileManagementDetailsViewModel>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.LOGS))
                 .ForMember(dest => dest.TimeStampDisplay, opt => opt.MapFrom(src => src.ERROR_TIME.HasValue ? src.ERROR_TIME.Value.ToString("dd MMM yyyy HH:mm:ss") : string.Empty))
                ;

            #endregion

            #region POA Delegation

            Mapper.CreateMap<POA_DELEGATIONDto, PoaDelegationFormViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PoaFrom, opt => opt.MapFrom(src => src.POA_FROM))
                .ForMember(dest => dest.PoaTo, opt => opt.MapFrom(src => src.POA_TO))
                .ForMember(dest => dest.DateFrom, opt => opt.MapFrom(src => src.DATE_FROM))
                .ForMember(dest => dest.DateFromDisplay, opt => opt.MapFrom(src => src.DATE_FROM.ToString("dd MMM yyyy")))
                .ForMember(dest => dest.DateTo, opt => opt.MapFrom(src => src.DATE_TO))
                .ForMember(dest => dest.DateToDisplay, opt => opt.MapFrom(src => src.DATE_TO.ToString("dd MMM yyyy")))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.REASON))
                ;

            Mapper.CreateMap<PoaDelegationFormViewModel, POA_DELEGATIONDto>().IgnoreAllNonExisting()
               .ForMember(dest => dest.POA_FROM, opt => opt.MapFrom(src => src.PoaFrom))
               .ForMember(dest => dest.POA_TO, opt => opt.MapFrom(src => src.PoaTo))
               .ForMember(dest => dest.DATE_FROM, opt => opt.MapFrom(src => src.DateFrom))
               .ForMember(dest => dest.DATE_TO, opt => opt.MapFrom(src => src.DateTo))
               .ForMember(dest => dest.REASON, opt => opt.MapFrom(src => src.Reason))
               ;

            #endregion

            #region Reversal

            Mapper.CreateMap<ReversalDto, DataReversal>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionDateDisplay, opt => opt.MapFrom(src => src.ProductionDate.Value.ToString("dd MMM yyyy")))
                ;

            Mapper.CreateMap<DataReversal, ReversalDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<ReversalIndexViewModel, ReversalGetByParamInput>().IgnoreAllNonExisting()
               .ForMember(dest => dest.DateProduction, opt => opt.MapFrom(src => src.ProductionDate))
               .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantWerks))
               ;

            Mapper.CreateMap<DataReversal, ReversalCreateParamInput>().IgnoreAllNonExisting();

            #endregion

            #region Scheduler Setting

            Mapper.CreateMap<SchedulerSetting, SchedulerSettingModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<SchedulerSettingModel, SchedulerSetting>().IgnoreAllNonExisting();
            #endregion

            #region Product Type

            Mapper.CreateMap<ZAIDM_EX_PRODTYP, ProductTypeFormViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProdCode, opt => opt.MapFrom(src => src.PROD_CODE))
                .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => src.PRODUCT_TYPE))
                .ForMember(dest => dest.ProductAlias, opt => opt.MapFrom(src => src.PRODUCT_ALIAS))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
               .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IS_DELETED.HasValue && src.IS_DELETED.Value ? "Yes" : "No"))
               .ForMember(dest => dest.Ck4CEditable, opt => opt.MapFrom(src => src.CK4CEDITABLE ? "Yes" : "No"))
               .ForMember(dest => dest.IsCk4CEditable, opt => opt.MapFrom(src => src.CK4CEDITABLE))
                ;

            Mapper.CreateMap<ProductTypeFormViewModel, ZAIDM_EX_PRODTYP>().IgnoreAllNonExisting()
              .ForMember(dest => dest.PROD_CODE, opt => opt.MapFrom(src => src.ProdCode))
              .ForMember(dest => dest.PRODUCT_TYPE, opt => opt.MapFrom(src => src.ProductType))
              .ForMember(dest => dest.PRODUCT_ALIAS, opt => opt.MapFrom(src => src.ProductAlias))
              .ForMember(dest => dest.CK4CEDITABLE, opt => opt.MapFrom(src => src.IsCk4CEditable))
              ;

            #endregion

            #region User Model
            Mapper.CreateMap<UserModel, CustomService.Data.USER>().IgnoreAllUnmapped()
                .ForMember(entity => entity.USER_ID, opt => opt.MapFrom(model => model.UserId))
                .ForMember(entity => entity.EMAIL, opt => opt.MapFrom(model => model.Email))
                .ForMember(entity => entity.FIRST_NAME, opt => opt.MapFrom(model => model.FirstName))
                .ForMember(entity => entity.LAST_NAME, opt => opt.MapFrom(model => model.LastName))
                .ForMember(entity => entity.ADDRESS, opt => opt.MapFrom(model => model.Address))
                ;
            Mapper.CreateMap<CustomService.Data.USER, UserModel>().IgnoreAllNonExisting()
                .ForMember(entity => entity.UserId, opt => opt.MapFrom(model => model.USER_ID))
                .ForMember(entity => entity.Email, opt => opt.MapFrom(model => model.EMAIL))
                .ForMember(entity => entity.FirstName, opt => opt.MapFrom(model => model.FIRST_NAME))
                .ForMember(entity => entity.LastName, opt => opt.MapFrom(model => model.LAST_NAME))
                .ForMember(entity => entity.Address, opt => opt.MapFrom(model => model.ADDRESS))
                ;
            #endregion

            #region Company Model
            Mapper.CreateMap<CompanyModel, CustomService.Data.T001>().IgnoreAllUnmapped()
                .ForMember(entity => entity.BUKRS, opt => opt.MapFrom(model => model.Id))
                .ForMember(entity => entity.BUTXT, opt => opt.MapFrom(model => model.Name))
                .ForMember(entity => entity.SPRAS, opt => opt.MapFrom(model => model.Address))
                .ForMember(entity => entity.ORT01, opt => opt.MapFrom(model => model.City))
                .ForMember(entity => entity.NPWP, opt => opt.MapFrom(model => model.Npwp))
                ;
            Mapper.CreateMap<CustomService.Data.T001, CompanyModel>().IgnoreAllNonExisting()
                .ForMember(entity => entity.Id, opt => opt.MapFrom(model => model.BUKRS))
                .ForMember(entity => entity.Name, opt => opt.MapFrom(model => model.BUTXT))
                .ForMember(entity => entity.Address, opt => opt.MapFrom(model => model.SPRAS))
                .ForMember(entity => entity.City, opt => opt.MapFrom(model => model.ORT01))
                .ForMember(entity => entity.Npwp, opt => opt.MapFrom(model => model.NPWP))
                ;
            #endregion

            #region References
            Mapper.CreateMap<ConfigurationCreateViewModel, CustomService.Data.SYS_REFFERENCES_TYPE>().IgnoreAllUnmapped()
                .ForMember(entity => entity.SYS_REFFERENCES_TYPE1, opt => opt.MapFrom(model => model.ConfigType))
                .ForMember(entity => entity.SYS_REFFERENCES_TEXT, opt => opt.MapFrom(model => model.ConfigText))
                ;
            Mapper.CreateMap<CustomService.Data.SYS_REFFERENCES_TYPE, ConfigurationCreateViewModel>().IgnoreAllNonExisting()
                .ForMember(entity => entity.ConfigType, opt => opt.MapFrom(model => model.SYS_REFFERENCES_TYPE1))
                .ForMember(entity => entity.ConfigText, opt => opt.MapFrom(model => model.SYS_REFFERENCES_TEXT))
                ;
            Mapper.CreateMap<ConfigurationViewModel, CustomService.Data.SYS_REFFERENCES>().IgnoreAllUnmapped()
                .ForMember(entity => entity.REFF_ID, opt => opt.MapFrom(model => model.REFF_ID))
                .ForMember(entity => entity.REFF_NAME, opt => opt.MapFrom(model => model.REFF_NAME))
                .ForMember(entity => entity.REFF_TYPE, opt => opt.MapFrom(model => model.REFF_TYPE))
                .ForMember(entity => entity.REFF_KEYS, opt => opt.MapFrom(model => model.REFF_KEYS))
                .ForMember(entity => entity.REFF_VALUE, opt => opt.MapFrom(model => model.REFF_VALUE))
                .ForMember(entity => entity.CREATED_BY, opt => opt.MapFrom(model => model.CREATED_BY))
                .ForMember(entity => entity.CREATED_DATE, opt => opt.MapFrom(model => model.CREATED_DATE))
                .ForMember(entity => entity.LASTMODIFIED_BY, opt => opt.MapFrom(model => model.LASTMODIFIED_BY))
                .ForMember(entity => entity.LASTMODIFIED_DATE, opt => opt.MapFrom(model => model.LASTMODIFIED_DATE))
                .ForMember(entity => entity.IS_ACTIVE, opt => opt.MapFrom(model => model.IS_ACTIVE))
                ;
            Mapper.CreateMap<CustomService.Data.SYS_REFFERENCES, ConfigurationViewModel>().IgnoreAllNonExisting()
                .ForMember(entity => entity.REFF_ID, opt => opt.MapFrom(model => model.REFF_ID))
                .ForMember(entity => entity.REFF_NAME, opt => opt.MapFrom(model => model.REFF_NAME))
                .ForMember(entity => entity.REFF_TYPE, opt => opt.MapFrom(model => model.REFF_TYPE))
                .ForMember(entity => entity.REFF_KEYS, opt => opt.MapFrom(model => model.REFF_KEYS))
                .ForMember(entity => entity.REFF_VALUE, opt => opt.MapFrom(model => model.REFF_VALUE))
                .ForMember(entity => entity.CREATED_BY, opt => opt.MapFrom(model => model.CREATED_BY))
                .ForMember(entity => entity.CREATED_DATE, opt => opt.MapFrom(model => model.CREATED_DATE))
                .ForMember(entity => entity.LASTMODIFIED_BY, opt => opt.MapFrom(model => model.LASTMODIFIED_BY))
                .ForMember(entity => entity.LASTMODIFIED_DATE, opt => opt.MapFrom(model => model.LASTMODIFIED_DATE))
                .ForMember(entity => entity.IS_ACTIVE, opt => opt.MapFrom(model => model.IS_ACTIVE))
                ;

            Mapper.CreateMap<CustomService.Data.SYS_REFFERENCES_TYPE, ReferenceTypeModel>().IgnoreAllNonExisting()
                .ForMember(entity => entity.Name, opt => opt.MapFrom(model => model.SYS_REFFERENCES_TYPE1))
                .ForMember(entity => entity.Value, opt => opt.MapFrom(model => model.SYS_REFFERENCES_TEXT))
                ;
            Mapper.CreateMap<ReferenceTypeModel, CustomService.Data.SYS_REFFERENCES_TYPE>().IgnoreAllUnmapped()
                .ForMember(entity => entity.SYS_REFFERENCES_TYPE1, opt => opt.MapFrom(model => model.Name))
                .ForMember(entity => entity.SYS_REFFERENCES_TEXT, opt => opt.MapFrom(model => model.Value))
                ;
            Mapper.CreateMap<CustomService.Data.SYS_REFFERENCES, ReferenceModel>().IgnoreAllNonExisting()
                .ForMember(entity => entity.Id, opt => opt.MapFrom(model => model.REFF_ID))
                .ForMember(entity => entity.Name, opt => opt.MapFrom(model => model.REFF_NAME))
                .ForMember(entity => entity.TypeID, opt => opt.MapFrom(model => model.REFF_TYPE))
                .ForMember(entity => entity.Key, opt => opt.MapFrom(model => model.REFF_KEYS))
                .ForMember(entity => entity.Value, opt => opt.MapFrom(model => model.REFF_VALUE))
                .ForMember(entity => entity.CreatedBy, opt => opt.MapFrom(model => model.CREATED_BY))
                .ForMember(entity => entity.CreatedDate, opt => opt.MapFrom(model => model.CREATED_DATE))
                .ForMember(entity => entity.LastModifiedBy, opt => opt.MapFrom(model => model.LASTMODIFIED_BY))
                .ForMember(entity => entity.LastModifiedDate, opt => opt.MapFrom(model => model.LASTMODIFIED_DATE))
                .ForMember(entity => entity.IsActive, opt => opt.MapFrom(model => model.IS_ACTIVE))
                .ForMember(entity => entity.Type, opt => opt.MapFrom(model => model.SYS_REFFERENCES_TYPE))
                ;
            Mapper.CreateMap<ReferenceModel, CustomService.Data.SYS_REFFERENCES>().IgnoreAllUnmapped()
                .ForMember(entity => entity.REFF_ID, opt => opt.MapFrom(model => model.Id))
                .ForMember(entity => entity.REFF_NAME, opt => opt.MapFrom(model => model.Name))
                .ForMember(entity => entity.REFF_TYPE, opt => opt.MapFrom(model => model.TypeID))
                .ForMember(entity => entity.REFF_KEYS, opt => opt.MapFrom(model => model.Key))
                .ForMember(entity => entity.REFF_VALUE, opt => opt.MapFrom(model => model.Value))
                .ForMember(entity => entity.CREATED_BY, opt => opt.MapFrom(model => model.CreatedBy))
                .ForMember(entity => entity.CREATED_DATE, opt => opt.MapFrom(model => model.CreatedDate))
                .ForMember(entity => entity.LASTMODIFIED_BY, opt => opt.MapFrom(model => model.LastModifiedBy))
                .ForMember(entity => entity.LASTMODIFIED_DATE, opt => opt.MapFrom(model => model.LastModifiedDate))
                .ForMember(entity => entity.IS_ACTIVE, opt => opt.MapFrom(model => model.IsActive))
                .ForMember(entity => entity.SYS_REFFERENCES_TYPE, opt => opt.MapFrom(model => model.Type))
                ;
            #endregion
#region Month Closing

            Mapper.CreateMap<MonthClosingDetail, MonthClosingDto>().IgnoreAllNonExisting();
            Mapper.CreateMap<MonthClosingDto, MonthClosingDetail>().IgnoreAllNonExisting();

            Mapper.CreateMap<MonthClosingDocDto, MonthClosingDocModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<MonthClosingDocModel, MonthClosingDocDto>().IgnoreAllNonExisting();

            #endregion

            #region Changes Log
            Mapper.CreateMap<CustomService.Data.CHANGES_HISTORY, ChangesHistoryItemModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.USERNAME,
                    opt => opt.MapFrom(src => src.USER != null ? src.USER.USER_ID : string.Empty))
                .ForMember(dest => dest.USER_FIRST_NAME,
                    opt => opt.MapFrom(src => src.USER != null ? src.USER.FIRST_NAME : string.Empty))
                .ForMember(dest => dest.USER_LAST_NAME,
                    opt => opt.MapFrom(src => src.USER != null ? src.USER.LAST_NAME : string.Empty))
                    .ForMember(dest => dest.FORM_TYPE_DESC, opt => opt.MapFrom(src => EnumHelper.GetDescription((Enums.MenuList)src.FORM_TYPE_ID)));
            Mapper.CreateMap<CustomService.Data.WORKFLOW_HISTORY, WorkflowHistoryViewModel>().IgnoreAllNonExisting()
               .ForMember(dest => dest.ACTION, opt => opt.MapFrom(src => EnumHelper.GetDescription((Enums.ActionType)src.ACTION)))
               .ForMember(dest => dest.USERNAME, opt => opt.MapFrom(src => src.ACTION_BY))
               .ForMember(dest => dest.USER_FIRST_NAME, opt => opt.MapFrom(src => src.USER.FIRST_NAME))
               .ForMember(dest => dest.USER_LAST_NAME, opt => opt.MapFrom(src => src.USER.LAST_NAME))
               .ForMember(dest => dest.Role, opt => opt.MapFrom(src => EnumHelper.GetDescription((Enums.UserRole)src.ROLE)))
               ;
            #endregion

            #region Finance Ratio
            Mapper.CreateMap<FinanceRatioModel, CustomService.Data.MASTER_FINANCIAL_RATIO>().IgnoreAllUnmapped()
                .ForMember(entity => entity.FINANCERATIO_ID, opt => opt.MapFrom(model => model.Id))
                .ForMember(entity => entity.BUKRS, opt => opt.MapFrom(model => model.Bukrs))
                .ForMember(entity => entity.YEAR_PERIOD, opt => opt.MapFrom(model => model.YearPeriod))
                .ForMember(entity => entity.CURRENT_ASSETS, opt => opt.MapFrom(model => model.CurrentAssets))
                .ForMember(entity => entity.CURRENT_DEBTS, opt => opt.MapFrom(model => model.CurrentDebts))
                .ForMember(entity => entity.TOTAL_ASSETS, opt => opt.MapFrom(model => model.TotalAssets))
                .ForMember(entity => entity.TOTAL_DEBTS, opt => opt.MapFrom(model => model.TotalDebts))
                .ForMember(entity => entity.TOTAL_CAPITAL, opt => opt.MapFrom(model => model.TotalCapital))
                .ForMember(entity => entity.NET_PROFIT, opt => opt.MapFrom(model => model.NetProfit))
                .ForMember(entity => entity.LIQUIDITY_RATIO, opt => opt.MapFrom(model => model.LiquidityRatio))
                .ForMember(entity => entity.SOLVENCY_RATIO, opt => opt.MapFrom(model => model.SolvencyRatio))
                .ForMember(entity => entity.RENTABLE_RATIO, opt => opt.MapFrom(model => model.RentabilityRatio))
                .ForMember(entity => entity.CREATED_BY, opt => opt.MapFrom(model => model.CreatedBy))
                .ForMember(entity => entity.CREATED_DATE, opt => opt.MapFrom(model => model.CreatedDate))
                .ForMember(entity => entity.LASTMODIFIED_BY, opt => opt.MapFrom(model => model.LastModifiedBy))
                .ForMember(entity => entity.LASTMODIFIED_DATE, opt => opt.MapFrom(model => model.LastModifiedDate))
                .ForMember(entity => entity.LASTAPPROVED_BY, opt => opt.MapFrom(model => model.LastApprovedBy))
                .ForMember(entity => entity.LASTAPPROVED_DATE, opt => opt.MapFrom(model => model.LastApprovedDate))
                .ForMember(entity => entity.STATUS_APPROVAL, opt => opt.MapFrom(model => model.ApprovalStatus))

                .ForMember(entity => entity.COMPANY, opt => opt.MapFrom(model => model.Company))
                .ForMember(entity => entity.CREATOR, opt => opt.MapFrom(model => model.Creator))
                .ForMember(entity => entity.LASTEDITOR, opt => opt.MapFrom(model => model.LastEditor))
                .ForMember(entity => entity.APPROVER, opt => opt.MapFrom(model => model.Approver))
                .ForMember(entity => entity.APPROVALSTATUS, opt => opt.MapFrom(model => model.ApprovalStatusDescription))
                ;

            Mapper.CreateMap<CustomService.Data.MASTER_FINANCIAL_RATIO, FinanceRatioModel>().IgnoreAllNonExisting()
                .ForMember(entity => entity.Id, opt => opt.MapFrom(model => model.FINANCERATIO_ID))
                .ForMember(entity => entity.Bukrs, opt => opt.MapFrom(model => model.BUKRS))
                .ForMember(entity => entity.YearPeriod, opt => opt.MapFrom(model => model.YEAR_PERIOD))
                .ForMember(entity => entity.CurrentAssets, opt => opt.MapFrom(model => model.CURRENT_ASSETS))
                .ForMember(entity => entity.CurrentDebts, opt => opt.MapFrom(model => model.CURRENT_DEBTS))
                .ForMember(entity => entity.TotalAssets, opt => opt.MapFrom(model => model.TOTAL_ASSETS))
                .ForMember(entity => entity.TotalDebts, opt => opt.MapFrom(model => model.TOTAL_DEBTS))
                .ForMember(entity => entity.TotalCapital, opt => opt.MapFrom(model => model.TOTAL_CAPITAL))
                .ForMember(entity => entity.NetProfit, opt => opt.MapFrom(model => model.NET_PROFIT))
                .ForMember(entity => entity.LiquidityRatio, opt => opt.MapFrom(model => model.LIQUIDITY_RATIO))
                .ForMember(entity => entity.SolvencyRatio, opt => opt.MapFrom(model => model.SOLVENCY_RATIO))
                .ForMember(entity => entity.RentabilityRatio, opt => opt.MapFrom(model => model.RENTABLE_RATIO))
                .ForMember(entity => entity.CreatedBy, opt => opt.MapFrom(model => model.CREATED_BY))
                .ForMember(entity => entity.CreatedDate, opt => opt.MapFrom(model => model.CREATED_DATE))
                .ForMember(entity => entity.LastModifiedBy, opt => opt.MapFrom(model => model.LASTMODIFIED_BY))
                .ForMember(entity => entity.LastModifiedDate, opt => opt.MapFrom(model => model.LASTMODIFIED_DATE))
                .ForMember(entity => entity.LastApprovedBy, opt => opt.MapFrom(model => model.LASTAPPROVED_BY))
                .ForMember(entity => entity.LastApprovedDate, opt => opt.MapFrom(model => model.LASTAPPROVED_DATE))
                .ForMember(entity => entity.ApprovalStatus, opt => opt.MapFrom(model => model.STATUS_APPROVAL))

                .ForMember(entity => entity.Company, opt => opt.MapFrom(model => model.COMPANY))
                .ForMember(entity => entity.Creator, opt => opt.MapFrom(model => model.CREATOR))
                .ForMember(entity => entity.LastEditor, opt => opt.MapFrom(model => model.LASTEDITOR))
                .ForMember(entity => entity.Approver, opt => opt.MapFrom(model => model.APPROVER))
                .ForMember(entity => entity.ApprovalStatusDescription, opt => opt.MapFrom(model => model.APPROVALSTATUS))
                ;
            #endregion

            #region Email Content
            Mapper.CreateMap<CustomService.Data.EMAILVARIABEL, EmailVariable>().IgnoreAllNonExisting()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.VARIABELID))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NAME))
               .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.NOTE))
               .ForMember(dest => dest.ContentID, opt => opt.MapFrom(src => src.CONTENTEMAILID))
               ;

            Mapper.CreateMap<CustomService.Data.CONTENTEMAIL, EmailContent>().IgnoreAllNonExisting()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CONTENTEMAILID))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.EMAILNAME))
               .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.EMAILSUBJECT))
               .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.EMAILCONTENT))
               .ForMember(dest => dest.Variables, opt => opt.MapFrom(src => src.EMAILVARIABELS))
               ;
            #endregion

            #region POA_EXCISER
            //Mapper.CreateMap<PoaExciserViewModel, CustomService.Data.POA_EXCISER>().IgnoreAllUnmapped()
            //  .ForMember(entity => entity.EXCISER_ID, opt => opt.MapFrom(model => model.EXCISER_ID))
            //  .ForMember(entity => entity.POA_ID, opt => opt.MapFrom(model => model.POA_ID))
            //  ;
            //Mapper.CreateMap<CustomService.Data.POA_EXCISER, PoaExciserViewModel>().IgnoreAllNonExisting()
            //    .ForMember(model => model.EXCISER_ID, opt => opt.MapFrom(entity => entity.EXCISER_ID))
            //    .ForMember(model => model.POA_ID, opt => opt.MapFrom(entity => entity.POA_ID))
            //    ;
            #endregion

            #region Product Type

            Mapper.CreateMap<CustomService.Data.MASTER_PRODUCT_TYPE, ProductTypeFormViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProdCode, opt => opt.MapFrom(src => src.PROD_CODE))
                .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => src.PRODUCT_TYPE))
                .ForMember(dest => dest.ProductAlias, opt => opt.MapFrom(src => src.PRODUCT_ALIAS))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IS_DELETED))
                .ForMember(dest => dest.Ck4CEditable, opt => opt.MapFrom(src => src.CK4CEDITABLE ? "Yes" : "No"))
                .ForMember(dest => dest.IsCk4CEditable, opt => opt.MapFrom(src => src.CK4CEDITABLE))
                .ForMember(dest => dest.LastApprovedBy, opt => opt.MapFrom(src => src.LASTAPPROVED_BY))
                .ForMember(dest => dest.LastApprovedDate, opt => opt.MapFrom(src => src.LASTAPPROVED_DATE))
                .ForMember(dest => dest.ApprovalStatus, opt => opt.MapFrom(src => src.APPROVED_STATUS))

                .ForMember(entity => entity.Creator, opt => opt.MapFrom(model => model.CREATOR))
                .ForMember(entity => entity.LastEditor, opt => opt.MapFrom(model => model.LASTEDITOR))
                .ForMember(entity => entity.Approver, opt => opt.MapFrom(model => model.APPROVER))
                .ForMember(entity => entity.ApprovalStatusDescription, opt => opt.MapFrom(model => model.APPROVALSTATUS))
                ;

            Mapper.CreateMap<ProductTypeFormViewModel, CustomService.Data.MASTER_PRODUCT_TYPE>().IgnoreAllUnmapped()
              .ForMember(dest => dest.PROD_CODE, opt => opt.MapFrom(src => src.ProdCode))
              .ForMember(dest => dest.PRODUCT_TYPE, opt => opt.MapFrom(src => src.ProductType))
              .ForMember(dest => dest.PRODUCT_ALIAS, opt => opt.MapFrom(src => src.ProductAlias))
              .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
              .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
              .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
              .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
              .ForMember(dest => dest.IS_DELETED, opt => opt.MapFrom(src => src.IsDeleted))
              .ForMember(dest => dest.CK4CEDITABLE, opt => opt.MapFrom(src => src.IsCk4CEditable))
              .ForMember(dest => dest.LASTAPPROVED_BY, opt => opt.MapFrom(src => src.LastApprovedBy))
              .ForMember(dest => dest.LASTAPPROVED_DATE, opt => opt.MapFrom(src => src.LastApprovedDate))
              .ForMember(dest => dest.APPROVED_STATUS, opt => opt.MapFrom(src => src.ApprovalStatus))

              .ForMember(entity => entity.CREATOR, opt => opt.MapFrom(model => model.Creator))
              .ForMember(entity => entity.LASTEDITOR, opt => opt.MapFrom(model => model.LastEditor))
              .ForMember(entity => entity.APPROVER, opt => opt.MapFrom(model => model.Approver))
              .ForMember(entity => entity.APPROVALSTATUS, opt => opt.MapFrom(model => model.ApprovalStatusDescription))
              ;

            #endregion
            #region Supporting Document
            Mapper.CreateMap<SupportDocModel, CustomService.Data.MASTER_SUPPORTING_DOCUMENT>().IgnoreAllUnmapped()
                .ForMember(entity => entity.DOCUMENT_ID, opt => opt.MapFrom(model => model.DocumentID))
                .ForMember(entity => entity.FORM_ID, opt => opt.MapFrom(model => model.FormID))
                .ForMember(entity => entity.SUPPORTING_DOCUMENT_NAME, opt => opt.MapFrom(model => model.SupportDocName))
                .ForMember(entity => entity.BUKRS, opt => opt.MapFrom(model => model.Bukrs))
                .ForMember(entity => entity.IS_ACTIVE, opt => opt.MapFrom(model => model.IsActive))
                .ForMember(entity => entity.CREATED_BY, opt => opt.MapFrom(model => model.CreatedBy))
                .ForMember(entity => entity.CREATED_DATE, opt => opt.MapFrom(model => model.CreatedDate))
                .ForMember(entity => entity.LASTMODIFIED_BY, opt => opt.MapFrom(model => model.LastModifiedBy))
                .ForMember(entity => entity.LASTMODIFIED_DATE, opt => opt.MapFrom(model => model.LastModifiedDate))
                .ForMember(entity => entity.LASTAPPROVED_BY, opt => opt.MapFrom(model => model.LastApprovedBy))
                .ForMember(entity => entity.LASTAPPROVED_DATE, opt => opt.MapFrom(model => model.LastApprovedDate))
                .ForMember(entity => entity.LASTAPPROVED_STATUS, opt => opt.MapFrom(model => model.ApprovalStatus))

                .ForMember(entity => entity.COMPANY, opt => opt.MapFrom(model => model.Company))
                .ForMember(entity => entity.CREATOR, opt => opt.MapFrom(model => model.Creator))
                .ForMember(entity => entity.EDITOR, opt => opt.MapFrom(model => model.LastEditor))
                .ForMember(entity => entity.APPROVER, opt => opt.MapFrom(model => model.Approver))
                .ForMember(entity => entity.APPROVALSTATUS, opt => opt.MapFrom(model => model.ApprovalStatusDescription))
                ;

            Mapper.CreateMap<CustomService.Data.MASTER_SUPPORTING_DOCUMENT, SupportDocModel>().IgnoreAllNonExisting()
                .ForMember(entity => entity.DocumentID, opt => opt.MapFrom(model => model.DOCUMENT_ID))
                .ForMember(entity => entity.FormID, opt => opt.MapFrom(model => model.FORM_ID))
                .ForMember(entity => entity.SupportDocName, opt => opt.MapFrom(model => model.SUPPORTING_DOCUMENT_NAME))
                .ForMember(entity => entity.Bukrs, opt => opt.MapFrom(model => model.BUKRS))
                .ForMember(entity => entity.IsActive, opt => opt.MapFrom(model => model.IS_ACTIVE))
                .ForMember(entity => entity.CreatedBy, opt => opt.MapFrom(model => model.CREATED_BY))
                .ForMember(entity => entity.CreatedDate, opt => opt.MapFrom(model => model.CREATED_DATE))
                .ForMember(entity => entity.LastModifiedBy, opt => opt.MapFrom(model => model.LASTMODIFIED_BY))
                .ForMember(entity => entity.LastModifiedDate, opt => opt.MapFrom(model => model.LASTMODIFIED_DATE))
                .ForMember(entity => entity.LastApprovedBy, opt => opt.MapFrom(model => model.LASTAPPROVED_BY))
                .ForMember(entity => entity.LastApprovedDate, opt => opt.MapFrom(model => model.LASTAPPROVED_DATE))
                .ForMember(entity => entity.ApprovalStatus, opt => opt.MapFrom(model => model.LASTAPPROVED_STATUS))

                .ForMember(entity => entity.Company, opt => opt.MapFrom(model => model.COMPANY))
                .ForMember(entity => entity.Creator, opt => opt.MapFrom(model => model.CREATOR))
                .ForMember(entity => entity.LastEditor, opt => opt.MapFrom(model => model.EDITOR))
                .ForMember(entity => entity.Approver, opt => opt.MapFrom(model => model.APPROVER))
                .ForMember(entity => entity.ApprovalStatusDescription, opt => opt.MapFrom(model => model.APPROVALSTATUS))
                ;
            #endregion
            #region Tariff
            Mapper.CreateMap<TariffModel, CustomService.Data.TARIFF>().IgnoreAllUnmapped()
              .ForMember(entity => entity.TARIFF_ID, opt => opt.MapFrom(model => model.Id))
              .ForMember(entity => entity.PROD_CODE, opt => opt.MapFrom(model => model.ProductTypeCode))
              .ForMember(entity => entity.HJE_FROM, opt => opt.MapFrom(model => model.MinimumHJE))
              .ForMember(entity => entity.HJE_TO, opt => opt.MapFrom(model => model.MaximumHJE))
              .ForMember(entity => entity.VALID_FROM, opt => opt.MapFrom(model => model.ValidStartDate))
              .ForMember(entity => entity.VALID_TO, opt => opt.MapFrom(model => model.ValidEndDate))
              .ForMember(entity => entity.TARIFF_VALUE, opt => opt.MapFrom(model => model.Tariff))
              .ForMember(entity => entity.STATUS_APPROVAL, opt => opt.MapFrom(model => model.ApprovalStatus))
              .ForMember(entity => entity.PRODUCT_TYPE, opt => opt.MapFrom(model => model.ProductType))
              .ForMember(entity => entity.APPROVALSTATUS, opt => opt.MapFrom(model => model.ApprovalStatusDescription))
              .ForMember(entity => entity.CREATED_BY, opt => opt.MapFrom(model => model.CreatedBy))
              .ForMember(entity => entity.CREATED_DATE, opt => opt.MapFrom(model => model.CreatedDate))
              .ForMember(entity => entity.CREATOR, opt => opt.MapFrom(model => model.Creator))
              .ForMember(entity => entity.LASTMODIFIED_BY, opt => opt.MapFrom(model => model.ModifiedBy))
              .ForMember(entity => entity.LASTMODIFIED_DATE, opt => opt.MapFrom(model => model.ModifiedDate))
              .ForMember(entity => entity.LASTAPPROVED_BY, opt => opt.MapFrom(model => model.ApprovedBy))
              .ForMember(entity => entity.LASTAPPROVED_DATE, opt => opt.MapFrom(model => model.ApprovedDate))
              ;
            Mapper.CreateMap<CustomService.Data.TARIFF, TariffModel>().IgnoreAllNonExisting()
                .ForMember(model => model.Id, opt => opt.MapFrom(entity => entity.TARIFF_ID))
                .ForMember(model => model.ProductTypeCode, opt => opt.MapFrom(entity => entity.PROD_CODE))
                .ForMember(model => model.MinimumHJE, opt => opt.MapFrom(entity => entity.HJE_FROM))
                .ForMember(model => model.MaximumHJE, opt => opt.MapFrom(entity => entity.HJE_TO))
                .ForMember(model => model.ValidStartDate, opt => opt.MapFrom(entity => entity.VALID_FROM))
                .ForMember(model => model.ValidEndDate, opt => opt.MapFrom(entity => entity.VALID_TO))
                .ForMember(model => model.Tariff, opt => opt.MapFrom(entity => entity.TARIFF_VALUE))
                .ForMember(model => model.ProductType, opt => opt.MapFrom(entity => entity.PRODUCT_TYPE))
                .ForMember(model => model.ApprovalStatus, opt => opt.MapFrom(entity => entity.STATUS_APPROVAL))
                .ForMember(model => model.ApprovalStatusDescription, opt => opt.MapFrom(entity => entity.APPROVALSTATUS))
                .ForMember(model => model.CreatedBy, opt => opt.MapFrom(entity => entity.CREATED_BY))
                .ForMember(model => model.CreatedDate, opt => opt.MapFrom(entity => entity.CREATED_DATE))
                .ForMember(model => model.Creator, opt => opt.MapFrom(entity => entity.CREATOR))
                .ForMember(model => model.ModifiedBy, opt => opt.MapFrom(entity => entity.LASTMODIFIED_BY))
                .ForMember(model => model.ModifiedDate, opt => opt.MapFrom(entity => entity.LASTMODIFIED_DATE))
                .ForMember(model => model.ApprovedBy, opt => opt.MapFrom(entity => entity.LASTAPPROVED_BY))
                .ForMember(model => model.ApprovedDate, opt => opt.MapFrom(entity => entity.LASTAPPROVED_DATE))
                ;
            #endregion

            #region New NPPBKC
            Mapper.CreateMap<NewNPPBKCViewModel.KppbcModel, CustomService.Data.MASTER_KPPBC>().IgnoreAllUnmapped()
                .ForMember(dest => dest.KPPBC_ID, opt => opt.MapFrom(src => src.KppbcId));

            Mapper.CreateMap<CustomService.Data.LFA1, NewNPPBKCViewModel.VendorModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.LIFNR))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.NAME1))
                .ForMember(dest => dest.KppbcName, opt => opt.MapFrom(src => src.NAME2))
                ;
            Mapper.CreateMap<NewNPPBKCViewModel.VendorModel, CustomService.Data.LFA1>().IgnoreAllUnmapped()
                .ForMember(dest => dest.LIFNR, opt => opt.MapFrom(src => src.Id));
            #endregion

#region Master Data Approval

            Mapper.CreateMap<MasterDataApprovalSettingDto, MasterDataSetting>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MasterDataSettingDetails, opt => opt.MapFrom(src => src.Details));
            Mapper.CreateMap<MasterDataApprovalSettingDetail, MasterDataSettingDetail>().IgnoreAllNonExisting();
            Mapper.CreateMap<MasterDataSettingDetail, MasterDataApprovalSettingDetail>().IgnoreAllNonExisting();
            Mapper.CreateMap<MasterDataSetting, MasterDataApprovalSettingDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.MasterDataSettingDetails)); ;

            Mapper.CreateMap<MASTER_DATA_APPROVAL, MasterDataApprovalDetailViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.MASTER_DATA_APPROVAL_DETAIL))
                .ForMember(dest => dest.PageDesciption, opt => opt.MapFrom(src => src.PAGE.MENU_NAME))
                .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.STATUS_ID)));
            Mapper.CreateMap<MASTER_DATA_APPROVAL_DETAIL, MasterDataApprovalDetail>().IgnoreAllNonExisting();

            #endregion

            #region Market Model
            Mapper.CreateMap<MarketModel, CustomService.Data.ZAIDM_EX_MARKET>().IgnoreAllUnmapped()
                .ForMember(entity => entity.MARKET_ID, opt => opt.MapFrom(model => model.Market_Id))
                .ForMember(entity => entity.MARKET_DESC, opt => opt.MapFrom(model => model.Market_Desc))
                .ForMember(entity => entity.IS_DELETED, opt => opt.MapFrom(model => model.Is_Deleted))               
                ;
            Mapper.CreateMap<CustomService.Data.ZAIDM_EX_MARKET, MarketModel>().IgnoreAllNonExisting()
                .ForMember(entity => entity.Market_Id, opt => opt.MapFrom(model => model.MARKET_ID))
                .ForMember(entity => entity.Market_Desc, opt => opt.MapFrom(model => model.MARKET_DESC))
                .ForMember(entity => entity.Is_Deleted, opt => opt.MapFrom(model => model.IS_DELETED))            
                ;
            #endregion

            #region Plant Model
            Mapper.CreateMap<T001WModel, CustomService.Data.MASTER_PLANT>().IgnoreAllUnmapped()
                .ForMember(entity => entity.WERKS, opt => opt.MapFrom(model => model.WERKS))
                .ForMember(entity => entity.NAME1, opt => opt.MapFrom(model => model.NAME1))
                .ForMember(entity => entity.ADDRESS, opt => opt.MapFrom(model => model.ADDRESS))
                ;
            Mapper.CreateMap<CustomService.Data.MASTER_PLANT, T001WModel>().IgnoreAllNonExisting()
                .ForMember(entity => entity.WERKS, opt => opt.MapFrom(model => model.WERKS))
                .ForMember(entity => entity.NAME1, opt => opt.MapFrom(model => model.NAME1))
                .ForMember(entity => entity.ADDRESS, opt => opt.MapFrom(model => model.ADDRESS))
                ;
            #endregion

         
            #region Product Development ( Brand Registration Transaction - Product Development )

            Mapper.CreateMap<CustomService.Data.PRODUCT_DEVELOPMENT, ProductDevelopmentModel>().IgnoreAllNonExisting()
                    .ForMember(dest => dest.PD_ID, opt => opt.MapFrom(src => src.PD_ID))
                    .ForMember(dest=> dest.PD_NO, opt=> opt.MapFrom(src=> src.PD_NO))
                    .ForMember(dest => dest.Created_By, opt => opt.MapFrom(src => src.CREATED_BY))
                    .ForMember(dest => dest.Created_Date, opt => opt.MapFrom(src => src.CREATED_DATE))
                    .ForMember(dest => dest.Modified_By, opt => opt.MapFrom(src => src.LASTMODIFIED_BY))
                    .ForMember(dest => dest.Modified_Date, opt => opt.MapFrom(src => src.LASTMODIFIED_DATE))                
                    .ForMember(dest => dest.Next_Action, opt => opt.MapFrom(src => src.NEXT_ACTION))                               
                    .ForMember(dest => dest.Creator, opt => opt.MapFrom(model => model.CREATOR))
                    .ForMember(dest => dest.LastEditor, opt => opt.MapFrom(model => model.LAST_EDITOR))
              ;

            Mapper.CreateMap<ProductDevelopmentModel, CustomService.Data.PRODUCT_DEVELOPMENT>().IgnoreAllUnmapped()
                  .ForMember(dest => dest.PD_ID, opt => opt.MapFrom(src => src.PD_ID))
                  .ForMember(dest=> dest.PD_NO, opt => opt.MapFrom(src=> src.PD_NO))
                  .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.Created_By))
                  .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.Created_Date))
                  .ForMember(dest => dest.LASTMODIFIED_BY, opt => opt.MapFrom(src => src.Modified_By))
                  .ForMember(dest => dest.LASTMODIFIED_DATE, opt => opt.MapFrom(src => src.Modified_Date))            
                  .ForMember(dest => dest.NEXT_ACTION, opt => opt.MapFrom(src => src.Next_Action))              
                  .ForMember(dest => dest.CREATOR, opt => opt.MapFrom(src => src.Creator))
                  .ForMember(dest => dest.LAST_EDITOR, opt => opt.MapFrom(src => src.LastEditor))
              ;

            Mapper.CreateMap<CustomService.Data.PRODUCT_DEVELOPMENT_DETAIL, ProductDevDetailModel>().IgnoreAllNonExisting()
                .ForMember(dest=> dest.PD_DETAIL_ID, opt=> opt.MapFrom(src=>src.PD_DETAIL_ID))
                .ForMember(dest => dest.Fa_Code_Old, opt => opt.MapFrom(src => src.FA_CODE_OLD))
                .ForMember(dest => dest.Fa_Code_New, opt => opt.MapFrom(src => src.FA_CODE_NEW))
                .ForMember(dest => dest.Hl_Code, opt => opt.MapFrom(src => src.HL_CODE))
                .ForMember(dest => dest.Market_Id, opt => opt.MapFrom(src => src.MARKET_ID))
                .ForMember(dest => dest.Fa_Code_Old_Desc, opt => opt.MapFrom(src => src.FA_CODE_OLD_DESCR))
                .ForMember(dest => dest.Fa_Code_New_Desc, opt => opt.MapFrom(src => src.FA_CODE_NEW_DESCR))
                .ForMember(dest => dest.Werks, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.Is_Import, opt => opt.MapFrom(src => src.IS_IMPORT))
                .ForMember(dest => dest.PD_ID, opt => opt.MapFrom(src => src.PD_ID))
                .ForMember(dest => dest.Request_No, opt => opt.MapFrom(src => src.REQUEST_NO))
                .ForMember(dest => dest.Bukrs, opt => opt.MapFrom(src => src.BUKRS))
                .ForMember(dest => dest.Approved_By, opt => opt.MapFrom(src => src.LASTAPPROVED_BY))
                .ForMember(dest => dest.Approved_Date, opt => opt.MapFrom(src => src.LASTAPPROVED_DATE))
                .ForMember(dest => dest.Approval_Status, opt => opt.MapFrom(src => src.STATUS_APPROVAL))                   
                .ForMember(dest => dest.Approver, opt => opt.MapFrom(model => model.APPROVER))
                .ForMember(dest => dest.ApprovalStatusDescription, opt => opt.MapFrom(model => model.APPROVAL_STATUS))
                .ForMember(dest => dest.Modified_By, opt => opt.MapFrom(src => src.LASTMODIFIED_BY))
                .ForMember(dest => dest.Modified_Date, opt => opt.MapFrom(src => src.LASTMODIFIED_DATE))
                .ForMember(dest => dest.CountryID, opt => opt.MapFrom(src => src.COUNTRY_ID))
                .ForMember(dest => dest.Week, opt => opt.MapFrom(src => src.WEEK))
                .ForMember(dest => dest.Company, opt => opt.MapFrom(model => model.T001))
                .ForMember(dest => dest.Market, opt => opt.MapFrom(model => model.ZAIDM_EX_MARKET))
                .ForMember(dest => dest.Plant, opt => opt.MapFrom(model => model.T001W))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(model => model.COUNTRY))                
             ;

            Mapper.CreateMap<ProductDevDetailModel, CustomService.Data.PRODUCT_DEVELOPMENT_DETAIL>().IgnoreAllUnmapped()         
                .ForMember(dest => dest.PD_DETAIL_ID, opt => opt.MapFrom(src => src.PD_DETAIL_ID))
                .ForMember(dest => dest.FA_CODE_OLD, opt => opt.MapFrom(src => src.Fa_Code_Old))
                .ForMember(dest => dest.FA_CODE_NEW, opt => opt.MapFrom(src => src.Fa_Code_New))
                .ForMember(dest => dest.HL_CODE, opt => opt.MapFrom(src => src.Hl_Code))
                .ForMember(dest => dest.MARKET_ID, opt => opt.MapFrom(src => src.Market_Id))
                .ForMember(dest => dest.FA_CODE_OLD_DESCR, opt => opt.MapFrom(src => src.Fa_Code_Old_Desc))
                .ForMember(dest => dest.FA_CODE_NEW_DESCR, opt => opt.MapFrom(src => src.Fa_Code_New_Desc))
                .ForMember(dest => dest.WERKS, opt => opt.MapFrom(src => src.Werks))
                .ForMember(dest => dest.IS_IMPORT, opt => opt.MapFrom(src => src.Is_Import))
                .ForMember(dest => dest.PD_ID, opt => opt.MapFrom(src => src.PD_ID))
                .ForMember(dest => dest.REQUEST_NO, opt => opt.MapFrom(src => src.Request_No))
                .ForMember(dest => dest.BUKRS, opt => opt.MapFrom(src => src.Bukrs))
                .ForMember(dest => dest.LASTAPPROVED_BY, opt => opt.MapFrom(src => src.Approved_By))
                .ForMember(dest => dest.LASTAPPROVED_DATE, opt => opt.MapFrom(src => src.Approved_Date))
                .ForMember(dest => dest.STATUS_APPROVAL, opt => opt.MapFrom(src => src.Approval_Status))                 
                .ForMember(dest => dest.APPROVER, opt => opt.MapFrom(model => model.Approver))
                .ForMember(dest => dest.APPROVAL_STATUS, opt => opt.MapFrom(model => model.ApprovalStatusDescription))
                .ForMember(dest => dest.LASTMODIFIED_BY, opt => opt.MapFrom(src => src.Modified_By))
                .ForMember(dest => dest.LASTMODIFIED_DATE, opt => opt.MapFrom(src => src.Modified_Date))
                .ForMember(dest => dest.COUNTRY_ID, opt => opt.MapFrom(src => src.CountryID))
                .ForMember(dest => dest.WEEK, opt => opt.MapFrom(src => src.Week))
                .ForMember(dest => dest.T001, opt => opt.MapFrom(model => model.Company))
                .ForMember(dest => dest.ZAIDM_EX_MARKET, opt => opt.MapFrom(model => model.Market))
                .ForMember(dest => dest.T001W, opt => opt.MapFrom(model => model.Plant))
                .ForMember(dest => dest.COUNTRY, opt => opt.MapFrom(model => model.Country))
            ;
        
            //  Mapper.CreateMap<CustomService.Data.PRODUCT_DEVELOPMENT_DETAIL, PDSummaryReportItem>().IgnoreAllNonExisting()
            //   .ForMember(dest => dest.PD_DETAIL_ID, opt => opt.MapFrom(src => src.PD_DETAIL_ID))
            //   .ForMember(dest => dest.Fa_Code_Old, opt => opt.MapFrom(src => src.FA_CODE_OLD))
            //   .ForMember(dest => dest.Fa_Code_New, opt => opt.MapFrom(src => src.FA_CODE_NEW))
            //   .ForMember(dest => dest.Hl_Code, opt => opt.MapFrom(src => src.HL_CODE))
            //   .ForMember(dest => dest.Market_Id, opt => opt.MapFrom(src => src.MARKET_ID))
            //   .ForMember(dest => dest.Fa_Code_Old_Desc, opt => opt.MapFrom(src => src.FA_CODE_OLD_DESCR))
            //   .ForMember(dest => dest.Fa_Code_New_Desc, opt => opt.MapFrom(src => src.FA_CODE_NEW_DESCR))
            //   .ForMember(dest => dest.Werks, opt => opt.MapFrom(src => src.WERKS))
            //   .ForMember(dest => dest.Is_Import, opt => opt.MapFrom(src => src.IS_IMPORT))
            //   .ForMember(dest => dest.PD_ID, opt => opt.MapFrom(src => src.PD_ID))
            //   .ForMember(dest => dest.Request_No, opt => opt.MapFrom(src => src.REQUEST_NO))
            //   .ForMember(dest => dest.Bukrs, opt => opt.MapFrom(src => src.BUKRS))
            //   .ForMember(dest => dest.Approved_By, opt => opt.MapFrom(src => src.LASTAPPROVED_BY))
            //   .ForMember(dest => dest.Approved_Date, opt => opt.MapFrom(src => src.LASTAPPROVED_DATE))
            //   .ForMember(dest => dest.Approval_Status, opt => opt.MapFrom(src => src.STATUS_APPROVAL))
            //   .ForMember(dest => dest.Approver, opt => opt.MapFrom(model => model.APPROVER))
            //   .ForMember(dest => dest.ApprovalStatusDescription, opt => opt.MapFrom(model => model.APPROVAL_STATUS))
            //   .ForMember(dest => dest.Modified_By, opt => opt.MapFrom(src => src.LASTMODIFIED_BY))
            //   .ForMember(dest => dest.Modified_Date, opt => opt.MapFrom(src => src.LASTMODIFIED_DATE))
            //   .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.PRODUCT_DEVELOPMENT.CREATED_BY))
            //   .ForMember(dest => dest.MarketDesc, opt => opt.MapFrom(src => src.ZAIDM_EX_MARKET.MARKET_DESC))
            //   .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.T001W.NAME1))
            //   .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.T001.BUTXT))
            //;

            #endregion

            #region Brand Registration ( Brand Registration Transaction - Map SKEP)
            Mapper.CreateMap<CustomService.Data.BRAND_REGISTRATION_REQ, BrandRegistrationReqModel>().IgnoreAllNonExisting()
                  .ForMember(dest => dest.Registration_ID, opt => opt.MapFrom(src => src.REGISTRATION_ID))
                  .ForMember(dest => dest.Registration_No, opt => opt.MapFrom(src => src.REGISTRATION_NO))
                  .ForMember(dest => dest.Submission_Date, opt => opt.MapFrom(src => src.SUBMISSION_DATE))
                  .ForMember(dest => dest.Registration_Type, opt => opt.MapFrom(src => src.REGISTRATION_TYPE))
                  .ForMember(dest => dest.Nppbkc_ID, opt => opt.MapFrom(src => src.NPPBKC_ID))
                  .ForMember(dest => dest.Effective_Date, opt => opt.MapFrom(src => src.EFFECTIVE_DATE))
                  .ForMember(dest => dest.Created_By, opt => opt.MapFrom(src => src.CREATED_BY))
                  .ForMember(dest => dest.Created_Date, opt => opt.MapFrom(src => src.CREATED_DATE))
                  .ForMember(dest => dest.LastModified_By, opt => opt.MapFrom(src => src.LASTMODIFIED_BY))
                  .ForMember(dest => dest.LastModified_Date, opt => opt.MapFrom(src => src.LASTMODIFIED_DATE))
                  .ForMember(dest => dest.LastApproved_By, opt => opt.MapFrom(src => src.LASTAPPROVED_BY))
                  .ForMember(dest => dest.LastApproved_Date, opt => opt.MapFrom(src => src.LASTAPPROVED_DATE))
                  .ForMember(dest => dest.LastApproved_Status, opt => opt.MapFrom(src => src.LASTAPPROVED_STATUS))                  
                  .ForMember(dest => dest.Decree_Status, opt => opt.MapFrom(src => src.DECREE_STATUS))
                  .ForMember(dest => dest.Decree_No, opt => opt.MapFrom(src => src.DECREE_NO))
                  .ForMember(dest => dest.Decree_Date, opt => opt.MapFrom(src => src.DECREE_DATE))
                  .ForMember(dest => dest.Decree_StartDate, opt => opt.MapFrom(src => src.DECREE_STARTDATE))

                  .ForMember(entity => entity.Creator, opt => opt.MapFrom(model => model.CREATOR))
                  .ForMember(entity => entity.LastEditor, opt => opt.MapFrom(model => model.LASTEDITOR))
                  .ForMember(entity => entity.Approver, opt => opt.MapFrom(model => model.APPROVER))
                  .ForMember(entity => entity.ApprovalStatusDescription, opt => opt.MapFrom(model => model.APPROVAL_STATUS))
              ;

            Mapper.CreateMap<BrandRegistrationReqModel, CustomService.Data.BRAND_REGISTRATION_REQ>().IgnoreAllUnmapped()
                 .ForMember(dest => dest.REGISTRATION_ID, opt => opt.MapFrom(src => src.Registration_ID))
                  .ForMember(dest => dest.REGISTRATION_TYPE, opt => opt.MapFrom(src => src.Registration_Type))
                  .ForMember(dest => dest.SUBMISSION_DATE, opt => opt.MapFrom(src => src.Submission_Date))
                  .ForMember(dest => dest.REGISTRATION_TYPE, opt => opt.MapFrom(src => src.Registration_Type))
                  .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.Nppbkc_ID))
                  .ForMember(dest => dest.EFFECTIVE_DATE, opt => opt.MapFrom(src => src.Effective_Date))
                  .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.Created_By))
                  .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.Created_Date))
                  .ForMember(dest => dest.LASTMODIFIED_BY, opt => opt.MapFrom(src => src.LastModified_By))
                  .ForMember(dest => dest.LASTMODIFIED_DATE, opt => opt.MapFrom(src => src.LastApproved_Date))
                  .ForMember(dest => dest.LASTAPPROVED_BY, opt => opt.MapFrom(src => src.LastApproved_By))
                  .ForMember(dest => dest.LASTAPPROVED_DATE, opt => opt.MapFrom(src => src.LastModified_Date))
                  .ForMember(dest => dest.LASTAPPROVED_STATUS, opt => opt.MapFrom(src => src.LastApproved_Status))                  
                  .ForMember(dest => dest.DECREE_STATUS, opt => opt.MapFrom(src => src.Decree_Status))
                  .ForMember(dest => dest.DECREE_NO, opt => opt.MapFrom(src => src.Decree_No))
                  .ForMember(dest => dest.DECREE_DATE, opt => opt.MapFrom(src => src.Decree_Date))
                  .ForMember(dest => dest.DECREE_STARTDATE, opt => opt.MapFrom(src => src.Decree_StartDate))

                  .ForMember(entity => entity.CREATOR, opt => opt.MapFrom(model => model.Creator))
                  .ForMember(entity => entity.LASTEDITOR, opt => opt.MapFrom(model => model.LastEditor))
                  .ForMember(entity => entity.APPROVER, opt => opt.MapFrom(model => model.Approver))
                  .ForMember(entity => entity.APPROVAL_STATUS, opt => opt.MapFrom(model => model.ApprovalStatusDescription))
              ;

            Mapper.CreateMap<CustomService.Data.BRAND_REGISTRATION_REQ_DETAIL, BrandRegistrationReqDetailModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PD_Detail_ID, opt => opt.MapFrom(src => src.PD_DETAIL_ID))
                .ForMember(dest => dest.Brand_Ce, opt => opt.MapFrom(src => src.BRAND_CE))
                .ForMember(dest => dest.Latest_Skep_No, opt => opt.MapFrom(src => src.LATEST_SKEP_NO))
                .ForMember(dest => dest.Prod_Code, opt => opt.MapFrom(src => src.PROD_CODE))
                .ForMember(dest => dest.Company_Tier, opt => opt.MapFrom(src => src.COMPANY_TIER))
                .ForMember(dest => dest.Hje, opt => opt.MapFrom(src => src.HJE))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.UNIT))
                .ForMember(dest => dest.Brand_Content, opt => opt.MapFrom(src => src.BRAND_CONTENT))
                .ForMember(dest => dest.Tariff, opt => opt.MapFrom(src => src.TARIFF))
                //.ForMember(dest => dest.Packaging_Material, opt => opt.MapFrom(src => src.MATERIAL_PACKAGE))
                .ForMember(dest => dest.Market_ID, opt => opt.MapFrom(src => src.MARKET_ID))
                .ForMember(dest => dest.Front_Side, opt => opt.MapFrom(src => src.FRONT_SIDE))
                .ForMember(dest => dest.Back_Side, opt => opt.MapFrom(src => src.BACK_SIDE))
                .ForMember(dest => dest.Left_Side, opt => opt.MapFrom(src => src.LEFT_SIDE))
                .ForMember(dest => dest.Right_Side, opt => opt.MapFrom(src => src.RIGHT_SIDE))
                .ForMember(dest => dest.Top_Side, opt => opt.MapFrom(src => src.TOP_SIDE))
                .ForMember(dest => dest.Bottom_Side, opt => opt.MapFrom(src => src.BOTTOM_SIDE))
                .ForMember(dest => dest.Bottom_Side, opt => opt.MapFrom(src => src.BOTTOM_SIDE))
                .ForMember(dest => dest.Registration_ID, opt => opt.MapFrom(src => src.REGISTRATION_ID))
            ;

            Mapper.CreateMap<BrandRegistrationReqDetailModel, CustomService.Data.BRAND_REGISTRATION_REQ_DETAIL>().IgnoreAllUnmapped()
                  .ForMember(dest => dest.PD_DETAIL_ID, opt => opt.MapFrom(src => src.PD_Detail_ID))
                .ForMember(dest => dest.BRAND_CE, opt => opt.MapFrom(src => src.Brand_Ce))
                .ForMember(dest => dest.LATEST_SKEP_NO, opt => opt.MapFrom(src => src.Latest_Skep_No))
                .ForMember(dest => dest.PROD_CODE, opt => opt.MapFrom(src => src.Prod_Code))
                .ForMember(dest => dest.COMPANY_TIER, opt => opt.MapFrom(src => src.Company_Tier))
                .ForMember(dest => dest.HJE, opt => opt.MapFrom(src => src.Hje))
                .ForMember(dest => dest.UNIT, opt => opt.MapFrom(src => src.Unit))
                .ForMember(dest => dest.BRAND_CONTENT, opt => opt.MapFrom(src => src.Brand_Content))
                .ForMember(dest => dest.TARIFF, opt => opt.MapFrom(src => src.Tariff))
                //.ForMember(dest => dest.MATERIAL_PACKAGE, opt => opt.MapFrom(src => src.Packaging_Material))
                .ForMember(dest => dest.MARKET_ID, opt => opt.MapFrom(src => src.Market_ID))
                .ForMember(dest => dest.FRONT_SIDE, opt => opt.MapFrom(src => src.Front_Side))
                .ForMember(dest => dest.BACK_SIDE, opt => opt.MapFrom(src => src.Back_Side))
                .ForMember(dest => dest.LEFT_SIDE, opt => opt.MapFrom(src => src.Left_Side))
                .ForMember(dest => dest.RIGHT_SIDE, opt => opt.MapFrom(src => src.Right_Side))
                .ForMember(dest => dest.TOP_SIDE, opt => opt.MapFrom(src => src.Top_Side))
                .ForMember(dest => dest.BOTTOM_SIDE, opt => opt.MapFrom(src => src.Bottom_Side))
                .ForMember(dest => dest.REGISTRATION_ID, opt => opt.MapFrom(src => src.Registration_ID))
              ;
            #endregion

            #region Received Decree ( Brand Registration Transaction - Penetapan SKEP)
            Mapper.CreateMap<CustomService.Data.RECEIVED_DECREE, ReceivedDecreeModel>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.Received_ID, opt => opt.MapFrom(src => src.RECEIVED_ID))
                 .ForMember(dest => dest.Received_No, opt => opt.MapFrom(src => src.RECEIVED_NO))
                 .ForMember(dest => dest.Nppbkc_ID, opt => opt.MapFrom(src => src.NPPBKC_ID))
                 .ForMember(dest => dest.Created_By, opt => opt.MapFrom(src => src.CREATED_BY))
                 .ForMember(dest => dest.Created_Date, opt => opt.MapFrom(src => src.CREATED_DATE))
                 .ForMember(dest => dest.LastModified_By, opt => opt.MapFrom(src => src.LASTMODIFIED_BY))
                 .ForMember(dest => dest.LastModified_Date, opt => opt.MapFrom(src => src.LASTMODIFIED_DATE))
                 .ForMember(dest => dest.LastApproved_By, opt => opt.MapFrom(src => src.LASTAPPROVED_BY))
                 .ForMember(dest => dest.LastApproved_Date, opt => opt.MapFrom(src => src.LASTAPPROVED_DATE))
                 .ForMember(dest => dest.LastApproved_Status, opt => opt.MapFrom(src => src.LASTAPPROVED_STATUS))                                  
                 .ForMember(dest => dest.Decree_No, opt => opt.MapFrom(src => src.DECREE_NO))
                 .ForMember(dest => dest.Decree_Date, opt => opt.MapFrom(src => src.DECREE_DATE))
                 .ForMember(dest => dest.Decree_StartDate, opt => opt.MapFrom(src => src.DECREE_STARTDATE))

                 .ForMember(entity => entity.Creator, opt => opt.MapFrom(model => model.CREATOR))
                 .ForMember(entity => entity.LastEditor, opt => opt.MapFrom(model => model.LASTEDITOR))
                 .ForMember(entity => entity.Approver, opt => opt.MapFrom(model => model.APPROVER))
                 .ForMember(entity => entity.ApprovalStatusDescription, opt => opt.MapFrom(model => model.APPROVAL_STATUS))
             ;

            Mapper.CreateMap<ReceivedDecreeModel, CustomService.Data.RECEIVED_DECREE>().IgnoreAllUnmapped()
                  .ForMember(dest => dest.RECEIVED_ID, opt => opt.MapFrom(src => src.Received_ID))
                 .ForMember(dest => dest.RECEIVED_NO, opt => opt.MapFrom(src => src.Received_No))
                 .ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.Nppbkc_ID))
                 .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.Created_By))
                 .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.Created_Date))
                 .ForMember(dest => dest.LASTMODIFIED_BY, opt => opt.MapFrom(src => src.LastModified_By))
                 .ForMember(dest => dest.LASTMODIFIED_DATE, opt => opt.MapFrom(src => src.LastModified_Date))
                 .ForMember(dest => dest.LASTAPPROVED_BY, opt => opt.MapFrom(src => src.LastApproved_By))
                 .ForMember(dest => dest.LASTAPPROVED_DATE, opt => opt.MapFrom(src => src.LastApproved_Date))
                 .ForMember(dest => dest.LASTAPPROVED_STATUS, opt => opt.MapFrom(src => src.LastApproved_Status))                 
                 .ForMember(dest => dest.DECREE_NO, opt => opt.MapFrom(src => src.Decree_No))
                 .ForMember(dest => dest.DECREE_DATE, opt => opt.MapFrom(src => src.Decree_Date))
                 .ForMember(dest => dest.DECREE_STARTDATE, opt => opt.MapFrom(src => src.Decree_StartDate))

                 .ForMember(entity => entity.CREATOR, opt => opt.MapFrom(model => model.Creator))
                 .ForMember(entity => entity.LASTEDITOR, opt => opt.MapFrom(model => model.LastEditor))
                 .ForMember(entity => entity.APPROVER, opt => opt.MapFrom(model => model.Approver))
                 .ForMember(entity => entity.APPROVAL_STATUS, opt => opt.MapFrom(model => model.ApprovalStatusDescription))
              ;

            Mapper.CreateMap<CustomService.Data.RECEIVED_DECREE_DETAIL, ReceivedDecreeDetailModel>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.Brand_CE, opt => opt.MapFrom(src => src.BRAND_CE))
                 .ForMember(dest => dest.Prod_Code, opt => opt.MapFrom(src => src.PROD_CODE))
                 .ForMember(dest => dest.Company_Tier, opt => opt.MapFrom(src => src.COMPANY_TIER))
                 .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.UNIT))
                 .ForMember(dest => dest.Brand_Content, opt => opt.MapFrom(src => src.BRAND_CONTENT))
                 .ForMember(dest => dest.Tariff, opt => opt.MapFrom(src => src.TARIFF))
                 .ForMember(dest => dest.Received_ID, opt => opt.MapFrom(src => src.RECEIVED_ID))
                 .ForMember(dest => dest.PD_Detail_ID, opt => opt.MapFrom(src => src.PD_DETAIL_ID))
                 .ForMember(dest => dest.Received_Detail_ID, opt => opt.MapFrom(src => src.RECEIVED_DETAIL_ID))                             
             ;

            Mapper.CreateMap<ReceivedDecreeDetailModel, CustomService.Data.RECEIVED_DECREE_DETAIL>().IgnoreAllUnmapped()
                 .ForMember(dest => dest.BRAND_CE, opt => opt.MapFrom(src => src.Brand_CE))
                 .ForMember(dest => dest.PROD_CODE, opt => opt.MapFrom(src => src.Prod_Code))
                 .ForMember(dest => dest.COMPANY_TIER, opt => opt.MapFrom(src => src.Company_Tier))
                 .ForMember(dest => dest.UNIT, opt => opt.MapFrom(src => src.Unit))
                 .ForMember(dest => dest.BRAND_CONTENT, opt => opt.MapFrom(src => src.Brand_Content))
                 .ForMember(dest => dest.TARIFF, opt => opt.MapFrom(src => src.Tariff))
                 .ForMember(dest => dest.RECEIVED_ID, opt => opt.MapFrom(src => src.Received_ID))
                 .ForMember(dest => dest.PD_DETAIL_ID, opt => opt.MapFrom(src => src.PD_Detail_ID))
                 .ForMember(dest => dest.RECEIVED_DETAIL_ID, opt => opt.MapFrom(src => src.Received_Detail_ID))
              ;
            #endregion

            #region File Upload

            Mapper.CreateMap<CustomService.Data.FILE_UPLOAD, FileUploadModel>().IgnoreAllNonExisting()
                   .ForMember(dest => dest.FileID, opt => opt.MapFrom(src => src.FILE_ID))
                   .ForMember(dest => dest.FormTypeID, opt => opt.MapFrom(src => src.FORM_TYPE_ID))
                   .ForMember(dest => dest.FormID, opt => opt.MapFrom(src => src.FORM_ID))
                   .ForMember(dest => dest.PathURL, opt => opt.MapFrom(src => src.PATH_URL))
                   .ForMember(dest => dest.UploadDate, opt => opt.MapFrom(src => src.UPLOAD_DATE))
                   .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                   .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                   .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.LASTMODIFIED_BY))
                   .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.LASTMODIFIED_DATE))
                   .ForMember(dest => dest.DocumentID, opt => opt.MapFrom(src => src.DOCUMENT_ID))
                   .ForMember(dest => dest.IsGovernmentDoc, opt => opt.MapFrom(src => src.IS_GOVERNMENT_DOC))
                   .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE))
                   .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FILE_NAME))
               ;

            Mapper.CreateMap<FileUploadModel, CustomService.Data.FILE_UPLOAD>().IgnoreAllUnmapped()
                  .ForMember(dest => dest.FILE_ID, opt => opt.MapFrom(src => src.FileID))
                  .ForMember(dest => dest.FORM_TYPE_ID, opt => opt.MapFrom(src => src.FormTypeID))
                  .ForMember(dest => dest.FORM_ID, opt => opt.MapFrom(src => src.FormID))
                  .ForMember(dest => dest.PATH_URL, opt => opt.MapFrom(src => src.PathURL))
                  .ForMember(dest => dest.UPLOAD_DATE, opt => opt.MapFrom(src => src.UploadDate))
                  .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                  .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                  .ForMember(dest => dest.LASTMODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                  .ForMember(dest => dest.LASTMODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                  .ForMember(dest => dest.DOCUMENT_ID, opt => opt.MapFrom(src => src.DocumentID))
                  .ForMember(dest => dest.IS_GOVERNMENT_DOC, opt => opt.MapFrom(src => src.IsGovernmentDoc))
                  .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive))
              ;
            #endregion

            #region Product Development Upload

            Mapper.CreateMap<CustomService.Data.PRODUCT_DEVELOPMENT_UPLOAD, ProductDevelopmentUploadModel>().IgnoreAllNonExisting()
                  .ForMember(dest => dest.File_ID, opt => opt.MapFrom(src => src.FILE_ID))
                  .ForMember(dest => dest.PD_Detail_ID, opt => opt.MapFrom(src => src.PD_DETAIL_ID))
                  .ForMember(dest => dest.Item_ID, opt => opt.MapFrom(src => src.ITEM_ID))
                  .ForMember(dest => dest.Path_Url, opt => opt.MapFrom(src => src.PATH_URL))
                  .ForMember(dest => dest.Upload_Date, opt => opt.MapFrom(src => src.UPLOAD_DATE))
                  .ForMember(dest => dest.Document_ID, opt => opt.MapFrom(src => src.DOCUMENT_ID))                  
                  .ForMember(dest => dest.Is_Active, opt => opt.MapFrom(src => src.IS_ACTIVE))
                  .ForMember(dest => dest.File_Name, opt => opt.MapFrom(src => src.FILE_NAME))
              ;

            Mapper.CreateMap<ProductDevelopmentUploadModel, CustomService.Data.PRODUCT_DEVELOPMENT_UPLOAD>().IgnoreAllUnmapped()
                  .ForMember(dest => dest.FILE_ID, opt => opt.MapFrom(src => src.File_ID))
                  .ForMember(dest => dest.PD_DETAIL_ID, opt => opt.MapFrom(src => src.PD_Detail_ID))
                  .ForMember(dest => dest.ITEM_ID, opt => opt.MapFrom(src => src.Item_ID))
                  .ForMember(dest => dest.PATH_URL, opt => opt.MapFrom(src => src.Path_Url))
                  .ForMember(dest => dest.UPLOAD_DATE, opt => opt.MapFrom(src => src.Upload_Date))                  
                  .ForMember(dest => dest.DOCUMENT_ID, opt => opt.MapFrom(src => src.Document_ID))                  
                  .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.Is_Active))
                  .ForMember(dest => dest.FILE_NAME, opt => opt.MapFrom(src => src.File_Name))
              ;
            #endregion

            #region Country Model
            Mapper.CreateMap<CountryModel, CustomService.Data.COUNTRY>().IgnoreAllUnmapped()
                .ForMember(entity => entity.COUNTRY_ID, opt => opt.MapFrom(model => model.CountryID))
                .ForMember(entity => entity.COUNTRY_CODE, opt => opt.MapFrom(model => model.CountryCode))
                .ForMember(entity => entity.COUNTRY_NAME, opt => opt.MapFrom(model => model.CountryName))
            ;
            Mapper.CreateMap<CustomService.Data.COUNTRY, CountryModel>().IgnoreAllNonExisting()
                .ForMember(entity => entity.CountryID, opt => opt.MapFrom(model => model.COUNTRY_ID))
                .ForMember(entity => entity.CountryCode, opt => opt.MapFrom(model => model.COUNTRY_CODE))
                .ForMember(entity => entity.CountryName, opt => opt.MapFrom(model => model.COUNTRY_NAME))
                ;
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