using System;
using System.Collections.Generic;
using System.Linq;
using ConfiguratorWeb.App.Models.OnLine;
using Digistat.FrameworkStd.Model.Ips;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class WaveformSnapshotToUniteRuleViewModelBuilder
   {
      public static WaveformSnapshotToUniteRuleViewModel Build(WaveformSnapshotToUniteRule source)
      {
         WaveformSnapshotToUniteRuleViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new WaveformSnapshotToUniteRuleViewModel
               {
                  Id = source.Id,
                  Priority = source.Priority,
                  IdDriver = source.IdDriver,
                  IdLocation = source.IdLocation,
                  IdLinkEvent = source.IdLinkEvent,
                  IdParam = source.IdParam,
                  Description = source.Description,
                  DriverName = source.Repository?.DriverName ?? "",
                  EventName = source.Event?.DescriptionLong ?? "",
                  Parameter = source.Parameter,
                  Location = source.Location
               };
            }
         }
         catch (Exception)
         {
            throw;
         }
         return objDest;
      }

      public static IEnumerable<WaveformSnapshotToUniteRuleViewModel> BuildList(IEnumerable<WaveformSnapshotToUniteRule> source/*, IDictionaryService dictionarySrv*/)
      {
         try
         {
            return source.Select(x => Build(x/*, dictionarySrv*/));
         }
         catch (Exception)
         {
            throw;
         }
      }

   }
}