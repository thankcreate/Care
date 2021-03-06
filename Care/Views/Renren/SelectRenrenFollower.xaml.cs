﻿using System;
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
        // 人人是从1开始算页数的
        private int m_nCurrentPage = 1;
        private bool m_bIsLastPage = false;
        private static int MAX_PER_PAGE = 50;

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
            GetFriendList(m_nCurrentPage);
        }

        private void GetFriendList(int cursor)
        {
            m_progressIndicatorHelper.PushTask();
            api.GetFriends(renren_GetFriendsCompletedHandler,  MAX_PER_PAGE, cursor);

        }

        public void renren_GetFriendsCompletedHandler(object sender, GetFriendsCompletedEventArgs e)
        {
            m_progressIndicatorHelper.PopTask();
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
                NavigationService.GoBack();
            }
            else
            {
                if (e.Result.Count == 0)
                {
                    return;
                }
                if (e.Result.Count < MAX_PER_PAGE)
                    m_bIsLastPage = true;
                else
                    m_bIsLastPage = false;
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    SearchResult.Clear();
                    foreach (Friend friend in e.Result)
                    {
                        RenrenSearchedMan man = new RenrenSearchedMan();
                        man.id = friend.id.ToString();
                        man.info = "";
                        man.name = friend.name;
                        man.isFriend = "1";
                        man.tinyurl = friend.tinyurl;
                        SearchResult.Add(man);
                    }
                    ScrollViewer v = VisualTreeHelper.GetChild(this.ResultListBox, 0) as ScrollViewer;
                    v.ScrollToVerticalOffset(0); 
                });
            }
        }


        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                return;
            }
            List<APIParameter> param = new List<APIParameter>();
            param.Add(new APIParameter("method", "friends.search"));
            param.Add(new APIParameter("name", txtName.Text));
            App.RenrenAPI.RequestAPIInterface(SearchCallback, param);
            m_progressIndicatorHelper.PushTask();
        }

        private void SearchCallback(object sender, APIRequestCompletedEventArgs e)
        {
            try
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RenrenSearchedManResult));
                RenrenSearchedManResult searchResult = serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(e.ResultJsonString))) as RenrenSearchedManResult;

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    m_progressIndicatorHelper.PopTask();
                    SearchResult.Clear();
                    if (searchResult != null && searchResult.friends != null)
                        foreach (RenrenSearchedMan friend in searchResult.friends)
                        {
                            //App.ViewModel.Friends.Add(friend);

                            SearchResult.Add(friend);
                        }
                    ScrollViewer v = VisualTreeHelper.GetChild(this.ResultListBox, 0) as ScrollViewer;
                    v.ScrollToVerticalOffset(0);
                });
            }
            catch (System.Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    m_progressIndicatorHelper.PopTask();                   
                });
            }
           
        }
     

        private void ListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ResultListBox.SelectedIndex;
            if (index == -1)
            {
                return;
            }
            RenrenSearchedMan item = SearchResult[index];
            String prefID = PreferenceHelper.GetPreference("Renren_FollowerID");
            if (prefID != item.id)
            {
                PreferenceHelper.SetPreference("Renren_FollowerID", item.id);
                PreferenceHelper.SetPreference("Renren_FollowerNickName", item.name);
                // 这个头像太不清晰了，要重新摘取一个高清无码大图
                PreferenceHelper.SetPreference("Renren_FollowerAvatar", item.tinyurl);
                api.GetUserInfo(item.id, null, (send, arg) =>
                {
                    UserList userList = arg.Result;
                    if (userList == null || userList.User_List == null || userList.User_List.Count == 0)
                        return;
                    UserDetails user = userList.User_List[0];
                    PreferenceHelper.SetPreference("Renren_FollowerAvatar2", user.headurl);
                });
                PreferenceHelper.SavePreference();

                App.ViewModel.IsChanged = true;
            }
           
            NavigationService.Navigate(new Uri("/Views/Renren/RenrenAccount.xaml", UriKind.Relative));
        }

        private void Next_Click(object sender, EventArgs e)
        {
            if (m_bIsLastPage)
            {
                MessageBox.Show("已经是最后一页");
                return;
            }
            m_nCurrentPage += 1;
            GetFriendList(m_nCurrentPage);
        }


        private void Previos_Click(object sender, EventArgs e)
        {
            if (m_nCurrentPage == 1)
            {
                MessageBox.Show("已经是第一页");
                return;
            }
            m_nCurrentPage -= 1;
            GetFriendList(m_nCurrentPage);
        }

        private void Help_Click(object sender, EventArgs e)
        {
            MessageBox.Show("可以用两种方式指定关注对象：\n1.指定其UID值\n2:在关注人列表中选则");
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