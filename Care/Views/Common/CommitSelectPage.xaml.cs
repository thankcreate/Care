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
using System.Text;
using Care.Tool;

namespace Care.Views.Common
{
    public partial class CommitSelectPage : PhoneApplicationPage
    {
        private string m_content = "";
        private string m_picURL = "";

        public CommitSelectPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {           
            IDictionary<string, string> queryString = this.NavigationContext.QueryString;
            if (queryString.ContainsKey("Content"))
            {
                m_content = queryString["Content"];             
            }
            if (queryString.ContainsKey("PicURL"))
            {
                m_picURL = queryString["PicURL"];   
            }
        }

        private void btnSina_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(PreferenceHelper.GetPreference("SinaWeibo_ID")))
            {
                MessageBox.Show("新浪帐号未登陆");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("/Views/Common/CommitView.xaml");
            sb.Append(string.Format("?Content={0}&PicURL={1}&Type={2}", m_content, m_picURL, EntryType.SinaWeibo));
            NavigationService.Navigate(new Uri(sb.ToString(), UriKind.Relative));
            
        }

        private void btnRenren_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(PreferenceHelper.GetPreference("Renren_ID")))
            {
                MessageBox.Show("人人帐号未登陆");
                return;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("/Views/Common/CommitView.xaml");
            sb.Append(string.Format("?Content={0}&PicURL={1}&Type={2}", m_content, m_picURL, EntryType.Renren));
            NavigationService.Navigate(new Uri(sb.ToString(), UriKind.Relative));
        }

        private void btnDouban_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(PreferenceHelper.GetPreference("Douban_ID")))
            {
                MessageBox.Show("豆瓣帐号未登陆");
                return;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("/Views/Common/CommitView.xaml");
            sb.Append(string.Format("?Content={0}&PicURL={1}&Type={2}", m_content, m_picURL, EntryType.Douban));
            NavigationService.Navigate(new Uri(sb.ToString(), UriKind.Relative));
        }        
    }
}