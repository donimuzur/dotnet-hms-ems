﻿using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface IMonthBLL
    {
        MONTH GetMonth(int id);
        List<MONTH> GetAll();

    }
}
