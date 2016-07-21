using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RestSql.Data
{
    [Serializable]
    public class Property : INotifyPropertyChanged
    {
        protected String m_Name = "";
        public String Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
                NotifyPropertyChanged();
            }
        }
        protected String m_Type = "";
        public String Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
                NotifyPropertyChanged();
            }
        }
        protected String m_Direction = "";
        public String Direction
        {
            get
            {
                return m_Direction;
            }
            set
            {
                m_Direction = value;
                NotifyPropertyChanged();
            }
        }
        protected String m_Description = "";
        public String Description
        {
            get
            {
                return m_Description;
            }
            set
            {
                m_Description = value;
                NotifyPropertyChanged();
            }
        }
        [NonSerialized]
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

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Property");
            writer.WriteStartElement("Description");
            writer.WriteString(Description);
            writer.WriteEndElement();
            writer.WriteStartElement("Direction");
            writer.WriteString(Direction);
            writer.WriteEndElement();
            writer.WriteStartElement("Type");
            writer.WriteString(Type);
            writer.WriteEndElement();
            writer.WriteStartElement("Name");
            writer.WriteString(Name);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        public String ToXml()
        {
            StringBuilder xml = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(xml);
            ToXml(writer);
            writer.Flush();
            writer.Close();
            return xml.ToString();
        }

        public static Property LoadXml(XmlNode node)
        {
            Property prop = null;
            if(node.Name == "Property")
            {
                prop = new Property();
                foreach(XmlNode cNode in node.ChildNodes)
                {
                    switch(cNode.Name)
                    {
                        case "Description":
                            prop.Description = cNode.InnerText;
                            break;
                        case "Direction":
                            prop.Direction = cNode.InnerText;
                            break;
                        case "Type":
                            prop.Type = cNode.InnerText;
                            break;
                        case "Name":
                            prop.Name = cNode.InnerText;
                            break;
                    }
                }
            }
            return prop;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            RestSql.Settings.Instance.Dirty = true;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
