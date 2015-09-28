using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;

namespace Sampoerna.EMS.Contract
{
    public interface IExGroupTypeBLL
   {
       
       void Save(EX_GROUP_TYPE GroupTypes);
      
       void UpdateGroupByGroupName(List<EX_GROUP_TYPE> listGroupTypes, string groupName);
       EX_GROUP_TYPE GetGroupTypeByName(string name);
       EX_GROUP_TYPE GetById(int id);
       List<ExGoodTyp> GetGroupTypesByName(string name);
       //List<ExGoodTyp> GetAll(bool includedeletedchild = true);
       List<ExGoodTyp> GetAll();
       List<string> GetGoodTypeByGroup(int groupid);
       void DeleteDetails(EX_GROUP_TYPE_DETAILS detail);
       bool IsGroupNameExist(string name);
       void InsertDetail(int groupid, List<EX_GROUP_TYPE_DETAILS> details, string userid);

        EX_GROUP_TYPE GetGroupByExGroupType(string goodTypeId);
   }
}
