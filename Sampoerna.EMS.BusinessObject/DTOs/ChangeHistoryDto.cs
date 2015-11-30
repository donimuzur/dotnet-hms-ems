using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.DTOs
{
   public class ChangeHistoryDto
    {
        public long CHANGES_HISTORY_ID { get; set; }
        public Sampoerna.EMS.Core.Enums.MenuList FORM_TYPE_ID { get; set; }
        public Nullable<long> FORM_ID { get; set; }
        public string FIELD_NAME { get; set; }
        public string OLD_VALUE { get; set; }
        public string NEW_VALUE { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public Nullable<int> MODIFIED_BY { get; set; }

        public string USERNAME { get; set; }
        public string USER_FIRST_NAME { get; set; }
        public string USER_LAST_NAME { get; set; }
    }
}
