using KA.Entities.Helpers;
using KA.Repositories;
using KA.Web.Admin.Middlewares;
using KA.Web.Admin.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace KA.Web.Admin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region # AddControllerWithViews #

            services.AddControllersWithViews()
                .AddNewtonsoftJson()
                .AddRazorRuntimeCompilation();

            #endregion

            #region # AddSingleton #

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<CommonService>();

            #endregion

            #region # AddTransient #

            services.AddTransient<IMainRepository, MainRepository>();
            services.AddTransient<IManagerRepository, ManagerRepository>();
            services.AddTransient<ICommonRepository, CommonRepository>();
            services.AddTransient<IAuctionRepository, AuctionRepository>();
            services.AddTransient<IMemberRepository, MemberRepository>();
            services.AddTransient<IContentRepository, ContentRepository>();
            services.AddTransient<ILogRepository, LogRepository>();
            services.AddTransient<EmailHelper>();

            #endregion

            #region # AddAuthentication #

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/Home/Login";
            });

            #endregion

            #region # In-Memory #

            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
            });

            #endregion

            #region # Compression #

            services.AddResponseCompression();

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler("/Home/Error");

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseResponseCompression();

            app.UseMiddleware<RequestMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Login}/{id?}");
            });
        }
    }
}
