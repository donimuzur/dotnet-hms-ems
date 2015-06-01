using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sampoerna.EMS.Website.Startup))]
namespace Sampoerna.EMS.Website
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
