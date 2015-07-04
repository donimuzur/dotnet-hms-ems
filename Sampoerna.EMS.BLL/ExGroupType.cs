using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class ExGroupType : IExGroupType
    {
        //private ILogger _logger;
        //private IUnitOfWork _uow;

        //private IGenericRepository<EX_GROUP_TYPE> _repository;

        //public ExGroupType(IUnitOfWork uow, ILogger logger)
        //{
        //    _logger = logger;
        //    _uow = uow;
        //    _repository = _uow.GetGenericRepository<EX_GROUP_TYPE>();
        //}

        //public void SaveGroup(List<EX_GROUP_TYPE> listGroupTypes)
        //{
        //    foreach (var listGroupType in listGroupTypes)
        //    {
        //        _repository.InsertOrUpdate(listGroupType);
        //    }
        //    _uow.SaveChanges();
        //}

        //public void UpdateGroupByGroupName(List<EX_GROUP_TYPE> listGroupTypes, string groupName)
        //{
        //    //delete first

        //    var dbGroup = _repository.Get(a => a.GROUP_NAME == groupName).ToList();

        //    foreach (var exGroupType in dbGroup)
        //    {
        //        _repository.Delete(exGroupType);
        //    }

        //    //insert the data
        //    foreach (var listGroupType in listGroupTypes)
        //    {

        //        _repository.InsertOrUpdate(listGroupType);
        //    }
        //    _uow.SaveChanges();
        //}

        //public EX_GROUP_TYPE GetGroupTypeByName(string name)
        //{
        //    return _repository.Get(g => g.GROUP_NAME == name).FirstOrDefault();
        //}

        //public List<EX_GROUP_TYPE> GetGroupTypesByName(string name)
        //{
        //    return _repository.Get(g => g.GROUP_NAME == name, null, "ZAIDM_EX_GOODTYP").ToList();
        //}

        //public List<string> GetGroupByGroupName()
        //{
        //    return _repository.Get().ToList().Select(type => type.GROUP_NAME).Distinct().ToList(); ;
        //}

        //public bool IsGroupNameExist(string name)
        //{
        //    var dbGroup = _repository.Get(g => g.GROUP_NAME == name).FirstOrDefault();
        //    if (dbGroup == null)
        //        return false;

        //    return true;
        //}
    }
}
