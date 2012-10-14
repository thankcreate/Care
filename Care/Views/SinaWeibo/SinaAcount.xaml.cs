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
using System.ComponentModel;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using WeiboSdk;
using WeiboSdk.PageViews;
using System.Diagnostics;
using Hammock;
using Hammock.Web;
using System.Runtime.Serialization.Json;
using Care.Tool;

namespace Care.Views
{
    public partial class SinaAcount : PhoneApplicationPage, INotifyPropertyChanged
    {
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
        public SinaAcount()
        {
            CurrentNickName = PreferenceHelper.GetPreference("SinaWeibo_NickName");
            FollowerNickName = PreferenceHelper.GetPreference("SinaWeibo_FollowerNickName");
            CurrentAvatar = PreferenceHelper.GetPreference("SinaWeibo_Avatar");
            FollowerAvatar = PreferenceHelper.GetPreference("SinaWeibo_FollowerAvatar");
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
            this.Loaded += new RoutedEventHandler(SinaAcount_Loaded);
        }

        

        private void SinaAcount_Loaded(object sender, RoutedEventArgs e)
        {
            //GetFriendList();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            AuthenticationView.OAuth2VerifyCompleted = (e1, e2, e3) => LoginVerifyBack(e1, e2, e3);
            AuthenticationView.OBrowserCancelled = new EventHandler(cancleEvent);

            // show the login page
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/WeiboSdk;component/PageViews/AuthenticationView.xaml"
                    , UriKind.Relative));
            });
        }

        private void cancleEvent(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.GoBack();
            });
        }


        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml"
                    , UriKind.Relative));
        }

        private void LoginVerifyBack(bool isSucess, SdkAuthError errCode, SdkAuth2Res response)
        {
            if (errCode.errCode == SdkErrCode.SUCCESS)
            {
                if (null != response)
                {
                    App.SinaWeibo_AccessToken = response.accesssToken;
                    App.SinaWeibo_RefleshToken = response.refleshToken;
                }

                PreferenceHelper.SetPreference("SinaWeibo_Token", response.accesssToken);
                PreferenceHelper.SavePreference();
                
                refreshMySinaAccountID();


            }
            else if (errCode.errCode == SdkErrCode.NET_UNUSUAL)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("检查网络");
                });
            }
            else if (errCode.errCode == SdkErrCode.SERVER_ERR)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("服务器返回错误，错误代码:" + errCode.specificCode);
                });
            }
            else
                Debug.WriteLine("Other Err.");
        }

        // 得到当前用户ID
        // 新浪微博必须要先得到当前用户ID，再根据此ID拿到当前用户信息
        private void refreshMySinaAccountID()
        {
            // Define a new net engine
            SdkNetEngine netEngine = new SdkNetEngine();

            // Define a new command base
            SdkCmdBase cmdBase = new SdkCmdBase
            {
                acessToken = App.SinaWeibo_AccessToken,
            };
            RestRequest request = new RestRequest();
            request.Method = WebMethod.Get;

            request.Path = "/account/get_uid.json";
            request.AddParameter("access_token", App.SinaWeibo_AccessToken);
            netEngine.SendRequest(request, cmdBase, (SdkResponse e1) =>
            {
                if (e1.errCode == SdkErrCode.SUCCESS)
                {
                    try
                    {
                        JSONObject json = JSONConvert.DeserializeObject(e1.content);
                        string uid = json["uid"] as string;
                        App.ViewModel.SinaWeiboAccount.id = uid;
                        PreferenceHelper.SetPreference("SinaWeibo_ID", uid);                        
                        refreshMySinaAccountInfo();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                        int a = 1;
                    }
                }
                else
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        MessageBoxResult result = MessageBox.Show("新浪微博用户尚未登陆，或登陆已过期", "提示", MessageBoxButton.OKCancel);
                        if (result == MessageBoxResult.OK)
                        {
                            //  WeiboLogin(null, null);
                        }
                    });
                }
            });
        }

        // 得到当前用户信息
        private void refreshMySinaAccountInfo()
        {
            // Define a new net engine
            SdkNetEngine netEngine = new SdkNetEngine();

            // Define a new command base
            SdkCmdBase cmdBase = new SdkCmdBase
            {
                acessToken = App.SinaWeibo_AccessToken,
            };
            RestRequest request = new RestRequest();
            request.Method = WebMethod.Get;

            request.Path = "/users/show.json";
            request.AddParameter("access_token", App.SinaWeibo_AccessToken);
            request.AddParameter("uid", App.ViewModel.SinaWeiboAccount.id);
            netEngine.SendRequest(request, cmdBase, (SdkResponse e1) =>
            {
                if (e1.errCode == SdkErrCode.SUCCESS)
                {
                    User user = null;
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(User));
                    user = ser.ReadObject(e1.stream) as User;                    
                    PreferenceHelper.SetPreference("SinaWeibo_NickName", user.screen_name);
                    PreferenceHelper.SetPreference("SinaWeibo_Avatar", user.profile_image_url);
                    // 设置显示当前用户昵称的UI
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        NavigationService.Navigate(new Uri("/Views/SinaWeibo/SinaAcount.xaml", UriKind.Relative));
                    });
                }
            });
        }

        private void btnExitLogin_Click(object sender, RoutedEventArgs e)
        {
            App.SinaWeibo_AccessToken = "";
            App.SinaWeibo_RefleshToken = "";
            PreferenceHelper.RemoveSinaWeiboPreference();
            CurrentNickName = "未登陆";
            FollowerNickName = "未关注";
            CurrentAvatar = "";
            FollowerAvatar = "";
        }

        private void btnSetFollower_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PreferenceHelper.GetPreference("SinaWeibo_ID")))
            {
                MessageBox.Show("请先登陆新浪帐号 -_-#");
                return;
            }
            NavigationService.Navigate(new Uri("/Views/SinaWeibo/SelectSinaFollower.xaml", UriKind.Relative));            
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
    }
}