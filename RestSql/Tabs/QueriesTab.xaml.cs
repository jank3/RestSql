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
            lsb_Procedures.DataContext = this;
        }

        public void loadProcedures(MySql.Data.MySqlClient.MySqlConnection dbConn)
        {
            if (dbConn != null && dbConn.State == System.Data.ConnectionState.Open)
            {
                m_DbConn = dbConn;
                // get procedures
                String sql = "SHOW PROCEDURE STATUS;";
                var cmd = m_DbConn.CreateCommand();
                cmd.CommandText = sql;
                var reader = cmd.ExecuteReader();
                Procedures.Clear();
                while (reader.HasRows && reader.Read())
                {
                    Procedures.Add(reader[1].ToString());
                }
                reader.Close();
            }
        }

        private void lsb_Procedures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // get procedure description
        }
    }
}
