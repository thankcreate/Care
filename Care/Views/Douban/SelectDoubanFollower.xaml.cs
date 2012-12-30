using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WeiboSdk;
using Hammock;
using Hammock.Web;
using System.Runtime.Serialization.Json;
using System.IO.IsolatedStorage;
using Care.Tool;
using System.Collections;
using System.Text.RegularExpressions;
using System.ComponentModel;
using DoubanSDK;
using System.Collections.Generic;


namespace Care.Views.Douban
{
    public partial class SelectDoubanFollower : PhoneApplicationPage, INotifyPropertyChanged
    {
        DoubanAPI m_doubanAPI = App.DoubanAPI;
        private static int MAX_PER_PAGE = 30;
        // 从0开始算
        private int m_nCurrentPage = 0;
        private bool m_bIsLastPage = false;
        private int m_pageCount = 0;

        // 这个只是用来标记是否已经加载过allList了，因为我们不想多次加载这个东西
        private bool m_bAllListLoaded = false;

        private ProgressIndicatorHelper m_progressIndicatorHelper;
        public ObservableCollection<FollowingUserInfo> FriendList { get; private set; }
        public List<FollowingUserInfo> m_allList;

        public SelectDoubanFollower()
        {
            FriendList = new ObservableCollection<FollowingUserInfo>();
            DataContext = this;
            InitializeComponent();            
            this.Loaded += new RoutedEventHandler(PageLoaded);
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Shell.SystemTray.ProgressIndicator = new Microsoft.Phone.Shell.ProgressIndicator();
            m_progressIndicatorHelper = new ProgressIndicatorHelper(Microsoft.Phone.Shell.SystemTray.ProgressIndicator, () => { });
            GetFriendList(m_nCurrentPage);
        }

        private void GetFriendList(int page)
        {
            m_progressIndicatorHelper.PushTask();
            String myID = PreferenceHelper.GetPreference("Douban_ID");
            if (String.IsNullOrEmpty(myID))
                return;

            if (!m_bAllListLoaded)
            {
                m_doubanAPI.GetFollowingUserList(myID, (GetFollowingUserListEventArgs e) =>
                {
                    if (e.errorCode == DoubanSdkErrCode.SUCCESS && e.userList != null)
                    {
                        m_allList = e.userList;
                        m_pageCount = (int)Math.Ceiling((double)m_allList.Count / (double)MAX_PER_PAGE);
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            FriendList.Clear();
                            int lengthLeft = m_allList.Count - page * MAX_PER_PAGE;
                            int cutLength = lengthLeft < MAX_PER_PAGE ? lengthLeft : MAX_PER_PAGE;
                            List<FollowingUserInfo> tempList = m_allList.GetRange(page * MAX_PER_PAGE, cutLength);
                            foreach (FollowingUserInfo user in tempList)
                            {
                                FriendList.Add(user);
                            }
                            m_progressIndicatorHelper.PopTask();
                        });
                    }
                    else
                    {
                        m_progressIndicatorHelper.PopTask();
                    }
                });
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    FriendList.Clear();
                    int lengthLeft = m_allList.Count - page * MAX_PER_PAGE;
                    int cutLength = lengthLeft < MAX_PER_PAGE ? lengthLeft : MAX_PER_PAGE;
                    List<FollowingUserInfo> tempList = m_allList.GetRange(page * MAX_PER_PAGE, cutLength);
                    foreach (FollowingUserInfo user in tempList)
                    {
                        FriendList.Add(user);
                    }
                    m_progressIndicatorHelper.PopTask();
                });
            }
           
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchName = nameBox.Text;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                FriendList.Clear();
                foreach (FollowingUserInfo user in m_allList)
                {
                    if (user.screen_name.ToLower().Contains(searchName.ToLower()))
                    {
                        FriendList.Add(user);
                    }
                }
            });            
        }

        private void ListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ResultListBox.SelectedIndex;
            if (index == -1)
            {
                return;
            }
            FollowingUserInfo item = FriendList[index];
            String prefID = PreferenceHelper.GetPreference("Douban_FollowerID");
            if (prefID != item.id)
            {
                PreferenceHelper.SetPreference("Douban_FollowerID", item.id);
                PreferenceHelper.SetPreference("Douban_FollowerNickName", item.screen_name);
                PreferenceHelper.SetPreference("Douban_FollowerAvatar", item.small_avatar);
                PreferenceHelper.SetPreference("Douban_FollowerAvatar2", item.large_avatar);
                PreferenceHelper.SavePreference();
                App.ViewModel.IsChanged = true;
            }
            NavigationService.Navigate(new Uri("/Views/Douban/DoubanAccount.xaml", UriKind.Relative));
        }

        private void Next_Click(object sender, EventArgs e)
        {
            if (m_nCurrentPage >= m_pageCount - 1)
            {
                MessageBox.Show("已经是最后一页");
                return;
            }
            m_nCurrentPage += 1;
            GetFriendList(m_nCurrentPage);
        }


        private void Previos_Click(object sender, EventArgs e)
        {
            if (m_nCurrentPage == 0)
            {
                MessageBox.Show("已经是第一页");
                return;
            }
            m_nCurrentPage -= 1;
            GetFriendList(m_nCurrentPage);
        }

        private void Help_Click(object sender, EventArgs e)
        {
            MessageBox.Show("直接输入昵称搜索，或者在列表中指定朋友");
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