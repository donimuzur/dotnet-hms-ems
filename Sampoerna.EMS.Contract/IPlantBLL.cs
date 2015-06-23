﻿using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface IPlantBLL
    {
        T1001W GetId(long id);
        
        List<T1001W> GetAll();
        
    }
}