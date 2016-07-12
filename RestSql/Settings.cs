using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RestSql
{
    [Serializable]
    public class Settings : INotifyPropertyChanged
    {
        
        protected ObservableCollection<Data.User> m_Users = new ObservableCollection<Data.User>();
        public ObservableCollection<Data.User> Users
        {
            get
            {
                return m_Users;
            }
        }
        protected ObservableCollection<Data.Database> m_Databases = new ObservableCollection<Data.Database>();
        public ObservableCollection<Data.Database> Databases
        {
            get
            {
                return m_Databases;
            }
        }
        protected ObservableCollection<String> m_Groups = new ObservableCollection<String>();
        public ObservableCollection<String> Groups
        {
            get
            {
                return m_Groups;
            }
        }
        protected bool m_UserRegistration = false;
        public bool UserRegistration
        {
            get
            {
                return m_UserRegistration;
            }
            set
            {
                m_UserRegistration = value;
                NotifyPropertyChanged();
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public Data.Database ActiveDatabase
        {
            get
            {
                Data.Database db = null;
                foreach(Data.Database db2 in m_Databases)
                {
                    if(db2.IsOpen)
                    {
                        db = db2;
                        break;
                    }
                }
                return db;
            }
        }
        [NonSerialized]
        protected String m_ProjectFile = "";
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public String ProjectFile
        {
            get
            {
                return m_ProjectFile;
            }
            set
            {
                m_ProjectFile = value;
                NotifyPropertyChanged();
            }
        }
        [NonSerialized]
        protected bool m_Dirty = false;
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public bool Dirty
        {
            get
            {
                return m_Dirty;
            }
            set
            {
                m_Dirty = value;
            }
        }

        [NonSerialized]
        protected static Settings m_Instance = null;
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public static Settings Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new Settings();
                return m_Instance;
            }
        }

        protected Settings()
        {
            Users.CollectionChanged += Users_CollectionChanged;
            Databases.CollectionChanged += Databases_CollectionChanged;
            Groups.CollectionChanged += Groups_CollectionChanged;
        }

        private void Groups_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Dirty = true;
        }

        private void Databases_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Dirty = true;
        }

        private void Users_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Dirty = true;
        }

        public void AddDatabase(Data.Database db)
        {
            if (!Databases.Contains(db))
                Databases.Add(db);
        }

        public void ReplaceDatabase(Data.Database db)
        {
            int index = Databases.IndexOf(db);
            if (index > -1)
                Databases.RemoveAt(index);
            AddDatabase(db);
        }

        public void AddUser(Data.User user)
        {
            if (!Users.Contains(user))
                Users.Add(user);
        }

        public void ReplaceUser(Data.User user)
        {
            int index = Users.IndexOf(user);
            if (index > -1)
                Users.RemoveAt(index);
            AddUser(user);
        }

        public void Load()
        {
            m_Dirty = false;
            if (File.Exists(ProjectFile))
            {
                String xml = File.ReadAllText(ProjectFile);
                this.LoadXml(xml);
            }
        }

        public void Save()
        {
            m_Dirty = false;
            //if(File.Exists(ProjectFile))
            {
                String xml = this.ToXml();
                if (File.Exists(ProjectFile))
                    File.Delete(ProjectFile);
                File.WriteAllText(ProjectFile, xml);
            }
        }

        public String ToXml()
        {
            StringBuilder xml = new StringBuilder();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",
                NewLineOnAttributes = true
            };
            XmlWriter writer = XmlWriter.Create(xml, xmlWriterSettings);
            writer.WriteStartDocument();
            writer.WriteStartElement("Settings");
            writer.WriteStartElement("UserRegistration");
            writer.WriteString(this.UserRegistration.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("Users");
            foreach (Data.User item in Users)
            {
                item.ToXml(writer);
            }
            writer.WriteEndElement();
            writer.WriteStartElement("Databases");
            foreach (Data.Database item in Databases)
            {
                item.ToXml(writer);
            }
            writer.WriteEndElement();
            writer.WriteStartElement("Groups");
            foreach (String item in Groups)
            {
                writer.WriteStartElement("Group");
                writer.WriteString(item);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
            return xml.ToString();
        }

        public void LoadXml(String xml)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xml);

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
