using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models
{
   public class ActionUrl
   {

      public string Url { get; set; }
      public string Text { get; set; }
      public string Action { get; set; }
      public string Controller { get; set; }
      public object Parameters { get; set; }
      public bool Active { get; set; }

      public ActionUrl()
          : this("", "")
      {
      }

      public ActionUrl(string url)
      {
         Url = url;
         Active = false;
      }

      public ActionUrl(string action, string controller, object parameters = null, string text = "")
      {
         Action = action;
         Controller = controller;
         Parameters = parameters;
         Text = text;
         Active = false;
      }
   }
}
