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
using System.Collections;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Care.Tool;

namespace Care.Views.Rss
{
    public partial class RssSearch : PhoneApplicationPage
    {
        public ObservableCollection<RssSiteItem> SiteItems { get; private set; }
        public String m_key;
        private ProgressIndicatorHelper m_progressIndicatorHelper;

        public RssSearch()
        {
            SiteItems = new ObservableCollection<RssSiteItem>();
            InitializeComponent();
            DataContext = this;
            this.Loaded += new RoutedEventHandler(Page_Loaded);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Shell.SystemTray.ProgressIndicator = new Microsoft.Phone.Shell.ProgressIndicator();
            m_progressIndicatorHelper = new ProgressIndicatorHelper(Microsoft.Phone.Shell.SystemTray.ProgressIndicator, () => { });

            SearchByKey(m_key);
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {           
            IDictionary<string, string> queryString = this.NavigationContext.QueryString;                      
            if (queryString.ContainsKey("Key"))
            {
                m_key = queryString["Key"];               
            }
            else
            {
                m_key = "";                
            } 
            base.OnNavigatedTo(e);
        }

        private void SearchByKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            string url = "http://www.search4rss.com/search.php?lang=en&q=";
            url += key;
            WebClient client = new WebClient();
            m_progressIndicatorHelper.PushTask();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(search_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri(url));
        }

        private void search_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                String html = e.Result;
                HandleHtml(html);       
            }
            catch (System.Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("网络无法连接，请检查当前网络状态。");
                    NavigationService.Navigate(new Uri("/Views/Rss/RssAcount.xaml", UriKind.Relative));
                });
            }    
        }

        private void HandleHtml(string html)
        {
            Regex regResult = new Regex("<div id=\'results\'>(?<1>.*?)preview");
            SiteItems.Clear();
            foreach (Match match in regResult.Matches(html))
            {
                HandleResult(match.Groups["1"].Value);
            }
            m_progressIndicatorHelper.PopTask();  
        }

        private void HandleResult(string result)
        {
            Regex regTitle = new Regex("<div>.*?<a.*?href=\'(?<OriginalURL>.*?)\'.*?<b>(?<Title>.*?)</b>.*?<a href=\'(?<FeedURL>.*?)\'");

            RssSiteItem item = new RssSiteItem();
            item.Title = regTitle.Match(result).Groups["Title"].Value;
            item.RssPath = regTitle.Match(result).Groups["FeedURL"].Value;
            item.OriginPath = regTitle.Match(result).Groups["OriginalURL"].Value;

            SiteItems.Add(item);
        }

        private void ListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ResultListBox.SelectedIndex;
            RssSiteItem item = SiteItems[index];
            PreferenceHelper.SetPreference("RSS_FollowerSite", item.Title);
            PreferenceHelper.SetPreference("RSS_FollowerPath", item.RssPath);
            PreferenceHelper.SetPreference("RSS_FollowerOrigin", item.OriginPath);
            PreferenceHelper.SavePreference();
            NavigationService.Navigate(new Uri("/Views/Rss/RssAcount.xaml", UriKind.Relative));
            App.ViewModel.IsChanged = true;
        }
    }
}