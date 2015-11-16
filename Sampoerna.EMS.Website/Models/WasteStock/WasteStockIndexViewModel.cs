using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.WasteStock
{
    public class WasteStockIndexViewModel : BaseModel
    {
        public WasteStockIndexViewModel()
        {
            ListWasteStocks = new List<WasteStockFormViewModel>();
        }
        public List<WasteStockFormViewModel> ListWasteStocks { get; set; }
    }
}