using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using AutoMapper;
using Org.BouncyCastle.Crypto.Agreement.Srp;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Utils;
using Sampoerna.EMS.Website.Models.PBCK7AndPBCK3;


namespace Sampoerna.EMS.Website
{
    public partial class EMSWebsiteMapper
    {
        public static void InitializePbck7AndPbck3()
        {
            #region PBCK7

            Mapper.CreateMap<Pbck7AndPbck3Dto, DataListIndexPbck7>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Pbck7Id))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.Pbck7Date.ToString("dd MMM yyyy")))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId + "-" + src.PlantName))
                .ForMember(dest => dest.Poa, opt => opt.MapFrom(src => src.ApprovedBy))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Pbck7Status)))
                .ForMember(dest => dest.Pbck7Number, opt => opt.MapFrom(src => src.Pbck7Number));

            Mapper.CreateMap<Pbck7IndexViewModel, Pbck7AndPbck3Input>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.Pbck7Date, opt => opt.MapFrom(src => src.ReportedOn))
                .ForMember(dest => dest.Pbck7AndPvck3Type, opt => opt.MapFrom(src => src.Pbck7Type))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.Poa, opt => opt.MapFrom(src => src.Poa))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator));

            #endregion

            #region PBCK3

            Mapper.CreateMap<Pbck3Dto, DataListIndexPbck3>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.Pbck7Id, opt => opt.MapFrom(src => src.Pbck7Id))
                 .ForMember(dest => dest.Pbck3Id, opt => opt.MapFrom(src => src.Pbck3Id))
                 .ForMember(dest => dest.Pbck3Number, opt => opt.MapFrom(src => src.Pbck3Number))
                 .ForMember(dest => dest.Pbck7Number, opt => opt.MapFrom(src => src.Pbck7Number))
                 .ForMember(dest => dest.Ck5Number, opt => opt.MapFrom(src => src.Ck5Number))
               
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbckId))
                .ForMember(dest => dest.ReportedOn, opt => opt.MapFrom(src => src.Pbck3Date.HasValue ? src.Pbck3Date.Value.ToString("dd MMM yyyy") : string.Empty))
                .ForMember(dest => dest.Plant, opt => opt.MapFrom(src => src.Plant))
                .ForMember(dest => dest.Poa, opt => opt.MapFrom(src => src.ApprovedBy))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Pbck3Status)));
                

            Mapper.CreateMap<Pbck3IndexViewModel, Pbck7AndPbck3Input>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.Pbck3Date, opt => opt.MapFrom(src => src.ReportedOn))
                .ForMember(dest => dest.Pbck7AndPvck3Type, opt => opt.MapFrom(src => src.Pbck3Type))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.Poa, opt => opt.MapFrom(src => src.Poa))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator));


            Mapper.CreateMap<Pbck7SummaryReportModel, Pbck7SummaryInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.SelectedNppbkc))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.SelectedPlant))
                .ForMember(dest => dest.Pbck7Number, opt => opt.MapFrom(src => src.SelectedNumber))
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To));

            Mapper.CreateMap<Pbck7ExportModel, Pbck7SummaryInput>().IgnoreAllNonExisting()
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.Plant))
                .ForMember(dest => dest.Pbck7Number, opt => opt.MapFrom(src => src.Pbck7No))
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.FromYear))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.ToYear))
                ;
            Mapper.CreateMap<Pbck3SummaryReportModel, Pbck3SummaryInput>().IgnoreAllNonExisting()
            .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.SelectedNppbkc))
            .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.SelectedPlant))
            .ForMember(dest => dest.Pbck3Number, opt => opt.MapFrom(src => src.SelectedNumber))
            .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
            .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
            ;

            Mapper.CreateMap<Pbck3ExportModel, Pbck3SummaryInput>().IgnoreAllNonExisting()
          .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NppbkcId))
          .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.Plant))
          .ForMember(dest => dest.Pbck3Number, opt => opt.MapFrom(src => src.Pbck7No))
          .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.FromYear))
          .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.ToYear))
          ;

            #endregion

            #region PBCK7adPBCK3 Create

            Mapper.CreateMap<Pbck7AndPbck3Dto, Pbck7Pbck3CreateViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Pbck7Id))
                .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.DocumentType))
                .ForMember(dest => dest.DocumentTypeDescription, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.DocumentType)))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.Pbck7StatusName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Pbck7Status)))
                .ForMember(dest => dest.Pbck3StatusName, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Pbck3Dto.Pbck3Status)))
                 .ForMember(dest => dest.Pbck7GovStatus, opt => opt.MapFrom(src => src.Pbck7GovStatus))
                 .ForMember(dest => dest.Pbck7GovStatusDesc, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.Pbck7GovStatus)))
                 .ForMember(dest => dest.UploadItems, opt => opt.MapFrom(src => Mapper.Map<List<Pbck7UploadViewModel>>(src.UploadItems)))
                  
                  ;

            Mapper.CreateMap<Pbck7Pbck3CreateViewModel, Pbck7AndPbck3Dto>().IgnoreAllNonExisting()
                   .ForMember(dest => dest.Pbck7Id, opt => opt.MapFrom(src => src.Id))
                   .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreatedDate))
                  .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
               
                ;




            #endregion

            Mapper.CreateMap<Pbck7UploadViewModel, Pbck7ItemsInput>().IgnoreAllNonExisting()
                  .ForMember(dest => dest.Plant, opt => opt.MapFrom(src => src.PlantId))
                  .ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FaCode))
                 .ForMember(dest => dest.Pbck7Qty, opt => opt.MapFrom(src => src.Pbck7Qty))
                 .ForMember(dest => dest.FiscalYear, opt => opt.MapFrom(src => src.FiscalYear))
               ;

            Mapper.CreateMap<Pbck7UploadViewModel, Pbck7ItemUpload>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.Content, opt => opt.MapFrom(src => ConvertHelper.ConvertToDecimalOrZero(src.Content)))
                .ForMember(dest => dest.Pbck7Qty, opt => opt.MapFrom(src => ConvertHelper.ConvertToDecimalOrZero(src.Pbck7Qty)))
                .ForMember(dest => dest.Back1Qty, opt => opt.MapFrom(src => ConvertHelper.ConvertToDecimalOrZero(src.Back1Qty)))
                .ForMember(dest => dest.FiscalYear, opt => opt.MapFrom(src => ConvertHelper.ConvertToInt32OrNull(src.FiscalYear)))
                .ForMember(dest => dest.Hje, opt => opt.MapFrom(src => ConvertHelper.ConvertToDecimalOrZero(src.Hje)))
                .ForMember(dest => dest.Tariff, opt => opt.MapFrom(src => ConvertHelper.ConvertToDecimalOrZero(src.Tariff)))
                .ForMember(dest => dest.ExciseValue, opt => opt.MapFrom(src => ConvertHelper.ConvertToDecimalOrZero(src.ExciseValue)))
                .ForMember(dest => dest.Pbck7Id, opt => opt.MapFrom(src => ConvertHelper.ConvertToInt32OrNull(src.Pbck7Id)))
              ;


            Mapper.CreateMap<Pbck7ItemUpload, Pbck7UploadViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Content, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.Content))
                .ForMember(dest => dest.Pbck7Qty, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.Pbck7Qty))
                .ForMember(dest => dest.Back1Qty, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.Back1Qty))
                .ForMember(dest => dest.FiscalYear, opt => opt.MapFrom(src => src.FiscalYear.HasValue ? src.FiscalYear.Value.ToString() : "0"))
                .ForMember(dest => dest.Hje, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.Hje))
                .ForMember(dest => dest.Tariff, opt => opt.ResolveUsing<DecimalToStringResolver>().FromMember(src => src.Tariff))
                .ForMember(dest => dest.Pbck7Id, opt => opt.MapFrom(src => src.Pbck7Id.HasValue ? src.Pbck7Id.Value.ToString() : "0"))
               ;

            Mapper.CreateMap<Pbck3CompositeDto, Pbck3ViewModel>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.Pbck3Id, opt => opt.MapFrom(src => src.PBCK3_ID))
                 .ForMember(dest => dest.Pbck3Number, opt => opt.MapFrom(src => src.PBCK3_NUMBER))
                 .ForMember(dest => dest.PBCK3_DATE, opt => opt.MapFrom(src => src.PBCK3_DATE))
                 .ForMember(dest => dest.Pbck3Status, opt => opt.MapFrom(src => src.STATUS))
                 .ForMember(dest => dest.Pbck3GovStatus, opt => opt.MapFrom(src => src.GOV_STATUS))
                 .ForMember(dest => dest.Pbck3StatusDescription, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.STATUS)))
                 .ForMember(dest => dest.Pbck3GovStatusDescription, opt => opt.MapFrom(src => EnumHelper.GetDescription(src.GOV_STATUS)))
                 .ForMember(dest => dest.FromPbck7, opt => opt.MapFrom(src => src.FromPbck7))
                 .ForMember(dest => dest.Pbck7Id, opt => opt.MapFrom(src => src.Pbck7Composite.Pbck7Id))
                 .ForMember(dest => dest.Pbck7Number, opt => opt.MapFrom(src => src.Pbck7Composite.Pbck7Number))
                 .ForMember(dest => dest.Pbck7Date, opt => opt.MapFrom(src => src.Pbck7Composite.Pbck7Date))
                 .ForMember(dest => dest.Pbck7Status, opt => opt.MapFrom(src => src.Pbck7Composite.Pbck7Status))
                 .ForMember(dest => dest.Pbck7StatusDescription, opt => opt.MapFrom(src => src.Pbck7Composite.Pbck7StatusDescription))
                 .ForMember(dest => dest.Pbck7GovStatus, opt => opt.MapFrom(src => src.Pbck7Composite.Pbck7GovStatus))
                 .ForMember(dest => dest.Pbck7GovStatusDescription, opt => opt.MapFrom(src => src.Pbck7Composite.Pbck7GovStatusDescription))
                 .ForMember(dest => dest.Pbck7UploadItems, opt => opt.MapFrom(src => Mapper.Map<List<Pbck7UploadViewModel>>(src.Pbck7Composite.Pbck7Documents)))

                 .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.Pbck7Composite.DocumentType))
                 .ForMember(dest => dest.DocumentTypeDescription, opt => opt.MapFrom(src => src.Pbck7Composite.DocumentTypeDescription))
                 .ForMember(dest => dest.ExecDateFrom, opt => opt.MapFrom(src => src.Pbck7Composite.ExecDateFrom))
                 .ForMember(dest => dest.ExecDateTo, opt => opt.MapFrom(src => src.Pbck7Composite.ExecDateTo))
                 .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.Pbck7Composite.NppbkcId))
                 .ForMember(dest => dest.Lampiran, opt => opt.MapFrom(src => src.Pbck7Composite.Lampiran))
                 .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.Pbck7Composite.PlantId))

                 .ForMember(dest => dest.Back1Id, opt => opt.MapFrom(src => src.Back1Id))
                 .ForMember(dest => dest.Back1Number, opt => opt.MapFrom(src => src.Back1Number))
                 .ForMember(dest => dest.Back1Date, opt => opt.MapFrom(src => src.Back1Date))

                 .ForMember(dest => dest.Ck2Id, opt => opt.MapFrom(src => src.Ck2Id))
                 .ForMember(dest => dest.Ck2Number, opt => opt.MapFrom(src => src.Ck2Number))
                 .ForMember(dest => dest.Ck2Date, opt => opt.MapFrom(src => src.Ck2Date))
                 .ForMember(dest => dest.Ck2Value, opt => opt.MapFrom(src => src.Ck2Value))
                ;

            Mapper.CreateMap<PBCK7_ITEM, Pbck7UploadViewModel>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PBCK7_ITEM_ID))
                .ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FA_CODE))
                .ForMember(dest => dest.ProdTypeAlias, opt => opt.MapFrom(src => src.PRODUCT_ALIAS))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.BRAND_CE))
                .ForMember(dest => dest.SeriesValue, opt => opt.MapFrom(src => src.SERIES_VALUE))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.BRAND_CONTENT.HasValue? src.BRAND_CONTENT.Value:0))
                .ForMember(dest => dest.Pbck7Qty, opt => opt.MapFrom(src => src.PBCK7_QTY.HasValue ? src.PBCK7_QTY.Value : 0))
                .ForMember(dest => dest.Back1Qty, opt => opt.MapFrom(src => src.BACK1_QTY.HasValue ? src.BACK1_QTY.Value : 0))
                .ForMember(dest => dest.FiscalYear, opt => opt.MapFrom(src => src.FISCAL_YEAR.HasValue ? src.FISCAL_YEAR.Value : 0))
                .ForMember(dest => dest.Hje, opt => opt.MapFrom(src => src.HJE.HasValue ? src.HJE.Value : 0))
                .ForMember(dest => dest.Tariff, opt => opt.MapFrom(src => src.TARIFF.HasValue ? src.TARIFF.Value : 0))
                .ForMember(dest => dest.ExciseValue, opt => opt.MapFrom(src => src.EXCISE_VALUE.HasValue ? src.EXCISE_VALUE.Value : 0))
                
                ;

            Mapper.CreateMap<Pbck3ViewModel, Pbck3Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck3Id, opt => opt.MapFrom(src => src.Pbck3Id))
                .ForMember(dest => dest.Pbck3Date, opt => opt.MapFrom(src => src.PBCK3_DATE))
                ;

            Mapper.CreateMap<Pbck7UploadViewModel, PBCK7_ITEMDto>().IgnoreAllNonExisting()
              .ForMember(dest => dest.PBCK7_ITEM_ID, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.PBCK7_QTY, opt => opt.MapFrom(src => ConvertHelper.ConvertToDecimalOrZero(src.Pbck7Qty)))
              .ForMember(dest => dest.BACK1_QTY, opt => opt.MapFrom(src => ConvertHelper.ConvertToDecimalOrZero(src.Back1Qty)))
              ;
        }
    }
}