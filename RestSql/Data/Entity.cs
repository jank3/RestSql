using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RestSql.Data
{
    [Serializable]
    public class Entity : INotifyPropertyChanged
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
        protected ObservableCollection<Property> m_Properties = new ObservableCollection<Property>();
        public ObservableCollection<Property> Properties
        {
            get
            {
                return m_Properties;
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
        protected bool m_GetVisible = false;
        public bool GetVisible
        {
            get
            {
                return m_GetVisible;
            }
            set
            {
                m_GetVisible = value;
                NotifyPropertyChanged();
            }
        }
        protected bool m_GetAuth = false;
        public bool GetAuth
        {
            get
            {
                return m_GetAuth;
            }
            set
            {
                m_GetAuth = value;
                NotifyPropertyChanged();
            }
        }
        protected ObservableCollection<String> m_GetAuthGroups = new ObservableCollection<String>();
        public ObservableCollection<String> GetAuthGroups
        {
            get
            {
                return m_GetAuthGroups;
            }
            set
            {
                m_GetAuthGroups = value;
            }
        }
        protected bool m_SaveVisible = false;
        public bool SaveVisible
        {
            get
            {
                return m_SaveVisible;
            }
            set
            {
                m_SaveVisible = value;
                NotifyPropertyChanged();
            }
        }
        protected bool m_SaveAuth = false;
        public bool SaveAuth
        {
            get
            {
                return m_SaveAuth;
            }
            set
            {
                m_SaveAuth = value;
                NotifyPropertyChanged();
            }
        }
        protected ObservableCollection<String> m_SaveAuthGroups = new ObservableCollection<String>();
        public ObservableCollection<String> SaveAuthGroups
        {
            get
            {
                return m_SaveAuthGroups;
            }
            set
            {
                m_SaveAuthGroups = value;
            }
        }
        [NonSerialized]
        protected Guid m_Hash = Guid.NewGuid();

        public Entity()
        {
            Properties.CollectionChanged += Properties_CollectionChanged;
            GetAuthGroups.CollectionChanged += GetAuthGroups_CollectionChanged;
            SaveAuthGroups.CollectionChanged += SaveAuthGroups_CollectionChanged;
        }

        private void SaveAuthGroups_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RestSql.Settings.Instance.Dirty = true;
        }

        private void GetAuthGroups_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RestSql.Settings.Instance.Dirty = true;
        }

        private void Properties_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RestSql.Settings.Instance.Dirty = true;
        }

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

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Entity");
            writer.WriteAttributeString("type", this.GetType().ToString());
            writer.WriteStartElement("SaveAuthGroups");
            foreach(String item in SaveAuthGroups)
            {
                writer.WriteStartElement("SaveAuthGroup");
                writer.WriteString(item);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteStartElement("SaveAuth");
            writer.WriteString(SaveAuth.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("SaveVisible");
            writer.WriteString(SaveVisible.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("GetAuthGroups");
            foreach (String item in GetAuthGroups)
            {
                writer.WriteStartElement("GetAuthGroup");
                writer.WriteString(item);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteStartElement("GetAuth");
            writer.WriteString(GetAuth.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("GetVisible");
            writer.WriteString(GetVisible.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("Description");
            writer.WriteString(Description);
            writer.WriteEndElement();
            writer.WriteStartElement("Properties");
            foreach (Property item in Properties)
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

        public static Entity LoadXml(XmlNode node)
        {
            Entity ent = null;
            if(node.Name == "Entity")
            {
                ent = new Data.Entity();
                if (node.Attributes["type"] != null)
                    ent = (Entity)Utilities.Reflection.newType(node.Attributes["type"].Value);
                foreach(XmlNode cNode in node.ChildNodes)
                {
                    bool val = false;
                    switch(cNode.Name)
                    {
                        case "SaveAuthGroups":
                            ent.SaveAuthGroups.Clear();
                            foreach(XmlNode child in cNode.ChildNodes)
                            {
                                ent.SaveAuthGroups.Add(child.InnerText);
                            }
                            break;
                        case "SaveAuth":
                            val = false;
                            bool.TryParse(cNode.InnerText, out val);
                            ent.SaveAuth = val;
                            break;
                        case "SaveVisible":
                            val = false;
                            bool.TryParse(cNode.InnerText, out val);
                            ent.SaveVisible = val;
                            break;
                        case "GetAuthGroups":
                            ent.GetAuthGroups.Clear();
                            foreach (XmlNode child in cNode.ChildNodes)
                            {
                                ent.GetAuthGroups.Add(child.InnerText);
                            }
                            break;
                        case "GetAuth":
                            val = false;
                            bool.TryParse(cNode.InnerText, out val);
                            ent.GetAuth = val;
                            break;
                        case "GetVisible":
                            val = false;
                            bool.TryParse(cNode.InnerText, out val);
                            ent.GetVisible = val;
                            break;
                        case "Description":
                            ent.Description = cNode.InnerText;
                            break;
                        case "Name":
                            ent.Name = cNode.InnerText;
                            break;
                    }
                }
            }
            return ent;
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
