using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Website.Models.POAMap
{
    public class PoaMapIndexViewModel : BaseModel
    {

        public List<POA_MAPDto> PoaMaps { get; set; }

        
    }
}