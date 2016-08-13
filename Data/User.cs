using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Data
{
    [Serializable]
    public class User : INotifyPropertyChanged
    {
        protected String m_UserName = "";
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
        public SecureString m_Password = new SecureString();
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
        protected ObservableCollection<String> m_Groups = new ObservableCollection<String>();
        public ObservableCollection<String> Groups
        {
            get
            {
                return m_Groups;
            }
        }
        protected bool m_ResetPassword = false;
        public bool ResetPassword
        {
            get
            {
                return m_ResetPassword;
            }
            set
            {
                m_ResetPassword = value;
                NotifyPropertyChanged();
            }
        }
        protected bool m_Disabled = false;
        public bool Disabled
        {
            get
            {
                return m_Disabled;
            }
            set
            {
                m_Disabled = value;
                NotifyPropertyChanged();
            }
        }
        protected bool m_IsAdmin = false;
        public bool IsAdmin
        {
            get
            {
                return m_IsAdmin;
            }
            set
            {
                m_IsAdmin = value;
                NotifyPropertyChanged();
            }
        }
        [NonSerialized]
        protected Guid m_Hash = Guid.NewGuid();

        public User()
        {
            Groups.CollectionChanged += Groups_CollectionChanged;
        }

        private void Groups_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("Groups");
        }

        public override string ToString()
        {
            return UserName;
        }

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("User");
            writer.WriteStartElement("IsAdmin");
            writer.WriteString(IsAdmin.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("Disabled");
            writer.WriteString(Disabled.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("ResetPassword");
            writer.WriteString(ResetPassword.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("Groups");
            foreach(String item in Groups)
            {
                writer.WriteStartElement("Group");
                writer.WriteString(item);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteStartElement("Password");
            if(Password != null)
                writer.WriteString(Utilities.StringEx.ConvertToUnsecureString(Password));
            writer.WriteEndElement();
            writer.WriteStartElement("UserName");
            writer.WriteString(UserName);
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

        public static User LoadXml(XmlNode node)
        {
            User user = null;
            if(node.Name == "User")
            {
                user = new User();
                foreach(XmlNode cNode in node.ChildNodes)
                {
                    bool val = false;
                    switch (cNode.Name)
                    {
                        case "IsAdmin":
                            bool.TryParse(cNode.InnerText, out val);
                            user.IsAdmin = val;
                            break;
                        case "Disabled":
                            val = false;
                            bool.TryParse(cNode.InnerText, out val);
                            user.Disabled = val;
                            break;
                        case "ResetPassword":
                            val = false;
                            bool.TryParse(cNode.InnerText, out val);
                            user.ResetPassword = val;
                            break;
                        case "Groups":
                            user.Groups.Clear();
                            foreach (XmlNode child in cNode.ChildNodes)
                            {
                                user.Groups.Add(child.InnerText);
                            }
                            break;
                        case "Password":
                            Utilities.StringEx.setSecureString(user.Password, cNode.InnerText);
                            break;
                        case "UserName":
                            user.UserName = cNode.InnerText;
                            break;
                    }
                }
            }
            return user;
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
