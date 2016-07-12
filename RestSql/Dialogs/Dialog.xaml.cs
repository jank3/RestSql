using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace RestSql.Dialogs
{
    public partial class Dialog : Window
    {
        protected bool m_SizeChanged = false;
        protected bool m_AutoSize = true;
        protected Size m_RequestedSize = new Size();

        public enum TYPE
        {
            OK,
            OK_CANCEL,
            YES_NO,
            NONE
        }

        public Dialog()
        {
            InitializeComponent();
            SizeChanged += ChildWindow_SizeChanged;
            LayoutUpdated += ChildWindow_LayoutUpdated;
            btn_OK.Click += OKButton_Click;
            btn_Cancel.Click += CancelButton_Click;
            setMaxSize();
            //App.Current.Host.Content.Resized += Content_Resized;
        }

        ~Dialog()
        {
            dispose();
        }

        public delegate void disposeDelegate();
        public void dispose()
        {
            if (grd_Content != null)
            {
                if (grd_Content.Dispatcher.CheckAccess())
                {
                    grd_Content.Children.Clear();
                }
                else
                {
                    grd_Content.Dispatcher.BeginInvoke(
                        new disposeDelegate(dispose));
                }
            }
        }

        //public object Tag { get; set; }

        protected void setMaxSize()
        {
            //float width = (float)App.Current.Host.Content.ActualWidth;
            //width = width - (width * 0.2f);
            //float height = (float)App.Current.Host.Content.ActualHeight;
            //height = height - (height * 0.2f);
            //this.MaxWidth = width;
            //this.MaxHeight = height;
        }

        protected void Content_Resized(object sender, EventArgs e)
        {
            setMaxSize();
            // FIX : Resizing doesn't work.  may need to flip the 2 below. - jam
            setRequestedSize();
            // FIXME - this is causing an exception when you resize the browser ---> updateLocation();
        }

        public void updateLocation()
        {
            float browserH = (float)this.MaxHeight;
            float browserW = (float)this.MaxWidth;
            float wndH = (float)this.Height;
            float wndW = (float)this.Width;

            float top = Math.Abs(browserH/2.0f - wndH/2.0f);
            float left = Math.Abs(browserW/2.0f - wndW/2.0f);

            Thickness mrg = this.Margin;
            mrg.Top = top;
            mrg.Left = left;
            this.Margin = mrg;
        }

        public void addContent(UIElement ctrl)
        {
            grd_Content.Children.Add(ctrl);
        }

        public T getContent<T>()
        {
            Type type = typeof(T);
            return (T)Convert.ChangeType(getContent(type), type, null);
        }

        public UIElement getContent(Type type)
        {
            UIElement elem = null;
            for (int i = 0; i < grd_Content.Children.Count; i++)
            {
                if (grd_Content.Children[i].GetType() == type)
                {
                    elem = grd_Content.Children[i];
                }
            }
            return elem;
        }

        public void setButtons(TYPE type)
        {
            switch (type)
            {
                case TYPE.OK:
                    btn_OK.Visibility = System.Windows.Visibility.Visible;
                    btn_Cancel.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case TYPE.OK_CANCEL:
                    btn_OK.Visibility = System.Windows.Visibility.Visible;
                    btn_Cancel.Visibility = System.Windows.Visibility.Visible;
                    break;
                case TYPE.YES_NO:
                    btn_OK.Visibility = System.Windows.Visibility.Visible;
                    btn_OK.Content = "Yes";
                    btn_Cancel.Visibility = System.Windows.Visibility.Visible;
                    btn_Cancel.Content = "No";
                    spl_Buttons.Children.Remove(btn_OK);
                    spl_Buttons.Children.Add(btn_OK);
                    break;
                case TYPE.NONE:
                    btn_Cancel.Visibility = System.Windows.Visibility.Collapsed;
                    btn_OK.Visibility = System.Windows.Visibility.Collapsed;
                    break;
            }
        }

        protected void setRequestedSize()
        {
            if (!m_AutoSize)
            {
                if (m_RequestedSize.Height < this.MaxHeight)
                    this.Height = m_RequestedSize.Height;
                else
                    this.Height = this.MaxHeight;
                if (m_RequestedSize.Width < this.MaxWidth)
                    this.Width = m_RequestedSize.Width;
                else
                    this.Width = this.MaxWidth;
            }
        }

        public void setSize(float w, float h)
        {
            m_AutoSize = false;
            m_RequestedSize.Width = w;
            m_RequestedSize.Height = h;
            setRequestedSize();
        }

        protected void ChildWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(m_AutoSize)
                m_SizeChanged = true;
        }

        protected void ChildWindow_LayoutUpdated(object sender, EventArgs e)
        {
            //if (m_SizeChanged)
            //{
            //    FrameworkElement contentRoot = this.GetTemplateChild("ContentRoot") as FrameworkElement;
            //    TransformGroup tg = contentRoot.RenderTransform as TransformGroup;
            //    IEnumerable<TranslateTransform> tts = tg.Children.OfType<TranslateTransform>();
            //    foreach (TranslateTransform t in tts)
            //    {
            //        t.X = 0;
            //        t.Y = 0;
            //    }
            //}
        }

        protected void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        protected void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public static bool showMessage(FrameworkElement parent, String message, CancelEventHandler onClose, Dialog.TYPE type = Dialog.TYPE.OK)
        {
            Dialog dlg = new Dialog();
            dlg.setButtons(type);
            dlg.Closing += onClose;
            Label lbl = new Label();
            lbl.Content = message;
            dlg.addContent(lbl);
            bool result = (bool)dlg.ShowDialog();
            return result;
        }

        protected delegate void showMessageAsyncDelegate2(FrameworkElement parent, String message, CancelEventHandler onClose, Dialog.TYPE type);
        public static void showMessageAsync(FrameworkElement parent, String message, CancelEventHandler onClose, Dialog.TYPE type = Dialog.TYPE.OK)
        {
            if (parent.Dispatcher.CheckAccess())
            {
                showMessage(parent, message, onClose, type);
            }
            else
            {
                parent.Dispatcher.BeginInvoke(new showMessageAsyncDelegate2(showMessageAsync),
                    new object[] { parent, message, onClose, type });
            }
        }

        public static bool showMessage(FrameworkElement parent, String message, String title = "", Dialog.TYPE type = Dialog.TYPE.OK)
        {
            Dialog dlg = new Dialog();
            if (!String.IsNullOrEmpty(title))
                dlg.Title = title;
            dlg.setButtons(type);
            Label lbl = new Label();
            lbl.Content = message;
            dlg.addContent(lbl);
            bool result = (bool)dlg.ShowDialog();
            return result;
        }

        public delegate void showMessageDelegate(FrameworkElement parent, String message, String title, Dialog.TYPE type);
        public static void showMessageAsync(FrameworkElement parent, String message, String title = "", Dialog.TYPE type = Dialog.TYPE.OK)
        {
            if (parent.Dispatcher.CheckAccess())
            {
                showMessage(parent, message, title, type);
            }
            else
            {
                parent.Dispatcher.BeginInvoke(new showMessageDelegate(showMessageAsync),
                    new object[] { parent, message, title, type });
            }
        }

        public static String showInput(String question, String title)
        {
            String input = "";
            Dialog dlg = new Dialog();
            StackPanel sp = new StackPanel();
            sp.HorizontalAlignment = HorizontalAlignment.Center;
            Label lbl = new Label();
            lbl.Content = question;
            sp.Children.Add(lbl);
            TextBox txb = new TextBox();
            txb.Width = 128;
            sp.Children.Add(txb);
            dlg.Title = title;
            dlg.addContent(sp);
            dlg.setSize(210, 140);
            if (dlg.ShowDialog().Value == true)
            {
                input = txb.Text;
            }
            return input;
        }
    }
}

