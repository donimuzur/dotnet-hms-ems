using Sampoerna.EMS.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.LACK2
{
    public class LACK2CreateViewModel : BaseModel
    {
        public LACK2CreateViewModel()
        {
            this.Lack2Model = new LACK2Model();
        }
        public SelectList CompanyCodesDDL { get; set; }
       
        public SelectList NPPBKCDDL { get; set; }
        
        public SelectList SendingPlantDDL { get; set; }
        
        public SelectList ExcisableGoodsTypeDDL { get; set; }

        public SelectList GovStatusDDL { get; set; }

        public SelectList MonthList { get; set; }

        public SelectList YearList { get; set; }

        //public SelectList StatusDDL { get; set; }

        public Enums.UserRole UsrRole { get; set; }

        public LACK2Model Lack2Model { get; set; }
        
    }
}