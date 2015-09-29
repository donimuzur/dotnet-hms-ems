using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Website.Models.POAMap
{
    public class PoaMapDetailViewModel : BaseModel
    {

        public POA_MAPDto PoaMap { get; set; }

        public SelectList NppbckIds { get; set; }

        public SelectList Plants { get; set; }

        public SelectList POAs { get; set; } 
    }
}