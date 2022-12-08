using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Std.Models.StockManagement
{
    public class OperatingBlockMasterDto
    {
        public int IDLocation { get; set; }
        public string Name { get; set; }        
        public int Index { get; set; }        
        public string Code { get; set; }        
        public string UniteCode { get; set; }        
        public string HuGuid { get; set; }        
    }
}
