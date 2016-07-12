using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace RestSql
{
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
        protected String m_ProjectPath = "";
        public String ProjectPath
        {
            get
            {
                return m_ProjectPath;
            }
            set
            {
                m_ProjectPath = value;
                NotifyPropertyChanged();
            }
        }
        protected bool m_Dirty = false;
        public bool Dirty { get { return m_Dirty; } }

        protected static Settings m_Instance = null;
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

        }

        public void Save()
        {
            m_Dirty = false;
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
