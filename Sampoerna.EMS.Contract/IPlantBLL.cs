﻿using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Contract
{
    public interface IPlantBLL
    {
        T001W GetT001W(string NppbkcId, bool? IsPlant);
        Plant GetId(string id);

        List<Plant> GetAll();

        List<T001W> GetAllPlant();
        void save(Plant plantT1001W,string userid);

        string GetPlantWerksById(string id);

        string GetPlantNameById(long id);
        
        List<PLANT_RECEIVE_MATERIAL> GetReceiveMaterials(string plantId);

        List<T001W> Get(string nppbkcId);
        
        T001WDto GetT001ById(string id);
    }
}