using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Std.Models.StockManagement
{
    public  class CabinetPositionResourcesDto
    {
        public string  PositionId { get; set; }
        public List<ResourceTreeListTemplate> ResourceTreeListTemplates { get; set; }
    }
}
