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

namespace RestSql.Dialogs
{
    /// <summary>
    /// Interaction logic for GroupsDlg.xaml
    /// </summary>
    public partial class GroupsDlg : UserControl
    {
        public String Title { get; set; }
        public EventHandler Closed;
        public CancelEventHandler Closing;
        protected Data.User m_User = null;
        public bool DialogResult { get; set; }

        public GroupsDlg(Data.User user)
        {
            InitializeComponent();

            if (user == null)
                throw new NullReferenceException("User cannot be null.");
            m_User = user;

            Title = "Groups";
            Closing += dlg_Closing;
            Closed += dlg_Closed;
            btn_RemoveGroup.Content = "<";
            lsb_AvGroups.ItemsSource = Settings.Instance.Groups;
            lsb_UserGroups.ItemsSource = user.Groups;
        }

        public void Show()
        {
            Dialog dlg = new Dialog();
            dlg.setSize(480, 340);
            dlg.addContent(this);
            dlg.Title = Title;
            dlg.Closing += Closing;
            dlg.Closed += Closed;
            DialogResult = (bool)dlg.ShowDialog();
        }

        private void dlg_Closed(object sender, EventArgs e)
        {
        }

        private void dlg_Closing(object sender, CancelEventArgs e)
        {
        }

        private void btn_AddAvGroup_Click(object sender, RoutedEventArgs e)
        {
            String input = "";
            input = Dialog.showInput("Group Name", "Add Group");
            if (!String.IsNullOrEmpty(input))
            {
                if (!Settings.Instance.Groups.Contains(input))
                    Settings.Instance.Groups.Add(input);
            }
        }

        private void btn_DeleteAvGroup_Click(object sender, RoutedEventArgs e)
        {
            Utilities.Controls.ConfirmDelete(lsb_AvGroups, Settings.Instance.Groups);
        }

        private void btn_AddGroup_Click(object sender, RoutedEventArgs e)
        {
            int index = lsb_AvGroups.SelectedIndex;
            if(index > -1)
            {
                if(!m_User.Groups.Contains(
                    Settings.Instance.Groups[index]))
                {
                    m_User.Groups.Add(
                        Settings.Instance.Groups[index]);
                }
            }
        }

        private void btn_RemoveGroup_Click(object sender, RoutedEventArgs e)
        {
            int index = lsb_UserGroups.SelectedIndex;
            if(index > -1)
            {
                m_User.Groups.RemoveAt(index);
                lsb_UserGroups.SelectedIndex = -1;
            }
        }
    }
}
