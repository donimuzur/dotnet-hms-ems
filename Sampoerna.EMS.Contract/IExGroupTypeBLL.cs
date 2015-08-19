using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
   public interface IExGroupTypeBLL
   {
       
       void Save(EX_GROUP_TYPE GroupTypes);
      
       void UpdateGroupByGroupName(List<EX_GROUP_TYPE> listGroupTypes, string groupName);
       EX_GROUP_TYPE GetGroupTypeByName(string name);
       EX_GROUP_TYPE GetById(int id);
       List<EX_GROUP_TYPE> GetGroupTypesByName(string name);
       List<EX_GROUP_TYPE> GetAll(bool includedeletedchild = true);
       List<string> GetGoodTypeByGroup(int groupid);
       void DeleteDetails(EX_GROUP_TYPE_DETAILS detail);
       bool IsGroupNameExist(string name);
       void InsertDetail(int groupid, List<EX_GROUP_TYPE_DETAILS> details, string userid);
   }
}
