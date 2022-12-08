using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Std4Stock.Models.StockManagement
{
    public class RoleUpdateRequest
    {
        public RoleUpdateRequest()
        {
            insert = new List<string>();
            delete = new List<string>();
        }
        public List<string> insert { get; set; }
        public List<string> delete { get; set; }
        public int roleId { get; set; }


    }
}
