﻿using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface ICompanyBLL
    {
       List<T1001> GetMasterData();

        T1001 GetById(long id);

    }
}
