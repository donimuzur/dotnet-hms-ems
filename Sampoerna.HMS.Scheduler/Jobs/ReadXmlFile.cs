using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using SimpleInjector;
using Voxteneo.WebComponents.Logger;

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
                //_CreateSQLiteService = _container.GetInstance<IGenerateSQLiteService>();

                //var generate = _CreateSQLiteService.GetGenerateFlag();
                //if (!generate.Generate) return;
                
                var loggerFactory = _container.GetInstance<ILoggerFactory>();
                ILogger logger = loggerFactory.GetLogger("Scheduler.ReadXmlFile");

                try
                {
                    logger.Info("Reading XML start");
                    //_CreateSQLiteService.CreateSQLite();
                    
                    logger.Info("Reading XML ended");
                }
                catch (Exception ex)
                {
                    logger.Error("Reading XML crashed", ex);
                }
              }
        }
    }
}
