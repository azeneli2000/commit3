using ConfiguratorWeb.App.Extensions.Helpers;
using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class StandardParameterEntityBuilder
   {

      public static StandardParameter Build(StandardParameterViewModel source)
      {
         StandardParameter objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new StandardParameter
               {
                  CaseSensitive     = source.CaseSensitive??String.Empty,
                  Classes           = source.Classes??String.Empty,
                  DataType          = source.DataType,
                  Description       = source.Description,
                  Devices           = source.Devices??String.Empty,
                  Id                = source.ID,
                  IsSystem          = source.IsSystem,
                  Mnemonic          = source.Mnemonic??String.Empty,
                  Notes             = source.Notes??String.Empty,
                  Parameters        = source.Parameters??String.Empty,
                  Print             = source.Print,
                  Type              = source.Type??String.Empty,
                  UCUMCaseSensitive = source.UCUMCaseSensitive??String.Empty,
                  UOMIds            = source.UOMIds??String.Empty,
                  
               };
            }
         }
         catch (Exception)
         {
         }

         return objDest;
      }

   


      public static IEnumerable<StandardParameter> BuildList(IEnumerable<StandardParameterViewModel> source)
      {
         try
         {
            return source.Select(Build);
         }
         catch (Exception)
         {

            throw;
         }
      }
   }
}
