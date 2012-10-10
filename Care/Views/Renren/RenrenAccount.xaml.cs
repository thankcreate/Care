﻿using System;
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
using System.ComponentModel;
using Care.Tool;
using RenrenSDKLibrary;
using System.Runtime.Serialization.Json;

namespace Care.Views
{
    public partial class RenrenAccount : PhoneApplicationPage, INotifyPropertyChanged
    {
        #region  无聊的Set&Get
        public String _CurrentNickName;
        public String CurrentNickName
        {
            get
            {
                return _CurrentNickName;
            }
            set
            {
                _CurrentNickName = value;
                NotifyPropertyChanged("CurrentNickName");
            }
        }

        public String _CurrentAvatar = "";
        public String CurrentAvatar
        {
            get
            {
                return _CurrentAvatar;
            }
            set
            {
                _CurrentAvatar = value;
                NotifyPropertyChanged("CurrentAvatar");
            }
        }

        public String _FollowerNickName;
        public String FollowerNickName
        {
            get
            {
                return _FollowerNickName;
            }
            set
            {
                _FollowerNickName = value;
                NotifyPropertyChanged("FollowerNickName");
            }
        }

        public String _FollowerAvatar = "";
        public String FollowerAvatar
        {
            get
            {
                return _FollowerAvatar;
            }
            set
            {
                _FollowerAvatar = value;
                NotifyPropertyChanged("FollowerAvatar");
            }
        }
        #endregion      
        RenrenAPI api = App.RenrenAPI;

        public RenrenAccount()
        {
            CurrentNickName = PreferenceHelper.GetPreference("Renren_NickName");
            FollowerNickName = PreferenceHelper.GetPreference("Renren_FollowerNickName");
            CurrentAvatar = PreferenceHelper.GetPreference("Renren_Avatar");
            FollowerAvatar = PreferenceHelper.GetPreference("Renren_FollowerAvatar");

            if (string.IsNullOrEmpty(CurrentNickName))
            {
                CurrentNickName = "未登陆";
            }
            if (string.IsNullOrEmpty(FollowerNickName))
            {
                FollowerNickName = "未关注";
            }
            
            InitializeComponent();
            DataContext = this;            
        }

        // 点击登陆
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            api.Login(this, renren_LoginCompletedHandler);
        }

        public void renren_LoginCompletedHandler(object sender, LoginCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                // 登陆成功后首先拿到自己的昵称和ID
                api.GetCurUserInfo(renren_GetCurUserInfoCompletedHandler);
                // 这个实函数实际上是运行在验证页中的，需要转向本页
                NavigationService.Navigate(new Uri("/Views/Renren/RenrenAccount.xaml", UriKind.Relative));
            }
            else
                MessageBox.Show(e.Error.Message);
        }

        public void renren_GetCurUserInfoCompletedHandler(object sender, GetUserUidCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
                NavigationService.GoBack();
            }
            else
            {
                UserDetails user = new UserDetails();
                user = e.Result;
                CurrentNickName = user.name;
                CurrentAvatar = user.headurl;
                PreferenceHelper.SetPreference("Renren_Avatar", user.headurl);
                PreferenceHelper.SetPreference("Renren_NickName", user.name);
                PreferenceHelper.SetPreference("Renren_ID", user.uid.ToString());               
            }
        }


        // 点击退出登陆
        private void btnExitLogin_Click(object sender, RoutedEventArgs e)
        {
            PreferenceHelper.RemoveRenrenPreference();
            CurrentNickName = "未登陆";
            FollowerNickName = "未关注";
            CurrentAvatar = "";
            FollowerAvatar = "";
        }

        private void btnSetFollower_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PreferenceHelper.GetPreference("Renren_ID")))
            {
                MessageBox.Show("请先登陆人人帐号 -_-#");
                return;
            }
            NavigationService.Navigate(new Uri("/Views/Renren/SelectRenrenFollower.xaml", UriKind.Relative));
        }
        #region  无聊的PropertyChangedEventHandler和NotifyPropertyChanged基友二人组
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