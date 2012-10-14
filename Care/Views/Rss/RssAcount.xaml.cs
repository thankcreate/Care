using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.ComponentModel;
using Care.Tool;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Text.RegularExpressions;

namespace Care.Views
{
    public partial class RssAcount : PhoneApplicationPage, INotifyPropertyChanged
    {

        public String _FollowerSiteName;
        public String FollowerSiteName
        {
            get
            {
                return _FollowerSiteName;
            }
            set
            {
                _FollowerSiteName = value;
                NotifyPropertyChanged("FollowerSiteName");
            }
        }

        public String _FollowerSitePath;
        public String FollowerSitePath
        {
            get
            {
                return _FollowerSitePath;
            }
            set
            {
                _FollowerSitePath = value;
                NotifyPropertyChanged("FollowerSitePath");
            }
        }


        public RssAcount()
        {
            DataContext = this;

            // init member data
            FollowerSitePath = PreferenceHelper.GetPreference("RSS_FollowerPath");
            FollowerSiteName = PreferenceHelper.GetPreference("RSS_FollowerSite");
            if (string.IsNullOrEmpty(FollowerSiteName))
            {
                FollowerSiteName = "未关注";
            }
            if (string.IsNullOrEmpty(FollowerSitePath))
            {
                FollowerSitePath = "未设置";
            }

            InitializeComponent();
        }

        private void Help_Click(object sender, EventArgs e)
        {
            MessageBox.Show("RSS是一种同步网站内容的格式。\n在地址框中输入其RSS地址，可以订阅其博客更新");
        }

        private void Confirm_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
        

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void UpdatePath_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 改掉这个写死的URL
            //string url = "http://blog.sina.com.cn/rss/1713845420.xml";
            //PreferenceHelper.SetPreference("RSS_FollowerPath", url); 
            //WebClient client = new WebClient();
            //client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            //client.DownloadStringAsync(new Uri(url));
            String text = textSitePath.Text;
            if (text.StartsWith("http"))
            {
                PreferenceHelper.SetPreference("RSS_FollowerPath", text);
                WebClient client = new WebClient();
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
                client.DownloadStringAsync(new Uri(text));
            }
            else
            {
                string url = "/Views/Rss/RssSearch.xaml?Key=";
                url += text;
                NavigationService.Navigate(new Uri(url, UriKind.Relative));
            }            
        }

        private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            XmlReader reader = XmlReader.Create(new StringReader(e.Result));
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                App.ViewModel.RssItems.Clear();
                FollowerSiteName = feed.Title.Text;  
                PreferenceHelper.SetPreference("RSS_FollowerSite", FollowerSiteName);  
            }            
            );
        }      
    }
}