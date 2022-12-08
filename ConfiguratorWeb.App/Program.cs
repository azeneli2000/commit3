using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;

using Digistat.FrameworkWebExtensions.Extensions;
using Microsoft.Extensions.Logging;

namespace ConfiguratorWeb.App
{
   public class Program
   {
      public static void Main(string[] args)
      {
         var host = CreateWebHostBuilder(args).
            ConfigureLogging((context, builder) => 
               builder.ClearProviders().AddConsole()
            ).Build();
         DigistatMainExtension.DigistatInitialization(host, ConfiguratorWeb.App.CommonStrings.MODULE_NAME);
         host.Run();

      }

      public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
          WebHost.CreateDefaultBuilder(args)
              .UseStartup<Startup>()
              .UseDefaultServiceProvider(options =>
                  options.ValidateScopes = false);
   }
}
