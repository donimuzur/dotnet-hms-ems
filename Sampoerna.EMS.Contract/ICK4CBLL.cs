﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
    public interface ICK4CBLL
    {
        List<Ck4CDto> GetAllByParam(Ck4CGetByParamInput input);

        List<Ck4CDto> GetAll();

        Ck4CDto Save(Ck4CDto item);

        Ck4CDto GetById(long id);

        void Ck4cWorkflow(Ck4cWorkflowDocumentInput input);
    }
}
