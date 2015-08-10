using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using AutoMapper;
using Sampoerna.EMS.AutoMapperExtensions;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class ExGroupTypeBLL : IExGroupTypeBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;

        private IGenericRepository<EX_GROUP_TYPE> _repository;
        private IChangesHistoryBLL _changesHistoryBll;
        private IGenericRepository<EX_GROUP_TYPE_DETAILS> _repositoryDetail;
        private string includeTables = "EX_GROUP_TYPE_DETAILS, EX_GROUP_TYPE_DETAILS.ZAIDM_EX_GOODTYP";

        public ExGroupTypeBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = uow.GetGenericRepository<EX_GROUP_TYPE>();
            _repositoryDetail = _uow.GetGenericRepository<EX_GROUP_TYPE_DETAILS>();
            _changesHistoryBll = new ChangesHistoryBLL(uow, logger);
        }


        public void SaveGroup(List<EX_GROUP_TYPE> listGroupTypes)
        {
            foreach (var listGroupType in listGroupTypes)
            {
                _repository.InsertOrUpdate(listGroupType);
            }
            _uow.SaveChanges();
        }

        public void Save(EX_GROUP_TYPE GroupTypes)
        {
            _repository.InsertOrUpdate(GroupTypes);
            _uow.SaveChanges();
        }

        public void UpdateGroupByGroupName(List<EX_GROUP_TYPE> listGroupTypes, string groupName)
        {
            //delete first

            var dbGroup = _repository.Get(a => a.GROUP_NAME == groupName, null, includeTables).ToList();



            //insert the data
            foreach (var listGroupType in listGroupTypes)
            {
                _repository.InsertOrUpdate(listGroupType);
            }
            _uow.SaveChanges();
        }

        public EX_GROUP_TYPE GetGroupTypeByName(string name)
        {
            return _repository.Get(g => g.GROUP_NAME == name).FirstOrDefault();
        }

        public EX_GROUP_TYPE GetById(int id)
        {
            return _repository.Get(p => p.EX_GROUP_TYPE_ID == id, null, includeTables).FirstOrDefault();
        }

        public List<EX_GROUP_TYPE> GetGroupTypesByName(string name)
        {
            return _repository.Get(g => g.GROUP_NAME == name, null, includeTables).ToList();
        }

        public List<EX_GROUP_TYPE> GetAll()
        {

            return _repository.Get(null, null, includeTables).OrderBy(x => x.GROUP_NAME).ToList();
        }

        public List<string> GetGoodTypeByGroup(int groupid)
        {
            return
                _repositoryDetail.Get(p => p.EX_GROUP_TYPE_ID == groupid, null, "ZAIDM_EX_GOODTYP")
                    .Select(p => p.ZAIDM_EX_GOODTYP.EXT_TYP_DESC)
                    .ToList();
        }

        public void DeleteDetails(EX_GROUP_TYPE_DETAILS details)
        {
            _repositoryDetail.Delete(details);
        }

        public bool IsGroupNameExist(string name)
        {
            var dbGroup = _repository.Get(g => g.GROUP_NAME == name).FirstOrDefault();
            if (dbGroup == null)
                return false;

            return true;
        }

        public void InsertDetail(EX_GROUP_TYPE_DETAILS detail)
        {
            //var dbData = _repository.Get(c => c.EX_GROUP_TYPE_ID == detail.EX_GROUP_TYPE_ID, null, includeTables).FirstOrDefault();
            //var exGoodTypeUpdate = Mapper.Map<EX_GROUP_TYPE_DETAILS>(detail);

            //SetChange(dbData, detail, exGoodTypeUpdate, userId);
            _repositoryDetail.Insert(detail);
            _uow.SaveChanges();
        }



        private void SetChange(EX_GROUP_TYPE origin, ExGoodTyp data, string userId,
            List<EX_GROUP_TYPE_DETAILS> originGoodType)
        {
            var changesData = new Dictionary<string, bool>();
            var originExgoodTyplDesc = string.Empty;
            if (originGoodType != null)
            {
                var orlength = originGoodType.Count;
                var currOr = 0;
                foreach (var or in originGoodType)
                {
                    currOr++;
                    originExgoodTyplDesc += or.ZAIDM_EX_GOODTYP.EXT_TYP_DESC;
                    if (currOr < orlength)
                    {
                        originExgoodTyplDesc = ",";
                    }

                }

            }
            var editExgoodTyplDesc = string.Empty;
            if (data.ZAIDM_EX_GOODTYP != null)
            {
                var orLenght = data.ZAIDM_EX_GOODTYP.Count;
                var currOr = 0;
                foreach (var or in data.ZAIDM_EX_GOODTYP)
                {
                    currOr++;
                    editExgoodTyplDesc += or.EXT_TYP_DESC;
                    if (currOr < orLenght)
                    {
                        editExgoodTyplDesc = ",";
                    }
                }
            }
            changesData.Add("Ex Grop Details", originExgoodTyplDesc == editExgoodTyplDesc);
            foreach (var listChange in changesData)
            {
                if (!listChange.Value)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Core.Enums.MenuList.MasterPlant,
                        FORM_ID = data.GROUP_NAME,
                        FIELD_NAME = listChange.Key,
                        MODIFIED_BY = userId,
                        MODIFIED_DATE = DateTime.Now
                    };
                    switch (listChange.Key)
                    {
                        case "RECEIVE_MATERIAL":
                            changes.OLD_VALUE = originExgoodTyplDesc;
                            changes.NEW_VALUE = editExgoodTyplDesc;
                            break;
                    }
                    _changesHistoryBll.AddHistory(changes);
                }
            }
        }
    }
}