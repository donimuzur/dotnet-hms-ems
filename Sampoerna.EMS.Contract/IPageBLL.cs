﻿using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;

namespace Sampoerna.EMS.Contract
{
    public interface IPageBLL
    {
        PAGE GetPageByID(int id);
        List<PAGE> GetPages();
    }
}
