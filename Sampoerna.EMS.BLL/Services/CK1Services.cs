using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
   public class CK1Services : ICK1Services
    {

       private IGenericRepository<CK1> _repository;
       
       private ILogger _logger;
       private IUnitOfWork _uow;

        private string includeTables = "";

        public CK1Services(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _repository = _uow.GetGenericRepository<CK1>();
            
        }

       public List<CK1> GetCk1ByNppbkc(string nppbkcId)
       {
           return _repository.Get(c => c.NPPBKC_ID == nppbkcId, null, "CK1_ITEM").ToList();
       }

       public CK1 GetCk1ByCk1Number(string ck1Number)
       {
           var dtData = _repository.Get(c => c.CK1_NUMBER == ck1Number, null, includeTables).FirstOrDefault();
           
           return dtData;

       }

       public CK1 GetCk1ById(long ck1Id)
       {
           return _repository.GetByID(ck1Id);
       }

       public List<CK1> GetCk1ByPlant(string plant)
       {
        
           return _repository.Get(c => c.PLANT_ID == plant, null, "CK1_ITEM").ToList();

         
       }

       public List<CK1_ITEM> GetCk1ItemByNppbkc(string nppbkcId)
       {
           var ck1List = GetCk1ByNppbkc(nppbkcId);

           
           var ck1AllowedMaterial = new List<CK1_ITEM>();
           foreach (var ck1 in ck1List)
           {
               ck1AllowedMaterial.AddRange(ck1.CK1_ITEM);
           }

           return ck1AllowedMaterial.GroupBy(x => new {x.MATERIAL_ID, x.WERKS,x.FA_CODE}).Select(x=> new CK1_ITEM()
           {
               FA_CODE = x.Key.FA_CODE,
               MATERIAL_ID = x.Key.MATERIAL_ID,
               WERKS = x.Key.WERKS
           }).ToList();
       }
    }
}
