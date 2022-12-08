using Configurator.Std.BL;
using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfiguratorWeb.App.ViewModelBuilders
{
   public static class HospitalUnitViewModelBuilder
   {
      public static HospitalUnitViewModel Build(HospitalUnit source)
      {
         HospitalUnitViewModel objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new HospitalUnitViewModel
               {
                  GUID = source.GUID,
                  Beeper = source.Beeper,
                  CellPhone = source.CellPhone,
                  Code = source.Code,
                  CostUnit = source.CostUnit,
                  Description = source.Description,
                  ExternalKey = source.ExternalKey,
                  InheritsSlots = source.InheritsSlots,
                  Mail = source.Mail,
                  Name = source.Name,
                  ParentGUID = source.ParentID,
                  Phone = source.Phone,
                  ShortName = source.ShortName,
                  Type = (HospitalUnitType)source.Type,
                  Version = source.Version,
                  Current = source.Current,
                  rc_ID = source.rc_ID,
                  rc_Version = source.rc_Version,
                  ParentUnit = source.Parent != null ? source.Parent.ShortName : String.Empty,
               };
            }
         }
         catch (Exception)
         {
            throw;
         }

         return objDest;
      }

      public static IEnumerable<HospitalUnitViewModel> BuildList(IEnumerable<HospitalUnit> source)
      {
         try
         {
            return source.Select(m => Build(m));
         }
         catch (Exception)
         {

            throw;
         }
      }
   }
}
