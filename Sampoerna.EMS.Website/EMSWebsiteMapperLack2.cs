using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Website.Models.LACK2;

namespace Sampoerna.EMS.Website
{
    public partial class EMSWebsiteMapper
    {
        public static void InitializeLACK2()
        {
            Mapper.CreateMap<Lack2SummaryReportDto, Lack2SummaryReportsItem>().IgnoreAllNonExisting()
                ;

            Mapper.CreateMap<Lack2SearchSummaryReportsViewModel, Lack2GetSummaryReportByParamInput>().IgnoreAllNonExisting()
                ;

            Mapper.CreateMap<Lack2DetailReportDto, Lack2DetailReportsItem>().IgnoreAllNonExisting()
                ;

            Mapper.CreateMap<Lack2SearchDetailReportsViewModel, Lack2GetDetailReportByParamInput>().IgnoreAllNonExisting()
                ;
        }
    }
}