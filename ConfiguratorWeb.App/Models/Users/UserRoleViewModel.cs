using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
    public class UserRoleViewModel
    {
      public string UserID { get; set; }

      public int RoleID { get; set; }


      public RoleViewModel RoleRef { get; set; }


   }
}