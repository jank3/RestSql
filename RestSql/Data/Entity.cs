using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSql.Data
{
    public class Entity
    {
        public String Name { get; set; }
        protected List<Property> m_Properties = new List<Property>();
        public List<Property> Properties
        {
            get
            {
                return m_Properties;
            }
        }
        public String Description { get; set; }
        public bool GetVisible { get; set; }
        public bool GetAuth { get; set; }
        public List<String> GetAuthGroups { get; set; }
        public bool SaveVisible { get; set; }
        public bool SaveAuth { get; set; }
        public List<String> SaveAuthGroups { get; set; }
        protected Guid m_Hash = Guid.NewGuid();

        public virtual void Load(System.Data.Common.DbConnection dbConn)
        {
            System.Data.DataTable schema = dbConn.GetSchema("Tables",
                new String[]
                {
                    null, // database
                    null, // owner
                    Name // table name
                });
            foreach (System.Data.DataColumn col in schema.Columns)
            {
                Property prop = new Property();
                prop.Name = col.ColumnName;
                prop.Type = col.DataType.ToString();
                prop.Description = col.Caption;

                if (!this.Properties.Contains(prop))
                    this.Properties.Add(prop);
            }
        }

        public override bool Equals(object obj)
        {
            bool test = false;
            if(obj is Entity)
            {
                Entity lhs = obj as Entity;
                if(lhs.Name == this.Name)
                {
                    test = true;
                }
            }
            return test;
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
