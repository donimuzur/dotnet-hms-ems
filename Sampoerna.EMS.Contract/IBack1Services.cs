using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface IBack1Services
    {
        void SaveBack1ByCk5Id(SaveBack1ByCk5IdInput input);

        BACK1 GetBack1ByCk5Id(long ck5Id);

        void SaveBack1ByPbck7Id(SaveBack1ByPbck7IdInput input);

        BACK1 GetBack1ByPbck7Id(int pbck7Id);

        void InsertOrDeleteBack1Documents(List<BACK1_DOCUMENTDto> input);
    }
}
