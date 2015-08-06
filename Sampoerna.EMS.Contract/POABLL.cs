﻿using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Outputs;

namespace Sampoerna.EMS.Contract
{
    public interface IPOABLL
    {
        POA GetById(string id);
        List<POA> GetAll();

        void Save(POA poa);

        void Update(POA poa);
        void Delete(string id);
        Core.Enums.UserRole GetUserRole(string userId);

        string GetManagerIdByPoaId(string poaId);
        POADto GetDetailsById(string id);
    }
}