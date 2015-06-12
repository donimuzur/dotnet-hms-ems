using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Website.Code;
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

        private static Container _container;
        public static TService GetInstance<TService>()
        where TService : class
     
        {
            return _container.GetInstance<TService>();
        }

        private static void Bootstrap()
        {

            //initialize mappers
            EMSWebsiteMapper.Initialize();
            BLLMapper.Initialize();

            // 1. Create a new Simple Injector container
            var container = new Container();

            // register unit of work / context by request
            // http://simpleinjector.codeplex.com/wikipage?title=ObjectLifestyleManagement#PerThread
            var webLifestyle = new WebRequestLifestyle();

            container.Register<IUnitOfWork, SqlUnitOfWork>(webLifestyle);
            container.Register<ILogger, NLogLogger>();
            container.Register<ICompanyBLL, CompanyBLL>();
            container.Register<IWorkflowBLL, WorkflowBLL>();
            container.Register<IUserBLL, UserBLL>();
            container.Register<IFormsAuthenticationService, FormsAuthenticationService>();
            container.Register<IPageBLL, PageBLL>();
            container.Register<IPBCK1BLL, PBCK1BLL>();
            container.Register<IZaidmExPOABLL, ZaidmExPOABLL>();
            container.Register<IZaidmExPOAMapBLL, ZaidmExPOAMapBLL>();
            container.Register<IZaidmExNPPBKCBLL, ZaidmExNPPBKCBLL>();

            // 3. Optionally verify the container's configuration.
            container.Verify();

            // 4. Store the container for use by Page classes.
            _container = container;

        }
        
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
           
            Bootstrap();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(_container));

        }
    }
}
