namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class GetByActionAndFormNumberInput
    {
        public string FormNumber { get; set; }
        public Core.Enums.ActionType ActionType { get; set; }
    }

    public class GetByFormTypeAndFormIdInput
    {
        public long FormId { get; set; }
        public Core.Enums.FormType FormType { get; set; }
    }

}
