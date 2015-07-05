using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class MaterialBLL : IMaterialBLL
    {
        private IGenericRepository<ZAIDM_EX_MATERIAL> _repository;
        
        private ILogger _logger;
        private IUnitOfWork _uow;
        private string includeTables = "T001W, UOM,ZAIDM_EX_GOODTYP, USER";
        private ChangesHistoryBLL _changesHistoryBll;

        public MaterialBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_MATERIAL>();
            _changesHistoryBll = new ChangesHistoryBLL(_uow,_logger);
        }

        public ZAIDM_EX_MATERIAL getByID(string materialId)
        {
            return _repository.Get(q => q.STICKER_CODE == materialId, null, includeTables).FirstOrDefault();
            //return _repository.GetByID(materialId);
        }

        public List<ZAIDM_EX_MATERIAL> getAll()
        {
            return _repository.Get(null, null, includeTables).ToList();
        }

        public MaterialOutput Save(ZAIDM_EX_MATERIAL data,string userId)
        {


            
                _repository.Update(data);
                var dbData =
                    _repository.Get(c => c.STICKER_CODE == data.STICKER_CODE, null, includeTables)
                        .FirstOrDefault();

                var dataUpdated = Mapper.Map<ZAIDM_EX_MATERIAL>(data);

                SetChanges(dbData, dataUpdated, userId);
                _repository.Update(data);
            

            
            var output = new MaterialOutput();

            try
            {
                _uow.SaveChanges();
                output.Success = true;
                output.materialId = data.STICKER_CODE;
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
                output.Success = false;
                output.ErrorCode = ExceptionCodes.BaseExceptions.unhandled_exception.ToString();
                output.ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
            }
            return output;
        }

        public void Delete(string id, string userId)
        {
            var existingData = _repository.GetByID(id);
            existingData.IS_DELETED = true;
            //existingData.CHANGED_BY = userId;
            //existingData.CHANGED_DATE = DateTime.Now;
            _repository.Update(existingData);

            var changes = new CHANGES_HISTORY
            {
                FORM_TYPE_ID = Core.Enums.MenuList.HeaderFooter,
                FORM_ID = existingData.STICKER_CODE,
                FIELD_NAME = "IS_DELETED",
                MODIFIED_BY = userId,
                MODIFIED_DATE = DateTime.Now,
                OLD_VALUE = existingData.IS_DELETED.HasValue ? existingData.IS_DELETED.Value.ToString() : "NULL",
                NEW_VALUE = true.ToString()
            };

            _changesHistoryBll.AddHistory(changes);

            var output = new MaterialOutput();
            try
            {
                _uow.SaveChanges();
                output.Success = true;
                output.materialId = existingData.STICKER_CODE;
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
                output.Success = false;
                output.ErrorCode = ExceptionCodes.BaseExceptions.unhandled_exception.ToString();
                output.ErrorMessage = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
            }
        }

        private void SetChanges(ZAIDM_EX_MATERIAL origin, ZAIDM_EX_MATERIAL data, string userId)
        {
            var changesData = new Dictionary<string, bool>();
            changesData.Add("MATERIAL_NUMBER", origin.STICKER_CODE.Equals(data.STICKER_CODE));
            changesData.Add("PLANT_ID", origin.WERKS.Equals(data.WERKS));
            changesData.Add("MATERIAL_DESC", origin.MATERIAL_DESC.Equals(data.MATERIAL_DESC));
            changesData.Add("PURCHASING_GROUP", origin.PURCHASING_GROUP.Equals(data.PURCHASING_GROUP));
            changesData.Add("MATERIAL_GROUP", origin.MATERIAL_GROUP.Equals(data.MATERIAL_GROUP));
            changesData.Add("BASE_UOM", origin.BASE_UOM_ID.Equals(data.BASE_UOM_ID));
            changesData.Add("ISSUE_STORANGE_LOC", origin.ISSUE_STORANGE_LOC.Equals(data.ISSUE_STORANGE_LOC));
            changesData.Add("EX_GOODTYP", origin.EXC_GOOD_TYP.Equals(data.EXC_GOOD_TYP));
            
            changesData.Add("IS_DELETED", origin.IS_DELETED.Equals(data.IS_DELETED));
            //changesData.Add("HEADER_FOOTER_FORM_MAP", origin.HEADER_FOOTER_FORM_MAP.Equals(poa.HEADER_FOOTER_FORM_MAP));

            foreach (var listChange in changesData)
            {
                if (!listChange.Value)
                {
                    var changes = new CHANGES_HISTORY
                    {
                        FORM_TYPE_ID = Core.Enums.MenuList.HeaderFooter,
                        FORM_ID = data.STICKER_CODE.ToString(),
                        FIELD_NAME = listChange.Key,
                        MODIFIED_BY = userId,
                        MODIFIED_DATE = DateTime.Now
                    };
                    switch (listChange.Key)
                    {
                        case "MATERIAL_NUMBER":
                            changes.OLD_VALUE = origin.STICKER_CODE;
                            changes.NEW_VALUE = data.STICKER_CODE;
                            break;
                        case "PLANT_ID":
                            changes.OLD_VALUE = origin.WERKS;
                            changes.NEW_VALUE = data.WERKS;
                            break;
                        case "MATERIAL_DESC":
                            changes.OLD_VALUE = origin.MATERIAL_DESC;
                            changes.NEW_VALUE = data.MATERIAL_DESC;
                            break;
                        case "PURCHASING_GROUP":
                            changes.OLD_VALUE = origin.PURCHASING_GROUP;
                            changes.NEW_VALUE = data.PURCHASING_GROUP;
                            break;
                        case "MATERIAL_GROUP":
                            changes.OLD_VALUE = origin.MATERIAL_GROUP;
                            changes.NEW_VALUE = data.MATERIAL_GROUP;
                            break;

                        case "BASE_UOM":
                            changes.OLD_VALUE = origin.BASE_UOM_ID.ToString();
                            changes.NEW_VALUE = data.BASE_UOM_ID.ToString();
                            break;
                        case "ISSUE_STORANGE_LOC":
                            changes.OLD_VALUE = origin.ISSUE_STORANGE_LOC;
                            changes.NEW_VALUE = data.ISSUE_STORANGE_LOC;
                            break;
                        case "EX_GOODTYP":
                            changes.OLD_VALUE = origin.EXC_GOOD_TYP.ToString();
                            changes.NEW_VALUE = data.EXC_GOOD_TYP.ToString();
                            break;
                        case "IS_DELETED":
                            changes.OLD_VALUE = origin.IS_DELETED.HasValue ? origin.IS_DELETED.Value.ToString() : "NULL";
                            changes.NEW_VALUE = data.IS_DELETED.HasValue ? data.IS_DELETED.Value.ToString() : "NULL";
                            break;
                    }
                    _changesHistoryBll.AddHistory(changes);
                }
            }
        }

        
    }
}
