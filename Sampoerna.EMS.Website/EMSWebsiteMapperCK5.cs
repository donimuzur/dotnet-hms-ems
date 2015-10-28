using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Core;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Models.CK5;

namespace Sampoerna.EMS.Website
{
    public partial class EMSWebsiteMapper
    {
        public static void InitializeCK5()
        {
           
          
            Mapper.CreateMap<CK5MaterialOutput, CK5UploadViewModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5Dto, CK5Item>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Ck5Id, opt => opt.MapFrom(src => src.CK5_ID))
                .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.SUBMISSION_NUMBER))
                .ForMember(dest => dest.Qty, opt => opt.ResolveUsing<CK5ListIndexQtyResolver>().FromMember(src => src))
                .ForMember(dest => dest.POA, opt => opt.ResolveUsing<CK5ListIndexPOAResolver>().FromMember(src => src))
                .ForMember(dest => dest.SourcePlant, opt => opt.MapFrom(src => src.SOURCE_PLANT_ID + " - " + src.SOURCE_PLANT_NAME))
                .ForMember(dest => dest.DestinationPlant, opt => opt.ResolveUsing<CK5ListIndexDestinationPlantResolver>().FromMember(src => src))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.STATUS_ID)));

            Mapper.CreateMap<CK5SearchViewModel, CK5GetByParamInput>().IgnoreAllNonExisting();


            Mapper.CreateMap<T001WDto, CK5PlantModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.WERKS))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.NAME1))
                .ForMember(dest => dest.PlantNpwp, opt => opt.MapFrom(src => src.Npwp))
                .ForMember(dest => dest.NPPBCK_ID, opt => opt.MapFrom(src => src.NPPBKC_ID))
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.CompanyCode))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.CompanyAddress, opt => opt.MapFrom(src => src.ADDRESS))
                .ForMember(dest => dest.KppBcName, opt => opt.MapFrom(src => src.KppbcCity + "-" + src.KppbcNo ))
                .ForMember(dest => dest.KppbcCity, opt => opt.MapFrom(src => src.KppbcCity))
                .ForMember(dest => dest.KppbcNo, opt => opt.MapFrom(src => src.KppbcNo))
                ;

            Mapper.CreateMap<CK5FormViewModel, CK5Dto>().IgnoreAllNonExisting()
              .ForMember(dest => dest.CK5_ID, opt => opt.MapFrom(src => src.Ck5Id))
               .ForMember(dest => dest.CK5_TYPE, opt => opt.MapFrom(src => src.Ck5Type))
               .ForMember(dest => dest.STATUS_ID, opt => opt.MapFrom(src => src.DocumentStatus))

               .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
               .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))

              .ForMember(dest => dest.KPPBC_CITY, opt => opt.MapFrom(src => src.KppBcCity))
              .ForMember(dest => dest.CE_OFFICE_CODE, opt => opt.MapFrom(src => src.CeOfficeCode))
              .ForMember(dest => dest.SUBMISSION_NUMBER, opt => opt.MapFrom(src => src.SubmissionNumber))
              .ForMember(dest => dest.SUBMISSION_DATE, opt => opt.MapFrom(src => src.SubmissionDate))
              .ForMember(dest => dest.REGISTRATION_NUMBER, opt => opt.MapFrom(src => src.RegistrationNumber))
              .ForMember(dest => dest.REGISTRATION_DATE, opt => opt.MapFrom(src => src.RegistrationDate))

              .ForMember(dest => dest.EX_GOODS_TYPE_DESC, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.GoodType)))
              .ForMember(dest => dest.EX_GOODS_TYPE, opt => opt.MapFrom(src => src.GoodType))

              .ForMember(dest => dest.EX_SETTLEMENT_ID, opt => opt.MapFrom(src => src.ExciseSettlement))
              .ForMember(dest => dest.EX_STATUS_ID, opt => opt.MapFrom(src => src.ExciseStatus))
              .ForMember(dest => dest.REQUEST_TYPE_ID, opt => opt.MapFrom(src => src.RequestType))

              .ForMember(dest => dest.SOURCE_PLANT_ID, opt => opt.MapFrom(src => src.SourcePlantId))
              .ForMember(dest => dest.SOURCE_PLANT_NPWP, opt => opt.MapFrom(src => src.SourceNpwp))
              .ForMember(dest => dest.SOURCE_PLANT_NPPBKC_ID, opt => opt.MapFrom(src => src.SourceNppbkcId))
              .ForMember(dest => dest.SOURCE_PLANT_COMPANY_CODE, opt => opt.MapFrom(src => src.SourceCompanyCode))
              .ForMember(dest => dest.SOURCE_PLANT_COMPANY_NAME, opt => opt.MapFrom(src => src.SourceCompanyName))
              .ForMember(dest => dest.SOURCE_PLANT_ADDRESS, opt => opt.MapFrom(src => src.SourceAddress))
              .ForMember(dest => dest.SOURCE_PLANT_KPPBC_NAME_OFFICE, opt => opt.MapFrom(src => src.SourceKppbcName))
              .ForMember(dest => dest.SOURCE_PLANT_NAME, opt => opt.MapFrom(src => src.SourcePlantName))

              .ForMember(dest => dest.DEST_PLANT_ID, opt => opt.MapFrom(src => src.DestPlantId))
              .ForMember(dest => dest.DEST_PLANT_NPWP, opt => opt.MapFrom(src => src.DestNpwp))
              .ForMember(dest => dest.DEST_PLANT_NPPBKC_ID, opt => opt.MapFrom(src => src.DestNppbkcId))
              .ForMember(dest => dest.DEST_PLANT_COMPANY_CODE, opt => opt.MapFrom(src => src.DestCompanyCode))
              .ForMember(dest => dest.DEST_PLANT_COMPANY_NAME, opt => opt.MapFrom(src => src.DestCompanyName))
              .ForMember(dest => dest.DEST_PLANT_ADDRESS, opt => opt.MapFrom(src => src.DestAddress))
              .ForMember(dest => dest.DEST_PLANT_KPPBC_NAME_OFFICE, opt => opt.MapFrom(src => src.DestKppbcName))
              .ForMember(dest => dest.DEST_PLANT_NAME, opt => opt.MapFrom(src => src.DestPlantName))

              .ForMember(dest => dest.LOADING_PORT, opt => opt.MapFrom(src => src.LoadingPort))
              .ForMember(dest => dest.LOADING_PORT_NAME, opt => opt.MapFrom(src => src.LoadingPortName))
              .ForMember(dest => dest.LOADING_PORT_ID, opt => opt.MapFrom(src => src.LoadingPortId))
              .ForMember(dest => dest.FINAL_PORT, opt => opt.MapFrom(src => src.FinalPort))
              .ForMember(dest => dest.FINAL_PORT_NAME, opt => opt.MapFrom(src => src.FinalPortName))
              .ForMember(dest => dest.FINAL_PORT_ID, opt => opt.MapFrom(src => src.FinalPortId))

               .ForMember(dest => dest.INVOICE_NUMBER, opt => opt.MapFrom(src => src.InvoiceNumber))
               .ForMember(dest => dest.INVOICE_DATE, opt => opt.MapFrom(src => src.InvoiceDate))
               .ForMember(dest => dest.PBCK1_DECREE_ID, opt => opt.MapFrom(src => src.PbckDecreeId))
                .ForMember(dest => dest.PbckNumber, opt => opt.MapFrom(src => src.PbckDecreeNumber))
               .ForMember(dest => dest.CARRIAGE_METHOD_ID, opt => opt.MapFrom(src => src.CarriageMethod))
               .ForMember(dest => dest.GRAND_TOTAL_EX, opt => opt.MapFrom(src => src.GrandTotalEx))
              .ForMember(dest => dest.PACKAGE_UOM_ID, opt => opt.MapFrom(src => src.PackageUomName))
              .ForMember(dest => dest.DEST_COUNTRY_CODE, opt => opt.MapFrom(src => src.CountryCode))
              .ForMember(dest => dest.DEST_COUNTRY_NAME, opt => opt.MapFrom(src => src.CountryName))

              .ForMember(dest => dest.RemainQuota, opt => opt.MapFrom(src => src.RemainQuota))
              .ForMember(dest => dest.CK5_MANUAL_TYPE, opt => opt.MapFrom(src => src.Ck5ManualType))
              .ForMember(dest => dest.CK5_REF_ID, opt => opt.MapFrom(src => src.Ck5RefId))
              ;

            Mapper.CreateMap<CK5Dto, CK5FormViewModel>().IgnoreAllNonExisting()
            .ForMember(dest => dest.Ck5Id, opt => opt.MapFrom(src => src.CK5_ID))
            .ForMember(dest => dest.Ck5Type, opt => opt.MapFrom(src => src.CK5_TYPE))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))

            .ForMember(dest => dest.DocumentStatus, opt => opt.MapFrom(src => src.STATUS_ID))
            .ForMember(dest => dest.DocumentStatusDescription, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.STATUS_ID)))

            .ForMember(dest => dest.KppBcCity, opt => opt.MapFrom(src => src.KPPBC_CITY))
            .ForMember(dest => dest.CeOfficeCode, opt => opt.MapFrom(src => src.CE_OFFICE_CODE))
            .ForMember(dest => dest.SubmissionNumber, opt => opt.MapFrom(src => src.SUBMISSION_NUMBER))
            .ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(src => src.SUBMISSION_DATE))
            .ForMember(dest => dest.RegistrationNumber, opt => opt.MapFrom(src => src.REGISTRATION_NUMBER))
            .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.REGISTRATION_DATE))

            .ForMember(dest => dest.GoodType, opt => opt.MapFrom(src => src.EX_GOODS_TYPE))
            .ForMember(dest => dest.GoodTypeName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.EX_GOODS_TYPE)))

            .ForMember(dest => dest.ExciseSettlement, opt => opt.MapFrom(src => src.EX_SETTLEMENT_ID))
            .ForMember(dest => dest.ExciseSettlementDesc, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.EX_SETTLEMENT_ID)))
            .ForMember(dest => dest.ExciseStatus, opt => opt.MapFrom(src => src.EX_STATUS_ID))
            .ForMember(dest => dest.ExciseStatusDesc, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.EX_STATUS_ID)))
            .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => src.REQUEST_TYPE_ID))
            .ForMember(dest => dest.RequestTypeDesc, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.REQUEST_TYPE_ID)))

            .ForMember(dest => dest.SourcePlantId, opt => opt.MapFrom(src => src.SOURCE_PLANT_ID))
            .ForMember(dest => dest.SourceNpwp, opt => opt.MapFrom(src => src.SOURCE_PLANT_NPWP))
            .ForMember(dest => dest.SourceNppbkcId, opt => opt.MapFrom(src => src.SOURCE_PLANT_NPPBKC_ID))
            .ForMember(dest => dest.SourceCompanyCode, opt => opt.MapFrom(src => src.SOURCE_PLANT_COMPANY_CODE))
            .ForMember(dest => dest.SourceCompanyName, opt => opt.MapFrom(src => src.SOURCE_PLANT_COMPANY_NAME))
            .ForMember(dest => dest.SourceAddress, opt => opt.MapFrom(src => src.SOURCE_PLANT_ADDRESS))
            .ForMember(dest => dest.SourceKppbcName, opt => opt.MapFrom(src => src.SOURCE_PLANT_KPPBC_NAME_OFFICE))
            .ForMember(dest => dest.SourcePlantName, opt => opt.MapFrom(src => src.SOURCE_PLANT_NAME))

            .ForMember(dest => dest.DestPlantId, opt => opt.MapFrom(src => src.DEST_PLANT_ID))
            .ForMember(dest => dest.DestNpwp, opt => opt.MapFrom(src => src.DEST_PLANT_NPWP))
            .ForMember(dest => dest.DestNppbkcId, opt => opt.MapFrom(src => src.DEST_PLANT_NPPBKC_ID))
            .ForMember(dest => dest.DestCompanyName, opt => opt.MapFrom(src => src.DEST_PLANT_COMPANY_NAME))
            .ForMember(dest => dest.DestCompanyCode, opt => opt.MapFrom(src => src.DEST_PLANT_COMPANY_CODE))
            .ForMember(dest => dest.DestAddress, opt => opt.MapFrom(src => src.DEST_PLANT_ADDRESS))
            .ForMember(dest => dest.DestKppbcName, opt => opt.MapFrom(src => src.DEST_PLANT_KPPBC_NAME_OFFICE))
            .ForMember(dest => dest.DestPlantName, opt => opt.MapFrom(src => src.DEST_PLANT_NAME))

             .ForMember(dest => dest.LoadingPort, opt => opt.MapFrom(src => src.LOADING_PORT))
              .ForMember(dest => dest.LoadingPortName, opt => opt.MapFrom(src => src.LOADING_PORT_NAME))
              .ForMember(dest => dest.LoadingPortId, opt => opt.MapFrom(src => src.LOADING_PORT_ID))
              .ForMember(dest => dest.FinalPort, opt => opt.MapFrom(src => src.FINAL_PORT))
              .ForMember(dest => dest.FinalPortName, opt => opt.MapFrom(src => src.FINAL_PORT_NAME))
              .ForMember(dest => dest.FinalPortId, opt => opt.MapFrom(src => src.FINAL_PORT_ID))

             .ForMember(dest => dest.InvoiceNumber, opt => opt.MapFrom(src => src.INVOICE_NUMBER))
             .ForMember(dest => dest.InvoiceDate, opt => opt.MapFrom(src => src.INVOICE_DATE))
             .ForMember(dest => dest.PbckDecreeId, opt => opt.MapFrom(src => src.PBCK1_DECREE_ID))
             .ForMember(dest => dest.PbckDecreeNumber, opt => opt.MapFrom(src => src.PbckNumber))
             .ForMember(dest => dest.CarriageMethod, opt => opt.MapFrom(src => src.CARRIAGE_METHOD_ID))
             .ForMember(dest => dest.CarriageMethodDesc, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.CARRIAGE_METHOD_ID)))
             .ForMember(dest => dest.GrandTotalEx, opt => opt.MapFrom(src => src.GRAND_TOTAL_EX))
            .ForMember(dest => dest.PackageUomName, opt => opt.MapFrom(src => src.PACKAGE_UOM_ID))

            .ForMember(dest => dest.DnNumber, opt => opt.MapFrom(src => src.DN_NUMBER))
            .ForMember(dest => dest.DnDate, opt => opt.MapFrom(src => src.DN_DATE))
            .ForMember(dest => dest.StoSenderNumber, opt => opt.MapFrom(src => src.STO_SENDER_NUMBER))
            .ForMember(dest => dest.StoReceiverNumber, opt => opt.MapFrom(src => src.STO_RECEIVER_NUMBER))
            .ForMember(dest => dest.StobNumber, opt => opt.MapFrom(src => src.STOB_NUMBER))
            .ForMember(dest => dest.GiDate, opt => opt.MapFrom(src => src.GI_DATE))
            .ForMember(dest => dest.GrDate, opt => opt.MapFrom(src => src.GR_DATE))
            .ForMember(dest => dest.SealingNotifNumber, opt => opt.MapFrom(src => src.SEALING_NOTIF_NUMBER))
            .ForMember(dest => dest.SealingNotifDate, opt => opt.MapFrom(src => src.SEALING_NOTIF_DATE))
            .ForMember(dest => dest.UnSealingNotifNumber, opt => opt.MapFrom(src => src.UNSEALING_NOTIF_NUMBER))
            .ForMember(dest => dest.UnsealingNotifDate, opt => opt.MapFrom(src => src.UNSEALING_NOTIF_DATE))

            .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.DEST_COUNTRY_CODE))
            .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.DEST_COUNTRY_NAME))
            .ForMember(dest => dest.DisplayDetailsDestinationCountry, opt => opt.MapFrom(src => src.DEST_COUNTRY_CODE + " - " + src.DEST_COUNTRY_NAME))
            .ForMember(dest => dest.Ck5ManualType, opt => opt.MapFrom(src => src.CK5_MANUAL_TYPE))
            .ForMember(dest => dest.Ck5ManualTypeString, opt => opt.MapFrom(src => Utils.EnumHelper.GetDescription(src.CK5_MANUAL_TYPE)))
            .ForMember(dest => dest.Ck5FileUploadModelList, opt => opt.MapFrom(src => Mapper.Map<List<CK5_FILE_UPLOADDto>>(src.Ck5FileUploadDtos)))
            .ForMember(dest => dest.Ck5RefId, opt => opt.MapFrom(src => src.CK5_REF_ID))
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.STATUS_ID == Enums.DocumentStatus.Completed ? true:false));

            Mapper.CreateMap<CK5_FILE_UPLOADDto, CK5FileUploadViewModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5UploadViewModel, CK5MaterialInput>().IgnoreAllNonExisting();



            Mapper.CreateMap<CK5UploadViewModel, CK5MaterialDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PLANT_ID, opt => opt.MapFrom(src => src.Plant))
                .ForMember(dest => dest.QTY, opt => opt.ResolveUsing<StringToDecimalResolver>().FromMember(src => src.Qty))
                .ForMember(dest => dest.CONVERTED_QTY, opt => opt.ResolveUsing<StringToDecimalResolver>().FromMember(src => src.ConvertedQty))
                .ForMember(dest => dest.EXCISE_VALUE, opt => opt.ResolveUsing<StringToDecimalResolver>().FromMember(src => src.ExciseValue))
                .ForMember(dest => dest.CONVERTION, opt => opt.ResolveUsing<StringToDecimalResolver>().FromMember(src => src.Convertion))
                .ForMember(dest => dest.USD_VALUE, opt => opt.ResolveUsing<StringToDecimalResolver>().FromMember(src => src.UsdValue))
                .ForMember(dest => dest.CONVERTED_UOM, opt => opt.MapFrom(src => src.ConvertedUom))
                .ForMember(dest => dest.MATERIAL_DESC, opt => opt.MapFrom(src => src.MaterialDesc));


            Mapper.CreateMap<CK5MaterialDto, CK5UploadViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Plant, opt => opt.MapFrom(src => src.PLANT_ID))
               .ForMember(dest => dest.Qty, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.QTY))
               .ForMember(dest => dest.ConvertedQty, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.CONVERTED_QTY))
               .ForMember(dest => dest.Convertion, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.CONVERTION))
               .ForMember(dest => dest.ExciseValue, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.EXCISE_VALUE))
               .ForMember(dest => dest.UsdValue, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.USD_VALUE))
               .ForMember(dest => dest.ConvertedUom, opt => opt.MapFrom(src => src.CONVERTED_UOM))
               .ForMember(dest => dest.MaterialDesc, opt => opt.MapFrom(src => src.MATERIAL_DESC));
               //.ForMember(dest => dest.ExGoodsType, opt => opt.MapFrom(src => src.EX_GOOD_TYPE_GROUP));




            Mapper.CreateMap<CK5Dto, CK5SummaryReportsItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Ck5Id, opt => opt.MapFrom(src => src.CK5_ID))
                //added
                .ForMember(dest => dest.Ck5TypeDescription, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.CK5_TYPE)))
                .ForMember(dest => dest.KppbcCityName, opt => opt.MapFrom(src => src.KPPBC_CITY))
                //.ForMember(dest => dest.SourcePlant, opt => opt.MapFrom(src => src.SOURCE_PLANT_ID))
                .ForMember(dest => dest.SubmissionNumber, opt => opt.MapFrom(src => src.SUBMISSION_NUMBER))
                .ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(src => src.SUBMISSION_DATE.HasValue ? src.SUBMISSION_DATE.Value.ToString("dd MMM yyyy") : string.Empty))
                .ForMember(dest => dest.ExGoodTypeDesc, opt => opt.MapFrom(src => src.EX_GOODS_TYPE_DESC))
                .ForMember(dest => dest.ExciseSettlement, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.EX_SETTLEMENT_ID)))
                .ForMember(dest => dest.ExciseStatus, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.EX_STATUS_ID)))
                .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.REQUEST_TYPE_ID)))
                .ForMember(dest => dest.SourcePlant, opt => opt.MapFrom(src => src.SOURCE_PLANT_ID))
                .ForMember(dest => dest.DestinationPlant, opt => opt.MapFrom(src => src.DEST_PLANT_ID))

                .ForMember(dest => dest.UnpaidExciseFacilityNumber, opt => opt.MapFrom(src => src.PbckNumber))
                .ForMember(dest => dest.UnpaidExciseFacilityDate, opt => opt.MapFrom(src => src.PbckDecreeDate.HasValue ? src.PbckDecreeDate.Value.ToString("dd MMM yyyy") : string.Empty))

                .ForMember(dest => dest.SealingNotificationDate, opt => opt.MapFrom(src => src.SEALING_NOTIF_DATE.HasValue ? src.SEALING_NOTIF_DATE.Value.ToString("dd MMM yyyy") : string.Empty))
                .ForMember(dest => dest.SealingNotificationNumber, opt => opt.MapFrom(src => src.SEALING_NOTIF_NUMBER))
                .ForMember(dest => dest.UnSealingNotificationDate, opt => opt.MapFrom(src => src.UNSEALING_NOTIF_DATE.HasValue ? src.UNSEALING_NOTIF_DATE.Value.ToString("dd MMM yyyy") : string.Empty))
                .ForMember(dest => dest.UnSealingNotificationNumber, opt => opt.MapFrom(src => src.UNSEALING_NOTIF_NUMBER))
                //.ForMember(dest => dest.Lack1, opt => opt.MapFrom(src => src.PbckNumber))
                //.ForMember(dest => dest.Lack2, opt => opt.MapFrom(src => src.PbckNumber))
                .ForMember(dest => dest.TanggalAju, opt => opt.MapFrom(src => src.SUBMISSION_DATE.HasValue ? src.SUBMISSION_DATE.Value.ToString("dd MMM yyyy") : string.Empty))
                .ForMember(dest => dest.NomerAju, opt => opt.MapFrom(src => src.SUBMISSION_NUMBER))
                .ForMember(dest => dest.TanggalPendaftaran, opt => opt.MapFrom(src => src.REGISTRATION_DATE.HasValue ? src.REGISTRATION_DATE.Value.ToString("dd MMM yyyy") : string.Empty))
                .ForMember(dest => dest.NomerPendaftaran, opt => opt.MapFrom(src => src.REGISTRATION_NUMBER))
                .ForMember(dest => dest.OriginCeOffice, opt => opt.MapFrom(src => src.KPPBC_CITY))
                .ForMember(dest => dest.OriginCompany, opt => opt.MapFrom(src => src.SOURCE_PLANT_COMPANY_NAME))
                .ForMember(dest => dest.OriginCompanyNppbkc, opt => opt.MapFrom(src => src.SOURCE_PLANT_NPPBKC_ID))
                .ForMember(dest => dest.OriginCompanyAddress, opt => opt.MapFrom(src => src.SOURCE_PLANT_ADDRESS))
                .ForMember(dest => dest.DestinationCountry, opt => opt.MapFrom(src => src.DEST_COUNTRY_NAME))
                .ForMember(dest => dest.NumberBox, opt => opt.MapFrom(src => src.GRAND_TOTAL_EX.ToString()))
                //.ForMember(dest => dest.ContainPerBox, opt => opt.MapFrom(src => src.PbckNumber))
                //.ForMember(dest => dest.TotalOfExcisableGoods, opt => opt.MapFrom(src => src.PbckNumber))
                //.ForMember(dest => dest.BanderolPrice, opt => opt.MapFrom(src => src.))
                //.ForMember(dest => dest.ExciseTariff, opt => opt.MapFrom(src => src.PbckNumber))
                //.ForMember(dest => dest.ExciseValue, opt => opt.MapFrom(src => src.PbckNumber))
                .ForMember(dest => dest.DestinationCeOffice, opt => opt.MapFrom(src => src.DEST_PLANT_KPPBC_NAME_OFFICE))
                .ForMember(dest => dest.DestCompanyAddress, opt => opt.MapFrom(src => src.DEST_PLANT_ADDRESS))
                .ForMember(dest => dest.DestCompanyNppbkc, opt => opt.MapFrom(src => src.DEST_PLANT_NPPBKC_ID))
                .ForMember(dest => dest.DestCompanyName, opt => opt.MapFrom(src => src.DEST_PLANT_NAME))
                .ForMember(dest => dest.LoadingPort, opt => opt.MapFrom(src => src.LOADING_PORT))
                .ForMember(dest => dest.LoadingPortName, opt => opt.MapFrom(src => src.LOADING_PORT_NAME))
                
                ;


            Mapper.CreateMap<CK5SearchSummaryReportsViewModel, CK5SummaryReportsItem>().IgnoreAllNonExisting();
                //.ForMember(dest => dest.NPPBKCOrigin, opt => opt.MapFrom(src => src.NppbkcId));

            Mapper.CreateMap<CK5FileUploadViewModel, CK5_FILE_UPLOADDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5SearchSummaryReportsViewModel, CK5GetSummaryReportByParamInput>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5FileDocumentItems, CK5UploadFileDocumentsInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PackageUomName, opt => opt.MapFrom(src => src.Uom))
               // .ForMember(dest => dest.InvoiceDate, opt => opt.MapFrom(src => src.InvoiceDateDisplay))
                ;

            Mapper.CreateMap<CK5FileUploadDocumentsOutput, CK5FileDocumentItems>().IgnoreAllNonExisting()
               .ForMember(dest => dest.Ck5Type, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.CK5_TYPE)))
               .ForMember(dest => dest.ExGoodType, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.EX_GOODS_TYPE)))
               .ForMember(dest => dest.ExciseSettlement, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.EX_SETTLEMENT_ID)))
               .ForMember(dest => dest.ExciseStatus, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.EX_STATUS_ID)))
               .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.REQUEST_TYPE_ID)))
               .ForMember(dest => dest.Uom, opt => opt.MapFrom(src => src.PackageUomName))
               .ForMember(dest => dest.InvoiceDateDisplay, opt => opt.MapFrom(src => src.INVOICE_DATE != null ? src.INVOICE_DATE.Value.ToString("dd MMMM yyyy") : string.Empty))
               //.ForMember(dest => dest.InvoiceDate, opt => opt.MapFrom(src => src.InvoiceDate))
               .ForMember(dest => dest.MesssageUploadFileDocuments, opt => opt.MapFrom(src => src.Message))
               ;

            Mapper.CreateMap<CK5FileDocumentItems, CK5Dto>().IgnoreAllNonExisting()
              .ForMember(dest => dest.CK5_TYPE, opt => opt.MapFrom(src => src.CK5_TYPE))
            
             .ForMember(dest => dest.KPPBC_CITY, opt => opt.MapFrom(src => src.KppBcCityName))
             .ForMember(dest => dest.EX_GOODS_TYPE_DESC, opt => opt.MapFrom(src => src.ExGoodType))
            .ForMember(dest => dest.INVOICE_NUMBER, opt => opt.MapFrom(src => src.InvoiceNumber))
            //.ForMember(dest => dest.INVOICE_DATE, opt => opt.MapFrom(src => src.InvoiceDate))
             .ForMember(dest => dest.PbckNumber, opt => opt.MapFrom(src => src.PbckDecreeNumber))
             // .ForMember(dest => dest.GRAND_TOTAL_EX, opt => opt.MapFrom(src => src.GrandTotalEx))
             //.ForMember(dest => dest.PACKAGE_UOM_ID, opt => opt.MapFrom(src => src.PackageUomName)

             // .ForMember(dest => dest.INVOICE_NUMBER, opt => opt.MapFrom(src => src.InvoiceNumber))
             //// .ForMember(dest => dest.INVOICE_DATE, opt => opt.MapFrom(src => src.InvoiceDate))
             // .ForMember(dest => dest.PBCK1_DECREE_ID, opt => opt.MapFrom(src => src.PbckDecreeId))
             //  .ForMember(dest => dest.PbckNumber, opt => opt.MapFrom(src => src.PbckDecreeNumber))
             // .ForMember(dest => dest.CARRIAGE_METHOD_ID, opt => opt.MapFrom(src => src.CarriageMethod))
             // .ForMember(dest => dest.GRAND_TOTAL_EX, opt => opt.MapFrom(src => src.GrandTotalEx))
             //.ForMember(dest => dest.PACKAGE_UOM_ID, opt => opt.MapFrom(src => src.PackageUomName))
             //.ForMember(dest => dest.DEST_COUNTRY_CODE, opt => opt.MapFrom(src => src.CountryCode))
             //.ForMember(dest => dest.DEST_COUNTRY_NAME, opt => opt.MapFrom(src => src.CountryName))
             ;

            Mapper.CreateMap<CK5FileDocumentItems, CK5FileDocumentDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<Pbck1Dto, Ck5ListPbck1Completed>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PbckId, opt => opt.MapFrom(src => src.Pbck1Id))
                .ForMember(dest => dest.PbckNumber, opt => opt.MapFrom(src => src.Pbck1Number))
                ;

            Mapper.CreateMap<MaterialDto, CK5InputManualViewModel>().IgnoreAllNonExisting()
                  .ForMember(dest => dest.MaterialNumber, opt => opt.MapFrom(src => src.STICKER_CODE))
                  .ForMember(dest => dest.MaterialDesc, opt => opt.MapFrom(src => src.GoodTypeDescription))
                  .ForMember(dest => dest.Hje, opt => opt.MapFrom(src => src.HJE))
                  .ForMember(dest => dest.Tariff, opt => opt.MapFrom(src => src.TARIFF))
                ;

            Mapper.CreateMap<CK5ExternalSupplierDto, CK5ExternalSupplierModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.SupplierPlant,opt => opt.MapFrom(src => src.SUPPLIER_PLANT))
                .ForMember(dest => dest.SupplierCompany, opt => opt.MapFrom(src => src.SUPPLIER_COMPANY))
                .ForMember(dest => dest.SupplierAddress, opt => opt.MapFrom(src => src.SUPPLIER_ADDRESS))
                .ForMember(dest => dest.SupplierNppbkcId, opt => opt.MapFrom(src => src.SUPPLIER_NPPBKC_ID))
                .ForMember(dest => dest.SupplierKppbcId, opt => opt.MapFrom(src => src.SUPPLIER_KPPBC_ID))
                .ForMember(dest => dest.SupplierKppbcName, opt => opt.MapFrom(src => src.SUPPLIER_KPPBC_NAME))
                .ForMember(dest => dest.SupplierPhone, opt => opt.MapFrom(src => src.SUPPLIER_PHONE))
                .ForMember(dest => dest.SupplierPortId, opt => opt.MapFrom(src => src.SUPPLIER_PORT_ID))
                .ForMember(dest => dest.SupplierPortName, opt => opt.MapFrom(src => src.SUPPLIER_PORT_NAME));

            Mapper.CreateMap<CK5ExternalSupplierDto, CK5PlantModel>().IgnoreAllNonExisting()
                //.ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.SUPPLIER_PLANT))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.SUPPLIER_PLANT))
                //.ForMember(dest => dest.PlantNpwp, opt => opt.MapFrom(src => src.supp))
                .ForMember(dest => dest.NPPBCK_ID, opt => opt.MapFrom(src => src.SUPPLIER_NPPBKC_ID))
                //.ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.s))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.SUPPLIER_COMPANY))
                .ForMember(dest => dest.CompanyAddress, opt => opt.MapFrom(src => src.SUPPLIER_ADDRESS))
                .ForMember(dest => dest.KppBcName,
                    opt => opt.MapFrom(src => src.SUPPLIER_KPPBC_NAME));
                //.ForMember(dest => dest.KppbcCity, opt => opt.MapFrom(src => src.supp))
                //.ForMember(dest => dest.KppbcNo, opt => opt.MapFrom(src => src.SUPPLIER_KPPBC_ID));
                ;

                Mapper.CreateMap<Ck5SummaryReportDto, CK5SummaryReportsItem>().IgnoreAllNonExisting();
        }
    }
}