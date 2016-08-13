using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Data
{
    [Serializable]
    public class Query : INotifyPropertyChanged
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
        protected ObservableCollection<Property> m_Parameters = new ObservableCollection<Property>();
        public ObservableCollection<Property> Parameters
        {
            get
            {
                return m_Parameters;
            }
            set
            {
                m_Parameters = value;
                NotifyPropertyChanged();
            }
        }
        protected String m_ReturnType = "";
        public String ReturnType
        {
            get
            {
                return m_ReturnType;
            }
            set
            {
                m_ReturnType = value;
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
        protected bool m_Visible = false;
        public bool Visible
        {
            get
            {
                return m_Visible;
            }
            set
            {
                m_Visible = value;
                NotifyPropertyChanged();
            }
        }
        protected bool m_Auth = false;
        public bool Auth
        {
            get
            {
                return m_Auth;
            }
            set
            {
                m_Auth = value;
                NotifyPropertyChanged();
            }
        }
        protected ObservableCollection<String> m_AuthGroups = new ObservableCollection<String>();
        public ObservableCollection<String> AuthGroups
        {
            get
            {
                return m_AuthGroups;
            }
            set
            {
                m_AuthGroups = value;
            }
        }
        [NonSerialized]
        protected Guid m_Hash = Guid.NewGuid();

        public Query()
        {
            AuthGroups.CollectionChanged += AuthGroups_CollectionChanged;
        }

        private void AuthGroups_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("AuthGroups");
        }

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


        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Query");
            writer.WriteAttributeString("type", this.GetType().ToString());
            writer.WriteStartElement("AuthGroups");
            foreach(String item in AuthGroups)
            {
                writer.WriteStartElement("AuthGroup");
                writer.WriteString(item);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteStartElement("Auth");
            writer.WriteString(Auth.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("Visible");
            writer.WriteString(Visible.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("Description");
            writer.WriteString(Description);
            writer.WriteEndElement();
            writer.WriteStartElement("ReturnType");
            writer.WriteString(ReturnType);
            writer.WriteEndElement();
            writer.WriteStartElement("Parameters");
            foreach (Property item in Parameters)
            {
                if(item != null)
                    item.ToXml(writer);
            }
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

        public static Query LoadXml(XmlNode node)
        {
            Query q = null;
            if(node.Name == "Query")
            {
                q = new Query();
                if (node.Attributes["type"] != null)
                    q = (Query)Utilities.Reflection.newType(node.Attributes["type"].Value);
                foreach (XmlNode cNode in node.ChildNodes)
                {
                    bool val = false;
                    switch (cNode.Name)
                    {
                        case "AuthGroups":
                            q.AuthGroups.Clear();
                            foreach (XmlNode child in cNode.ChildNodes)
                            {
                                q.AuthGroups.Add(child.InnerText);
                            }
                            break;
                        case "Auth":
                            val = false;
                            bool.TryParse(cNode.InnerText, out val);
                            q.Auth = val;
                            break;
                        case "Visible":
                            val = false;
                            bool.TryParse(cNode.InnerText, out val);
                            q.Visible = val;
                            break;
                        case "Description":
                            q.Description = cNode.InnerText;
                            break;
                        case "ReturnType":
                            q.ReturnType = cNode.InnerText;
                            break;
                        case "Parameters":
                            q.Parameters.Clear();
                            foreach (XmlNode child in cNode.ChildNodes)
                            {
                                q.Parameters.Add(Property.LoadXml(child));
                            }
                            break;
                        case "Name":
                            q.Name = cNode.InnerText;
                            break;
                    }
                }
            }
            return q;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
