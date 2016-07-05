using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSql.Data
{
    public class Property
    {
        public String Name { get; set; }
        public String Type { get; set; }
        public String Description { get; set; }
        protected Guid m_Hash = Guid.NewGuid();

        public override bool Equals(object obj)
        {
            bool test = false;
            if(obj is Property)
            {
                Property lhs = obj as Property;
                if(lhs.Name == this.Name &&
                    lhs.Type == this.Type)
                {
                    test = true;
                }
            }
            return test;
        }

        public override int GetHashCode()
        {
            return BitConverter.ToInt32(m_Hash.ToByteArray(), 0);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
