using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;
using System;

namespace Sampoerna.EMS.BLL
{
    public class ExGroupType : IExGroupType
    {
        private ILogger _logger;
        private IUnitOfWork _uow;

        private IGenericRepository<EX_GROUP_TYPE> _repository;
        private IGenericRepository<EX_GROUP_TYPE_DETAILS> _repositoryDetails;
        private IGenericRepository<ZAIDM_EX_GOODTYP> _repositoryGoodType;
        private ChangesHistoryBLL _changeBll;

        public ExGroupType(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<EX_GROUP_TYPE>();
            _repositoryDetails = _uow.GetGenericRepository<EX_GROUP_TYPE_DETAILS>();
            _repositoryGoodType = _uow.GetGenericRepository<ZAIDM_EX_GOODTYP>();
            _changeBll = new ChangesHistoryBLL(uow, logger);
        }

        public void SaveGroup(List<EX_GROUP_TYPE_DETAILS> listGroupTypes,string groupname)
        {
            EX_GROUP_TYPE group = new EX_GROUP_TYPE();
            group.GROUP_NAME = groupname;
            
            foreach (var listGroupType in listGroupTypes)
            {
                
                _repositoryDetails.InsertOrUpdate(listGroupType);
            }
            _uow.SaveChanges();
            
        }

        public void SaveGroup(EX_GROUP_TYPE group)
        {
            //EX_GROUP_TYPE group = new EX_GROUP_TYPE();
            //group.GROUP_NAME = groupname;
            //foreach (var listGroupType in listGroupTypes)
            //{
            //    _repository.InsertOrUpdate(listGroupType);
            //}
            _repository.InsertOrUpdate(group);
            _uow.SaveChanges();

            var groupid = group.EX_GROUP_TYPE_ID;

            foreach (var detail in group.EX_GROUP_TYPE_DETAILS) {
                detail.EX_GROUP_TYPE_ID = groupid;
            }

            _repository.InsertOrUpdate(group);

            _uow.SaveChanges();
        }

        public void UpdateGroupByGroupName(List<EX_GROUP_TYPE_DETAILS> listGroupTypes, string groupName)
        {
            //delete first

            var dbGroup = _repository.Get(a => a.GROUP_NAME == groupName).FirstOrDefault();
            var dbDetail = _repositoryDetails.Get(a => a.EX_GROUP_TYPE.GROUP_NAME == groupName).ToList();
            foreach (var exGroupType in dbDetail)
            {
                _repositoryDetails.Delete(exGroupType);
            }
            //SetChanges(dbGroup,)
            _repository.InsertOrUpdate(dbGroup);



            //insert the data
            foreach (var listGroupType in listGroupTypes)
            {
                if (_repositoryDetails.Get(obj => obj.EX_GROUP_TYPE_DETAIL_ID == listGroupType.EX_GROUP_TYPE_DETAIL_ID && obj.EX_GROUP_TYPE_ID == dbGroup.EX_GROUP_TYPE_ID, null, "").Count() == 0) {
                    listGroupType.EX_GROUP_TYPE_ID = dbGroup.EX_GROUP_TYPE_ID;
                    _repositoryDetails.Insert(listGroupType);
                }
                
            }
            _uow.SaveChanges();
        }

        

        public List<EX_GROUP_TYPE_DETAILS> GetGroupTypesByName(string name)
        {
            var data = _repository.Get(g => g.GROUP_NAME.Equals(name,System.StringComparison.CurrentCultureIgnoreCase) , null, "EX_GROUP_TYPE_DETAILS").Select(obj => obj.EX_GROUP_TYPE_ID).FirstOrDefault();

            return _repositoryDetails.Get(g => g.EX_GROUP_TYPE_ID == data, null, "ZAIDM_EX_GOODTYP").ToList();
        }

        public List<string> GetGroupByGroupName()
        {
            return _repository.Get().ToList().Select(type => type.GROUP_NAME).Distinct().ToList(); ;
        }

        public bool IsGroupNameExist(string name)
        {
            var dbGroup = _repository.Get(g => g.GROUP_NAME == name).FirstOrDefault();
            if (dbGroup == null)
                return false;

            return true;
        }

        public List<ZAIDM_EX_GOODTYP> getAllGoodTypeId() {
            return _repositoryGoodType.Get().ToList();
        }

        private void SetChanges(EX_GROUP_TYPE origin, EX_GROUP_TYPE data, string userId)
        {
            var changesData = new Dictionary<string, bool>();
            changesData.Add("GROUP_NAME", origin.GROUP_NAME.Equals(data.GROUP_NAME));
            
            //changesData.Add("HEADER_FOOTER_FORM_MAP", origin.HEADER_FOOTER_FORM_MAP.Equals(poa.HEADER_FOOTER_FORM_MAP));

            foreach (var listChange in changesData)
            {
                if (!listChange.Value)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Core.Enums.MenuList.HeaderFooter,
                        FORM_ID = data.EX_GROUP_TYPE_ID.ToString(),
                        FIELD_NAME = listChange.Key,
                        MODIFIED_BY = userId,
                        MODIFIED_DATE = DateTime.Now
                    };
                    switch (listChange.Key)
                    {
                        case "GROUP_NAME":
                            changes.OLD_VALUE = origin.GROUP_NAME;
                            changes.NEW_VALUE = data.GROUP_NAME;
                            break;
                        default: break;
                    }
                    _changeBll.AddHistory(changes);
                }
            }
        }

        void IExGroupType.SaveGroup(List<EX_GROUP_TYPE> listGroupTypes)
        {
            throw new NotImplementedException();
        }

        void IExGroupType.UpdateGroupByGroupName(List<EX_GROUP_TYPE> listGroupTypes, string groupName)
        {
            throw new NotImplementedException();
        }

        EX_GROUP_TYPE IExGroupType.GetGroupTypeByName(string name)
        {
            throw new NotImplementedException();
        }

        List<EX_GROUP_TYPE> IExGroupType.GetGroupTypesByName(string name)
        {
            throw new NotImplementedException();
        }

        List<string> IExGroupType.GetGroupByGroupName()
        {
            throw new NotImplementedException();
        }

        bool IExGroupType.IsGroupNameExist(string name)
        {
            throw new NotImplementedException();
        }
    }
}
