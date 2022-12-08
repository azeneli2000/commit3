using Digistat.FrameworkStd.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models
{
   public class DigistatRepositoryViewModel
   {
      public DigistatRepositoryViewModel()
      {
         
      }

      public string ID { get; set; }
      public string FileName { get; set; }
      public DigistatRepositoryType Type { get; set; }
      public string Application { get; set; }
      public int Size { get; set; }
      public DateTime LastUpdate { get; set; }
      public string Stream { get; set; }
   }
}
