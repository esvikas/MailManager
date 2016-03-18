using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MailManager.Startup))]
namespace MailManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
