using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Sampoerna.EMS.Core;
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
                    Console.WriteLine("start...");
                    logger.Info("Reading XML start on " + DateTime.Now);
                    Service svc = new Service();
                    Console.WriteLine("POA Running...");
                    svc.PoaRunning();
                    Console.WriteLine("POA Map Running...");
                    svc.PoaMapRunning();
                    Console.WriteLine("Company Running...");
                    svc.CompanyRunning();
                    Console.WriteLine("KPPBC Running...");
                    svc.KPPBCRunning();
                    Console.WriteLine("NPPBCK Running...");
                    svc.NPPBKCRunning();
                    Console.WriteLine("Vendor Running...");
                    svc.VendorRunning();
                    Console.WriteLine("PCode Running...");
                    svc.PCodeRunning();
                    Console.WriteLine("Plant Running...");
                    svc.PlantRunning();
                    Console.WriteLine("Market Running...");
                    svc.MarketRunning();
                    Console.WriteLine("GoodType Running...");
                   
                    svc.GoodTypeRunning();
                    Console.WriteLine("UoM Running...");
                   
                    svc.UoMRunning();
                    Console.WriteLine("ProdType Running...");
                    svc.ProdTypeRunning();
                    Console.WriteLine("Series Running...");
                   
                    svc.SeriesRunning();
                    Console.WriteLine("Brand Running...");
                   
                    svc.BrandRunning();
                    Console.WriteLine("Material Running...");
                   
                    svc.MaterialRunning();
                    logger.Info("Reading XML ended On " + DateTime.Now);
                }
                catch (Exception ex)
                {
                    EmailUtility.Email("mugia@voxteneo.asia", ex.Message, "Test Error Email", "EMSScheduler@gmail.com", "EMS Scheduler", "adnan@voxteneo.asia", "Adnan_989", null);
           
                    logger.Error("Reading XML crashed", ex);
                }
              }
        }
    }
}
