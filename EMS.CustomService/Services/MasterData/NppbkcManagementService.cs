using Sampoerna.EMS.CustomService.Repositories;
using Sampoerna.EMS.CustomService.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sampoerna.EMS.CustomService.Services.MasterData
{
    public class NppbkcManagementService : GenericService
    {
        private SystemReferenceService refService;

        public NppbkcManagementService()
        {
            this.refService = new SystemReferenceService();
        }

        public MASTER_NPPBKC Find(string id)
        {
            try
            {
                return this.uow.NppbkcRepository.Find(id);
            }
            catch (Exception ex)
            {
                throw this.HandleException("Exception occured on NppbkcManagementService. See Inner Exception property to see details", ex);
            }
        }

        public MASTER_NPPBKC Save(MASTER_NPPBKC data, string user, IEnumerable<CHANGES_HISTORY> logs, bool delete = false)
        {
            using (var context = new EMSDataModel())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var now = DateTime.Now;
                        var old = context.ZAIDM_EX_NPPBKC.Find(data.NPPBKC_ID);
                        if (old == null) // insert
                        {
                            data.CREATED_DATE = now;
                            data.CREATED_BY = user;
                            //data.IS_DELETED = false;

                            context.ZAIDM_EX_KPPBC.Add(new MASTER_KPPBC()
                            {
                                KPPBC_ID = data.KPPBC_ID,
                                CREATED_DATE = now,
                                CREATED_BY = user
                            });

                            context.ZAIDM_EX_NPPBKC.Add(data);
                        }
                        else
                        {
                            data.CREATED_DATE = old.CREATED_DATE;
                            data.CREATED_BY = old.CREATED_BY;
                            data.MODIFIED_DATE = now;
                            data.MODIFIED_BY = user;
                            //data.IS_DELETED = false;
                            context.Entry(old).CurrentValues.SetValues(data);
                        }
                        foreach (var log in logs)
                        {
                            context.CHANGES_HISTORY.Add(log);
                        }
                        context.SaveChanges();

                        transaction.Commit();
                        return data;
                    }
                    catch (Exception ex)
                    {
                        throw this.HandleException("Exception occured on NppbkcManagementService. See Inner Exception property to see details", ex);
                    }
                }
                
            }
            
        }
    }
}
