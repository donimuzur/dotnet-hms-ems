using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Core;

namespace Sampoerna.EMS.Contract
{
    public interface IChangesHistoryBLL
    {
        CHANGES_HISTORY GetById(long id);
        List<CHANGES_HISTORY> GetByFormTypeId(Enums.MenuList formTypeId);

        List<CHANGES_HISTORY> GetAll();
        void AddHistory(CHANGES_HISTORY history);

        List<CHANGES_HISTORY> GetByFormTypeAndFormId(Enums.MenuList formTypeId, string id);

        void DeleteByFormIdAndNewValue(string formId, string newValue);
    }
}