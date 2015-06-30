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
        private string includeTables = "T1001W, UOM,ZAIDM_EX_GOODTYP, USER";

        public MaterialBLL(IUnitOfWork uow, ILogger logger){
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_MATERIAL>();
        }

        public ZAIDM_EX_MATERIAL getByID(long materialId)
        {
            return _repository.Get(q => q.MATERIAL_ID == materialId, null, includeTables).FirstOrDefault();
            //return _repository.GetByID(materialId);
        }

        public List<ZAIDM_EX_MATERIAL> getAll()
        {
            return _repository.Get(null, null, includeTables).ToList();
        }

        public MaterialOutput Save(ZAIDM_EX_MATERIAL data)
        {


            //insert
            if (data.MATERIAL_ID == 0)
            {
                _repository.Insert(data);
            }
            else
            {
                _repository.Update(data);
            }

            
            var output = new MaterialOutput();

            try
            {
                _uow.SaveChanges();
                output.Success = true;
                output.materialId = data.MATERIAL_ID;
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

        
    }
}
