﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Contract.Services;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL.Services
{
    public class ZaidmExNppbkcService : IZaidmExNppbkcService
    {
        private IGenericRepository<POA_MAP> _poaMapRepository;
        private IGenericRepository<ZAIDM_EX_NPPBKC> _nppbkcRepository;
        
        private ILogger _logger;
        private IUnitOfWork _uow;

        public ZaidmExNppbkcService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _poaMapRepository = _uow.GetGenericRepository<POA_MAP>();
            _nppbkcRepository = _uow.GetGenericRepository<ZAIDM_EX_NPPBKC>();
        }

        public List<ZAIDM_EX_NPPBKC> GetNppbkcsByPoa(string poaId)
        {
            //query by nppbkc, main plant and active poa
            Expression<Func<POA_MAP, bool>> queryFilter = c => c.POA_ID == poaId
                && c.POA.IS_ACTIVE.HasValue && c.POA.IS_ACTIVE.Value;

            var dbData = _poaMapRepository.Get(queryFilter, null, "ZAIDM_EX_NPPBKC");
            var poaList = dbData.ToList().Select(d => d.ZAIDM_EX_NPPBKC);
            return poaList.ToList();
        }
        public List<ZAIDM_EX_NPPBKC> GetNppbkcMainPlantOnlyByPoa(string poaId)
        {
            //query by nppbkc, main plant and active poa
            Expression<Func<POA_MAP, bool>> queryFilter = c => c.POA_ID == poaId
                && c.T001W.IS_MAIN_PLANT.HasValue && c.T001W.IS_MAIN_PLANT.Value
                && c.POA.IS_ACTIVE.HasValue && c.POA.IS_ACTIVE.Value;

            var dbData = _poaMapRepository.Get(queryFilter, null, "ZAIDM_EX_NPPBKC");
            var poaList = dbData.ToList().Select(d => d.ZAIDM_EX_NPPBKC);
            return poaList.ToList();
        }

        public ZAIDM_EX_NPPBKC GetById(string nppbkcId)
        {
            return _nppbkcRepository.GetByID(nppbkcId);
        }


        public List<ZAIDM_EX_NPPBKCCompositeDto> GetNppbkcList(List<string> nppbkcList)
        {
            Expression<Func<ZAIDM_EX_NPPBKC, bool>> queryFilter = c => nppbkcList.Contains(c.NPPBKC_ID);
            var data = _nppbkcRepository.Get(queryFilter);

            return Mapper.Map<List<ZAIDM_EX_NPPBKCCompositeDto>>(data);
        }
    }
}
