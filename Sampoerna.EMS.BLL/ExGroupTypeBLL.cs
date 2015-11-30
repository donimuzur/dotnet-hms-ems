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
        private IGenericRepository<ZAIDM_EX_GOODTYP> _repositoryGoodType;
        private string includeTables = "EX_GROUP_TYPE_DETAILS, EX_GROUP_TYPE_DETAILS.ZAIDM_EX_GOODTYP";

        public ExGroupTypeBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = uow.GetGenericRepository<EX_GROUP_TYPE>();
            _repositoryDetail = _uow.GetGenericRepository<EX_GROUP_TYPE_DETAILS>();
            _repositoryGoodType = _uow.GetGenericRepository<ZAIDM_EX_GOODTYP>();
            _changesHistoryBll = new ChangesHistoryBLL(uow, logger);
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

        public List<ExGoodTyp> GetGroupTypesByName(string name)
        {
            var data = _repository.Get(g => g.GROUP_NAME == name, null, includeTables).ToList();
            return Mapper.Map<List<ExGoodTyp>>(data);
        }

        public List<ExGoodTyp> GetAll()
        {
            List<ExGoodTyp> dataList = new List<ExGoodTyp>();
            
            var deletedexgoodtyplist = _repositoryGoodType.Get(x => x.IS_DELETED == true).Select(x=> x.EXC_GOOD_TYP).ToList();
            var inactiveGroup = _repositoryDetail.Get().Where(x => deletedexgoodtyplist.Contains(x.GOODTYPE_ID)).Select(x => x.EX_GROUP_TYPE_ID).ToList();

            var tempdata = Mapper.Map<List<ExGoodTyp>>(_repository.Get(null, null, includeTables).OrderBy(x => x.GROUP_NAME).ToList());
            foreach (var obj in tempdata) {
                var _data = Mapper.Map<ExGoodTyp>(obj);
                if (inactiveGroup.Contains(obj.EX_GROUP_TYPE_ID))
                {
                    _data.Inactive = true;
                }
                else {
                    _data.Inactive = false;
                }
                dataList.Add(_data);
            }
           
            
                
           
            return dataList;
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

        public void InsertDetail(int groupid,List<EX_GROUP_TYPE_DETAILS> details,string userid)
        {
            //var dbData = _repository.Get(c => c.EX_GROUP_TYPE_ID == detail.EX_GROUP_TYPE_ID, null, includeTables).FirstOrDefault();
            //var exGoodTypeUpdate = Mapper.Map<EX_GROUP_TYPE_DETAILS>(detail);
            var objectToDelete = _repositoryDetail.Get(x => x.EX_GROUP_TYPE_ID == groupid).ToList();
            var origin = _repository.Get(c => c.EX_GROUP_TYPE_ID == groupid, null, includeTables).FirstOrDefault();
            
            SetChange(origin, details, userid);
            foreach (var obj in objectToDelete)
            {
                _repositoryDetail.Delete(obj);
            }
            foreach (var obj in details) {
                _repositoryDetail.Insert(obj);
            }
            _uow.SaveChanges();
        }

        public EX_GROUP_TYPE GetGroupByExGroupType(string goodTypeId)
        {
            var data = _repositoryDetail.Get(x => x.GOODTYPE_ID == goodTypeId, null, "EX_GROUP_TYPE").Select(x=>x.EX_GROUP_TYPE).FirstOrDefault();
            return data;
        }


        private void SetChange(EX_GROUP_TYPE origin, List<EX_GROUP_TYPE_DETAILS> data, string userId)
        {
            var changesData = new Dictionary<string, bool>();
            var originExgoodTyplDesc = string.Empty;
            var goodtypeslist = _repositoryGoodType.Get().ToList();
            if (origin.EX_GROUP_TYPE_DETAILS != null)
            {
                var orlength = origin.EX_GROUP_TYPE_DETAILS.Count;
                var currOr = 0;
                foreach (var or in origin.EX_GROUP_TYPE_DETAILS)
                {
                    currOr++;
                    originExgoodTyplDesc += or.ZAIDM_EX_GOODTYP.EXT_TYP_DESC;
                    if (currOr < orlength)
                    {
                        originExgoodTyplDesc += ",";
                    }

                }

            }
            var editExgoodTyplDesc = string.Empty;
            if (data != null)
            {
                var newLenght = data.Count;
                var currNew = 0;
                foreach (var newdata in data)
                {
                    currNew++;
                    editExgoodTyplDesc += goodtypeslist.Where(x => x.EXC_GOOD_TYP ==  newdata.GOODTYPE_ID).Select(x=> x.EXT_TYP_DESC).FirstOrDefault();
                    if (currNew < newLenght)
                    {
                        editExgoodTyplDesc += ",";
                    }
                }
            }
            changesData.Add("Excisable Group Details", originExgoodTyplDesc == editExgoodTyplDesc);
            foreach (var listChange in changesData)
            {
                if (!listChange.Value)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Core.Enums.MenuList.GoodsTypeGroup,
                        FORM_ID = origin.EX_GROUP_TYPE_ID.ToString(),
                        FIELD_NAME = listChange.Key,
                        MODIFIED_BY = userId,
                        MODIFIED_DATE = DateTime.Now
                    };
                    switch (listChange.Key)
                    {
                        case "Excisable Group Details":
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