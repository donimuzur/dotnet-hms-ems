using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Spi;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.DAL;
using SimpleInjector;
using Voxteneo.WebCompoments.NLogLogger;
using Voxteneo.WebComponents.Logger;


namespace Sampoerna.HMS.Scheduler
{
    public static class QuartzScheduler
    {
        private static Container _quartzContainer { get; set; }

        private static void Initialize()
        {
            var container = new Container();
            //Solution injections
           
            container.Register<ILogger, NLogLogger>();
            container.Register<ILoggerFactory, NLogLoggerFactory>();
            container.RegisterLifetimeScope<IUnitOfWork, SqlUnitOfWork>();

             
            container.EnableLifetimeScoping();
            container.Verify();

            var schedulerFactory = new StdSchedulerFactory();
            _quartzContainer = new Container();
            _quartzContainer.RegisterSingle<ILogger, NLogLogger>();
            _quartzContainer.RegisterSingle<ILoggerFactory, NLogLoggerFactory>();
            _quartzContainer.RegisterSingle<IJobFactory>(() => new SimpleInjectorJobFactory(container));
            _quartzContainer.RegisterSingle<ISchedulerFactory>(schedulerFactory);
            
            _quartzContainer.Register<IScheduler>(() =>
            {
                var scheduler = schedulerFactory.GetScheduler();
                scheduler.JobFactory = _quartzContainer.GetInstance<IJobFactory>();
                return scheduler;
            }
            );
            _quartzContainer.Verify();
            
        }
        public static void StartJobs()
        {
            Initialize();
            var loggerFactory = _quartzContainer.GetInstance<ILoggerFactory>();
            var logger = loggerFactory.GetLogger("Scheduler");

            try
            {
                //Ask the scheduler factory for a scheduler
                IScheduler scheduler = _quartzContainer.GetInstance<IScheduler>();
                scheduler.Start();

                logger.Info("Starting scheduler, job listed : ");
                IList<string> jobGroups = scheduler.GetJobGroupNames();
                logger.Info("groups: " + string.Join(" - ", jobGroups.ToArray()));
                IList<string> triggerGroups = scheduler.GetTriggerGroupNames();

                foreach (string group in jobGroups)
                {
                    
                    var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
                    var jobKeys = scheduler.GetJobKeys(groupMatcher);
                    foreach (var jobKey in jobKeys)
                    {
                        var detail = scheduler.GetJobDetail(jobKey);
                        var triggers = scheduler.GetTriggersOfJob(jobKey);
                        foreach (ITrigger trigger in triggers)
                        {
                            logger.Info("group: " + group);
                            logger.Info("jobkey name: " + jobKey.Name);
                            logger.Info("detail description: " + detail.Description);
                            logger.Info("trigger key name: " + trigger.Key.Name);
                            logger.Info("trigger key group: " + trigger.Key.Group);
                            logger.Info("trigger type name: " + trigger.GetType().Name);
                            logger.Info("trigger state: " + scheduler.GetTriggerState(trigger.Key).ToString());
                            DateTimeOffset? nextFireTime = trigger.GetNextFireTimeUtc();
                            if (nextFireTime.HasValue)
                            {
                                logger.Info("nextFireTime : " + nextFireTime.Value.LocalDateTime.ToString());
                            }

                            DateTimeOffset? previousFireTime = trigger.GetPreviousFireTimeUtc();
                            if (previousFireTime.HasValue)
                            {
                                logger.Info("previousFireTime : " + previousFireTime.Value.LocalDateTime.ToString());
                            }
                        }
                    }
                }
                
              
            }
            catch (Exception ex)
            {
                logger.Error("first" +ex);
            }
        }
        public static void StopJobs()
        {
            ILoggerFactory loggerFactory = _quartzContainer.GetInstance<ILoggerFactory>();
            ILogger logger = loggerFactory.GetLogger("Scheduler");

            try
            {
                //Ask the scheduler factory for a scheduler
                IScheduler scheduler = _quartzContainer.GetInstance<IScheduler>();

                logger.Info("Stopping jobs...");
                scheduler.Shutdown(true);
                logger.Info("Jobs stopped...");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }

}
