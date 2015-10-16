using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
   public interface ICK2Services
   {
       CK2 GetCk2ByPbck3Id(int pbck3Id);
   }
}
