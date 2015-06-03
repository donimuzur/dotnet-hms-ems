using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Website.Models;
using Sampoerna.EMS.Core;
namespace Sampoerna.EMS.Website
{
    public class SampoernaEMSMapper
    {
        public static void Initialize()
        {
            //AutoMapper
            Mapper.CreateMap<T1001, CompanyViewModel>().IgnoreAllNonExisting();

          
        }
    }
}