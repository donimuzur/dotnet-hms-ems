﻿using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;
using System;

namespace Sampoerna.EMS.BLL
{
    public class POAMapBLL : IPOAMapBLL
    {

        
        private IUnitOfWork _uow;
        private IGenericRepository<POA_MAP> _repository;
        private ChangesHistoryBLL _changeBLL;
        private string _includeProperties = "T001W, POA, POA.USER";
        public POAMapBLL(IUnitOfWork uow, ILogger logger)
        {
            
            _uow = uow;
            _repository = _uow.GetGenericRepository<POA_MAP>();
            _changeBLL = new ChangesHistoryBLL(uow, logger);
            
        }

        public List<POA_MAP> GetByNppbckId(string NppbckdId)
        {
            return _uow.GetGenericRepository<POA_MAP>().Get(p => p.NPPBKC_ID == NppbckdId, null, _includeProperties ).ToList();
        }

        public POA_MAP GetById(int Id)
        {
            return _uow.GetGenericRepository<POA_MAP>().Get(p => p.POA_MAP_ID == Id, null, _includeProperties).FirstOrDefault();
        }

        public List<POA_MAP> GetAll()
        {
            return _uow.GetGenericRepository<POA_MAP>().Get(null, null, _includeProperties).ToList();
        }

        public void Save(POA_MAP poaMap)
        {
            var repo = _uow.GetGenericRepository<POA_MAP>();
            repo.InsertOrUpdate(poaMap);
            _uow.SaveChanges();
        }

        public void Delete(int id)
        {
            var repo = _uow.GetGenericRepository<POA_MAP>();
            repo.Delete(id);
            _uow.SaveChanges();
        }

        public POA_MAP GetByNppbckId(string nppbkc, string plant, string poa)
        {
            return _uow.GetGenericRepository<POA_MAP>().Get(p=>p.NPPBKC_ID == nppbkc && p.WERKS == plant && p.POA_ID == poa, null, _includeProperties).FirstOrDefault();
        }
        
        public List<string> GetPlantByPoaId(string id)
        {
            var list = new List<string>();

            var data = _uow.GetGenericRepository<POA_MAP>().Get(p => p.POA_ID == id && p.POA.IS_ACTIVE.Value == true, null, _includeProperties);

            foreach (var item in data)
            {
                list.Add(item.WERKS);
            }

            return list;
        }
        
        public List<string> GetNppbkcByPoaId(string id)
        {
            var list = new List<string>();

            var data = _uow.GetGenericRepository<POA_MAP>().Get(p => p.POA_ID == id && p.POA.IS_ACTIVE.Value == true, null, _includeProperties);

            foreach (var item in data)
            {
                list.Add(item.NPPBKC_ID);
            }

            return list;
        }

        public List<string> GetCompanyByPoaId(string id)
        {
            _includeProperties += ", T001W.T001K";

            var list = new List<string>();

            var data = _uow.GetGenericRepository<POA_MAP>().Get(p => p.POA_ID == id && p.POA.IS_ACTIVE.Value == true, null, _includeProperties);

            var company = string.Empty;

            foreach (var item in data)
            {
                var comp = item.T001W.T001K == null ? string.Empty : item.T001W.T001K.BUKRS;

                if (company != comp)
                {
                    list.Add(comp);

                    company = comp;
                }
            }

            return list;
        }
    }
}
