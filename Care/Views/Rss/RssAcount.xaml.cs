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



        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml"
                    , UriKind.Relative));
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
            String text = textSitePath.Text;
            if (text.StartsWith("http"))
            {
                PreferenceHelper.SetPreference("RSS_FollowerPath", text);
                WebClient client = new WebClient();
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
                try
                {
                    client.DownloadStringAsync(new Uri(text));
                }
                catch
                {
                   Deployment.Current.Dispatcher.BeginInvoke(() =>
                   {
                       MessageBox.Show("请输入合法地址");
                   });
                }                
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
            try
            {
                XmlReader reader = XmlReader.Create(new StringReader(e.Result));
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    App.ViewModel.RssItems.Clear();
                    FollowerSiteName = feed.Title.Text;
                    PreferenceHelper.SetPreference("RSS_FollowerSite", FollowerSiteName);
                    App.ViewModel.IsChanged = true;
                    MessageBox.Show("关注成功");
                }
           );
            }
            catch (System.Exception ex)
            {                
                MessageBox.Show("关注站点失败，如果直接输入地址，请输入rss地址\n即类似于www.xxx.com/feed，而不是直接输入www.xxx.com",
                "失败", MessageBoxButton.OK);
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    FollowerSitePath = "未设置";
                    FollowerSiteName = "未关注";
                    PreferenceHelper.RemoveRssPreference();
                });
            }
        }

        private void StopFeed_Click(object sender, RoutedEventArgs e)
        {
            FollowerSitePath = "未设置";
            FollowerSiteName = "未关注"; 
            PreferenceHelper.RemoveRssPreference();
            App.ViewModel.IsChanged = true;            
        }      
    }
}