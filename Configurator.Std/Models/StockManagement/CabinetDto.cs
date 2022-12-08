using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Std.Models.StockManagement
{
    public  class CabinetDto
    {
        public string CabinetName { get; set; }
        public string CabinetShortName { get; set; }
        public string CabinetDescription { get; set; }
        public int CabinetIndex { get; set; }
        public string CabinetNumLocations { get; set; }
        public string CabinetId { get; set; }
        public string CabinetGroupId { get; set; }
        public bool CabinetIsBasket { get; set; }
        public bool CabinetIsTrolley { get; set; }
        public bool CabinetIsGenericKit { get; set; }
        public bool CabinetIsForNewPosition { get; set; }
    }
}
