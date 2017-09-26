using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Sampoerna.EMS.Website.Models.Shared;
using System.Web.Mvc;
using Sampoerna.EMS.CustomService.Core;

namespace Sampoerna.EMS.Website.Models.ExciseCredit
{
    public class ExciseCreditPrintoutModel
    {
        public long ExciseId { set; get; }
        public ReferenceKeys.PrintoutLayout PrintoutType { set; get; }
        public ReferenceKeys.PrintoutLayout PrintoutTypeName { set; get; }
        public String PrintoutName { set; get; }
        public PrintoutLayout Layout { set; get; }
        
        public bool IsAllowedToEdit { set; get; }
        public bool IsDrafted { set; get; }
        public Shared.ConfirmDialogModel Confirmation { set; get; }
    }
}