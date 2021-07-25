using Medius.DataAccess.Data;
using Medius.DataAccess.Repository;
using Medius.DataAccess.Repository.IRepository;
using Medius.Middleware;
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
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using System.Linq;
using Castle.Core.Internal;

namespace Medius
{
    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // add services to the DI container
            //db connection
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            //cors 
            services.AddCors();

            //controller
            services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);

            //services.AddCors(options => options.AddDefaultPolicy(
            //    builder => builder.AllowAnyOrigin()));

            //automapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // configure DI for application services
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            services.AddScoped<EmailSender>();

            //services.AddTransient<IClaimsTransformation, ClaimsTransformer>();
            services.AddAuthentication(IISServerDefaults.AuthenticationScheme);
            //services.AddTransient<IClaimsTransformation, ClaimsTransformer>();
            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });

            services.Configure<EmailOptions>(Configuration);
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            services.Configure<BrainTreeSettings>(Configuration.GetSection("BrainTree"));
            services.AddSingleton<IBrainTreeGate, BrainTreeGate>();
            //Twilio Account
            //var twilioSection =
            //    Configuration.GetSection("Twilio");
            //..
            //services.AddDataProtection()
            //    .PersistKeysToFileSystem(new System.IO.DirectoryInfo(@"./"))
                /*.ProtectKeysWithCertificate(GetCertificate())*/

            services.AddRazorPages();
            //services.ConfigureApplicationCookie(options =>
            //{
            //    // Cookie settings
            //    options.Cookie.HttpOnly = true;
            //    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

            //    options.LoginPath = $"/Identity/Account/Login";
            //    options.LogoutPath = $"/Identity/Account/Logout";
            //    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            //    options.SlidingExpiration = true;
            //});
            //services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromMinutes(30);
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.IsEssential = true;
            //});
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
            // migrate database changes on startup (includes initial db creation)
            //context.Database.Migrate();

            app.UseStaticFiles(); // For the wwwroot folder

            //app.UseHttpsRedirection();

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .SetIsOriginAllowed(origin => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            // global error handler
            app.UseMiddleware<ErrorHandlerMiddleware>();

            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // The provider is the only object we need to redefine. See below for the implementation


            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");


        }
        //private X509Certificate2 GetCertificate()
        //{
        //    var assembly = typeof(Startup).GetTypeInfo().Assembly;
        //    using (Stream stream = assembly.GetManifestResourceStream(assembly.GetManifestResourceNames().FirstOrDefault(r => r.EndsWith("cnblogs.pfx"))))

        //    using (StreamReader reader = new StreamReader(stream))
        //    {
        //        string result = reader.ReadToEnd();
        //        var bytes = new byte[stream.Length];
        //        stream.Read(bytes, 0, bytes.Length);
        //        return new X509Certificate2(bytes);
        //    }


        //    //using (var stream = assembly.GetManifestResourceStream(
        //    //    assembly.GetManifestResourceNames().FirstOrDefault(r => r.EndsWith("cnblogs.pfx"))))
        //    //{
        //    //    if (stream == null)
        //    //        throw new ArgumentNullException(nameof(stream));

        //    //var bytes = new byte[stream.Length];
        //    //stream.Read(bytes, 0, bytes.Length);
        ////}

        //}
    }
}