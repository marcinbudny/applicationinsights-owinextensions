using System;
using System.Web.Http;
using ApplicationInsights.OwinExtensions;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(TestWebsite.Startup))]

namespace TestWebsite
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            appBuilder.UseApplicationInsights();

            ConfigureAuth(appBuilder);

            appBuilder.RestoreOperationIdContext();

            ConfigureWebApi(appBuilder);
        }

        private static void ConfigureAuth(IAppBuilder appBuilder)
        {
            var options = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new OAuthAuthorizationServerProvider(),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true
            };

            appBuilder.UseOAuthAuthorizationServer(options);

            appBuilder.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }

        private static void ConfigureWebApi(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            appBuilder.UseWebApi(config);
        }
    }

}