using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConfiguratorWeb.App.Models
{
    public class DeviceDriverSocketViewModel
    {
        [UIHint("SocketTypeListEditor")]
        [Display(Name = "Socket Type")]
        public string SocketType { get; set; }

         [Display(Name = "Hostname")]
        public string HostName { get; set; }
        public int Port { get; set; }
    }
}