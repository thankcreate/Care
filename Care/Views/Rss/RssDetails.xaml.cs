using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Care.Tool;
using System.Threading;

namespace Care
{
    public partial class RssDetails : PhoneApplicationPage
    {
        public ItemViewModel m_itemViewModel;

        private ProgressIndicatorHelper m_progressIndicatorHelper;

        public RssDetails()
        {
            InitializeComponent();
            InitHeaderHeight();
            this.Loaded += new RoutedEventHandler(RssDetails_Loaded);           
        }

        private void InitHeaderHeight()
        {
            double perc = ((double)190) / ((double)800);
            headerGrid.Height = (int)(Application.Current.RootVisual.RenderSize.Height * perc);
        }

        private void RssDetails_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_itemViewModel != null)
            {
                Microsoft.Phone.Shell.SystemTray.ProgressIndicator = new Microsoft.Phone.Shell.ProgressIndicator();
                m_progressIndicatorHelper = new ProgressIndicatorHelper(Microsoft.Phone.Shell.SystemTray.ProgressIndicator, () => { });
                m_progressIndicatorHelper.PushTaskInUIThread();
                WebBrowser.Navigated += new EventHandler<System.Windows.Navigation.NavigationEventArgs>(WebBrowser_Navigated);
                WebBrowser.NavigateToString(ConvertExtendedASCII(m_itemViewModel.RssSummary));                
            }
        }

        private void WebBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            m_progressIndicatorHelper.PopTask();
        }


        private static string ConvertExtendedASCII(string HTML)
        {
            string retVal = "";
            char[] s = HTML.ToCharArray();
            foreach (char c in s)
            {
                if (Convert.ToInt32(c) > 127)
                    retVal += "&#" + Convert.ToInt32(c) + ";";
                else
                    retVal += c;
            }
            return retVal;
        }

        private void BrowserNavigating(object sender, NavigatingEventArgs e)
        {
            string a = e.Uri.AbsoluteUri;
            if (e.Uri.AbsoluteUri.Contains("closeWindow()") || e.Uri.AbsoluteUri.Contains("error=access_denied"))
            {
                int bbb = 1;
            }
        }
        
    }
}