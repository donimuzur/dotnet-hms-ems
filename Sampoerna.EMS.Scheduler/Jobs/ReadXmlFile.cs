using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using SimpleInjector;
using Voxteneo.WebComponents.Logger;
using Sampoerna.EMS.XMLReader;
using Sampoerna.EMS.Contract;
namespace Sampoerna.HMS.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    public class ReadXmlFile : IJob
    {
        private readonly Container _container;

        public ReadXmlFile(Container container)
        {
            _container = container;
        }
        public void Execute(IJobExecutionContext context)
        {
            using (_container.BeginLifetimeScope())
            {
                
                
                var loggerFactory = _container.GetInstance<ILoggerFactory>();
                ILogger logger = loggerFactory.GetLogger("Scheduler");

                try
                {
                    logger.Info("Reading XML start on " + DateTime.Now);
                    Service svc = new Service();
                    svc.PoaRunning();
                    svc.PoaMapRunning();
                    svc.CompanyRunning();
                    svc.KPPBCRunning();
                    svc.NPPBKCRunning();
                    svc.VendorRunning();
                    svc.PCodeRunning();
                    svc.PlantRunning();
                    svc.MarketRunning();
                    svc.GoodTypeRunning();
                    svc.UoMRunning();
                    svc.ProdTypeRunning();
                    svc.SeriesRunning();
                    svc.BrandRunning();
                    svc.MaterialRunning();
                    logger.Info("Reading XML ended On " + DateTime.Now);
                }
                catch (Exception ex)
                {
                    logger.Error("Reading XML crashed", ex);
                }
              }
        }
    }
}
