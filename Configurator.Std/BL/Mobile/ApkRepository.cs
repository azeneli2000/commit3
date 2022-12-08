
using Digistat.Dal.Data;
using Digistat.FrameworkStd.Interfaces;
using Digistat.FrameworkStd.Model.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Configurator.Std.BL.Mobile
{
   public class ApkRepository
   {
      private readonly IDigistatConfiguration mobjDigConfig;

      public ApkRepository(IDigistatConfiguration digConfig)
      {
         mobjDigConfig = digConfig;
      }


      public List<DigistatMobileAPK> GetApkList()
      {
     
         using (var context = new DigistatDBContext(mobjDigConfig))
         {
            var data = from b in context.Set<DigistatMobileAPK>()
                        select new DigistatMobileAPK
                        {
                           Version = b.Version,
                           Name = b.Name,
                           File = b.File
                        };
            return data.ToList();
         }
      
      }

      public void Remove(DigistatMobileAPK apk)
      {
         using (var context = new DigistatDBContext(mobjDigConfig))
         {
            context.Set<DigistatMobileAPK>().Remove(apk);
            context.SaveChanges();
         }
      }

      public async Task Add(DigistatMobileAPK apk)
      {
         using (var context = new DigistatDBContext(mobjDigConfig))
         {
            await context.Set<DigistatMobileAPK>().AddAsync(apk);
            await context.SaveChangesAsync();
         }
      }
   }
}
