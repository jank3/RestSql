﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RestSql.Dialogs
{
    /// <summary>
    /// Interaction logic for DatabaseConfigDlg.xaml
    /// </summary>
    public partial class DatabaseConfigDlg : UserControl
    {
        public String Title { get; set; }
        public EventHandler Closed;
        public CancelEventHandler Closing;

        public DatabaseConfigDlg()
        {
            InitializeComponent();

            Title = "Database Config";
            Closing += dlg_Closing;
            Closed += dlg_Closed;
        }
        public void Show()
        {
            Dialog dlg = new Dialog();
            dlg.addContent(this);
            dlg.Title = Title;
            dlg.Closing += Closing;
            dlg.Closed += Closed;
            dlg.Show();
        }

        private void dlg_Closed(object sender, EventArgs e)
        {
        }

        private void dlg_Closing(object sender, CancelEventArgs e)
        {
        }
    }
}
