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
        public bool Visible { get; set; }
        public bool Auth { get; set; }
        public List<String> AuthGroups { get; set; }
        protected Guid m_Hash = Guid.NewGuid();


        public virtual void Load(System.Data.Common.DbConnection dbConn)
        {
        }

        public override int GetHashCode()
        {
            return BitConverter.ToInt32(this.m_Hash.ToByteArray(), 0);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
