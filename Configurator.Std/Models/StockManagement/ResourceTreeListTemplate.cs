using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Std.Models.StockManagement
{
    public class ResourceTreeListTemplate
    {

        public string ResourceId { get; set; }
        public string ResourceCode { get; set; }
        public string ResourceShortName { get; set; }
        public string ResourceName { get; set; }
        public string ResourceDescription { get; set; }
        public string MainStore { get; set; }
        public double? IdealQuantity { get; set; }
        public double? AlarmQuantity { get; set; }
        public double? MinQuantity { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public string SpGuid { get; set; }


    }
}
