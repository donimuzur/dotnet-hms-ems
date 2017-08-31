using Sampoerna.EMS.Website.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sampoerna.EMS.Website.Models.Tariff
{
    /// <summary>
    /// A class that serve as data model for views under module Tariff (~/Tariff)
    /// </summary>
    public class TariffViewModel : MasterViewModel
    {
        public TariffViewModel() : base()
        {
            ViewModel = new TariffModel();
            TariffList = new List<TariffModel>();
        }

        /// <summary>
        /// Property to bind list of tariff data displayed on data grid
        /// </summary
        public List<TariffModel> TariffList
        {
            set; get;
        }

        /// <summary>
        /// Property to support form data binding reperesenting Business Object of tariff
        ///
        /// </summary>
        public TariffModel ViewModel
        {
            set; get;
        }

        /// <summary>
        /// Property to bind list of product types data displayed as select list
        /// </summary>
        public SelectList ProductTypeList
        {
            set; get;
        }
    }
}