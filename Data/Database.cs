using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Data
{
    [Serializable]
    public class Database : INotifyPropertyChanged
    {
#if DEBUG
        protected bool m_DebugInfo = true;
#else
        protected bool m_DebugInfo = false;
#endif
        protected String m_ServerAddress;
        public long m_MaxConnections = long.MaxValue;
        private System.Object m_OpenConnectionsLock = new System.Object();
        private long m_OpenConnections = 0;
        public long OpenConnections { get { return m_OpenConnections; } }
        protected void incrementOpenConnections()
        {
            lock (m_OpenConnectionsLock)
                m_OpenConnections++;
        }
        protected void decrementOpenConnections()
        {
            lock (m_OpenConnectionsLock)
            {
                m_OpenConnections--;
                if (m_OpenConnections < 0)
                    m_OpenConnections = 0;
            }
        }
        public String ServerAddress
        {
            get
            {
                return m_ServerAddress;
            }
            set
            {
                m_ServerAddress = value;
                NotifyPropertyChanged();
            }
        }
        protected String m_Name;
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

        protected String m_UserName;
        public String UserName
        {
            get
            {
                return m_UserName;
            }
            set
            {
                m_UserName = value;
                NotifyPropertyChanged();
            }
        }
        protected SecureString m_Password = new SecureString();
        public SecureString Password
        {
            get
            {
                return m_Password;
            }
            set
            {
                m_Password = value;
                NotifyPropertyChanged();
            }
        }
        protected ConnectionSettings m_Settings = null;
        public ConnectionSettings Settings
        {
            get
            {
                return m_Settings;
            }
            set
            {
                m_Settings = value;
                NotifyPropertyChanged();
            }
        }
        protected DatabaseTypes.DB_TYPES m_DatabaseType = DatabaseTypes.DB_TYPES.UNKNOWN;
        public DatabaseTypes.DB_TYPES DatabaseType
        {
            get
            {
                return m_DatabaseType;
            }
            set
            {
                m_DatabaseType = value;
                NotifyPropertyChanged();
            }
        }
        [NonSerialized]
        protected DbConnection m_ActiveConnection = null;
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public DbConnection ActiveConnection
        {
            get
            {
                return m_ActiveConnection;
            }
            set
            {
                m_ActiveConnection = value;
                NotifyPropertyChanged();
            }
        }
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public bool IsOpen
        {
            get
            {
                bool open = false;
                if (ActiveConnection != null)
                    open = (ActiveConnection.State == System.Data.ConnectionState.Open);
                return open;
            }
        }
        [NonSerialized]
        protected Guid m_Hash = Guid.NewGuid();

        protected ObservableCollection<Entity> m_Entities = new ObservableCollection<Entity>();
        public ObservableCollection<Entity> Entities
        {
            get
            {
                return m_Entities;
            }
        }

        protected ObservableCollection<Query> m_Queries = new ObservableCollection<Query>();
        public ObservableCollection<Query> Queries
        {
            get
            {
                return m_Queries;
            }
        }

        public Database()
        {

        }

        public Database(Database db)
        {
            Clone(db);
        }

        ~Database()
        {
            Close();
        }

        public virtual void Connect()
        {
            Close();
            ActiveConnection = (DbConnection)Activator.CreateInstance(this.getDatabaseType());
            ActiveConnection.ConnectionString = this.getConnectionString();
            ActiveConnection.Open();
        }

        public virtual DbConnection ConnectAsync()
        {
            DbConnection dbConn = null; 
            do
            {
                try
                {
                    dbConn = (DbConnection)Activator.CreateInstance(this.getDatabaseType());
                    dbConn.ConnectionString = this.getConnectionString();
                    if (OpenConnections < m_MaxConnections)
                    {
                        dbConn.Open();
                        incrementOpenConnections();
                    }
                    else
                    {
                        throw new Exception("Too many connections.");
                    }
                }
                catch (Exception ex)
                {
                    if(m_DebugInfo)
                        Console.WriteLine("Failed to connect.  " + ex.Message);
                    Task.Delay(200).Wait();
                    try
                    {
                        dbConn.Close();
                    }
                    finally
                    {
                        dbConn.Dispose();
                    }
                }
            } while (dbConn == null || dbConn.State != System.Data.ConnectionState.Open);
            return dbConn;
        }

        public virtual void Close()
        {
            if(ActiveConnection != null &&
                ActiveConnection.State == System.Data.ConnectionState.Open)
            {
                ActiveConnection.Close();
            }
        }

        public virtual string getConnectionString()
        {
            throw new NotImplementedException("Must override the Connection String function");
        }

        public virtual void LoadEntities()
        {
            throw new NotImplementedException("Must override the LoadEntities function");
        }

        public virtual void LoadQueries()
        {
            throw new NotImplementedException("Must override the LoadQueries function");
        }

        public virtual int save(String tableName, Data.Object row)
        {
            throw new NotImplementedException("Must override the save function");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parms"></param>
        /// <returns>rows[colum_values]</returns>
        public virtual System.Data.DataTable CallQuery(string name, List<object> parms)
        {
            throw new NotImplementedException("Must override the CallQuery function");
        }

        public virtual System.Data.DataTable getFiltered(String tableName,
                filtering.BaseFilter filter = null,
                filtering.Sort sort = null,
                filtering.Paging page = null)
        {
            throw new NotImplementedException("Must override the getFiltered function");
        }

        public Type getDatabaseType()
        {
            Type type = DatabaseTypes.convert(DatabaseType);
            return type;
        }

        public void Clone(Database db)
        {
            this.ServerAddress = db.ServerAddress;
            this.Name = db.Name;
            this.UserName = db.UserName;
            this.Password = db.Password;
            this.Settings = db.Settings;
            this.DatabaseType = db.DatabaseType;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            bool test = false;
            if(obj is Database)
            {
                Database lhs = obj as Database;
                if(lhs.Name == this.Name &&
                    lhs.ServerAddress == this.ServerAddress &&
                    lhs.UserName == this.UserName)
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

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Database");
            writer.WriteAttributeString("type", this.GetType().ToString());
            writer.WriteStartElement("DatabaseType");
            writer.WriteString(Enum.GetName(typeof(DatabaseTypes.DB_TYPES), DatabaseType));
            writer.WriteEndElement();
            if(Settings != null)
                Settings.ToXml(writer);
            writer.WriteStartElement("Password");
            if(Password != null)
                writer.WriteString(Utilities.StringEx.ConvertToUnsecureString(Password));
            writer.WriteEndElement();
            writer.WriteStartElement("UserName");
            writer.WriteString(UserName);
            writer.WriteEndElement();
            writer.WriteStartElement("Name");
            writer.WriteString(Name);
            writer.WriteEndElement();
            writer.WriteStartElement("ServerAddress");
            writer.WriteString(ServerAddress);
            writer.WriteEndElement();

            writer.WriteStartElement("Entities");
            foreach (Data.Entity item in Entities)
            {
                item.ToXml(writer);
            }
            writer.WriteEndElement();
            writer.WriteStartElement("Queries");
            foreach (Data.Query item in Queries)
            {
                item.ToXml(writer);
            }
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

        public static Database LoadXml(XmlNode node)
        {
            Database db = null;
            if(node.Name == "Database")
            {
                db = new Database();
                if (node.Attributes["type"] != null)
                    db = (Database)Utilities.Reflection.newType(node.Attributes["type"].Value);
                foreach (XmlNode cNode in node.ChildNodes)
                {
                    switch(cNode.Name)
                    {
                        case "DatabaseType":
                            db.DatabaseType = DatabaseTypes.DB_TYPES.UNKNOWN;
                            if (cNode.InnerText != null)
                            {
                                db.DatabaseType = DatabaseTypes.convert(cNode.InnerText);
                            }
                            break;
                        case "Password":
                            Utilities.StringEx.setSecureString(db.Password, cNode.InnerText);
                            break;
                        case "UserName":
                            db.UserName = cNode.InnerText;
                            break;
                        case "Name":
                            db.Name = cNode.InnerText;
                            break;
                        case "ServerAddress":
                            db.ServerAddress = cNode.InnerText;
                            break;
                        case "ConnectionSettings":
                            db.Settings.LoadXml(cNode);
                            break;
                        case "Entities":
                            db.Entities.Clear();
                            foreach (XmlNode child in cNode.ChildNodes)
                            {
                                db.Entities.Add(Data.Entity.LoadXml(child));
                            }
                            break;
                        case "Queries":
                            db.Queries.Clear();
                            foreach (XmlNode child in cNode.ChildNodes)
                            {
                                db.Queries.Add(Data.Query.LoadXml(child));
                            }
                            break;
                    }
                }
            }
            return db;
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
