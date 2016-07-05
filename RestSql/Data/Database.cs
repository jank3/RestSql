using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace RestSql.Data
{
    [Serializable]
    public class Database : INotifyPropertyChanged
    {
        protected String m_ServerAddress;
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
        protected String m_Password;
        public String Password
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
        protected DbConnection m_ActiveConnection = null;
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
        protected Guid m_Hash = Guid.NewGuid();

        protected ObservableCollection<Entity> m_Entities = new ObservableCollection<Entity>();
        public ObservableCollection<Entity> Entities
        {
            get
            {
                return m_Entities;
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
