using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
   public class ActualDeviceViewModel
   {
      public int Id { get; set; }
      public int DeviceType { get; set; }
      [Display(Name = "Type")]
      public string DeviceTypeDescription { get; set; }
      public string Name { get; set; }
      public string SerialNumber { get; set; }
      public string Label { get; set; }
      public bool Mobile { get; set; }
      public string Thumbnail { get; set; }
      public string Extension { get; set; }
   }
}