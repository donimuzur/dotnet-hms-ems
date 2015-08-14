using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface IExGroupTypeBLL
   {
       void SaveGroup(List<EX_GROUP_TYPE> listGroupTypes);
       void Save(EX_GROUP_TYPE GroupTypes);
      
       void UpdateGroupByGroupName(List<EX_GROUP_TYPE> listGroupTypes, string groupName);
       EX_GROUP_TYPE GetGroupTypeByName(string name);
       EX_GROUP_TYPE GetById(int id);
       List<EX_GROUP_TYPE> GetGroupTypesByName(string name);
       List<EX_GROUP_TYPE> GetAll();
       List<string> GetGoodTypeByGroup(int groupid);
       void DeleteDetails(EX_GROUP_TYPE_DETAILS detail);
       bool IsGroupNameExist(string name);
       void InsertDetail(EX_GROUP_TYPE_DETAILS detail);
   }
}
