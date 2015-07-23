using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Models.CK5;

namespace Sampoerna.EMS.Website
{
    public partial class EMSWebsiteMapper
    {
        public static void InitializeCK5()
        {
            //Mapper.CreateMap<CK5, CK5Item>().IgnoreAllNonExisting()
            //    .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.SUBMISSION_NUMBER))
            //    .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.GRAND_TOTAL_EX)) //todo ask
            //    .ForMember(dest => dest.UOM, opt => opt.MapFrom(src => src.UOM.UOM_NAME))
            //    .ForMember(dest => dest.POA, opt => opt.MapFrom(src => src.CK5_TYPE));
            ////.ForMember(dest => dest.POA, opt => opt.MapFrom(src => src.po)) //todo ask
            //.ForMember(dest => dest.NPPBKC_ID, opt => opt.MapFrom(src => src.T1001W.ZAIDM_EX_NPPBKC.NPPBKC_NO))//todo ask
            // .ForMember(dest => dest.SourcePlant, opt => opt.MapFrom(src => src.T1001W.CITY)) //todo ask
            //.ForMember(dest => dest.SourcePlant, opt => opt.MapFrom(src => src.)) //todo ask
            // .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.STATUS.STATUS_NAME));

            //Mapper.CreateMap<CK5SearchViewModel, CK5Input>().IgnoreAllNonExisting();
            //Mapper.CreateMap<SUPPLIER_PORT, SelectItemModel>().IgnoreAllNonExisting()
            //    .ForMember(dest => dest.TextField, opt => opt.MapFrom(src => src.SUPPLIER_PORT_ID + "-" + src.PORT_NAME))
            //    .ForMember(dest => dest.ValueField, opt => opt.MapFrom(src => src.SUPPLIER_PORT_ID))
            //    ;

            Mapper.CreateMap<CK5MaterialOutput, CK5UploadViewModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5Dto, CK5Item>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Ck5Id, opt => opt.MapFrom(src => src.CK5_ID))
                .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.SUBMISSION_NUMBER))
                .ForMember(dest => dest.Qty, opt => opt.ResolveUsing<CK5ListIndexQtyResolver>().FromMember(src => src))
                .ForMember(dest => dest.POA, opt => opt.MapFrom(src => src.APPROVED_BY));
                //.ForMember(dest => dest.SourcePlant, opt => opt.MapFrom(src => src.SourcePlantWerks + " - " + src.SourcePlantName))
                //.ForMember(dest => dest.DestinationPlant, opt => opt.MapFrom(src => src.DestPlantWerks + " - " + src.DestPlantName))
                //.ForMember(dest => dest.Status, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.STATUS_ID)));

            Mapper.CreateMap<CK5SearchViewModel, CK5GetByParamInput>().IgnoreAllNonExisting();


            Mapper.CreateMap<T001W, CK5PlantModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.PlantNpwp, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC.T001.NPWP))
                .ForMember(dest => dest.NPPBCK_ID, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.ZAIDM_EX_NPPBKC.T001.BUTXT))
                .ForMember(dest => dest.CompanyAddress, opt => opt.MapFrom(src => src.ADDRESS))
                .ForMember(dest => dest.KppBcName, opt => opt.MapFrom(src => src.NAME1));

            Mapper.CreateMap<CK5FormViewModel, CK5Dto>().IgnoreAllNonExisting()
              .ForMember(dest => dest.CK5_TYPE, opt => opt.MapFrom(src => src.Ck5Type))
              .ForMember(dest => dest.KPPBC_CITY, opt => opt.MapFrom(src => src.KppBcCityId))
              .ForMember(dest => dest.SUBMISSION_NUMBER, opt => opt.MapFrom(src => src.SubmissionNumber))
              .ForMember(dest => dest.REGISTRATION_NUMBER, opt => opt.MapFrom(src => src.RegistrationNumber))
              //.ForMember(dest => dest.EX_GOODS_TYPE_ID, opt => opt.MapFrom(src => src.GoodTypeId))
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
               // .ForMember(dest => dest.KppBcCityName, opt => opt.MapFrom(src => src.KppbcCityName))
               // .ForMember(dest => dest.CeOfficeCode, opt => opt.MapFrom(src => src.CeOfficeCode))
                .ForMember(dest => dest.SubmissionNumber, opt => opt.MapFrom(src => src.SUBMISSION_NUMBER))
                .ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(src => src.SUBMISSION_DATE))
                .ForMember(dest => dest.RegistrationNumber, opt => opt.MapFrom(src => src.REGISTRATION_NUMBER))
               // .ForMember(dest => dest.GoodTypeId, opt => opt.MapFrom(src => src.EX_GOODS_TYPE_ID))
                //.ForMember(dest => dest.GoodTypeName, opt => opt.MapFrom(src => src.GoodTypeDesc))
                .ForMember(dest => dest.ExciseSettlementId, opt => opt.MapFrom(src => src.EX_SETTLEMENT_ID))
                //.ForMember(dest => dest.ExciseSettlementName, opt => opt.MapFrom(src => src.ExSettlementName))
                .ForMember(dest => dest.ExciseStatusId, opt => opt.MapFrom(src => src.EX_STATUS_ID))
                //.ForMember(dest => dest.ExciseStatusName, opt => opt.MapFrom(src => src.ExStatusName))
                //.ForMember(dest => dest.RequestTypeId, opt => opt.MapFrom(src => src.REQUEST_TYPE_ID))
                //.ForMember(dest => dest.RequestTypeName, opt => opt.MapFrom(src => src.RequestTypeName))
                .ForMember(dest => dest.SourcePlantId, opt => opt.MapFrom(src => src.SOURCE_PLANT_ID))
                //.ForMember(dest => dest.SourcePlantName, opt => opt.MapFrom(src => src.SourcePlantWerks))
                .ForMember(dest => dest.DestPlantId, opt => opt.MapFrom(src => src.DEST_PLANT_ID))
                //.ForMember(dest => dest.DestPlantName, opt => opt.MapFrom(src => src.DestPlantWerks))
                .ForMember(dest => dest.InvoiceNumber, opt => opt.MapFrom(src => src.INVOICE_NUMBER))
                .ForMember(dest => dest.PbckDecreeId, opt => opt.MapFrom(src => src.PBCK1_DECREE_ID))
                .ForMember(dest => dest.PbckDecreeNumber, opt => opt.MapFrom(src => src.PbckNumber))
                .ForMember(dest => dest.PbckDecreeDate, opt => opt.MapFrom(src => src.PbckDecreeDate))
                .ForMember(dest => dest.CarriageMethodId, opt => opt.MapFrom(src => src.CARRIAGE_METHOD_ID))
                //.ForMember(dest => dest.CarriageMethodName, opt => opt.MapFrom(src => src.CarriageMethodName))
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
                .ForMember(dest => dest.UnsealingNotifDate, opt => opt.MapFrom(src => src.UNSEALING_NOTIF_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY));
            ////todo attachment
            ////todo Requisitioner

            Mapper.CreateMap<CK5UploadViewModel, CK5MaterialDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.QTY, opt => opt.ResolveUsing<StringToDecimalResolver>().FromMember(src => src.Qty))
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
                
            
        }
    }
}