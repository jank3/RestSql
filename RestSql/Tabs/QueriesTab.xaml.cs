using System;
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

namespace RestSql.Tabs
{
    /// <summary>
    /// Interaction logic for ProceduresTab.xaml
    /// </summary>
    public partial class QueriesTab : UserControl
    {
        private BindingList<String> m_Procedures = new BindingList<String>();
        public BindingList<string> Procedures
        {
            get
            {
                return m_Procedures;
            }

            set
            {
                m_Procedures = value;
            }
        }

        protected MySql.Data.MySqlClient.MySqlConnection m_DbConn;

        public QueriesTab()
        {
            InitializeComponent();
        }
        
        private void lsb_Procedures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.DataContext = lsb_Procedures.SelectedItem;
        }

        private void UserControl_LayoutUpdated(object sender, EventArgs e)
        {
            if (Settings.Instance.ActiveDatabase != null)
                lsb_Procedures.ItemsSource = Settings.Instance.ActiveDatabase.Queries;
        }
    }
}
