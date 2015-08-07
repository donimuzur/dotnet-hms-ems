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

        private string StringErrorList(List<string> errorList)
        {
            string result = string.Empty;
            result += "<p>There are error of these files below :</p>";
            foreach (var error in errorList)
            {
                if (error.Contains("String was not recognized as a valid DateTime"))
                {
                    result += "<p>" + error + ", valid format datetime is yyyy-MM-dd</p>";
                    continue;
                    
                }
                result += "<p>"+error+"</p>";
            }
            return result;
        }

        public void Execute(IJobExecutionContext context)
        {
            using (_container.BeginLifetimeScope())
            {

                var config = EmailConfiguration.GetConfig();
                var loggerFactory = _container.GetInstance<ILoggerFactory>();
                ILogger logger = loggerFactory.GetLogger("Scheduler");
                var errorList = new List<string>();
                try
                {
                     
                    logger.Info("Reading XML start on " + DateTime.Now);
                    Service svc = new Service();
                    errorList.AddRange(svc.Run());
                    logger.Info("Reading XML ended On " + DateTime.Now);
                    
                }
                catch (Exception ex)
                {
                    
                    logger.Error("Reading XML crashed", ex);
                }
                if (errorList.Count > 0)
                {
                    foreach (var err in errorList)
                    {
                        
                        logger.Info(err);
                    }
                    logger.Error(EmailUtility.Email(StringErrorList(errorList), null));
           
                }

            }
        }
    }
}
