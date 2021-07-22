using Medius.DataAccess.Data;
using Medius.DataAccess.Repository;
using Medius.DataAccess.Repository.IRepository;
using Medius.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Medius
{
    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddCors(options => options.AddDefaultPolicy(
                builder => builder.AllowAnyOrigin()));

            services
         .AddAuthentication(options =>
         {
             options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
         })
         .AddCookie(options =>
         {
            // Change the options as needed
        });
            //db connection
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));


            //     services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //.AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<EmailOptions>(Configuration);
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            services.Configure<BrainTreeSettings>(Configuration.GetSection("BrainTree"));
            services.Configure<TwilioSettings>(Configuration.GetSection("Twilio"));
            services.AddSingleton<IBrainTreeGate, BrainTreeGate>();
            //repositories connection

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddRazorPages();
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Medius", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Medius v1"));
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
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

            // The provider is the only object we need to redefine. See below for the implementation


            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");


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
}
