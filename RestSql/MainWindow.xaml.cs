using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RestSql
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected bool promptToSave()
        {
            bool result = Dialogs.Dialog.showMessage(
                            this,
                            "You have unsaved changes.\nDo you want to save them first?",
                            "Warning",
                            Dialogs.Dialog.TYPE.YES_NO);
            return result;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // check to see if settings as been saved
            if (Settings.Instance.Dirty)
            {
                // if not prompt user to save
                bool result = promptToSave();
                if(result)
                {
                    e.Cancel = true;
                }
            }
        }

        private void mi_Open_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Instance.Dirty)
            {
                // prompt to save first
                bool result = promptToSave();
                if (result)
                {
                    save();
                }
            }
            // Show folder dialog
            String savePath = showSaveFolder(true);
            // set save path
            if (String.IsNullOrEmpty(savePath))
            {
                Settings.Instance.ProjectFile = savePath;
                // load selected path
                Settings.Instance.Load();
            }
        }

        private void mi_Save_Click(object sender, RoutedEventArgs e)
        {
            save();
        }

        protected String showSaveFolder(bool checkIfExists = false)
        {
            String savePath = "";
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = checkIfExists;
            dlg.CheckPathExists = true;
            dlg.Multiselect = false;
            dlg.DefaultExt = "proj";
            System.Windows.Forms.DialogResult dResult = dlg.ShowDialog();
            if (dResult == System.Windows.Forms.DialogResult.OK)
            {
                savePath = dlg.FileName;
            }
            return savePath;
        }

        protected void save()
        {
            if (String.IsNullOrEmpty(Settings.Instance.ProjectFile))
            {
                // show save folder dialog
                String savePath = showSaveFolder();
                // set save path
                if (!String.IsNullOrEmpty(savePath))
                    Settings.Instance.ProjectFile = savePath;
            }
            if (!String.IsNullOrEmpty(Settings.Instance.ProjectFile))
            {
                // save project
                Settings.Instance.Save();
            }
            else
                Dialogs.Dialog.showMessage(this, "Error saving file.", "Error");
        }

        private void mi_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            // show folder dialog
            String savePath = showSaveFolder();
            // save to selected path
            if (String.IsNullOrEmpty(savePath))
            {
                Settings.Instance.ProjectFile = savePath;
                Settings.Instance.Save();
            }
        }

        private void mi_Exit_Click(object sender, RoutedEventArgs e)
        {
            // check to see if settings as been saved
            if (Settings.Instance.Dirty)
            {
                // if not prompt user to save
                bool result = promptToSave();
                if (result)
                {
                    save();
                }
            }
        }
    }
}
