using System;
using System.Collections.Generic;

namespace Sampoerna.EMS.Website.Models.PrintHistory
{
    public class PrintHistoryViewModel : BaseModel
    {
        public List<PrintHistoryItemModel> Details { get; set; }
        public string Pbck1Id { get; set; }
        public string Pbck1Number { get; set; }
        public DateTime CurrentDateTime { get; set; }
    }

    public class PrintHistoryItemModel
    {
        public long PRINT_HISTORY_ID { get; set; }
        public Core.Enums.FormType FORM_TYPE_ID { get; set; }
        public long FORM_ID { get; set; }
        public string FORM_NUMBER { get; set; }
        public System.DateTime PRINT_DATE { get; set; }
        public string PRINT_BY { get; set; }
    }

}