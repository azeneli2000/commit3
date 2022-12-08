using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json.Serialization;
using NetCore.AutoRegisterDi;

using Digistat.FrameworkStd.Interfaces;
using Newtonsoft.Json;
using Configurator.Std.BL.Hubs;
using ConfiguratorWeb.App.Helpers;
using Configurator.Std.BL.Configurator;
using ConfiguratorWeb.App.Controllers;
using ConfiguratorWeb.App.Filters;
using Digistat.Dal.Data;
using Digistat.Dal.Initializator;
using Digistat.FrameworkWebExtensions.Environment;
using Digistat.FrameworkStd.Services;
using Digistat.WebComponents.Extensions;
using ConfiguratorWeb.App.Services;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Filters;
using Configurator.Std.BL.Mobile;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Digistat.WebComponents;
using Digistat.FrameworkWebExtensions.Filters;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Microsoft.AspNetCore.HttpOverrides;

namespace ConfiguratorWeb.App
{
   public class Startup
   {
      private readonly ILogger _logger;
      public Startup(IConfiguration configuration, ILogger<Startup> logger)
      {
         Configuration = configuration;
         LogManager.Configuration = new NLogLoggingConfiguration(configuration.GetSection("NLog"));

         _logger = logger;
      }

      public IConfiguration Configuration { get; }

      // This method gets called by the runtime. Use this method to add services to the container.
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddSingleton<IConfiguration>(Configuration);
         Digistat.Dal.Initializator.DalServiceActivator.AddDalServices(services);
         //
         services.AddSingleton<IExternalLoggerService, Digistat.ExternalLogger.Nlog.NlogLoggerService>();
         //
         /*services.Configure<CookiePolicyOptions>(options =>
         {
               // This lambda determines whether user consent for non-essential cookies is needed for a given request.
               options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
         });*/


        

         /*Necessary to load components from embedded resource when using Digistat.WebComponents embedded components*/
         services.ConfigureOptions(typeof(CommonUIConfigureOptions));
         UIExtensions.AddDigistatWebComponents(services);

         services.AddSingleton<IExternalLoggerService, Digistat.ExternalLogger.Nlog.NlogLoggerService>();


         services.AddSingleton<IReportGeneratorService, ReportGeneratorService>();

         /* Extended Configurator */
         services.AddSingleton<IConfiguratorWebConfiguration, ConfiguratorWebConfiguration>();
         services.AddSingleton<IDigistatConfiguration>(x => x.GetService<IConfiguratorWebConfiguration>());
         /* End Extended Configurator */


         /*Register Menu service . It needs Memorycache*/
         services.AddMemoryCache();
         services.AddSingleton<IMenuService, MenuService>();


         /*Register Dal extensions*/
         services.AddDalServices();

         /*Register controlbar services [it must be executer after Configuration Service Extension]*/
         services.AddDigistatMiddlewareWeb();

         //In other Digistat projects this is not needed. In configurator we do not use ctrlbar so I need to register IWebModuleButtonsService.
         services.TryAddSingleton<IWebModuleButtonsService, EmptyWebModuleButtons>();


         //This registers the service layer: I only register the classes who name ends with "Service" (at the moment)
         services.RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(Configurator.Std.BL.UsersManager)))
             .Where(c => c.Name.EndsWith("Manager"))
             .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

         services.AddSingleton<Configurator.Std.BL.DasDrivers.IDriverMonitorService, Configurator.Std.BL.DasDrivers.DriverMonitorService>();

         services.AddResponseCaching();

         /*Position Mobile Service */
         services.AddSingleton<IPositionService, PositionService>();
         var objMvcBuilder = services.AddMvc(option => option.EnableEndpointRouting = false)
             .AddNewtonsoftJson(options =>
             {
                options.SerializerSettings.ContractResolver =
                 new DefaultContractResolver();
                //Commented because with Kendo grid the visualization of Enum is not working
                //   if you need, put on your jsonResult
                //options.SerializerSettings.Converters.Add(new StringEnumConverter());
             });
