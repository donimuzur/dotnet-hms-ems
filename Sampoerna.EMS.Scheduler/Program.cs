using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.HMS.Scheduler
{
    class Program
    {
        static void Main(string[] args)
      {
          //ServiceBase[] ServicesToRun;
          //ServicesToRun = new ServiceBase[] 
          //  { 
          //      new SchedulerService(),  
          //  };
          //ServiceBase.Run(ServicesToRun);
          if (System.Diagnostics.Process.GetProcessesByName
              (System.IO.Path.GetFileNameWithoutExtension
              (System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1) return;
          QuartzScheduler.StartJobs();
         
           
        }
    }
}
