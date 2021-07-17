using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using MediusApi.Providers;
using Medius.DataAccess.Data;

namespace MediusApi
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            IAppBuilder appBuilder = app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ExternalCookie
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true
            };
            // This is a key step of the solution as we need to supply a meaningful and fully working
            // implementation of the OAuthBearerOptions object when we configure the OAuth Bearer authentication mechanism. 
            // The trick here is to reuse the previously defined OAuthOptions object that already
            // implements almost everything we need
            OAuthBearerOptions =
                new OAuthBearerAuthenticationOptions
                {
                    AccessTokenFormat = OAuthOptions.AccessTokenFormat,
                    AccessTokenProvider = OAuthOptions.AccessTokenProvider,
                    AuthenticationMode = OAuthOptions.AuthenticationMode,
                    AuthenticationType = OAuthOptions.AuthenticationType,
                    Description = OAuthOptions.Description,
                    Provider = new CustomBearerAuthenticationProvider(),
                    SystemClock = OAuthOptions.SystemClock,
                };
            app.UseOAuthAuthorizationServer(OAuthOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);
            app.UseWebApi(new HttpConfiguration());

            // The provider is the only object we need to redefine. See below for the implementation


            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            app.UseFacebookAuthentication(
                appId: "380684719253182",
                appSecret: "ywEXeXE6LEfJrLd3twG1WZ5N");

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "226622787567-742e4oa02f6gcos15odblb721eqivnjv.apps.googleusercontent.com",
                ClientSecret = "bT1D1UkZFI9grX1juN-JAFtl",
                Scope = { "profile" }
            });
        }
    }

        public class CustomBearerAuthenticationProvider : OAuthBearerAuthenticationProvider
        {
            // This validates the identity based on the issuer of the claim.
            // The issuer is set in the API endpoint that logs the user in
            public override Task ValidateIdentity(OAuthValidateIdentityContext context)
            {
                var claims = context.Ticket.Identity.Claims;
                if (claims.Count() == 0 || claims.Any(claim => claim.Issuer != "Facebook" && claim.Issuer != "LOCAL_AUTHORITY"))
                    context.Rejected();
                return Task.FromResult<object>(null);
            }
        }
}