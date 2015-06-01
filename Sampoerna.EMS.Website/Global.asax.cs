using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Sampoerna.EMS.Contract;
using Voxteneo.WebCompoments.NLogLogger;
using Voxteneo.WebComponents.Logger;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using Sampoerna.EMS.BLL;
using Sampoerna.EMS.DAL;
namespace Sampoerna.EMS.Website
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            var container = new Container();
            // register unit of work / context by request
            // http://simpleinjector.codeplex.com/wikipage?title=ObjectLifestyleManagement#PerThread
            var webLifestyle = new WebRequestLifestyle();

            container.Register<IUnitOfWork, SqlUnitOfWork>(webLifestyle);
            container.Register<ILogger, NLogLogger>();
            container.Register<IEmployeeBLL, EmployeeBLL>();
            container.Register<ICompanyBLL, CompanyBLL>();
           
            container.Verify();
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));

        }
    }
}
