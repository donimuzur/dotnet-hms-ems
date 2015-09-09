using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using System.Collections.Generic;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Contract
{
    public interface IDocumentSequenceNumberBLL
    {
        string GenerateNumber(GenerateDocNumberInput input);

        string GenerateNumberByFormType(Enums.FormType formType);

        List<DOC_NUMBER_SEQ> GetDocumentSequenceList();
    }
}