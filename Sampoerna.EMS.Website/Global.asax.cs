using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Sampoerna.EMS.BusinessObject;
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
            container.Register<ICK4CBLL, CK4CBLL>();
            container.Register<IZaidmExPOAMapBLL, ZaidmExPOAMapBLL>();
            container.Register<IVirtualMappingPlantBLL, VirtualMappingPlantBLL>();
            container.Register<IMasterDataBLL, MasterDataBLL>();
            container.Register<IZaidmExNPPBKCBLL, ZaidmExNPPBKCBLL>();
            container.Register<IPlantBLL, PlantBLL>();
            container.Register<IZaidmExGoodTypeBLL, ZaidmExGoodTypeBLL>();
            container.Register<IBrandRegistrationBLL, BrandRegistrationBLL>();
            container.Register<ICK5BLL, CK5BLL>();
            container.Register<IZaidmExProdTypeBLL, ZaidmExProdTypeBLL>();
            container.Register<IMonthBLL, MonthBLL>();
            container.Register<IDocumentSequenceNumberBLL, DocumentSequenceNumberBLL>();
            container.Register<IHeaderFooterBLL, HeaderFooterBLL>();
            container.Register<IExGroupTypeBLL, ExGroupTypeBLL>();
            container.Register<IZaidmExKPPBCBLL, ZaidmExKPPBCBLL>();
            container.Register<IChangesHistoryBLL, ChangesHistoryBLL>();
            container.Register<IMaterialBLL, MaterialBLL>();
            container.Register<IWorkflowHistoryBLL,WorkflowHistoryBLL>();
            container.Register <IUnitOfMeasurementBLL, UnitOfMeasurementBLL>();
            container.Register<IPOASKBLL, POASKBLL>();
             container.Register<IPOABLL, POABLL>();
            container.Register<IWorkflowSettingBLL, WorkflowSettingBLL>();
            container.Register<IEmailTemplateBLL, EmailTemplateBLL>();

            container.Register<IPbck1DecreeDocBLL, Pbck1DecreeDocBLL>();
            container.Register<IPbck1ProdPlanBLL, Pbck1ProdPlanBLL>();
            container.Register<ILACK1BLL, LACK1BLL>();
            container.Register<ILACK2BLL, LACK2BLL>();
            container.Register<IPrintHistoryBLL, PrintHistoryBLL>();
            container.Register<IUserAuthorizationBLL, UserAuthorizationBLL>();
            container.Register<IPOAMapBLL, POAMapBLL>();
            container.Register<ILFA1BLL, LFA1BLL>();
            container.Register<IT001KBLL, T001KBLL>();
            container.Register<IUserPlantMapBLL, UserPlantMapBLL>();
            container.Register<ICountryBLL, CountryBLL>();
            container.Register<ISupplierPortBLL, SupplierPortBLL>();
            container.Register<IPBCK7And3BLL,PBCK7AndPBCK3BLL>();
            container.Register<IBACK1BLL, BACK1BLL>();
            container.Register<IProductionBLL, ProductionBLL>();
            container.Register<IPBCK4BLL, PBCK4BLL>();
            container.Register<ICK1BLL, CK1BLL>();
            container.Register<IWasteBLL, WasteBLL>();
            container.Register<IBlockStockBLL, BlockStockBLL>();

            // 3. Optionally verify the container's configuration.
            container.Verify();

            // 4. Store the container for use by Page classes.
            _container = container;

        }
        
        protected void Application_Start()
        {
            //SqlServerTypes.Utilities.LoadNativeAssemblies(Server.MapPath("~/bin"));
            //DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
           
            Bootstrap();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(_container));
            
        }
    }
}
