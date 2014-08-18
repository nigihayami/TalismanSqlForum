using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TalismanSqlForum.Startup))]
namespace TalismanSqlForum
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
