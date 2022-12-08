using ConfiguratorWeb.App.Models;
using Digistat.FrameworkStd.Enums;
using Digistat.FrameworkStd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.EntityBuilders
{
   public static class HospitalUnitEntityBuilder
   {
      public static HospitalUnit Build(HospitalUnitViewModel source)
      {
         HospitalUnit objDest = null;
         try
         {
            if (source != null)
            {
               objDest = new HospitalUnit
               {
                  Beeper = source.Beeper,
                  CellPhone = source.CellPhone,
                  Code = source.Code,
                  CostUnit = source.CostUnit,
                  Description = source.Description,
                  ExternalKey = source.ExternalKey,
                  GUID = source.GUID,
                  InheritsSlots = source.InheritsSlots,
                  Mail = source.Mail,
                  Name = source.Name,
                  Phone = source.Phone,
                  ParentID=source.ParentGUID,
                  ShortName = source.ShortName,
                  Type = Convert.ToInt16((int)source.Type),
                  Version = source.Version,
                  Current = source.Current,
                  rc_ID = source.rc_ID,
                  rc_Version = source.rc_Version
               };
            }
         }
         catch (Exception)
         {

            throw;
         }

         return objDest;
      }
      public static IEnumerable<HospitalUnit> BuildList(IEnumerable<HospitalUnitViewModel> source)
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
