using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace RestSql.Tabs
{
    /// <summary>
    /// Interaction logic for EntitiesTab.xaml
    /// </summary>
    public partial class EntitiesTab : UserControl
    {
        public EntitiesTab()
        {
            InitializeComponent();
            Settings.Instance.ActiveDbChanged += Instance_ActiveDbChanged;
        }

        private void Instance_ActiveDbChanged(object sender, EventArgs e)
        {
            if (Settings.Instance.ActiveDatabase != null)
                lsb_Tables.ItemsSource = Settings.Instance.ActiveDatabase.Entities;
        }

        private void lsb_Tables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.DataContext = lsb_Tables.SelectedItem;
        }

        private void UserControl_LayoutUpdated(object sender, EventArgs e)
        {
            if(Settings.Instance.ActiveDatabase != null)
                lsb_Tables.ItemsSource = Settings.Instance.ActiveDatabase.Entities;
        }
    }
}
