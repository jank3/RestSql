using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSql.Data
{
    public class Query
    {
        public String Name { get; set; }
        public List<Property> Parameters { get; set; }
        public String ReturnType { get; set; }
        public String Description { get; set; }
    }
}
