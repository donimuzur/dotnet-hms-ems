using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Contract
{
    public interface IPrintHistoryBLL
    {
        List<PrintHistoryDto> GetByFormNumber(string formNumber);
        List<PrintHistoryDto> GetByFormTypeAndFormId(Enums.FormType formType, long formId);
        PrintHistoryDto GetById(long id);
        void AddPrintHistory(PrintHistoryDto printHistoryData);
    }
}