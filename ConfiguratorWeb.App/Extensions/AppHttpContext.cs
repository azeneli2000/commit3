using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Helpers
{
   public static class AppHttpContext
   {
      static IServiceProvider services = null;

      /// <summary>
      /// Provides static access to the framework's services provider
      /// </summary>
      public static IServiceProvider Services
      {
         get { return services; }
         set
         {
            if (services != null)
            {
               throw new Exception("Can't set once a value has already been set.");
            }
            services = value;
         }
      }

      /// <summary>
      /// Provides static access to the current HttpContext
      /// </summary>
      public static HttpContext Current
      {
         get
         {
            IHttpContextAccessor httpContextAccessor = services.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
            return httpContextAccessor.HttpContext;
         }
      }

      /// <summary>
      /// Determines whether the specified HTTP request is an AJAX request.
      /// </summary>
      /// <param name="this">The @this to act on.</param>
      /// <returns>True if the <paramref name="this"/> is an AJAX request; otherwise false.</returns>
      public static bool IsAjaxRequest(this HttpRequest @this)
      {
         return (@this.Query != null && @this.Query["X-Requested-With"] == "XMLHttpRequest")
                || (@this.Headers != null && (@this.Headers["X-Requested-With"] == "XMLHttpRequest" 
                || @this.Headers["X-UMS-Requested-With"] == "fetch"));
      }
   }
}
