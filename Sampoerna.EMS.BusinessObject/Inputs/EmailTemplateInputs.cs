using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class EmailTemplateGetByDocumentAndActionTypeInput
    {
        public Enums.FormType FormType { get; set; }
        public Enums.ActionType ActionType { get; set; }
    }
}
