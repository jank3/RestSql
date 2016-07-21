using System;
using System.Collections.Generic;
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
    /// Interaction logic for UserTab.xaml
    /// </summary>
    public partial class UserTab : UserControl
    {
        public UserTab()
        {
            InitializeComponent();

            DataContext = new Data.User() { UserName = "New user" };
            lsb_Users.Items.Clear();
            lsb_Users.ItemsSource = Settings.Instance.Users;
            cbx_AllowRegistration.DataContext = Settings.Instance;
        }

        private void UserControl_LayoutUpdated(object sender, EventArgs e)
        {

        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            Data.User user = (Data.User)DataContext;
            Settings.Instance.AddUser(user);
            int index = Settings.Instance.Users.IndexOf(user);
            if (index > -1)
                lsb_Users.SelectedIndex = index;
            else
                MessageBox.Show("Failed to create User.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            Utilities.Controls.ConfirmDelete(lsb_Users, Settings.Instance.Users);
        }

        private void lsb_Users_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = lsb_Users.SelectedIndex;
            if (index > -1 && index < Settings.Instance.Users.Count)
            {
                DataContext = Settings.Instance.Users[index];
                txb_Password.Password = Utilities.String.ConvertToUnsecureString(
                    Settings.Instance.Users[index].Password);
            }
        }

        private void btn_SetAuthGroups_Click(object sender, RoutedEventArgs e)
        {
            int index = lsb_Users.SelectedIndex;
            if (index > -1)
            {
                Dialogs.GroupsDlg dlg = new Dialogs.GroupsDlg(
                    Settings.Instance.Users[index]);
                dlg.Show();
            }

        }

        private void txb_Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(DataContext is Data.User)
            {
                Data.User user = DataContext as Data.User;
                Utilities.String.setSecureString(user.Password, txb_Password.Password);
            }
        }
    }
}
