﻿using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;

namespace Sampoerna.EMS.Contract
{
    public interface IPageBLL
    {
        PAGE GetPageByID(int id);
        List<PAGE> GetPages();

        List<PAGE> GetModulePages();

        void Save(PAGE_MAP pageMap);

        void DeletePageMap(int id);

        List<PAGE> GetParentPages();

        List<int?> GetAuthPages(Login user);


    }
}
