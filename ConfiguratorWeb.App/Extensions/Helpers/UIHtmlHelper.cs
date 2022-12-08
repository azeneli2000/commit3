using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ConfiguratorWeb.App.Helpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

using ConfiguratorWeb.App.Models;

using Kendo.Mvc.UI;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.DictionaryTerms;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model;
using Digistat.FrameworkStd.Services;
using Digistat.FrameworkStd.UMSLegacy;
using Kendo.Mvc.Extensions;
using ConfiguratorWeb.App.Services;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace ConfiguratorWeb.App.Extensions.Helpers
{
   public static class UIHtmlHelper
   {

        private readonly static LinkGenerator mobjLinkGenerator = null;


        public static IHtmlContent CreateMenu(this IHtmlHelper helper)
      {
         StringBuilder objStrBuilder = new StringBuilder(" <ul id=\"main-menu\" class=\"sm sm-blue\">");
         //UrlHelper url = new UrlHelper(helper.ViewContext);
         LinkGenerator objLinkGenerator = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<LinkGenerator>();
            try
         {
            IEnumerable<MenuViewModel> menuList = UtilityHelper.GetMenuList();

            foreach (var item in menuList.Where(m => m.ParentId == null))
            {
               var children = menuList.Where(m => m.ParentId == item.Id);
               if (children != null && children.Count()>0)
               {
                  objStrBuilder.AppendLine("<li><a href=\"#\">" + item.Text + "</a>");
                  objStrBuilder.AppendLine("<ul>" + AddMenuItemsRecursive(item, menuList, objLinkGenerator,helper.ViewContext.HttpContext) + "</ul>");
                  objStrBuilder.AppendLine("</li>"); //close main menu
               }
               else
               {
                  objStrBuilder.AppendLine("<li><a href=\"" + mobjLinkGenerator.GetPathByAction(item.Url.Action,item.Url.Controller) + "\">" + item.Text + "</a></li>");
               }

            }

            //List<MenuViewModel> model = (List<MenuViewModel>)helper.ViewData.Model;
            //UrlHelper url = new UrlHelper(helper.ViewContext.RequestContext);
            //MenuBuilder objMenuBuilder = helper.Kendo().Menu().Name("Menu").Items(menu =>
            //        {
            //            foreach (var item in menuList.Where(m => m.ParentId == null))
            //            {
            //                var builder = menu.Add().Text(item.Text);
            //                if (item.Url != null)
            //                {
            //                    builder = builder.Action(item.Url.Action, item.Url.Controller);
            //                }
            //                AddMenuChildren(builder, item, menuList);
            //            }
            //        }
            //    );

            //return helper.Raw(objMenuBuilder.ToHtmlString());

            objStrBuilder.AppendLine("</ul>"); //close main menu
         }
         catch (Exception)
         {
            //Log
            return helper.Raw(string.Empty);
         }
         return helper.Raw(objStrBuilder.ToString());
      }
      
      public static string AddMenuItemsRecursive(MenuViewModel itemParent, IEnumerable<MenuViewModel> items, LinkGenerator linkGenerator,HttpContext httpContext )
      {

         StringBuilder objMenu = new StringBuilder();
         try
         {
            var children = items.Where(m => m.ParentId == itemParent.Id);
            if (children != null && children.Count()>0)
            {
               foreach (var child in children)
               {
                  List<MenuViewModel> objChildren = items.Where(m => m.ParentId == child.Id).ToList();
                  if (objChildren != null && objChildren.Count() > 0)
                  {
                     objMenu.AppendLine("<li><a href=\"#\">" + child.Text + "</a>");
                     objMenu.AppendLine("<ul>");
                     objMenu.AppendLine(AddMenuItemsRecursive(child, items, linkGenerator, httpContext));
                     objMenu.AppendLine("</ul></li>");
                  }
                  else
                  {
                     var urlActionLink = "#";
                     if (child.Url != null && child.Url.Action != null && child.Url.Controller != null )
                     {
                        urlActionLink = linkGenerator.GetPathByAction(httpContext,child.Url.Action,child.Url.Controller);
                     }
                  
                     objMenu.AppendLine("<li><a href=\"" + urlActionLink + "\">" + child.Text + "</a></li>");
                     //objMenu.AppendLine("<li><span class='description'>" + child.Description + "</span></li>");
                     //objMenu.AppendLine("<li><a href=\"" + url.Action(child.Url.Action, child.Url.Controller) + "\">" + child.Text + "</a></li>");
                  }
               }
            }
         }
         catch (Exception)
         {

         }

         return objMenu.ToString();
      }
      
      public static IHtmlContent CreateMenuKendo(this IHtmlHelper helper, IMenuService menuService,HttpContext httpContext)
      {

         string product = "DIGISTAT";
        LinkGenerator objLinkGenerator = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<LinkGenerator>();
        //IProductVersionCore prod = mobjDigEnvironmentService.GetProductVersion();
        IDigistatEnvironmentService mobjDigEnvSvc = (DigistatWebEnvironmentService)AppHttpContext.Services.GetRequiredService<IDigistatEnvironmentService>();
         IProductVersionCore prod = mobjDigEnvSvc.GetProductVersion();
         if (prod != null)
         {
            product = prod.Product;
         }
         StringBuilder objStrBuilder = new StringBuilder(" <ul id=\"main-menu-kendo\" style=\"visibility:hidden\"  >");
         UrlHelper url = new UrlHelper(helper.ViewContext);
         try
         {
            List<MenuViewModel> menuList = menuService.GetMenuForCurrentUser();
            //menuList = UtilityHelper.GetMenuList(product);

            foreach (var item in menuList.Where(m => m.ParentId == null))
            {
               var children = menuList.Where(m => m.ParentId == item.Id);
               if (children != null && children.Count() > 0)
               {
                  
                  string addMenuItemsRecursive = AddMenuItemsRecursive(item, menuList, objLinkGenerator,httpContext);
                  if (addMenuItemsRecursive.Trim().Length>0)
                  {
                     objStrBuilder.AppendLine("<li><a href=\"#\">" + item.Text + "</a>");
                     objStrBuilder.AppendLine("<ul>" + addMenuItemsRecursive + "</ul>");
                     objStrBuilder.AppendLine("</li>"); //close main menu
                  }
                  
               }
               else
               {
                  if (item.ParentId ==null)
                     continue;

                  string urlActionLink = "#";
                  if (item.Url != null && item.Url.Action != null && item.Url.Controller != null )
                  {
                        // urlActionLink = url.Action(item.Url.Action, item.Url.Controller);
                        urlActionLink = objLinkGenerator.GetPathByAction(httpContext,item.Url.Action, item.Url.Controller);
                  }
                  
                  objStrBuilder.AppendLine("<li><a href=\"" + urlActionLink + "\">" + item.Text + "</a></li>");
               }

            }

            objStrBuilder.AppendLine("</ul>"); //close main menu
         }
         catch (Exception)
         {
            //Log
            return helper.Raw(string.Empty);
         }
         return helper.Raw(objStrBuilder.ToString());
      }

      public static IHtmlContent CreateAdminAbout(this IHtmlHelper helper, IPermissionsService permSvc, ISynchronizationService syncSvc)
      {
         User currUsr = syncSvc.GetCurrentUser();
         string divClassIdBtnsystemdetailsDivDiv = "<div class='system-cell'><div class='{0}' id='btnSystemDetails'></div></div>";
         string strClassAdminAbout = "d-none";
         if (permSvc.CheckPermission(Configurator.Std.Defs.Permissions.permissionAdminAboutView,currUsr))
         {
            strClassAdminAbout = "system-image";
         }
         return helper.Raw(divClassIdBtnsystemdetailsDivDiv.FormatWith(strClassAdminAbout));
      }
      public static IHtmlContent CreateSiteMap(this IHtmlHelper helper, IMenuService menuService,HttpContext httpContext)
      {

         string product = "DIGISTAT";
        LinkGenerator objLinkGenerator = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<LinkGenerator>();
        //IProductVersionCore prod = mobjDigEnvironmentService.GetProductVersion();
        IDigistatEnvironmentService mobjDigEnvSvc = (DigistatWebEnvironmentService)AppHttpContext.Services.GetRequiredService<IDigistatEnvironmentService>();
         IProductVersionCore prod = mobjDigEnvSvc.GetProductVersion();
         if (prod != null)
         {
            product = prod.Product;
         }
         StringBuilder objStrBuilder = new StringBuilder(" <ul id=\"main-sitemap\" class=\" mx-1 row\"  >");
         UrlHelper url = new UrlHelper(helper.ViewContext);
         
         try
         {
            List<MenuViewModel> menuList = menuService.GetMenuForCurrentUser();
            //menuList = UtilityHelper.GetMenuList(product);

            foreach (var item in menuList.Where(m => m.ParentId == null))
            {
               var children = menuList.Where(m => m.ParentId == item.Id);
               string liAttribute = "class=\"col-xl-2 col-lg-3 col-md-4 col-sm-6 card \"";
               if (children != null && children.Count() > 0)
               {
                  
                  if (item.ParentId!=null)
                  {
                     liAttribute = "";
                  }
                  string addSiteMapItemsRecursive = AddSiteMapItemsRecursive(item, menuList, objLinkGenerator, httpContext);
                  if (addSiteMapItemsRecursive.Trim().Length>0)
                  {
                     objStrBuilder.AppendLine("<li "+liAttribute+">");
                     objStrBuilder.AppendLine("<ul class=\"px-0\">");

                     objStrBuilder.AppendLine("<li class=\"ums-sitemap-header \">" + item.Text + "</li>");
                  
                     objStrBuilder.AppendLine("<ul>" + addSiteMapItemsRecursive + "</ul>");
                  
                     //objStrBuilder.AppendLine("<li class=\"divider\"></li>"); //close main menu
                     objStrBuilder.AppendLine("</ul>"); //close main menu
                     objStrBuilder.AppendLine("</li>"); //close main menu
                  }
                  
               }
               else
               {
                  if (item.ParentId ==null)
                     continue;

                  string urlActionLink = "#";
                  if (item.Url != null && item.Url.Action != null && item.Url.Controller != null )
                  {
                    // urlActionLink = url.Action(item.Url.Action, item.Url.Controller);
                    urlActionLink = objLinkGenerator.GetPathByAction(httpContext, item.Url.Action, item.Url.Controller);
                    }
                  
                  objStrBuilder.AppendLine("<li "+liAttribute+"><a href=\"" + urlActionLink + "\">" + item.Text + "</a></li>");
               }

            }

            objStrBuilder.AppendLine("</ul>"); //close main menu
         }
         catch (Exception ex)
         {
            //Log
            return helper.Raw(string.Empty);
         }
         return helper.Raw(objStrBuilder.ToString());
      }


      public static string AddSiteMapItemsRecursive(MenuViewModel itemParent, IEnumerable<MenuViewModel> items, LinkGenerator linkGenerator,HttpContext httpContext)
      {

         StringBuilder objMenu = new StringBuilder();
         try
         {
            var children = items.Where(m => m.ParentId == itemParent.Id);
            if (children != null && children.Count()>0)
            {
               foreach (var child in children)
               {
                  List<MenuViewModel> objChildren = items.Where(m => m.ParentId == child.Id).ToList();
                  if (objChildren != null && objChildren.Count() > 0)
                  {
                     objMenu.AppendLine("<li class=\"ums-sitemap-header-subgroup\">" + child.Text + "");
                     objMenu.AppendLine("<ul>");
                     objMenu.AppendLine(AddSiteMapItemsRecursive(child, items, linkGenerator, httpContext));
                     objMenu.AppendLine("</ul></li>");
                  }
                  else
                  {
                     var urlActionLink = "#";
                     if (child.Url != null && child.Url.Action != null && child.Url.Controller != null )
                     {
                        //urlActionLink = url.Action(child.Url.Action, child.Url.Controller);
                        urlActionLink = linkGenerator.GetPathByAction(httpContext, child.Url.Action, child.Url.Controller); 
                    }
                  
                     objMenu.AppendLine("<li><a href=\"" + urlActionLink + "\">" + child.Text + "</a></li>");
                     objMenu.AppendLine("<li><span class='description'>" + child.Description + "</span></li>");
                  }
               }
            }
         }
         catch (Exception e)
         {
            //do something
         }

         return objMenu.ToString();
      }
      
      public static IHtmlContent SystemOptionValueByType<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, OptionType soType, string guid, object htmlAttributes = null) where TModel : SystemOptionViewModel
      {
         string strGuidFunction = (!string.IsNullOrWhiteSpace(guid) ? guid.Replace("-", "_") : "");
         SystemOptionViewModel objModel = (SystemOptionViewModel)htmlHelper.ViewData.Model;
         var attributes = (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes ?? new object());
         if (!attributes.ContainsKey("data-sotype"))
         {
            attributes.Add("data-sotype", soType.ToString());
         }
         AddOrInsertAttributes(attributes, "id", "Value_"+guid);
         var listHtml = new HtmlContentBuilder();
         try
         {
            double lowerVal = (objModel.LowerLimit.HasValue ? objModel.LowerLimit.Value : Int32.MinValue);
            double upperVal = (objModel.UpperLimit.HasValue ? objModel.UpperLimit.Value : Int32.MaxValue);

            switch (soType)
            {
               case OptionType.Bool:
                  // listHtml.AppendHtml("<input id=\"Value_" + guid + "\" type=\"checkbox\" name=\"Value_" + guid + "\" data-sotype=\"" + soType.ToString() + "\" />");
                  //AddOrInsertAttributes(attributes, "style", "margin-top:6px;");
                  listHtml.AppendHtml(htmlHelper.Kendo().CheckBox().Name("Value").Size( ComponentSize.Large).HtmlAttributes(attributes).Checked(UtilityHelper.ConvertStringToBool(objModel.Value)));
                  break;
               case OptionType.Float:
                  double dblValueFloat;
                  bool isDoubleFloat = Double.TryParse(objModel.Value, NumberStyles.AllowDecimalPoint, new NumberFormatInfo{CurrencyDecimalSeparator = "."},out dblValueFloat);
                  double? valFloat = (isDoubleFloat ? dblValueFloat : double.NaN);
                  string htmlString = htmlHelper.Kendo().NumericTextBox().Size( ComponentSize.Large).Name("Value").Step(.1).Min(lowerVal).Max(upperVal).HtmlAttributes(new { data_sotype = soType.ToString() }).Value(valFloat).ToHtmlString();
                  if (valFloat.HasValue)
                  {
                     //Should check che user culture?
                     htmlString = htmlString.Replace($"value=\"{valFloat.ToString().Replace(".", ",")}\"", $"value=\"{valFloat.ToString().Replace(",", ".")}\"");
                  }
                  listHtml.AppendHtml(htmlString);
                  break;
               case OptionType.Text:
                  AddOrInsertAttributes(attributes, "style", "width: 100%;max-height: 300px; resize: none;");
                  AddOrInsertAttributes(attributes, "rows", "10",false);
                  //AddOrInsertAttributes(attributes, "readonly", "",false);
                  AddOrInsertAttributes(attributes, "class", "k-textarea");
                  listHtml.AppendHtml(htmlHelper.TextAreaFor(expression, attributes));
                  break;
               case OptionType.File:
                  AddOrInsertAttributes(attributes, "class", "k-input k-textbox");
                  listHtml.AppendHtml(htmlHelper.Kendo().TextBoxFor(expression).HtmlAttributes(attributes).ToHtmlString()); // htmlHelper.TextBoxFor(expression, attributes)
                  break;
               case OptionType.Url:
                  AddOrInsertAttributes(attributes, "class", "k-input k-textbox");
                  listHtml.AppendHtml(htmlHelper.Kendo().TextBoxFor(expression).HtmlAttributes(attributes).ToHtmlString()); //listHtml.AppendHtml(htmlHelper.TextBoxFor(expression, attributes).ToString());
                  break;
               case OptionType.Integer:
                  double dblValue;
                  bool isDouble = Double.TryParse(objModel.Value, out dblValue);
                  double? val = (isDouble ? dblValue : double.NaN);
                  listHtml.AppendHtml(htmlHelper.Kendo().NumericTextBox().Name("Value").Decimals(0).Format("##").Step(1).Value(val.Value).Min(lowerVal).Max(upperVal).HtmlAttributes(new { data_sotype = soType.ToString() }).ToHtmlString());
                  break;
               case OptionType.Date:
                  listHtml.AppendHtml(htmlHelper.Kendo().DatePicker().Name("Value").Value(objModel.Value).HtmlAttributes(new { data_sotype = soType.ToString() }).ToHtmlString());
                  break;
               case OptionType.Time:
                  listHtml.AppendHtml(htmlHelper.Kendo().TimePicker().Name("Value").Format("HH:mm:ss").Value(objModel.Value).HtmlAttributes(new { data_sotype = soType.ToString(), style ="max-width: 120px !important;" }).ToHtmlString());
                  break;
               case OptionType.Path:
                  AddOrInsertAttributes(attributes, "class", "k-input k-textbox");
                  listHtml.AppendHtml(htmlHelper.Kendo().TextBoxFor(expression).HtmlAttributes(new { data_sotype = soType.ToString(), /*style = "width: 89%;" */}).ToHtmlString());
                  break;
               case OptionType.Binary:

                  listHtml.AppendHtml("<button type=\"button\" id=\"btnBinaryButton\" class=\"btn border\" onclick =\"manageBinaryValue('" + objModel.Value + "');\" >" + objModel.ValueDisplayBinary  + "</button>");
                  listHtml.AppendHtml("<input type=\"hidden\" name=\"Value\" id=\"Value\" value=\"" + objModel.Value + "\" />");
                  break;
               default:
                  AddOrInsertAttributes(attributes, "class", "k-input k-textbox");
                  listHtml.AppendHtml(htmlHelper.Kendo().TextBoxFor(expression).HtmlAttributes(new { data_sotype = soType.ToString(), style = "width: 89%;" }).ToHtmlString());
                  break;
            }
         }
         catch (Exception)
         {

         }
         return listHtml;
      }


      public static void AddOrInsertAttributes(IDictionary<string, object> dictAttributes, string key, string value, bool appendIfExists = true)
      {
         try
         {
            if (!dictAttributes.ContainsKey(key))
            {
               dictAttributes.Add(key, value);
            }
            else
            {
               if (appendIfExists)
               {
                  dictAttributes[key] = dictAttributes[key] + " " + value;
               }
               else
               {
                  dictAttributes[key] = value;
               }
            }
         }
         catch (Exception)
         {
            throw;
         }
      }


      public static IUrlHelper GetUrlHelper(this IHtmlHelper html)
      {
         var urlFactory = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
         var actionAccessor = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IActionContextAccessor>();
         return urlFactory.GetUrlHelper(actionAccessor.ActionContext);
      }


      public static string GetDeviceTypeCentral(IDictionaryService dictionaryService)
      {
         return dictionaryService.XLate(DasDictionaryTerms.driverTypeCentral, StringParseMethod.Html);
      }

      //check if is central

      public static bool IsDriverCentralType(this DeviceDriverViewModel deviceDriver)
      {
         DeviceDriver3 driver = new DeviceDriver3 { DriverType = deviceDriver.DriverType };
         return driver.IsCentralDriverType();
      }

      //objList.Add(new SelectListItem(mobjDictionaryService.XLate(Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetConnectionTypeDescription(type)), type.ToString()));

      //public static string GetConnnectionTypeDesc(IDictionaryService dictionaryService, int type)
      //{
      //   return dictionaryService.XLate(Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetConnectionTypeDescription(type));
      //}

      public static int GetSocketIndexValue()
      {
         return Digistat.FrameworkStd.UMSLegacy.UMSFrameworkParser.GetSocketTypeIndex();
      }
   }


}
