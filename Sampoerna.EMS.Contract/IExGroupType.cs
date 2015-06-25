using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
   public interface IExGroupType
   {
       void SaveGroup(List<EX_GROUP_TYPE> listGroupTypes);
       void UpdateGroupByGroupName(List<EX_GROUP_TYPE> listGroupTypes, string groupName);
       EX_GROUP_TYPE GetGroupTypeByName(string name);
       List<EX_GROUP_TYPE> GetGroupTypesByName(string name);
       List<string> GetGroupByGroupName();
   }
}
