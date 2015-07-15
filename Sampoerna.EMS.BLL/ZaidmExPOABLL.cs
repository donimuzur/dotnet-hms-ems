using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core.Exceptions;
using Sampoerna.EMS.Utils;
using Voxteneo.WebComponents.Logger;
using Enums = Sampoerna.EMS.Core.Enums;

namespace Sampoerna.EMS.BLL
{
    public class ZaidmExPOABLL : IZaidmExPOABLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<ZAIDM_EX_POA> _repository;
        private string includeTables = "ZAIDM_POA_MAP, USER, USER1";
        
        public ZaidmExPOABLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_POA>();
            
        }


        public ZAIDM_EX_POA GetById(int id)
        {
            return _repository.Get(p=>p.POA_ID == id, null, includeTables).FirstOrDefault();
        }

        public List<ZAIDM_EX_POA> GetAll()
        {
            return _repository.Get(null, null, includeTables).ToList();
        }

        public void Save(ZAIDM_EX_POA poa)
        {
            
                
            
            try
            {
                //Insert
                _repository.Insert(poa);
            
                _uow.SaveChanges();
           
            }
            catch (Exception exception)
            {
                _uow.RevertChanges();
                _logger.Error(exception);
              
            }
            
        }




      
        public void Delete(int id)
        {
            var existingPoa = GetById(id);

            if (existingPoa.IS_DELETED == false)
            {
                existingPoa.IS_DELETED = true;
            }
            else
            {
                existingPoa.IS_DELETED = false;
            }
            //existingPoa.IS_DELETED = true;
            _repository.Update(existingPoa);
            _uow.SaveChanges();
        }

        public void Update( ZAIDM_EX_POA poa)
        {
            try
            {
                 _repository.Update(poa);
                _uow.SaveChanges();
            }
            catch (Exception ex)
            {
                _uow.RevertChanges();
                throw;
            }
          
        }

        public Enums.UserRole GetUserRole(int id)
        {
            var poa = GetAll();

            if (poa.Any(zaidmExPoa => zaidmExPoa.MANAGER_ID == id))
                return Enums.UserRole.Manager;
           
            if (poa.Any(zaidmExPoa => zaidmExPoa.USER_ID == id))
                return Enums.UserRole.POA;
          

            return Enums.UserRole.User;
        }
    }
}
