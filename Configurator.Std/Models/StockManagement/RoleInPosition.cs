using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Std.Models.StockManagement
{

    public class RoleInPosition
    {
        public string ScGuid { get; set; }
        public string StockRoomName { get; set; }
        public string CabinetGroupName { get; set; }
        public string CabinetName { get; set; }
        [DisplayName("Enabled")]
        public bool Allow { get; set; }
        public bool IsTrolley { get; set; }
        public bool IsBasket { get; set; }
        public bool IsGenericKit { get; set; }
    }
}
