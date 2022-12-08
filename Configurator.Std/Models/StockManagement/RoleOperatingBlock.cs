using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Std.Models.StockManagement
{
    public class RoleOperatingBlock
    {
        public string OperatingBlockGuid { get; set; }        
        public string OperatingBlockDescription { get; set; }
        public string OperatingBlockName { get; set; }
        [DisplayName("Enabled")]
        public bool Allow { get; set; }
    }
}