#if DEBUG
         objMvcBuilder.AddRazorRuntimeCompilation();
#endif

         services.AddKendo();

         services.AddRazorPages();

         //services.AddSwaggerExamples();
         services.AddSwaggerGen(c =>
         {

            c.SwaggerDoc("v1", new OpenApiInfo() { Title = "Digistat API", Version = "v1" });
            c.SchemaFilter<AutoRestSchemaFilter>();
            c.ExampleFilters();
            //c.DescribeAllEnumsAsStrings();
            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
            c.EnableAnnotations();
            c.AddSecurityDefinition("basicAuth", new OpenApiSecurityScheme()
            {
               Type = SecuritySchemeType.Http,
               Scheme = "basic",
               Description = "Input your username and password to access this API"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
               {
                  new OpenApiSecurityScheme
                  {
                     Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                        Id = "basicAuth" }
                  }, new List<string>() }
            });
         });
         services.AddSwaggerExamplesFromAssemblyOf<ClinicalLogApiModelExample>();
         services.AddSwaggerGenNewtonsoftSupport();

         //Basic Auth filter
         services.AddScoped<BasicAuthFilter>();
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
      {

         if (env.IsDevelopment())
         {
            _logger.LogInformation("       *******************       DEVELOPMENT       *******************");
            app.UseDeveloperExceptionPage();
            //app.UseExceptionHandler("/Home/Error");
         }
         else
         {
            _logger.LogInformation("       *******************       PRODUCTION       *******************");
            app.UseExceptionHandler("/Home/Error");
         }
         AppHttpContext.Services = app.ApplicationServices;

         IConfiguratorWebConfiguration mobjConfiguration = (ConfiguratorWebConfiguration)AppHttpContext.Services.GetRequiredService<IConfiguratorWebConfiguration>();
         _logger.LogInformation($"Message center:\t{mobjConfiguration.MessageCenter}:{mobjConfiguration.MessageCenterInstance}");
         
         IDigistatEnvironmentService mobjDigEnvSvc = (DigistatWebEnvironmentService)AppHttpContext.Services.GetRequiredService<IDigistatEnvironmentService>();
         mobjDigEnvSvc.LoginActionController = new Tuple<string, string>("Login", "ConfiguratorSecurity");
         mobjDigEnvSvc.DenyAnonymousUser = true;
         
         // If authorized forward proxies are set, enable UseForwardedHeaders.
         if (!string.IsNullOrEmpty(mobjConfiguration.AuthorizedForwardProxies))
         {
            app.UseForwardedHeaders();
         }
         
         app.UseRouting();
         app.AddCtrlBar(env);

         app.UseStaticFiles();
         app.UseCookiePolicy();
         app.UseAuthentication();

         app.UseRequestLocalization();


         app.UseEndpoints(endpoints =>
         {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapControllerRoute(
                   name: "api",
                   pattern: "api/{controller}/{action}");
            endpoints.MapGet("/cfg", async c =>
            {
               await c.Response.WriteAsync($"Message center:\t{mobjConfiguration.MessageCenter}:{mobjConfiguration.MessageCenterInstance}");
            });

         });



         app.UseSwagger(c =>
          {
             c.RouteTemplate = "/api/swagger/{documentname}/swagger.json";
          });

         app.UseSwaggerUI(c =>
         {
            c.IndexStream = () => GetType().Assembly.GetManifestResourceStream("ConfiguratorWeb.App.Swagger.index.html");
            c.InjectStylesheet("../../css/ums.swagger.css");
            c.SwaggerEndpoint("v1/swagger.json", "Digistat Api V1");
            c.RoutePrefix = "api/swagger";

            c.DefaultModelsExpandDepth(-1);
            //this disable the try it out button
            //c.SupportedSubmitMethods(new Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod[] { });
         });
      }
   }

}
