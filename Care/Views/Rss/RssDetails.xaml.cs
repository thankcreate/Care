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

namespace Care
{
    public partial class RssDetails : PhoneApplicationPage
    {
        int m_nIndex = -1;

        public RssDetails()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(RssDetails_Loaded);
        }

        private void RssDetails_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_nIndex != -1)
            {
                WebBrowser.NavigateToString(ConvertExtendedASCII(App.ViewModel.Items[m_nIndex].RssSummary));                
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IDictionary<string, string> queryString = this.NavigationContext.QueryString;
            if (queryString.ContainsKey("Index"))
            {
                m_nIndex = int.Parse(queryString["Index"]);
                this.DataContext = App.ViewModel.Items[m_nIndex];
            }

            base.OnNavigatedTo(e);
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