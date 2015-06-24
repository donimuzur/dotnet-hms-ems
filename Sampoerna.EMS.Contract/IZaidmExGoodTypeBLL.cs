using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface IZaidmExGoodTypeBLL
    {
        ZAIDM_EX_GOODTYP GetById(int id);
        
        List<ZAIDM_EX_GOODTYP> GetAll();

        List<ZAIDM_EX_GOODTYP> GetAllChildName();

        void SaveGroup(List<EX_GROUP_TYPE> listGroupTypes);

        List<EX_GROUP_TYPE> GetAllGroup();

        EX_GROUP_TYPE GetGroupTypeByName(string name);

        List<EX_GROUP_TYPE> GetGroupTypesByName(string name);
    }
}