using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BLL
{
    public partial class BLLMapper
    {
        public static void InitializeCK5()
        {
            #region CK5

            Mapper.CreateMap<CK5MaterialInput, CK5MaterialOutput>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5, CK5Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedUser, opt => opt.MapFrom(src => src.USER1.USER_ID))
                .ForMember(dest => dest.PackageUomName, opt => opt.MapFrom(src => src.UOM.UOM_DESC))
                .ForMember(dest => dest.PbckNumber, opt => opt.MapFrom(src => src.PBCK1.NUMBER))
                .ForMember(dest => dest.PbckDecreeDate, opt => opt.MapFrom(src => src.PBCK1.DECREE_DATE))
                .ForMember(dest => dest.IsCk5Export, opt => opt.MapFrom(src => src.CK5_TYPE == Enums.CK5Type.Export))
                .ForMember(dest => dest.IsCk5PortToImporter, opt => opt.MapFrom(src => src.CK5_TYPE == Enums.CK5Type.PortToImporter))
                .ForMember(dest => dest.IsCk5Manual, opt => opt.MapFrom(src => src.CK5_TYPE == Enums.CK5Type.Manual))
                .ForMember(dest => dest.IsWaitingGovApproval, opt => opt.MapFrom(src => src.STATUS_ID == Enums.DocumentStatus.WaitingGovApproval))
                .ForMember(dest => dest.Ck5FileUploadDtos, opt => opt.MapFrom(src => Mapper.Map<List<CK5_FILE_UPLOADDto>>(src.CK5_FILE_UPLOAD)))
                .ForMember(dest => dest.GIDateStr, opt => opt.MapFrom(src => src.SUBMISSION_DATE == null ?string.Empty : Convert.ToDateTime(src.SUBMISSION_DATE).ToString("dd MMM yyy")));


            Mapper.CreateMap<CK5Dto, CK5>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5MaterialDto, CK5_MATERIAL>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5_MATERIAL, CK5MaterialDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5_FILE_UPLOAD, CK5_FILE_UPLOADDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5_FILE_UPLOADDto, CK5_FILE_UPLOAD>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5, CK5ReportDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ReportDetails,opt => opt.MapFrom(src => Mapper.Map<CK5ReportDetailsDto>(src)))
                .ForMember(dest => dest.ListMaterials, opt => opt.MapFrom(src => Mapper.Map<List<CK5ReportMaterialDto>>(src.CK5_MATERIAL)))

                ;

            Mapper.CreateMap<CK5, CK5ReportDetailsDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.OfficeName, opt => opt.MapFrom(src => src.KPPBC_CITY))
                //.ForMember(dest => dest.OfficeCode, opt => opt.MapFrom(src => src.CE_OFFICE_CODE))
                .ForMember(dest => dest.SubmissionNumber, opt => opt.MapFrom(src => src.SUBMISSION_NUMBER))
                //.ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(src => src.SUBMISSION_DATE.HasValue?src.SUBMISSION_DATE.Value.ToString("dd MMMM yyyy") : ""))
                .ForMember(dest => dest.RegistrationNumber, opt => opt.MapFrom(src => src.REGISTRATION_NUMBER))
                //.ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.REGISTRATION_DATE.HasValue ? src.REGISTRATION_DATE.Value.ToString("dd MMM yyyy") : ""))
                //.ForMember(dest => dest.ExGoodType, opt => opt.MapFrom(src => src.EX_GOODS_TYPE_DESC))//todo add 1 field to get id
                .ForMember(dest => dest.ExciseSettlement, opt => opt.MapFrom(src => (Convert.ToInt32(src.EX_SETTLEMENT_ID)).ToString()))
                .ForMember(dest => dest.ExciseStatus, opt => opt.MapFrom(src => (Convert.ToInt32(src.EX_STATUS_ID)).ToString()))
                .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => (Convert.ToInt32(src.REQUEST_TYPE_ID)).ToString()))

                .ForMember(dest => dest.SourcePlantNpwp, opt => opt.MapFrom(src => src.SOURCE_PLANT_NPWP))
                .ForMember(dest => dest.SourcePlantNppbkc, opt => opt.MapFrom(src => src.SOURCE_PLANT_NPPBKC_ID))
                .ForMember(dest => dest.SourcePlantName, opt => opt.MapFrom(src => src.SOURCE_PLANT_NAME))
                .ForMember(dest => dest.SourcePlantAddress, opt => opt.MapFrom(src => src.SOURCE_PLANT_ADDRESS))
                .ForMember(dest => dest.SourceOfficeName, opt => opt.MapFrom(src => src.SOURCE_PLANT_COMPANY_NAME))
                .ForMember(dest => dest.SourceOfficeCode, opt => opt.MapFrom(src => src.SOURCE_PLANT_COMPANY_CODE))

                //.ForMember(dest => dest.DestPlantNpwp, opt => opt.MapFrom(src => src.DEST_PLANT_NPWP))
                //.ForMember(dest => dest.DestPlantNppbkc, opt => opt.MapFrom(src => src.DEST_PLANT_NPPBKC_ID))
                //.ForMember(dest => dest.DestPlantName, opt => opt.MapFrom(src => src.DEST_PLANT_NAME))
                //.ForMember(dest => dest.DestPlantAddress, opt => opt.MapFrom(src => src.DEST_PLANT_ADDRESS))
                //.ForMember(dest => dest.DestOfficeName, opt => opt.MapFrom(src => src.DEST_PLANT_COMPANY_NAME))
                //.ForMember(dest => dest.DestOfficeCode, opt => opt.MapFrom(src => src.DEST_PLANT_COMPANY_CODE))

                .ForMember(dest => dest.FacilityNumber, opt => opt.MapFrom(src => src.PBCK1.NUMBER))
                .ForMember(dest => dest.FacilityDate, opt => opt.MapFrom(src => src.PBCK1.DECREE_DATE.HasValue ? src.PBCK1.DECREE_DATE.Value.ToString("dd MMM yyyy") : string.Empty))
                .ForMember(dest => dest.InvoiceNumber, opt => opt.MapFrom(src => src.INVOICE_NUMBER))

                .ForMember(dest => dest.CarriageMethod, opt => opt.MapFrom(src => (Convert.ToInt32(src.CARRIAGE_METHOD_ID)).ToString()))

                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.GRAND_TOTAL_EX.HasValue ? src.GRAND_TOTAL_EX.Value.ToString() : "0"))
                .ForMember(dest => dest.Uom, opt => opt.MapFrom(src => src.UOM.UOM_DESC))

                .ForMember(dest => dest.Uom, opt => opt.MapFrom(src => src.UOM.UOM_DESC))
                .ForMember(dest => dest.Uom, opt => opt.MapFrom(src => src.UOM.UOM_DESC))

                ;


            Mapper.CreateMap<CK5_MATERIAL, CK5ReportMaterialDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.QTY.HasValue ? src.QTY.Value.ToString() : "0"))
                .ForMember(dest => dest.Uom, opt => opt.MapFrom(src => src.UOM))
               
                //.ForMember(dest => dest.UomConverted, opt => opt.MapFrom(src => src.CONVERTED_UOM))
                //.ForMember(dest => dest.ExGoodTypeDesc, opt => opt.MapFrom(src => src.BRAND))
                .ForMember(dest => dest.Convertion, opt => opt.MapFrom(src => src.CONVERTION.HasValue ? src.CONVERTION.Value.ToString() : "0"))
                .ForMember(dest => dest.ConvertedQty, opt => opt.MapFrom(src => src.CONVERTED_QTY.HasValue ? src.CONVERTED_QTY.Value.ToString() : "0"))
                .ForMember(dest => dest.ConvertedUom, opt => opt.MapFrom(src => src.CONVERTED_UOM))
                .ForMember(dest => dest.Hje, opt => opt.MapFrom(src => src.HJE.HasValue ? src.HJE.Value.ToString() : "0"))
                .ForMember(dest => dest.Tariff, opt => opt.MapFrom(src => src.TARIFF.HasValue ? src.TARIFF.Value.ToString() : "0"))
                .ForMember(dest => dest.ExciseValue, opt => opt.MapFrom(src => src.EXCISE_VALUE.HasValue ? src.EXCISE_VALUE.Value.ToString() : "0"))
                .ForMember(dest => dest.UsdValue, opt => opt.MapFrom(src => src.USD_VALUE.HasValue ? src.USD_VALUE.Value.ToString() : "0"))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.NOTE))
                .ForMember(dest => dest.MaterialDescription, opt => opt.MapFrom(src => src.MATERIAL_DESC))
                ;

            #endregion

            Mapper.CreateMap<CK5UploadFileDocumentsInput, CK5FileUploadDocumentsOutput>().IgnoreAllNonExisting();

            Mapper.CreateMap<CK5FileDocumentDto, CK5Dto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.KPPBC_CITY, opt => opt.MapFrom(src => src.KppBcCityName))
                ;

            Mapper.CreateMap<CK5FileDocumentDto, CK5MaterialDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UOM, opt => opt.MapFrom(src => src.UomMaterial))
                .ForMember(dest => dest.CONVERTED_UOM, opt => opt.MapFrom(src => src.ConvertedUom))
                .ForMember(dest => dest.CONVERTED_QTY, opt => opt.MapFrom(src => Convert.ToDecimal(src.ConvertedQty)))
                .ForMember(dest => dest.EXCISE_VALUE, opt => opt.MapFrom(src => src.ExciseValue))
                .ForMember(dest => dest.BRAND, opt => opt.MapFrom(src => src.MatNumber))
                ;

            Mapper.CreateMap<CK5, CK5XmlDto>().IgnoreAllNonExisting()
               .ForMember(dest => dest.Ck5Material, opt => opt.MapFrom(src => Mapper.Map<List<CK5MaterialDto>>(src.CK5_MATERIAL)))
               ;


        }
    }
}
