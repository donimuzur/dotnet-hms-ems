using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class PlantBLL : IPlantBLL
    {

        private IGenericRepository<T1001W> _repository;
        private ILogger _logger;
        private IUnitOfWork _uow;
        private string includeTables = "ZAIDM_EX_NPPBKC, ZAIDM_EX_GOODTYP";
        
        public PlantBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<T1001W>();
        }

        public T1001W GetId(long id)
        {
            return _repository.Get(c => c.PLANT_ID == id,null, includeTables).FirstOrDefault();
        }

        public List<T1001W> GetAll()
        {

           return _repository.Get(null, null, includeTables).ToList();
            
        }

        public void save(T1001W plantT1001W)
        {
            if (plantT1001W.PLANT_ID != 0)
            {
                //update
                _repository.Update(plantT1001W);
            }
            else
            {
                //Insert
                _repository.Insert(plantT1001W);
            }

            try
            {
                _uow.SaveChanges();

            }
            catch (Exception exception)
            {
                _logger.Error(exception);

            }
        }
    }
}
