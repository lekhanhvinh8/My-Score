using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using MyScore.Core.Domain;
using MyScore.Persistence;
using Owin;

namespace MyScore.App_Start
{
    public class IdentityConfig
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext(() => new ApplicationDbContext());
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            app.CreatePerOwinContext<RoleManager<Role>>((options, context) =>
                new RoleManager<Role>(
                    new RoleStore<Role>(context.Get<ApplicationDbContext>())));
            app.CreatePerOwinContext<SignInManager<User, string>>((options, context) =>
                new SignInManager<User, string>(
                    context.GetUserManager<AppUserManager>(), context.Authentication));

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Home/Login"),
            });
        }
    }
}