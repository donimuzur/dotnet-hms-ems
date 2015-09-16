using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.BLL
{
    public partial class BLLMapper
    {
        public static void InitializePbck7And3()
        {
            #region PBCK7

            Mapper.CreateMap<PBCK7, Pbck7AndPbck3Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Pbck7Id, opt => opt.MapFrom(src => src.PBCK7_ID))
                .ForMember(dest => dest.Pbck7Number, opt => opt.MapFrom(src => src.PBCK7_NUMBER))
                .ForMember(dest => dest.Pbck7Date, opt => opt.MapFrom(src => src.PBCK7_DATE))
                .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.DOCUMENT_TYPE))
                .ForMember(dest => dest.Pbck7Date, opt => opt.MapFrom(src => src.PBCK7_DATE))
                .ForMember(dest => dest.NppbkcId, opt => opt.MapFrom(src => src.NPPBKC))
                .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PLANT_ID))
                .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.PLANT_NAME))
                .ForMember(dest => dest.PlantCity, opt => opt.MapFrom(src => src.PLANT_CITY))
                .ForMember(dest => dest.ExecDateFrom, opt => opt.MapFrom(src => src.EXEC_DATE_FROM))
                .ForMember(dest => dest.ExecDateTo, opt => opt.MapFrom(src => src.EXEC_DATE_TO))
                .ForMember(dest => dest.GovStatus, opt => opt.MapFrom(src => src.GOV_STATUS))
                .ForMember(dest => dest.Pbck7Status, opt => opt.MapFrom(src => src.STATUS))
                .ForMember(dest => dest.ApprovedBy, opt => opt.MapFrom(src => src.APPROVED_BY))
                .ForMember(dest => dest.ApprovedDate, opt => opt.MapFrom(src => src.APPROVED_DATE))
                .ForMember(dest => dest.ApprovedByManager, opt => opt.MapFrom(src => src.APPROVED_BY_MANAGER))
                .ForMember(dest => dest.ApprovedDateManager, opt => opt.MapFrom(src => src.APPROVED_BY_MANAGER_DATE))
                .ForMember(dest => dest.RejectedBy, opt => opt.MapFrom(src => src.REJECTED_BY))
                .ForMember(dest => dest.RejectedDate, opt => opt.MapFrom(src => src.REJECTED_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                 .ForMember(dest => dest.UploadItems, opt => opt.MapFrom(src => src.PBCK7_ITEM))
                ;


            Mapper.CreateMap<Pbck7ItemUpload, PBCK7_ITEM>().IgnoreAllNonExisting()
             .ForMember(dest => dest.BRAND_CE, opt => opt.MapFrom(src => src.Brand))
             .ForMember(dest => dest.FA_CODE, opt => opt.MapFrom(src => src.FaCode))
             .ForMember(dest => dest.PRODUCT_ALIAS, opt => opt.MapFrom(src => src.ProdTypeAlias))
             .ForMember(dest => dest.BRAND_CONTENT, opt => opt.MapFrom(src => src.Content))
             .ForMember(dest => dest.FISCAL_YEAR, opt => opt.MapFrom(src => src.FiscalYear))
             .ForMember(dest => dest.SERIES_VALUE, opt => opt.MapFrom(src => src.SeriesValue))
             .ForMember(dest => dest.EXCISE_VALUE, opt => opt.MapFrom(src => src.ExciseValue))
             .ForMember(dest => dest.HJE, opt => opt.MapFrom(src => src.Hje))
             .ForMember(dest => dest.TARIFF, opt => opt.MapFrom(src => src.Tariff))
             .ForMember(dest => dest.EXCISE_VALUE, opt => opt.MapFrom(src => src.ExciseValue))
             .ForMember(dest => dest.PBCK7_QTY, opt => opt.MapFrom(src => src.Pbck7Qty))
             .ForMember(dest => dest.BACK1_QTY, opt => opt.MapFrom(src => src.Back1Qty))
                ;
            Mapper.CreateMap<PBCK7_ITEM, Pbck7ItemUpload>().IgnoreAllNonExisting()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PBCK7_ITEM_ID))
           .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.BRAND_CE))
           .ForMember(dest => dest.FaCode, opt => opt.MapFrom(src => src.FA_CODE))
           .ForMember(dest => dest.ProdTypeAlias, opt => opt.MapFrom(src => src.PRODUCT_ALIAS))
           .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.BRAND_CONTENT))
           .ForMember(dest => dest.FiscalYear, opt => opt.MapFrom(src => src.FISCAL_YEAR))
           .ForMember(dest => dest.Hje, opt => opt.MapFrom(src => src.HJE))
           .ForMember(dest => dest.SeriesValue, opt => opt.MapFrom(src => src.SERIES_VALUE))
           .ForMember(dest => dest.ExciseValue, opt => opt.MapFrom(src => src.EXCISE_VALUE))
           .ForMember(dest => dest.Tariff, opt => opt.MapFrom(src => src.TARIFF))
           .ForMember(dest => dest.SeriesValue, opt => opt.MapFrom(src => src.SERIES_VALUE))
           .ForMember(dest => dest.Pbck7Qty, opt => opt.MapFrom(src => src.PBCK7_QTY))
           .ForMember(dest => dest.Back1Qty, opt => opt.MapFrom(src => src.BACK1_QTY))
              ;
            Mapper.CreateMap<Pbck7AndPbck3Dto, PBCK7>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.PBCK7_ID, opt => opt.MapFrom(src => src.Pbck7Id))
                .ForMember(dest => dest.EXEC_DATE_FROM, opt => opt.MapFrom(src => src.ExecDateFrom))
                .ForMember(dest => dest.EXEC_DATE_TO, opt => opt.MapFrom(src => src.ExecDateTo))
                .ForMember(dest => dest.PLANT_ID, opt => opt.MapFrom(src => src.PlantId))
                .ForMember(dest => dest.PLANT_CITY, opt => opt.MapFrom(src => src.PlantCity))
                .ForMember(dest => dest.PLANT_NAME, opt => opt.MapFrom(src => src.PlantName))
                .ForMember(dest => dest.DOCUMENT_TYPE, opt => opt.MapFrom(src => src.DocumentType))
                .ForMember(dest => dest.NPPBKC, opt => opt.MapFrom(src => src.NppbkcId))
                .ForMember(dest => dest.STATUS, opt => opt.MapFrom(src => src.Pbck7Status))
                .ForMember(dest => dest.GOV_STATUS, opt => opt.MapFrom(src => src.GovStatus))
                .ForMember(dest => dest.PBCK7_ITEM, opt => opt.MapFrom(src => src.UploadItems))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.APPROVED_BY, opt => opt.MapFrom(src => src.ApprovedBy))
                .ForMember(dest => dest.APPROVED_DATE, opt => opt.MapFrom(src => src.ApprovedDate))
                .ForMember(dest => dest.APPROVED_BY_MANAGER, opt => opt.MapFrom(src => src.ApprovedByManager))
                .ForMember(dest => dest.APPROVED_BY_MANAGER_DATE, opt => opt.MapFrom(src => src.ApprovedDateManager))
                .ForMember(dest => dest.REJECTED_BY, opt => opt.MapFrom(src => src.RejectedBy))
                 .ForMember(dest => dest.REJECTED_DATE, opt => opt.MapFrom(src => src.RejectedDate))
               
                ;

         

            #endregion

        }
    }
}

