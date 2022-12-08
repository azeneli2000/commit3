using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace ConfiguratorWeb.App.Models
{
   public class ActualDeviceImageViewModel
   {
      public ActualDeviceImageViewModel() {
         IsNewRecord = true;
      }

      public bool IsNewRecord { get; set; }
      [Required(ErrorMessage = "The Type field is required.")]
      public int DeviceType { get; set; }
      public string DeviceTypeDescription { get; set; }
      public string DeviceName { get; set; }
      public string DeviceSerialNumber { get; set; }
      public string Extension { get; set; }
      public string Thumbnail { get; set; }
      public string Image { get; set; }
      public IFormFile ImageFile { get; set; }

      /// <summary>
      /// Used to identify location for temporary cached files
      /// </summary>
      [Required(ErrorMessage = "The image file is required")]
      public string BinariesCacheIdentifier { get; set; }
      public string ImageStatusDescription { get; set; }

   }
}