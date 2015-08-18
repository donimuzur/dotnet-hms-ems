using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Inputs;
using System.Collections.Generic;

namespace Sampoerna.EMS.Contract
{
    public interface IDocumentSequenceNumberBLL
    {
        string GenerateNumber(GenerateDocNumberInput input);

        List<DOC_NUMBER_SEQ> GetDocumentSequenceList();
    }
}