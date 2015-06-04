namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class UserInput
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? IsActive { get; set; }
        public int? ManagerId { get; set; }
        public string SortOrderColumn { get; set; }
    }
}
