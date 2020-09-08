using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Esso.Web.App_Start.Startup))]
namespace Esso.Web.App_Start
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
