﻿using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface IZaidmExGoodTypeBLL
    {
        ZAIDM_EX_GOODTYP GetById(string id);

        List<ZAIDM_EX_GOODTYP> GetAll();

        string GetGoodTypeDescById(string id);
    }
}