using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for DatabaseTab.xaml
    /// </summary>
    public partial class DatabaseTab : UserControl
    {
        public DatabaseTab()
        {
            InitializeComponent();

            DataContext = new Data.Database() { Name = "New DB"};
            lsb_Databases.Items.Clear();
            lsb_Databases.ItemsSource = Settings.Instance.Databases;

            foreach (Data.DatabaseTypes.DB_TYPES dbType in
                        Enum.GetValues(typeof(Data.DatabaseTypes.DB_TYPES)))
                cmb_DatabaseType.Items.Add(dbType);
        }

        ~DatabaseTab()
        {
        }

        private void btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            btn_Connect.IsEnabled = false;
            if (btn_Connect.Content.ToString() == "Connect")
            {
                int index = lsb_Databases.SelectedIndex;
                if (index > -1 && index < Settings.Instance.Databases.Count)
                {
                    // connect to database
                    try
                    {
                        ((Data.Database)this.DataContext).Connect();
                        if (((Data.Database)this.DataContext).IsOpen)
                        {
                            btn_Connect.Content = "Disconnect";
                        }
                    }
                    catch (Exception ex)
                    {
                        Window parent = null;
                        FrameworkElement ct = this;
                        while (ct != null && !(ct.Parent is Window))
                            ct = (FrameworkElement)ct.Parent;
                        if (ct != null)
                            parent = (Window)ct.Parent;

                        MessageBox.Show(parent,
                            "Error connecting to the database",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please add or select a database first.",
                        "Warning",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            else
            {
                // close database
                ((Data.Database)this.DataContext).Close();
                // clear views
                btn_Connect.Content = "Connect";
            }
            btn_Connect.IsEnabled = true;
        }

        private void lsb_Databases_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // get database connection settings
            int index = lsb_Databases.SelectedIndex;
            if (index > -1 && index < Settings.Instance.Databases.Count)
            {
                DataContext = Settings.Instance.Databases[index];
                txb_Password.Password = Utilities.String.ConvertToUnsecureString(
                    Settings.Instance.Databases[index].Password);

                Settings.Instance.ActiveDatabase = Settings.Instance.Databases[index];
            }
        }

        private void btn_Config_Click(object sender, RoutedEventArgs e)
        {
            // TODO : Create a dialog for the settings of a given database type
        }
        
        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            Data.Database db = (Data.Database)DataContext;
            Settings.Instance.AddDatabase(db);
            int index = Settings.Instance.Databases.IndexOf(db);
            if (index > -1)
                lsb_Databases.SelectedIndex = index;
            else
                MessageBox.Show("Failed to create database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            Utilities.Controls.ConfirmDelete(lsb_Databases, Settings.Instance.Databases);
        }

        private void cmb_DatabaseType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 &&
                e.AddedItems[0] is Data.DatabaseTypes.DB_TYPES)
            {
                this.DataContext = Data.DatabaseTypes.convert(this.DataContext,
                    (Data.DatabaseTypes.DB_TYPES)e.AddedItems[0]);
            }
        }

        private void txb_Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is Data.Database)
            {
                Data.Database db = DataContext as Data.Database;
                Utilities.String.setSecureString(db.Password, txb_Password.Password);
            }
        }
    }
}
