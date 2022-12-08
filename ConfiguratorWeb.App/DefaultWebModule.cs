using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.ControlBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App
{
   public class DefaultWebModule : IDefaultWebModule
   {

      private readonly IDigistatConfiguration mobjDigConfig;
      public DefaultWebModule(IDigistatConfiguration digCfg)
      {
         mobjDigConfig = digCfg;
      }

      /// <summary>
      /// WebModuleConfig should retrieve a WebModule object with buttons included. Currently ModuleName is provided by WebModule itself, 
      /// in a future version it should be applied directly by engine. So it is reccomended to set the ModuleName exactly like IDigistatConfiguration.ModuleName
      /// to avoid issues.
      /// </summary>
      public WebModule WebModuleConfig
      {
         get
         {
            WebModule objModule = new WebModule()
            {
               ModuleName = mobjDigConfig.ModuleName,
               //Url = "http:// /" ** Not mandatory, can be automatically generated
            };
            return objModule;
         }
      }
   }
}
