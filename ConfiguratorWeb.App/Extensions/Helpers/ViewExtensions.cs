using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Extensions.Helpers
{
   public static class ViewExtensions
   {
      public static async Task<string> RenderViewAsync<TModel>(this Controller controller, string viewName, TModel model, bool partial = false)
      {
         try
         {
            if (string.IsNullOrEmpty(viewName))
            {
               viewName = controller.ControllerContext.ActionDescriptor.ActionName;
            }

            controller.ViewData.Model = model;
            using (var writer = new StringWriter())
            {
               IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
               ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, !partial);

               if (viewResult.Success == false)
               {
                  return $"A view with the name {viewName} could not be found";
               }

               ViewDataDictionary objVD = new ViewDataDictionary<TModel>(
                  metadataProvider: new EmptyModelMetadataProvider(),
                  modelState: new ModelStateDictionary())
               {
                  Model = model
               };

               ViewContext viewContext = new ViewContext(
                   controller.ControllerContext,
                   viewResult.View,
                   objVD,
                   controller.TempData,
                   writer,
                   new HtmlHelperOptions()
               );


               await viewResult.View.RenderAsync(viewContext);

               return writer.GetStringBuilder().ToString();
            }
         }
         catch (Exception exc)
         {
            return exc.ToString();
         }
         
         
      }
   }
}
