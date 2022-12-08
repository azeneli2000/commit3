using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models
{
   public class ReportTemplateViewModel
   {
      public Guid ID { get; set; }
      public int Version { get; set; }
      public DateTime? ValidToDate { get; set; }
      public bool Current { get; set; }
      public Guid UserID { get; set; }
      public int UserVersion { get; set; }
      public DateTime?  CreationDate { get; set; }
      [Required]
      public string Name { get; set; }
      [Required]
      public string Author { get; set; }
      public string Description { get; set; }
      public string Stream { get; set; }
      [Required]
      public string Filename { get; set; }
      public string Application { get; set; }
      public string Module { get; set; }
      public string FileContent { get; set; }
   }
}
