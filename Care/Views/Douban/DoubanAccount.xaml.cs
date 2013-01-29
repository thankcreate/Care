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
using System.ComponentModel;
using Care.Tool;
using System.Runtime.Serialization.Json;
using DoubanSDK;

namespace Care.Views.Douban
{
    public partial class DoubanAccount : PhoneApplicationPage, INotifyPropertyChanged
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

        DoubanAPI m_doubanAPI = App.DoubanAPI;

        public DoubanAccount()
        {
            CurrentNickName = PreferenceHelper.GetPreference("Douban_NickName");
            FollowerNickName = PreferenceHelper.GetPreference("Douban_FollowerNickName");
            CurrentAvatar = PreferenceHelper.GetPreference("Douban_Avatar");
            FollowerAvatar = PreferenceHelper.GetPreference("Douban_FollowerAvatar");

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
            this.Loaded += new RoutedEventHandler(DoubanAccount_Loaded);
        }

        private void DoubanAccount_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshRealData();
        }

        private void RefreshRealData()
        {
            refreshMyDoubanAccount();
        }

        // 点击登陆
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            AuthenticationView.OAuth2VerifyCompleted = (e1, e2, e3) => LoginVerifyBack(e1, e2, e3);
            AuthenticationView.OBrowserCancelled = new EventHandler(cancleEvent);

            // show the login page
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/DoubanSdk;component/View/AuthenticationView.xaml"
                    , UriKind.Relative));
            });            
        }

        // 刷新当前登陆用户信息，不回退到当前页
        private void refreshMyDoubanAccount()
        {
            m_doubanAPI.GetMyUserInfo((args) =>
            {
                String token = PreferenceHelper.GetPreference("Douban_Token");
                if (args.errorCode == DoubanSdkErrCode.SUCCESS && args.userInfo != null)
                {
                    UserInfo info = args.userInfo;
                    PreferenceHelper.SetPreference("Douban_ID", info.id);
                    PreferenceHelper.SetPreference("Douban_NickName", info.name);
                    PreferenceHelper.SetPreference("Douban_Avatar", info.avatar);                    
                }
                // 若过期
                else if(!string.IsNullOrEmpty(token) && m_doubanAPI.IsAccessTokenOutOfDate())
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        CurrentNickName = "";
                        CurrentAvatar = "";
                    });                              
                }
            });
        }

        // 登陆成功后，刷新当前登陆用户信息，再回退到当前页
        private void refreshMyDoubanAccountIDAndGoback()
        {
            m_doubanAPI.GetMyUserInfo((args) =>
            {
                if(args.errorCode == DoubanSdkErrCode.SUCCESS && args.userInfo != null)
                {
                    UserInfo info = args.userInfo;
                    PreferenceHelper.SetPreference("Douban_ID", info.id);
                    PreferenceHelper.SetPreference("Douban_NickName", info.name);
                    PreferenceHelper.SetPreference("Douban_Avatar", info.avatar);
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {   
                        NavigationService.Navigate(new Uri("/Views/Douban/DoubanAccount.xaml", UriKind.Relative));
                    });
                }
            });
        }

        private void LoginVerifyBack(bool isSucess, DoubanSdkAuthError errCode, DoubanSdkAuth2Res response)
        {
            if (errCode.errCode == DoubanSdkErrCode.SUCCESS)
            {   
                PreferenceHelper.SetPreference("Douban_Token", response.accesssToken);
                PreferenceHelper.SavePreference();
                refreshMyDoubanAccountIDAndGoback(); 
            }
            else if (errCode.errCode == DoubanSdkErrCode.NET_UNUSUAL)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("检查网络");
                });
            }
            else if (errCode.errCode == DoubanSdkErrCode.SERVER_ERR)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("服务器返回错误，错误代码:" + errCode.specificCode);
                });
            }            
        }

        private void cancleEvent(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.GoBack();
            });
        }


        // 点击退出登陆
        private void btnExitLogin_Click(object sender, RoutedEventArgs e)
        {
            PreferenceHelper.RemoveDoubanPreference();
            CurrentNickName = "未登陆";
            FollowerNickName = "未关注";
            CurrentAvatar = "";
            FollowerAvatar = "";
            App.DoubanAPI.LogOut();
            App.ViewModel.IsChanged = true;
        }

        private void btnSetFollower_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PreferenceHelper.GetPreference("Douban_ID")))
            {
                MessageBox.Show("请先登陆豆瓣帐号 -_-#");
                return;
            }
            NavigationService.Navigate(new Uri("/Views/Douban/SelectDoubanFollower.xaml", UriKind.Relative));
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml"
                    , UriKind.Relative));
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