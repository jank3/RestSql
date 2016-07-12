using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RestSql.Utilities
{
    public class Controls
    {
        public static void ConfirmDelete(ListBox lsb, IList items)
        {
            int index = lsb.SelectedIndex;
            if (index > -1)
            {
                if (MessageBox.Show(
                       System.String.Format("Are you sure you want to delete the {0} item?",
                                     items[index].ToString()),
                       "Confirm", MessageBoxButton.YesNo)
                    == MessageBoxResult.Yes)
                {
                    items.RemoveAt(index);
                    lsb.SelectedIndex = -1;
                }
            }
        }
    }
}
