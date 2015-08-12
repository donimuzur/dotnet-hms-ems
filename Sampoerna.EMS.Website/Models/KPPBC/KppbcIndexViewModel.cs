using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Website.Models.KPPBC
{
    public class KppbcIndexViewModel : BaseModel
    {

        public List<ZAIDM_EX_KPPBCDto> Kppbcs { get; set; }

        
    }
}