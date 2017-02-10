using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Object : INotifyPropertyChanged
    {
        protected Dictionary<String, object> m_Properties = new Dictionary<String, object>();
        protected bool m_HasChanged = false;
        public bool HasChanged { get { return m_HasChanged; } set
            {
                m_HasChanged = value;
            } }

        public object get(String name)
        {
            object obj = null;
            if (m_Properties.ContainsKey(name))
                obj = m_Properties[name];
            return obj;
        }

        public long set(String name, object value)
        {
            long status = -1;
            m_Properties[name] = value;
            NotifyPropertyChanged(name);
            return status;
        }

        public String[] getPropertyNames()
        {
            String[] keys = new String[m_Properties.Keys.Count];
            m_Properties.Keys.CopyTo(keys, 0);
            return keys;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            this.m_HasChanged = true;
        }
    }
}
