using System.Web.Mvc;
using System.Web.Routing;

namespace Sampoerna.EMS.Website
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute("GetPOAByNppbkcId",
                            "poa/getpoabynppbkcid/",
                            new { controller = "Address", action = "GetPOAByNppbkcId" },
                            new[] { "Sampoerna.EMS.Website.Controllers" });

            routes.MapRoute("GetMapping",
                            "WorkflowSettings/GetMapping/",
                            new { controller = "Address", action = "GetMapping" },
                            new[] { "Sampoerna.EMS.Website.Controllers" });
        }
    }
}
