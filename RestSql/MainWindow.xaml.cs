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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // check to see if settings as been saved
            // if not prompt user to save
        }

        private void mi_Open_Click(object sender, RoutedEventArgs e)
        {
            // Show folder dialog
            // load selected path
        }

        private void mi_Save_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrEmpty(Settings.Instance.ProjectPath))
            {
                // show save folder dialog
                // set save path
            }
            // save project
        }

        private void mi_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            // show folder dialog
            // save to selected path
        }

        private void mi_Exit_Click(object sender, RoutedEventArgs e)
        {
            // check to see if settings as been saved
            // if not prompt user to save
        }
    }
}
