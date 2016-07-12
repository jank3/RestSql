using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RestSql.Dialogs
{
    /// <summary>
    /// This is just a reference class to copy to a class
    /// that uses the dialog window.
    /// </summary>
    public class DialogBase : UserControl
    {
        public String Title { get; set; }
        public EventHandler Closed;
        public CancelEventHandler Closing;

        private void Constructor()
        {
            Title = "Window Title";
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
