using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;
using System;

namespace Sampoerna.EMS.BLL
{
    public class UnitOfMeasurementBLL : IUnitOfMeasurementBLL
    {

        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<UOM> _repository;
        private ChangesHistoryBLL _changeBLL;
        
        public UnitOfMeasurementBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<UOM>();
            _changeBLL = new ChangesHistoryBLL(uow, logger);
            
        }
        public UOM GetById(string id)
        {
            return _repository.GetByID(id);
        }

        public UOM GetByName(string uomName)
        {
            return _repository.Get(c => c.UOM_DESC == uomName, null, string.Empty).FirstOrDefault();
        }

        public List<UOM> GetAll()
        {
            return _repository.Get().ToList();
        }

        public void Save(UOM uom,string userid, bool IsEdit) 
        {
            
            if (IsEdit)
            {

                UOM data = _repository.GetByID(uom.UOM_ID);
                SetChanges(data, uom, userid);
                data.UOM_DESC = uom.UOM_DESC;

                _repository.Update(data);

            }
            else
            {
                 uom.CREATED_DATE = DateTime.Now;
                 _repository.InsertOrUpdate(uom);
            }
               
              
            
            _uow.SaveChanges();
        }


        private void SetChanges(UOM origin, UOM data, string userId)
        {
            var changesData = new Dictionary<string, bool>();
            changesData.Add("UOM_ID", origin.UOM_ID == data.UOM_ID);
            changesData.Add("UOM_DESC", origin.UOM_DESC == data.UOM_DESC);
            
            //changesData.Add("HEADER_FOOTER_FORM_MAP", origin.HEADER_FOOTER_FORM_MAP.Equals(poa.HEADER_FOOTER_FORM_MAP));

            foreach (var listChange in changesData)
            {
                if (!listChange.Value)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Core.Enums.MenuList.Uom,
                        FORM_ID = data.UOM_ID.ToString(),
                        FIELD_NAME = listChange.Key,
                        MODIFIED_BY = userId,
                        MODIFIED_DATE = DateTime.Now
                    };
                    switch (listChange.Key)
                    {
                        case "UOM_ID":
                            changes.OLD_VALUE = origin.UOM_ID.ToString();
                            changes.NEW_VALUE = data.UOM_ID.ToString();
                            break;
                        case "UOM_DESC":
                            changes.OLD_VALUE = origin.UOM_DESC;
                            changes.NEW_VALUE = data.UOM_DESC;
                            break;
                        default: break;
                    }
                    _changeBLL.AddHistory(changes);
                }
            }
        }

        public string GetUomNameById(int? id)
        {
            var dbData = _repository.GetByID(id);

            if (dbData == null)
                return string.Empty;

            return dbData.UOM_DESC;
        }

        public bool IsUomIdExist(string uomId)
        {
            //var dbData = _repository.Get(u => u.UOM_DESC.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            var dbData = _repository.GetByID(uomId);
            return dbData != null;
        }

    }
}
