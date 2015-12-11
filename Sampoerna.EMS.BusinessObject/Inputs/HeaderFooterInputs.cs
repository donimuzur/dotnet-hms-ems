using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.BusinessObject.Inputs
{
    public class HeaderFooterGetByComanyAndFormTypeInput
    {
        public string CompanyCode { get; set; }
        public Enums.FormType FormTypeId { get; set; }
    }
}
