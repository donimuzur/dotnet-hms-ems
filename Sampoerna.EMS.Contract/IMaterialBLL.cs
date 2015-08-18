using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Outputs;
using System.Collections.Generic;

namespace Sampoerna.EMS.Contract
{
    public interface IMaterialBLL
    {
        MaterialDto getByID(string materialnumber, string plant);

        List<MaterialDto> getAll();
        List<string> getStickerCode();

        List<T001W> getAllPlant(string materialnumber);

        MaterialOutput Save(MaterialDto data, string userId);

        void SaveUoM(MATERIAL_UOM data,string userid);

        void Delete(string materialnumber, string plant, string userId);

        int DeleteMaterialUom(int id, string userId, string materialnumber, string plant);

        List<ZAIDM_EX_MATERIAL> GetByFlagDeletion(bool? isDelete, string plant = "");

    }
}
