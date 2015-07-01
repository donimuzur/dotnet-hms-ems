using System;
using System.ComponentModel.DataAnnotations;

namespace Sampoerna.EMS.Website.Models.ChangesHistory
{

    public class ChangesHistoryItemModel
    {
        public long CHANGES_HISTORY_ID { get; set; }
        public Core.Enums.FormType FORM_TYPE_ID { get; set; }
        public string FORM_TYPE_DESC { get; set; }
        public long? FORM_ID { get; set; }
        public string FIELD_NAME { get; set; }
        public string OLD_VALUE { get; set; }
        public string NEW_VALUE { get; set; }
        [UIHint("FormatDateTime")]
        public DateTime? MODIFIED_DATE { get; set; }
        public int? MODIFIED_BY { get; set; }
        public string USERNAME { get; set; }
        public string USER_FIRST_NAME { get; set; }
        public string USER_LAST_NAME { get; set; }
    }

}