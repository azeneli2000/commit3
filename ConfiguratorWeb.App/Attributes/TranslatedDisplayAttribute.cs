using ConfiguratorWeb.App.Helpers;
using Digistat.FrameworkStd.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Attributes
{
   public class TranslatedDisplayAttribute : DisplayNameAttribute
   {

      private readonly IDictionaryService mobjDicSvc;

      public TranslatedDisplayAttribute(IDictionaryService dicSvc)
      {
         mobjDicSvc = dicSvc;
      }

      public TranslatedDisplayAttribute(string expression) : base(expression)
      {
         mobjDicSvc = (IDictionaryService) AppHttpContext.Services.GetService(typeof(IDictionaryService));
      }


      public override string DisplayName
      {
         get
         {
            return mobjDicSvc.XLate(base.DisplayName);
         }
      }
   }
}
