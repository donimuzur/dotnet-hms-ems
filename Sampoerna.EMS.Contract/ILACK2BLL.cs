using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.Contract
{
    public interface ILACK2BLL
    {
        List<Lack2Dto> GetAll();
        
        List<Lack2Dto> GetDocumentByParam(Lack2GetByParamInput input);

        List<Lack2Dto> GetOpenDocument();

        Lack2Dto GetById(int id);

        Lack2Dto GetByIdAndItem(int id);

        Lack2Dto Insert(Lack2Dto item);

        void InsertDocument(LACK2_DOCUMENT document);

        int RemoveDoc(int docId);

        List<Lack2Dto> GetCompletedDocument();

        void RemoveExistingItem(long id);

        List<Lack2SummaryReportDto> GetSummaryReportsByParam(Lack2GetSummaryReportByParamInput input);

        List<Lack2DetailReportDto> GetDetailReportsByParam(Lack2GetDetailReportByParamInput input);
    }
}
