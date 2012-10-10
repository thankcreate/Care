using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Text;
using Microsoft.Phone.Controls;
using RenrenSDKLibrary;
using System.Runtime.Serialization.Json;
using System.ComponentModel;
using Care.Tool;

namespace Care.Views
{
    public partial class SelectRenrenFollower : PhoneApplicationPage, INotifyPropertyChanged
    {
        private RenrenAPI api = App.RenrenAPI;
        private ProgressIndicatorHelper m_progressIndicatorHelper;

        public ObservableCollection<RenrenSearchedMan> SearchResult { get; private set; }
        
        public SelectRenrenFollower()
        {
            DataContext = this;
            SearchResult = new ObservableCollection<RenrenSearchedMan>();
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(PageLoaded);
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Shell.SystemTray.ProgressIndicator = new Microsoft.Phone.Shell.ProgressIndicator();
            m_progressIndicatorHelper = new ProgressIndicatorHelper(Microsoft.Phone.Shell.SystemTray.ProgressIndicator, () => { });
            
        }


        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            List<APIParameter> param = new List<APIParameter>();
            param.Add(new APIParameter("method", "friends.search"));
            param.Add(new APIParameter("name", "佳琦"));
            App.RenrenAPI.RequestAPIInterface(SearchCallback, param);
            m_progressIndicatorHelper.PushTask();
        }

        private void SearchCallback(object sender, APIRequestCompletedEventArgs e)
        {
            JSONObject root = JSONConvert.DeserializeObject(e.ResultJsonString);
            JSONArray frieds = root["friends"] as JSONArray;
            String friedsJson = JSONConvert.SerializeArray(frieds);

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<RenrenSearchedMan>));
            List<RenrenSearchedMan> searchResult = serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(friedsJson))) as List<RenrenSearchedMan>;
            
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                m_progressIndicatorHelper.PopTask();
                SearchResult.Clear();
                foreach (RenrenSearchedMan friend in searchResult)
                {
                    //App.ViewModel.Friends.Add(friend);
                    SearchResult.Add(friend);
                }
            });
        }
     

        private void ListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ResultListBox.SelectedIndex;           
            RenrenSearchedMan item = SearchResult[index];
            PreferenceHelper.SetPreference("Renren_FollowerID", item.id);
            PreferenceHelper.SetPreference("Renren_FollowerNickName", item.name);
            PreferenceHelper.SetPreference("Renren_FollowerAvatar", item.tinyurl);
            NavigationService.Navigate(new Uri("/Views/Renren/RenrenAccount.xaml", UriKind.Relative));
        }

        #region 你还真烦啊！到处都少不了你！！！！！
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

    }
}