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

    public class GetByFormNumberInput
    {
        public string FormNumber { get; set; }
        public Core.Enums.DocumentStatus DocumentStatus { get; set; }
        public string NppbkcId { get; set; }
        public bool IsRejected { get; set; }
        public string RejectedBy { get; set; }

        public long FormId { get; set; }

        public Sampoerna.EMS.Core.Enums.FormType FormType { get; set; }

        public string FormNumberSource { get; set; }
        public string PlantId { get; set; }
        public string DocumentCreator { get; set; }
    }

    
}
