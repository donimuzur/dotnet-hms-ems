using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.HMS.Scheduler
{
    partial class SchedulerService : ServiceBase
    {
        public SchedulerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
               
                QuartzScheduler.StartJobs();
            }
            catch (Exception ex)
            {
                LogErr(ex.Message);
            }
            
        }

        protected override void OnStop()
        {
            try
            {
                QuartzScheduler.StopJobs();
            }
            catch (Exception ex)
            {
                LogErr(ex.Message);
            }
        }
        private void LogErr(string sMessage)
        {
            if (!System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "logs"))
                System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "logs");

            using (
                System.IO.StreamWriter file =
                    new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory +
                                               @"logs\FieldIQ-log.txt"))
            {
                file.WriteLine("Test...." + sMessage);
            }
        }
    }
}
