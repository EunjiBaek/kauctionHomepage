using KA.Entities.Helpers;
using KA.Repositories;
using KA.Web.Public.Hubs;
using KA.Web.Public.Middlewares;
using KA.Web.Public.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Net;
using Microsoft.Extensions.Hosting;

namespace KA.Web.Public
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        public static IConfiguration Config { get; private set; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            Configuration = configuration;
            Config = configuration;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region # AddControllerWithViews #

            services.AddControllersWithViews()
                .AddNewtonsoftJson()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddRazorRuntimeCompilation();

            #endregion

            #region # AddSignalR #

            services.AddSignalR();
            services.AddHostedService<LiveBackService>();

            #endregion

            #region # Localization #

            // Add the localization services to the services container
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            // Configure supported cultures and localization options
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("ko-KR"),
                    new CultureInfo("en-US")
                };

                // State what the default culture for your application is. This will be used if no specific culture
                // can be determined for a given request.
                // options.DefaultRequestCulture = new RequestCulture(culture: "ko-KR", uiCulture: "ko-KR");

                // You must explicitly state which cultures your application supports.
                // These are the cultures the app supports for formatting numbers, dates, etc.
                options.SupportedCultures = supportedCultures;

                // These are the cultures the app supports for UI strings, i.e. we have localized resources for.
                options.SupportedUICultures = supportedCultures;
            });

            #endregion

            #region # AddSingleton #

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<CommonService>();
            services.AddSingleton<IRsaCryptoService, RsaCryptoService>();

            #endregion

            #region # AddTransient #

            services.AddTransient<ICommonRepository, CommonRepository>();
            services.AddTransient<IMainRepository, MainRepository>();
            services.AddTransient<IAuctionRepository, AuctionRepository>();
            services.AddTransient<IContentRepository, ContentRepository>();
            services.AddTransient<IMemberRepository, MemberRepository>();
            services.AddTransient<ILogRepository, LogRepository>();
            services.AddTransient<ILiveRepository, LiveRepository>();
            services.AddTransient<EmailHelper>();

            #endregion

            #region # AddAuthentication #

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.LoginPath = "/Member/Login";
            });

            #endregion

            #region # In-Memory #

            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            //services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromMinutes(10);
            //});

            #endregion

            #region # Compression #

            services.AddResponseCompression(options => {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            #endregion

            #region # Session #

            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromHours(24 * 30);
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.IsEssential = true;
                options.Cookie.HttpOnly = true;
            });

            #endregion

            #region SPA

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot/liveclient/build";
            });

            #endregion

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin());
            });
            
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyMethod().AllowAnyHeader();
            }));
            
            if (!_env.IsDevelopment())
            {
                services.AddHttpsRedirection(options =>
                {
                    options.RedirectStatusCode = (int)HttpStatusCode.PermanentRedirect;
                    options.HttpsPort = 443;
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions?.Value);

            app.UseExceptionHandler("/Home/Error");

            //app.Use(async (context, next) =>
            //{
            //    await next();
            //    if (context.Response.StatusCode == 404)
            //    {
            //        context.Request.Path = "/Home/Error";
            //        await next();
            //    }
            //});

            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();
            
            app.UseStaticFiles();
            
            app.UseSpaStaticFiles();

            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseResponseCompression();

            app.UseMiddleware<RequestMiddleware>();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<AuctionHub>("/auctionHub");
                endpoints.MapHub<LiveHub>("/liveHub");
            });
            
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "wwwroot/liveclient";
                if (_env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
