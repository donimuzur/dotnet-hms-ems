namespace Sampoerna.EMS.BusinessObject.Business
{
    public class Login
    {
        public int USER_ID { get; set; }
        public string USERNAME { get; set; }
        public int? MANAGER_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public int? USER_GROUP_ID { get; set; }
    }
}
