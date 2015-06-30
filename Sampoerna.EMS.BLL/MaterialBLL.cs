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
        private string includeTables = "T1001W, ZAIDM_EX_BRAND, ZAIDM_EX_GOODTYP, UOM";

        public MaterialBLL(IUnitOfWork uow, ILogger logger){
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<ZAIDM_EX_MATERIAL>();
        }

        public Material getByID(long materialId)
        {
            return Mapper.Map<Material>(_repository.GetByID(materialId));
        }

        public List<Material> getAll()
        {
            return Mapper.Map <List<Material >>(_repository.Get(null, null, includeTables).ToList());
        }

        public MaterialOutput Save(Material data)
        {
            ZAIDM_EX_MATERIAL saveData = null;

            //edit
            if (data.MATERIAL_ID > 0)
            {
                
                saveData = Mapper.Map<Material, ZAIDM_EX_MATERIAL>(data);
            }
            else { //insert
                saveData = Mapper.Map<ZAIDM_EX_MATERIAL>(data);
                _repository.Insert(saveData);
            }

            var output = new MaterialOutput();

            try
            {
                _uow.SaveChanges();
                output.Success = true;
                output.materialId = saveData.MATERIAL_ID;
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

        ZAIDM_EX_MATERIAL IMaterialBLL.getByID(long materialId)
        {
            throw new NotImplementedException();
        }

        List<ZAIDM_EX_MATERIAL> IMaterialBLL.getAll()
        {
            throw new NotImplementedException();
        }

        public MaterialOutput Save(ZAIDM_EX_MATERIAL data)
        {
            throw new NotImplementedException();
        }
    }
}
