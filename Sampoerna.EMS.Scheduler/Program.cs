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
          QuartzScheduler.StartJobs();
         
           
        }
    }
}
