using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Filters
{
   public class RequestFormSizeLimitAttribute : Attribute, IAuthorizationFilter, IOrderedFilter

   {

      private readonly FormOptions mobjFormOptions;



      public RequestFormSizeLimitAttribute(int valueCountLimit)

      {

         mobjFormOptions = new FormOptions()

         {

            ValueCountLimit = valueCountLimit

         };

      }



      public int Order { get; set; }



      public void OnAuthorization(AuthorizationFilterContext context)

      {

         var features = context.HttpContext.Features;

         var formFeature = features.Get<IFormFeature>();



         if (formFeature == null || formFeature.Form == null)

         {

            features.Set<IFormFeature>(new FormFeature(context.HttpContext.Request,

            mobjFormOptions));

         }

      }

   }
}
