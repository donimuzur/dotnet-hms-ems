using System;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class UserInput
    {
        public string USERNAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public bool? IS_ACTIVE { get; set; }
        public int? MANAGER_ID { get; set; }
    }
}
