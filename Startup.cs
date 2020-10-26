using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(APP.Startup))]
namespace APP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
