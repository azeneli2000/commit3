using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfiguratorWeb.App.Extensions.Helpers;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Fluent;
using Digistat.FrameworkStd.Interfaces;
using Kendo.Mvc.Resources;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace ConfiguratorWeb.App.Extensions
{

   public static class UmsGridHelpers
   {

      /// <summary>
      /// Finds a Filter Member with the "memberName" name and renames it for "newMemberName".
      /// </summary>
      /// <param name="request">The DataSourceRequest instance. <see cref="Kendo.Mvc.UI.DataSourceRequest"/></param>
      /// <param name="memberName">The Name of the Filter to be renamed.</param>
      /// <param name="newMemberName">The New Name of the Filter.</param>
      public static void RenameRequestFilterMember(this DataSourceRequest request, string memberName,string newMemberName)
      {
         foreach (var filter in request.Filters)
         {
            if (filter is CompositeFilterDescriptor)
            {
               foreach (var cfilter in ((CompositeFilterDescriptor)filter).FilterDescriptors)
               {
                  var descriptor = cfilter as Kendo.Mvc.FilterDescriptor;
                  if (descriptor!=null && descriptor.Member.Equals(memberName))
                     descriptor.Member = newMemberName;
               }
            }
            else
            {
               var descriptor = filter as Kendo.Mvc.FilterDescriptor;
               if (descriptor != null && descriptor.Member.Equals(memberName))
                  descriptor.Member = newMemberName;
            }
            
         }
         //foreach (var sorter in request.Sorts)
         //{
         //   var descriptor = sorter as Kendo.Mvc.SortDescriptor;
         //   if (descriptor.Member.Equals(memberName))
         //      descriptor.Member = newMemberName;

         //}
      }


      public static List<FilterDescriptor> ToFilterDescriptor(this IList<IFilterDescriptor> filters)
      {
         var result = new List<FilterDescriptor>();
         if (filters.Any())
         {
            foreach (var filter in filters)
            {
               var descriptor = filter as FilterDescriptor;
               if (descriptor != null)
               {
                  result.Add(descriptor);
               }
               else
               {
                  var compositeFilterDescriptor = filter as CompositeFilterDescriptor;
                  if (compositeFilterDescriptor != null)
                  {
                     result.AddRange(compositeFilterDescriptor.FilterDescriptors.ToFilterDescriptor());
                  }
               }
            }
         }
         return result;
      }

      /// <summary>
      /// Default option of Kendo grid to use in main page
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="builder"></param>
      /// <param name="dicSvc"></param>
      /// <returns></returns>
      public static GridBuilder<T> UmsDefaultSettings<T>(this GridBuilder<T> builder, IDictionaryService dicSvc)
         where T : class =>
         builder
            .Filterable(filter => filter.Operators(o =>
            {
               o.ForString(s => { 
                  s.Clear();
                  s.Contains(Messages.Filter_StringContains);
                  s.DoesNotContain(Messages.Filter_StringDoesNotContain );
                  s.StartsWith( Messages.Filter_StringStartsWith );
                  s.EndsWith(Messages.Filter_StringEndsWith );
                  s.IsEqualTo(Messages.Filter_StringIsEqualTo );
                  s.IsNotEqualTo( Messages.Filter_StringIsNotEqualTo );
                  s.IsNull(Messages.Filter_StringIsNull );
                  s.IsNotNull(Messages.Filter_StringIsNotNull );
                  s.IsEmpty(Messages.Filter_StringIsEmpty );
                  s.IsNotEmpty(Messages.Filter_StringIsNotEmpty );
                  s.IsNullOrEmpty( Messages.Filter_StringIsNullOrEmpty );
                  s.IsNotNullOrEmpty(Messages.Filter_StringIsNotNullOrEmpty);
               });
            }))
            .HtmlAttributes(new { style = "height:500px;margin:1px;" })
            .NoRecords(x => x.Template("<div class='empty-grid'>" + dicSvc.XLate(CommonStrings.NO_RECORD_FOUND) + "</div>"))
            .NoRecords(true)
            //.LoaderType(GridLoaderType.Skeleton)
            .Sortable()
            .Selectable(e => e.Enabled(true))
            .Scrollable()
            .Navigatable(true)
            .Resizable(resize => resize.Columns(true))
         ;
      public static GridBuilder<T> UmsDefaultPager<T>(this GridBuilder<T> builder, IDictionaryService dicSvc)
         where T : class =>
         builder
            .Pageable(pager => pager
               //.Input(true)
               .Numeric(true)
               .ButtonCount(5)
               .Info(true) /*show 'rec X-Y of Z item'*/
               .PreviousNext(true)
               .Refresh(true)
               .PageSizes(new int[] {5, 10, 20, 50, 100,500})
            );
      /// <summary>
      /// Default Toolbar's Option (Add New + Excel)
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="builder"></param>
      /// <param name="addNewId">html id of [Add new Item] - default:addNewItem</param>
      /// <param name="addNewText">text of button (will be translate) - <b>default</b>:"ADD NEW ITEM"</param>
      /// <param name="dicSvc"></param>
      /// <returns></returns>
      public static GridBuilder<T> UmsDefaultToolbar<T>(this GridBuilder<T> builder,
         string addNewId,
         string addNewText,
         IDictionaryService dicSvc) where T : class
      {
         return UmsDefaultToolbar(builder, addNewId, addNewText, "", dicSvc);
      }
      public static GridToolBarCustomCommandBuilder UmsGridButtonGeneric(this GridToolBarCustomCommandBuilder builder,string customId,string customText)
      {
         return UmsGridButtonGeneric(builder,customId, customText,"");
      }
      public static GridToolBarCustomCommandBuilder UmsGridButtonGeneric(this GridToolBarCustomCommandBuilder builder,string customId,string customText,string customOnClick)
      {
         var ret = builder.Text(customText).HtmlAttributes(new {@id = customId});
         if (!string.IsNullOrWhiteSpace(customOnClick))
         {
            ret = ret.HtmlAttributes( new { onclick = customOnClick });
         }
         return ret;
      }
      public static GridBuilder<T> UmsDefaultToolbar<T>(this GridBuilder<T> builder,
         string addNewId,
         string addNewText,
         string addNewClickFunction,
         IDictionaryService dicSvc) where T : class
      {
         if (builder == null) throw new ArgumentNullException(nameof(builder));
         //if (String.IsNullOrWhiteSpace(addNewId))
         //   addNewId = "addNewItem";
         if (String.IsNullOrWhiteSpace(addNewText))
            addNewText = "ADD NEW ITEM";
         if (dicSvc == null) throw new ArgumentNullException(nameof(dicSvc));
         var ret = builder
            
            .Excel(excel => excel.AllPages(true)
               .FileName("Grid_{0}.xlsx".FormatWith(DateTime.Now.ToString("s")))
            );
         if(!String.IsNullOrWhiteSpace(addNewId)){
            ret = ret.ToolBar(tools =>
                  tools.Custom().UmsGridButtonGeneric(addNewId, dicSvc.XLate(addNewText), addNewClickFunction)
                  );
         }

         ret = ret.ToolBar(tools =>
            {
               tools.Excel().HtmlAttributes(new
                  {@class = " kendoExport", title = dicSvc.XLate("Export grid as Excel")});
            }
         );
         return ret;
      }
      
      public static ToolBarBuilder UmsDefaultDiv(this ToolBarBuilder builder, IDictionaryService dicSvc,string controlId,string @class,bool useSeparator)
      {
         
         string divClass = "pt-3 pb-1";
         if (!String.IsNullOrWhiteSpace(@class))
         {
            divClass = @class;
         }
         string divId = "errors";
         if (!String.IsNullOrWhiteSpace(controlId))
         {
            divId = controlId;
         }
         var retTool =builder.Items(items =>   
                     {
                        items.Add().Type(CommandType.Button).Template("<div id='{0}' class='{1}'></div>".FormatWith(divId,divClass));
                        
                     });
         if (useSeparator)
         {
            retTool.Items(items =>   
            {
               items.Add().Type(CommandType.Separator);
                        
            });
         }
         return retTool;
      }
      public static ToolBarBuilder UmsDefaultErrors(this ToolBarBuilder builder, IDictionaryService dicSvc,string controlId )
      {
         return UmsDefaultDiv(builder,dicSvc,controlId,"errorSpan",true);
      }
      public static ToolBarBuilder UmsDefaultErrors(this ToolBarBuilder builder, IDictionaryService dicSvc)
      {
         return UmsDefaultErrors(builder,dicSvc,"errors");
      }
      public static ToolBarBuilder UmsDefaultInfo(this ToolBarBuilder builder, IDictionaryService dicSvc)
      {
         return UmsDefaultDiv(builder,dicSvc,"u-info","pt-3 pb-1 ",false);
      }
      public static ToolBarBuilder UmsDefaultSettings(this ToolBarBuilder builder, IDictionaryService dicSvc,string postId)
      {
         return builder
               .Items(items =>
               {
                  items.Add().Type(CommandType.Button).Text(dicSvc.XLate("Save"))  .Hidden(true).Id("btnSave" + postId)  .UmsHtmlButtonSave();
                  items.Add().Type(CommandType.Button).Text(dicSvc.XLate("Cancel")).Hidden(true).Id("btnCancel" + postId).UmsHtmlButtonCancel();
                  items.Add().Type(CommandType.Button).Text(dicSvc.XLate("Edit"))  .Hidden(true).Id("btnEdit" + postId)  .UmsHtmlButtonEdit(); 
                  items.Add().Type(CommandType.Button).Text(dicSvc.XLate("Close")) .Hidden(true).Id("btnClose" + postId) .UmsHtmlButtonClose(); 
               })
            ;
      }
      /// <summary>
      /// Set Html attribute class
      /// </summary>
      /// <param name="builder">item</param>
      /// <param name="classCss">Class of button</param>
      /// <returns>builder.HtmlAttributes(new { @class = classCss});</returns>
      public static ToolBarItemBuilder UmsHtmlButtonGeneric(this ToolBarItemBuilder builder, string classCss)
      {
         return builder.HtmlAttributes(new { @class = "k-buttonLarge " + classCss });
      }
      /// <summary>
      /// return button with class: "btnSave btnGreen modify-mode"
      /// </summary>
      /// <param name="builder"></param>
      /// <returns></returns>
      public static ToolBarItemBuilder UmsHtmlButtonSave(this ToolBarItemBuilder builder)
      {
         return builder.UmsHtmlButtonGeneric( "k-buttonLarge btnSave btnRed modify-mode");
      }
      /// <summary>
      /// return button with class: "btnCancel btnOrange modify-mode"
      /// </summary>
      /// <param name="builder"></param>
      /// <returns></returns>
      public static ToolBarItemBuilder UmsHtmlButtonCancel(this ToolBarItemBuilder builder)
      {
         return builder.UmsHtmlButtonGeneric(" k-buttonLarge btnCancel modify-mode");
      }
      /// <summary>
      /// return button with class: "btnEdit btnGreen view-mode"
      /// </summary>
      /// <param name="builder"></param>
      /// <returns></returns>
      public static ToolBarItemBuilder UmsHtmlButtonEdit(this ToolBarItemBuilder builder)
      {
         return builder.UmsHtmlButtonGeneric("k-buttonLarge btnEdit view-mode");
      }
      /// <summary>
      /// return button with class: "btnClose btnOrange view-mode"
      /// </summary>
      /// <param name="builder"></param>
      /// <returns></returns>
      public static ToolBarItemBuilder UmsHtmlButtonClose(this ToolBarItemBuilder builder)
      {
         return builder.UmsHtmlButtonGeneric("k-buttonLarge btnClose view-mode");
      }
      //items.Add().Type(CommandType.Button).Template("<div id='errors'></div>");
      //items.Add().Type(CommandType.Separator);
      //items.Add().Type(CommandType.Button).Text(@DicSvc.XLate("SAVE"))  .Id("btnSave_" + Model.ID).HtmlAttributes(new { @class = "btnSave modify-mode" }); 
      //items.Add().Type(CommandType.Button).Text(@DicSvc.XLate("CANCEL")).Id("btnCancel_" + Model.ID).HtmlAttributes(new { @class = "btnCancel modify-mode" });
      //items.Add().Type(CommandType.Button).Text(@DicSvc.XLate("EDIT"))  .Id("btnEdit_" + Model.ID).HtmlAttributes(new { @class = "btnEdit view-mode" }); ;
      //items.Add().Type(CommandType.Button).Text(@DicSvc.XLate("CLOSE")) .Id("btnClose_" + Model.ID).HtmlAttributes(new { @class = "btnClose view-mode" }); ;

  
      

   }
   public static class FilterAttributesHelper
    {
        public static DataSourceRequest FilterAttributesMapping(this DataSourceRequest request, IDictionary<string, string> mappings)
        {
           if (request.Filters == null || request.Filters.Count==0)
           {
              return request;
           }
            return request.SelectAttributesToReplace(mappings).ReplaceMappings(request, mappings);
        }
 
        private static DataSourceRequest ReplaceMappings(this IEnumerable<FilterDescriptor> attributesToReplace, DataSourceRequest request, IDictionary<string, string> mappings)
        {
            attributesToReplace.ToList().ForEach(oldMapping => 
                    oldMapping.RemoveOldMapping(request).ApplyNewMapping(request, mappings)
                        );
 
            return request;
        }
 
        private static void ApplyNewMapping(this FilterDescriptor oldMapping, DataSourceRequest request, IDictionary<string, string> mappings)
        {
            request.Filters.Add(PrepareDescriptor(mappings.NewMapping(oldMapping), oldMapping));
        }
 
        private static FilterDescriptor PrepareDescriptor(string newMapping, FilterDescriptor oldMapping)
        {
            return new FilterDescriptor()
            {
                Member = newMapping,
                Operator = oldMapping.Operator,
                Value = oldMapping.Value
            };
        }
 
        private static IEnumerable<FilterDescriptor> SelectAttributesToReplace(this DataSourceRequest request, IDictionary<string, string> mappings)
        {
           List<FilterDescriptor>  locaFilterDescriptors = new List<FilterDescriptor> ();
           foreach (var filters in request.Filters)
           {
              if (filters is CompositeFilterDescriptor)
              {
                 foreach (FilterDescriptor cfilter in ((CompositeFilterDescriptor)filters).FilterDescriptors)
                 {
                    if (mappings.ContainsKey(cfilter.Member))
                    {
                       locaFilterDescriptors.Add(cfilter);
                    }
                    
                 }
              }
              else
              {
                 locaFilterDescriptors = request.Filters.Select(filter => filter as FilterDescriptor ?? new FilterDescriptor()).Where(filter => mappings.ContainsKey(filter.Member)).ToList();   
              }
           }

           return locaFilterDescriptors ;
        }
 
        private static FilterDescriptor RemoveOldMapping(this FilterDescriptor oldMapping, DataSourceRequest request)
        {
            request.Filters.Remove(oldMapping);
 
            return oldMapping;
        }
 
        private static string NewMapping(this IDictionary<string, string> mappings, FilterDescriptor oldMapping)
        {
            return mappings[oldMapping.Member];
        }
    }
   public static class GroupAttributesHelper
    {
        public static DataSourceRequest GroupAttributesMapping(this DataSourceRequest request, IDictionary<string, string[]> mappings)
        {
           if (request.Groups == null || request.Groups.Count==0)
           {
              return request;
           }
            return request.SelectAttributesToReplace(mappings).ReplaceMappings(request, mappings);
        }
 
        private static DataSourceRequest ReplaceMappings(this IEnumerable<GroupDescriptor> attributesToReplace, DataSourceRequest request, IDictionary<string, string[]> mappings)
        {
            attributesToReplace.ToList().ForEach(oldMapping =>
                    oldMapping.RemoveOldMapping(request).ApplyNewMapping(request, mappings)
                        );
 
            return request;
        }
 
        private static void ApplyNewMapping(this GroupDescriptor oldMapping, DataSourceRequest request, IDictionary<string, string[]> mappings)
        {
            mappings.NewMappings(oldMapping).ToList().ForEach(newMapping => request.Groups.Add(PrepareDescriptor(newMapping, oldMapping)));
        }
 
        private static GroupDescriptor PrepareDescriptor(string newMapping, GroupDescriptor oldMapping)
        {
            var obj = new GroupDescriptor()
            {
                Member = newMapping,
                SortDirection = oldMapping.SortDirection,
                DisplayContent = oldMapping.DisplayContent
            };
 
            obj.AggregateFunctions.AddRange(oldMapping.AggregateFunctions);
 
            return obj;
        }
 
        private static GroupDescriptor RemoveOldMapping(this GroupDescriptor oldMapping, DataSourceRequest request)
        {
            request.Groups.Remove(oldMapping);
 
            return oldMapping;
        }
 
        private static IEnumerable<GroupDescriptor> SelectAttributesToReplace(this DataSourceRequest request, IDictionary<string, string[]> mappings)
        {
            return request.Groups.Where(group => mappings.ContainsKey(group.Member)).ToList();
        }
 
        private static IEnumerable<string> NewMappings(this IDictionary<string, string[]> mappings, GroupDescriptor oldMapping)
        {
            return mappings[oldMapping.Member];
        }
    }
   public static class SortAttributesHelper
   {
      public static DataSourceRequest SortAttributesMapping(this DataSourceRequest request, IDictionary<string, string[]> mappings)
      {
         if (request.Sorts == null || request.Sorts.Count==0)
         {
            return request;
         }
         return request.SelectAttributesToReplace(mappings).ReplaceMappings(request, mappings);
      }
 
      private static void ApplyNewMapping(this  SortDescriptor oldMapping, DataSourceRequest request, IDictionary<string, string[]> mappings)
      {
         mappings.NewMappings(oldMapping).ToList().ForEach(newMapping => request.Sorts.Add(PrepareDescriptor(newMapping, oldMapping)));
      }
 
      private static DataSourceRequest ReplaceMappings(this IEnumerable<SortDescriptor> attributesToReplace, DataSourceRequest request, IDictionary<string, string[]> mappings)
      {
         attributesToReplace.ToList().ForEach(oldMapping =>
            oldMapping.RemoveOldMapping(request).ApplyNewMapping(request, mappings)
         );
 
         return request;
      }
 
      private static SortDescriptor RemoveOldMapping(this SortDescriptor oldMapping, DataSourceRequest request)
      {
         request.Sorts.Remove(oldMapping);
 
         return oldMapping;
      }
 
      private static SortDescriptor PrepareDescriptor(string newMapping, SortDescriptor oldMapping)
      {
         return new SortDescriptor(newMapping, oldMapping.SortDirection);
      }
 
      private static IEnumerable<SortDescriptor> SelectAttributesToReplace(this DataSourceRequest request, IDictionary<string, string[]> mappings)
      {
         return request.Sorts.Where(sort => mappings.ContainsKey(sort.Member)).ToList();
      }
 
      private static IEnumerable<string> NewMappings(this IDictionary<string, string[]> mappings, SortDescriptor oldMapping)
      {
         return mappings[oldMapping.Member];
      }
   }
}
