using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
   public interface ICK2Services
   {
       CK2 GetCk2ByPbck3Id(int pbck3Id);

       void SaveCk2ByPbck3Id(SaveCk2ByPbck3IdInput input);

       bool IsExistCk2DocumentByPbck3(int pbck3Id);

       void InsertOrDeleteCk2Item(List<CK2_DOCUMENTDto> input);
   }
}
