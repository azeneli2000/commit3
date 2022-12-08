using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
    public class UserPermissionViewModel
    {
      public string PermissionName { get; set; }
      public bool Allow { get; set; }

   }
}