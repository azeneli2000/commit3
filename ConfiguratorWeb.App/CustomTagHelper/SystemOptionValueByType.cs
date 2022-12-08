//using ConfiguratorWeb.App.Models;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using Microsoft.AspNetCore.Razor.TagHelpers;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.Design;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ConfiguratorWeb.App.CustomTagHelper
//{
//   [HtmlTargetElement("systemoption-value")]
//   public class SystemOptionValueByTypeTagHelper : TagHelper
//   {

//      IDictionaryService mobjDicSvc = null;

//      public SystemOptionValueByTypeTagHelper(IDictionaryService dicsvc)
//      {
//         mobjDicSvc = dicsvc;
//      }

//      [HtmlAttributeName("for")]
//      public ModelExpression For { get; set; }
//      public override void Process(TagHelperContext context, TagHelperOutput output)
//      {
//         StringBuilder sb = new StringBuilder();
//         SystemOptionViewModel objModel = (SystemOptionViewModel)For.Model;
//         if (objModel != null)
//         {

//            string strGuidFunction = (!string.IsNullOrWhiteSpace(objModel.GUID) ? objModel.GUID.Replace("-", "_") : "");
//            TagBuilder objTagBuilder = new TagBuilder("div");
//            objTagBuilder.Attributes.Add("data-sotype", objModel.Type.ToString());
            
//            try
//            {
//               using (var writer = new StringWriter())
//               {
//                  objTagBuilder.InnerHtml.AppendHtml(innerHtml);
                 

//                  switch (objModel.Type)
//                  {
//                     case Enums.OptionTypeEnum.Bool:
//                        return objTagBuilder.InnerHtml.AppendHtml("<input id=\"Value_" + objModel.GUID + "\" type=\"checkbox\" name=\"Value_" + objModel.GUID + "\" data-sotype=\"" + objModel.Type.ToString() + "\" />");
//                     case Enums.OptionTypeEnum.Float:
//                        double dblValueFloat;
//                        bool isDoubleFloat = Double.TryParse(objModel.Value, out dblValueFloat);

//                        return MvcHtmlString.Create(htmlHelper.Kendo().NumericTextBox().Name("Value_" + objModel.GUID).Step(1).Max(1).Min(0).Events(ev => ev.Change("onChangeValue" + strGuidFunction)).Value(dblValueFloat).HtmlAttributes(new { data_sotype = objModel.Type.ToString() }).ToString());
//                     case Enums.OptionTypeEnum.Text:
//                        attributes.Add("style", "width: 88%;height: 72px;max-width: 88%;max-height: 72px;overflow-y:scroll;");
//                        return htmlHelper.TextAreaFor(expression, attributes);
//                     case Enums.OptionTypeEnum.File:
//                        attributes.Add("class", "k-input k-textbox");
//                        return htmlHelper.TextBoxFor(expression, attributes);
//                     case Enums.OptionTypeEnum.Url:
//                        attributes.Add("class", "k-input k-textbox");
//                        return htmlHelper.TextBoxFor(expression, attributes);
//                     case Enums.OptionTypeEnum.Integer:
//                        double dblValue;
//                        bool isDouble = Double.TryParse(objModel.Value, out dblValue);
//                        return MvcHtmlString.Create(htmlHelper.Kendo().NumericTextBox().Name("Value_" + objModel.GUID).Step(1).Max(1).Min(0).Events(ev => ev.Change("onChangeValue" + strGuidFunction)).Value(dblValue).HtmlAttributes(new { data_sotype = objModel.Type.ToString() }).ToString());
//                     case Enums.OptionTypeEnum.Date:
//                        return MvcHtmlString.Create(htmlHelper.Kendo().DatePicker().Name("Value_" + objModel.GUID).Events(ev => ev.Change("onChangeValue" + strGuidFunction)).Value(objModel.Value).HtmlAttributes(new { data_sotype = objModel.Type.ToString() }).ToString());
//                     case Enums.OptionTypeEnum.Time:
//                        return MvcHtmlString.Create(htmlHelper.Kendo().TimePicker().Name("Value_" + objModel.GUID).Events(ev => ev.Change("onChangeValue" + strGuidFunction)).Format("HH:mm:ss").Value(objModel.Value).HtmlAttributes(new { data_sotype = objModel.Type.ToString() }).ToString());
//                     case Enums.OptionTypeEnum.Path:
//                        attributes.Add("class", "k-input k-textbox");
//                        return MvcHtmlString.Create(htmlHelper.Kendo().TextBoxFor(expression).HtmlAttributes(new { data_sotype = objModel.Type.ToString(), style = "width: 89%;" }).ToString());
//                     case Enums.OptionTypeEnum.Binary:
//                        attributes.Add("class", "k-input k-textbox");
//                        return MvcHtmlString.Create(htmlHelper.Kendo().TextBoxFor(expression).HtmlAttributes(new { data_sotype = objModel.Type.ToString(), style = "width: 89%;" }).ToString());
//                     default:
//                        attributes.Add("class", "k-input k-textbox");
//                        return MvcHtmlString.Create(htmlHelper.Kendo().TextBoxFor(expression).HtmlAttributes(new { data_sotype = objModel.Type.ToString(), style = "width: 89%;" }).ToString());
//                  }

//                  objTagBuilder.InnerHtml.AppendHtml(writer.ToString());
//               }
//            }
//            catch (Exception ex)
//            {

//            }
//            return MvcHtmlString.Create("<input id=\"IsSystem_" + guid + "\" type=\"checkbox\" name=\"IsSystem_" + guid + " data-sotype=\"" + soType.ToString() + "\"/>"); 
//         }
//      }
//   }
//}
