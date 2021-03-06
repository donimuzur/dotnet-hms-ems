﻿using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;

namespace Sampoerna.EMS.Contract
{
    public interface ICK4CItemBLL
    {
        void DeleteByCk4cId(long ck4cId);

        List<Ck4cItem> GetDataByPlantAndFacode(string plant, string facode, string levelPlant);

        List<Ck4cItem> GetDataByParentPlant(string levelPlant);

        List<CK4C_ITEM> GetAllCk4cItems();
    }
}