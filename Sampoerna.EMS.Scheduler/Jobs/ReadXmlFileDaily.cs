using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Sampoerna.EMS.BusinessObject.Outputs;
using Sampoerna.EMS.Core;
using SimpleInjector;
using Voxteneo.WebComponents.Logger;
using Sampoerna.EMS.XMLReader;
using Sampoerna.EMS.Contract;
using Container = SimpleInjector.Container;

namespace Sampoerna.HMS.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    public class ReadXmlFileDaily : IJob
    {
        private readonly Container _container;
        private Service _svc = null;
        public ReadXmlFileDaily(Container container)
        {
            _container = container;
            _svc = new Service();


        }

        private string StringErrorList(List<string> errorList)
        {
            string result = string.Empty;
            result += "<p>There are error of these files below :</p>";
            foreach (var error in errorList)
            {
                if (error.ToLower().Contains("string was not recognized as a valid datetime"))
                {
                    result += "<p>" + error + ", valid format datetime is yyyy-MM-dd</p>";
                    continue;

                }
                result += String.Format("<p>[{0}] {1}</p>", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), error);
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
                var errorFileList = new List<MovedFileOutput>();
                try
                {

                    logger.Info("Reading XML Daily start on " + DateTime.Now);
                    errorFileList.AddRange(_svc.Run(SchedulerEnums.Schedule.Daily));
                    foreach (var errorFile in errorFileList)
                    {
                        logger.Warn(String.Format("Error on files : {0}", errorFile.FileName));
                        var body = String.Format("Error on files : {0}", errorFile.FileName);
                        
                        
                        
                        foreach (var error in errorFile.ErrorList)
                        {
                            logger.Info(String.Format(error));
                            body += String.Format("<p>{0}</p>", error);
                        }

                        logger.Error(EmailUtility.Email(body, null));
                    }
                    logger.Info("Reading XML Daily ended On " + DateTime.Now);

                }
                catch (Exception ex)
                {
                    logger.Error("Reading XML crashed", ex);
                    var body = String.Format("<p>{0}</p>",ex.Message);
                    body += String.Format("<p>{0}</p>", ex.StackTrace);

                    if (!String.IsNullOrEmpty(body))
                        logger.Error(EmailUtility.Email(body,"Scheduler is down"));
                    
                }
                //if (errorList.Count > 0)
                //{
                //    foreach (var err in errorList)
                //    {
                        
                //        logger.Info(err);
                //    }
                    

                //}
                if (errorFileList.Count == 0)
                {
                    var body = string.Empty;
                    if (_svc.filesMoved.Count > 0)
                    {
                        foreach (var file in _svc.filesMoved)
                        {
                            string info = String.Format("<p>XML file {0} : {1}</p>"
                                ,file.IsError? "Error" : "Archieved"
                                ,file.FileName);
                            if (file.IsError)
                            {
                                body += info;
                            }
                                
                            logger.Info(info);
                        }
                        if(!String.IsNullOrEmpty(body))
                            logger.Error(EmailUtility.Email(body));
                    }


                }

            }
        }
    }
}
