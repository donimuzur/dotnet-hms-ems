using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Quartz.Spi;
using SimpleInjector;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.HMS.Scheduler
{
    public class SimpleInjectorJobFactory : IJobFactory
    {
        private readonly Container _container;
        //This classk is part of your Composition Root
        public class LifetimeScopeJobDecorator : IJob
        {
            private readonly IJob _decoratee;
            private readonly Container _container;

            public LifetimeScopeJobDecorator(IJob decoratee, Container container)
            {
                _decoratee = decoratee;
                _container = container;
            }

            /// <summary>
            /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.ITrigger" />
            /// fires that is associated with the <see cref="T:Quartz.IJob" />.
            /// </summary>
            /// <param name="context">The execution context.</param>
            /// <remarks>
            /// The implementation may wish to set a  result object on the
            /// JobExecutionContext before this method exits.  The result itself
            /// is meaningless to Quartz, but may be informative to
            /// <see cref="T:Quartz.IJobListener" />s or
            /// <see cref="T:Quartz.ITriggerListener" />s that are watching the job's
            /// execution.
            /// </remarks>
            public void Execute(IJobExecutionContext context)
            {
                ILoggerFactory loggerFactory = _container.GetInstance<ILoggerFactory>();
                ILogger logger = loggerFactory.GetLogger("Scheduler.LifetimeScopeJobDecorator");

                logger.Debug("LifetimeScopeJobDecorator - Execute " + Thread.CurrentThread.Name);
                try
                {
                    using (_container.BeginLifetimeScope())
                    {
                        _decoratee.Execute(context);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("LifetimeScopeJobDecorator - Execute", ex);
                }
            }
        }
        public SimpleInjectorJobFactory(Container container)
        {
            _container = container;
            _container.Verify();
        }
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            ILoggerFactory loggerFactory = _container.GetInstance<ILoggerFactory>();
            ILogger logger = loggerFactory.GetLogger("Scheduler.SimpleInjectorJobFactory");

            try
            {
                IJobDetail jobDetail = bundle.JobDetail;
                Type jobType = jobDetail.JobType;
                logger.Debug(string.Format("Producing Instance of job: {0}, class: {1}", jobDetail.Key, jobType.FullName));
                var job = (IJob)_container.GetInstance(jobType);
                return new LifetimeScopeJobDecorator(job, _container);
            }
            catch (Exception ex)
            {
                logger.Error("Problem instantiating class", ex);
                throw new SchedulerException("Problem instantiating class", ex);
            }
        }
        public void ReturnJob(IJob job)
        {
        }
    }

}
