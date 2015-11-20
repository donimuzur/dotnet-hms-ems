using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class WasteRoleBLL : IWasteRoleBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<WASTE_ROLE> _repository;

        private IChangesHistoryBLL _changesHistoryBll;

        private string _includeTables = "T001W, USER";
        public WasteRoleBLL(IUnitOfWork uow, ILogger logger)
        {
           _uow = uow;
           _logger = logger;
           _repository = _uow.GetGenericRepository<WASTE_ROLE>();

            _changesHistoryBll = new ChangesHistoryBLL(_uow, _logger);
        }

        public List<WasteRoleDto> GetAllData(bool isIncludeTables = true)
        {
            string include = "";
            if (isIncludeTables)
                include = _includeTables;
            var listData = _repository.Get(null, null, include).ToList();

            return Mapper.Map<List<WasteRoleDto>>(listData);
        }

        public List<WasteRoleDto> GetAllDataOrderByUserAndGroupRole()
        {
            Func<IQueryable<WASTE_ROLE>, IOrderedQueryable<WASTE_ROLE>> orderByFilter =
                n => n.OrderBy(z => z.USER_ID).ThenBy(z => z.GROUP_ROLE);

            var listData = _repository.Get(null, orderByFilter, _includeTables).ToList();

            return Mapper.Map<List<WasteRoleDto>>(listData);
        }

        public WasteRoleDto GetById(int id)
        {
            var dtData = _repository.GetByID(id);
            if (dtData == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return Mapper.Map<WasteRoleDto>(dtData);

        }

        public WasteRoleDto GetById(int id,bool isIncludeTable)
        {
            string include = "";
            if (isIncludeTable)
            {
                include = _includeTables;
                var dtData = _repository.Get(c => c.WASTE_ROLE_ID == id, null, include).FirstOrDefault();
                if (dtData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                return Mapper.Map<WasteRoleDto>(dtData);
            }

            return GetById(id);
        }

        private void UpdatePlantByUserId(string userId, string plantUpdate)
        {
            var listWasteRole = _repository.Get(c => c.USER_ID == userId).ToList();
            foreach (var waste in listWasteRole)
            {
                var wasteRole = _repository.GetByID(waste.WASTE_ROLE_ID);
                if (wasteRole != null)
                {
                    wasteRole.WERKS = plantUpdate;
                    _repository.Update(wasteRole);
                }
            }
        }
        private void ValidateWasteRole(WasteRoleSaveInput input)
        {
            var listData = _repository.Get(c => c.USER_ID == input.WasteRoleDto.USER_ID).ToList();

            if (listData.Count > 0)
            {
                if (input.WasteRoleDto.WASTE_ROLE_ID > 0)
                {
                    var originalData = _repository.GetByID(input.WasteRoleDto.WASTE_ROLE_ID);
                    if (originalData == null)
                        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                    if (input.WasteRoleDto.WERKS != originalData.WERKS)
                    {
                        //find is user have another plant
                        var data =
                            _repository.Get(
                                c => c.USER_ID == input.WasteRoleDto.USER_ID && c.WERKS != input.WasteRoleDto.WERKS).ToList();
                        if (data.Count > 0)
                            throw new Exception(string.Format("User already have Plant : {0} .", data[0].WERKS));

                    }

                }
                else
                {
                    foreach (var wasteRole in listData)
                    {
                        if (wasteRole.WERKS != input.WasteRoleDto.WERKS)
                            throw new Exception(string.Format("User already have Plant : {0} .", wasteRole.WERKS));
                            //throw new BLLException(ExceptionCodes.BLLExceptions.UserHavePlantExist);
                    }

                  
                }

                var dataUser =
                      _repository.Get(
                          c =>
                              c.USER_ID == input.WasteRoleDto.USER_ID && c.WERKS == input.WasteRoleDto.WERKS &&
                              c.GROUP_ROLE == input.WasteRoleDto.GROUP_ROLE).FirstOrDefault();

                if (dataUser != null)
                    throw new Exception(string.Format("User And Plant already have Role : {0} .", EnumHelper.GetDescription(input.WasteRoleDto.GROUP_ROLE)));
                   
            }
           

           
        }

        public WasteRoleDto SaveWasteRole(WasteRoleSaveInput input)
        {
            ValidateWasteRole(input);

            WASTE_ROLE dbData = null;
            if (input.WasteRoleDto.WASTE_ROLE_ID > 0)
            {
                //update
                dbData = _repository.GetByID(input.WasteRoleDto.WASTE_ROLE_ID);
                if (dbData == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                //set changes history
                var origin = Mapper.Map<WasteRoleDto>(dbData);

                SetChangesHistory(origin, input.WasteRoleDto, input.UserId);

                Mapper.Map<WasteRoleDto, WASTE_ROLE>(input.WasteRoleDto, dbData);

                dbData.MODIFIED_DATE = DateTime.Now;
                dbData.MODIFIED_BY = input.UserId;

            }
            else
            {
                input.WasteRoleDto.CREATED_DATE = DateTime.Now;
                input.WasteRoleDto.CREATED_BY = input.UserId;
                dbData = new WASTE_ROLE();
                Mapper.Map<WasteRoleDto, WASTE_ROLE>(input.WasteRoleDto, dbData);
                _repository.Insert(dbData);
            }



            try
            {
                _uow.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }


            return Mapper.Map<WasteRoleDto>(dbData); 

        }

        private bool SetChangesHistory(WasteRoleDto origin, WasteRoleDto data, string userId)
        {
            bool isModified = false;

            var changesData = new Dictionary<string, bool>();
            changesData.Add("USER_ID", origin.USER_ID == data.USER_ID);
            changesData.Add("PLANT_ID", origin.WERKS == data.WERKS);
            changesData.Add("WASTE_GROUP", origin.GROUP_ROLE == data.GROUP_ROLE);

            foreach (var listChange in changesData)
            {
                if (listChange.Value) continue;
                var changes = new CHANGES_HISTORY();
                changes.FORM_TYPE_ID = Enums.MenuList.WasteRole;
                changes.FORM_ID = origin.WASTE_ROLE_ID.ToString();
                changes.FIELD_NAME = listChange.Key;
                changes.MODIFIED_BY = userId;
                changes.MODIFIED_DATE = DateTime.Now;
                switch (listChange.Key)
                {
                    case "USER_ID":
                        changes.OLD_VALUE = origin.USER_ID;
                        changes.NEW_VALUE = data.USER_ID;
                        break;

                    case "PLANT_ID":
                        changes.OLD_VALUE = origin.WERKS;
                        changes.NEW_VALUE = data.WERKS;
                        break;

                    case "WASTE_GROUP":
                        changes.OLD_VALUE = EnumHelper.GetDescription(origin.GROUP_ROLE);
                        changes.NEW_VALUE = EnumHelper.GetDescription(data.GROUP_ROLE);
                        break;

                }

                _changesHistoryBll.AddHistory(changes);
                isModified = true;
            }
            return isModified;
        }
    }
}
