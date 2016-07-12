﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace RestSql.Data
{
    public class User
    {
        public String UserName { get; set; }
        public SecureString Password { get; set; }
        protected ObservableCollection<String> m_Groups = new ObservableCollection<String>();
        public ObservableCollection<String> Groups
        {
            get
            {
                return m_Groups;
            }
        }
        public bool ResetPassword { get; set; }
        public bool Disabled { get; set; }
        public bool IsAdmin { get; set; }

        public override string ToString()
        {
            return UserName;
        }
    }
}
