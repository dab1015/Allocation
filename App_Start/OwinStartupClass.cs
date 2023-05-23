using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(SNRWMSPortal.App_Start.OwinStartupClass))]
namespace SNRWMSPortal.App_Start
{
    public class OwinStartupClass
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            OAuthAuthorizationServerOptions oAuthAuthorizationServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = false,
                TokenEndpointPath = new PathString("/Authenticate"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                Provider = new MyOAuthAuthorizationServerProvider()
            };

            app.UseOAuthAuthorizationServer(oAuthAuthorizationServerOptions);

            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            CookieAuthenticationOptions cookieAuthenticationOptions = new CookieAuthenticationOptions()
            {
                LoginPath = new PathString("/Account/Login"),
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                ExpireTimeSpan = TimeSpan.FromMinutes(30),
            };

            app.UseCookieAuthentication(cookieAuthenticationOptions);

        }

        class MyOAuthAuthorizationServerProvider : OAuthAuthorizationServerProvider
        {
            public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
            {
                return base.GrantResourceOwnerCredentials(context);
            }

            public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
            {
                return base.ValidateClientAuthentication(context);
            }
        }



    }

}