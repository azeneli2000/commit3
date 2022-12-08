using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Std.Models.StockManagement
{
    public class RoleOperatingRoom
    {
        public string OperatingRoomGuid { get; set; }
        public string OperatingRoomDescription { get; set; }        
        public string OperatingBlockName { get; set; }
        public string OperatingRoomName { get; set; }
        [DisplayName("Enabled")]
        public bool Allow { get; set; }
    }
}
