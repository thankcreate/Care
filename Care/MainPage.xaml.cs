﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using WeiboSdk;
using WeiboSdk.PageViews;
using Hammock;
using Hammock.Web;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.IO.IsolatedStorage;
using System.Xml;
using System.ServiceModel.Syndication;
using Care.Views;
using Care.Tool;
using RenrenSDKLibrary;

namespace Care
{

    public partial class MainPage : PhoneApplicationPage
    {
        DoubanHelper m_doubanHelper;
        ProgressIndicatorHelper m_progressIndicatorHelper;
        bool m_bIsNavigateFromSelectPage;
        string m_strShowType = "";
        string m_strDataSource = "";

        // Constructor
        public MainPage()
        {

            m_bIsNavigateFromSelectPage = false;

            TiltEffect.TiltableItems.Add(typeof(TiltableControl));

            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            m_doubanHelper = new DoubanHelper();
            InitSinaWeiboInfo();
        }

        private void InitSinaWeiboInfo()
        {
            SdkData.AppKey = "1385220836";
            SdkData.AppSecret = "6c8a1cd05efe4ff69839de695c68e8f0";
            SdkData.RedirectUri = "www.thankcreate.com";

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("SinaWeibo_Token"))
            {
                App.SinaWeibo_AccessToken = settings["SinaWeibo_Token"] as string;
            }
            if (settings.Contains("SinaWeibo_FollowerID"))
            {
                App.ViewModel.SinaWeiboCareID = settings["SinaWeibo_FollowerID"] as string;
            }
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.ViewModel.IsChanged)
            {
                refreshMainViewModel();
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string value = string.Empty;
            IDictionary<string, string> queryString = this.NavigationContext.QueryString;
            string showType = "";
            string dataSource = "";
            if (queryString.ContainsKey("ShowType") && queryString.ContainsKey("DataSource"))
            {
                m_strShowType = queryString["ShowType"];
                m_strDataSource = queryString["DataSource"];
                m_bIsNavigateFromSelectPage = true;
            }
            else
            {
                m_strShowType = "";
                m_strDataSource = "";
                m_bIsNavigateFromSelectPage = false;
            }
            // 针对不同的过滤选项，进行跳转页设置
            if (m_strShowType == SelectOnly.DATASOURCE_SINAWEIBO)
            {
                MainPanorama.DefaultItem = MainPanorama.Items[0];
            }
            else if (m_strShowType == SelectOnly.SHOWTYPE_PICTURES)
            {
                MainPanorama.DefaultItem = MainPanorama.Items[1];
            }
            base.OnNavigatedTo(e);
        }

        private void WeiboLogin(object sender, RoutedEventArgs e)
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

        private void LoginVerifyBack(bool isSucess, SdkAuthError errCode, SdkAuth2Res response)
        {
            if (errCode.errCode == SdkErrCode.SUCCESS)
            {
                if (null != response)
                {
                    App.SinaWeibo_AccessToken = response.accesssToken;
                    App.SinaWeibo_RefleshToken = response.refleshToken;
                }

                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                try
                {
                    settings.Add("SinaWeibo_Token", response.accesssToken);
                }
                catch (ArgumentException ex)
                {
                    settings["SinaWeibo_Token"] = response.accesssToken;
                }
                settings.Save();

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MainPanorama.DefaultItem = MainPanorama.Items[0];
                    refreshMainViewModel();
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));

                });
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

        private void cancleEvent(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.GoBack();
            });
        }
        private void Refresh_Click(object sender, EventArgs e)
        {
            refreshMainViewModel();
        }

        private void Filt_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SelectOnly.xaml", UriKind.Relative));
        }

        private void Microscope_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Lab/LabMainPage.xaml", UriKind.Relative));
        }

        private void refreshMainViewModel()
        {
            // 如果是由SelectOnly页面转发过来，则不进行网络数据请求，只更新UI
            if (m_bIsNavigateFromSelectPage)
            {
                filtPage();
            }
            else
            {
                refreshNewsPage();
            }

        }

        // filePage是由SelectOnly页面跳转过来时触发的刷新操作
        // 只刷新某个页面
        private void filtPage()
        {
            if (m_strShowType == SelectOnly.SHOWTYPE_NEWS)
            {
                filtNewsPgae();
            }
            else if (m_strShowType == SelectOnly.SHOWTYPE_PICTURES)
            {
                filtPicturePage();
            }
        }

        private void filtNewsPgae()
        {
            App.ViewModel.ListItems.Clear();

            // switch begin
            if (m_strDataSource == SelectOnly.DATASOURCE_SINAWEIBO)
            {
                App.ViewModel.ListItems.AddRange(App.ViewModel.SinaWeiboItems);
            }
            else if (m_strDataSource == SelectOnly.DATASOURCE_RSS)
            {
                App.ViewModel.ListItems.AddRange(App.ViewModel.RssItems);
            }
            // switch end

            App.ViewModel.ListItems.Sort(
                delegate(ItemViewModel a, ItemViewModel b)
                {
                    return (a.TimeObject < b.TimeObject ? 1 : a.TimeObject == b.TimeObject ? 0 : -1);
                }
                );
            App.ViewModel.Items.Clear();
            App.ViewModel.ListItems.ForEach(p => App.ViewModel.Items.Add(p));
        }

        private void filtPicturePage()
        {
            App.ViewModel.ListPictureItems.Clear();
            App.ViewModel.PictureItems.Clear();

            // switch begin
            if (m_strDataSource == SelectOnly.DATASOURCE_SINAWEIBO)
            {
                App.ViewModel.ListPictureItems.AddRange(App.ViewModel.SinaWeiboPicItems);
            }
            else if (m_strDataSource == SelectOnly.DATASOURCE_RSS)
            {
                App.ViewModel.ListPictureItems.AddRange(App.ViewModel.RssPicItems);
            }
            // switch end

            App.ViewModel.ListPictureItems.Sort(
                delegate(PictureItem a, PictureItem b)
                {
                    return (a.TimeObject < b.TimeObject ? 1 : a.TimeObject == b.TimeObject ? 0 : -1);
                });
            int count = App.ViewModel.ListPictureItems.Count;
            if (count < 9)
            {
                int remain = 9 - count;
                for (; remain != 0; --remain)
                {
                    App.ViewModel.ListPictureItems.Add(new PictureItem());
                }
            }
            for (int i = 0; i < 9; i++)
                App.ViewModel.PictureItems.Add(App.ViewModel.ListPictureItems[i]);

        }

        private void refreshNewsPage()
        {
            Microsoft.Phone.Shell.SystemTray.ProgressIndicator = new Microsoft.Phone.Shell.ProgressIndicator();
            m_progressIndicatorHelper = new ProgressIndicatorHelper(Microsoft.Phone.Shell.SystemTray.ProgressIndicator, RefreshViewHelper.RefreshViewItems);

            m_progressIndicatorHelper.PushTask("Weibo");
            m_progressIndicatorHelper.PushTask("Rss");
            m_progressIndicatorHelper.PushTask("Renren");
            
            App.ViewModel.Items.Clear();
            // 1.Weibo
            refreshModelSinaWeibo();
            // 2.Rss
            refreshModelRssFeed();
            // 3.Renren
            refreshModelRenren();

            //m_doubanHelper.getRequestToken();


            App.ViewModel.IsChanged = false;
        }

        private void refreshModelRenren()
        {            
            if (!App.RenrenAPI.IsAccessTokenValid())
            {
                m_progressIndicatorHelper.PopTask("Renren");
                return;
            }

            String renrenFollowID = PreferenceHelper.GetPreference("Renren_FollowerID");
            if(String.IsNullOrEmpty(renrenFollowID))
            {
                m_progressIndicatorHelper.PopTask("Renren");
                return;
            }

            List<APIParameter> param = new List<APIParameter>();
            param.Add(new APIParameter("method", "feed.get"));
            // 当前只获取     
            // 10:更新状态
            // 20:发表日志 
            // 30:上传照片
            // 的新鲜事，以逗号分隔
            param.Add(new APIParameter("type", "10"));
            param.Add(new APIParameter("uid", renrenFollowID));            
            // TODO: 注意此处30条合不合适
            param.Add(new APIParameter("count", "30"));
            App.RenrenAPI.RequestAPIInterface(RenrenFeedGetCallback, param);
        }

        private void RenrenFeedGetCallback(object sender, APIRequestCompletedEventArgs e)
        {           
            // Success
            if (e.Error == null)
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<RenrenNews>));
                App.ViewModel.RenrenItems.Clear();
                List<RenrenNews> searchResult = serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(e.ResultJsonString))) as List<RenrenNews>;
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    searchResult.ForEach(p => App.ViewModel.RenrenItems.Add(RenrenModelConverter.ConvertRenrenNewsToCommon(p)));
                    m_progressIndicatorHelper.PopTask("Renren");
                });   
            }
            // Fail
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    m_progressIndicatorHelper.PopTask("Renren");
                });
            }   
        }

        // Weibo logic
        private SdkCmdBase cmdBase;
        private SdkNetEngine netEngine;

        private void refreshMySinaAccount()
        {
            // Define a new net engine
            netEngine = new SdkNetEngine();

            // Define a new command base
            cmdBase = new SdkCmdBase
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
                            WeiboLogin(null, null);
                        }
                    });
                }
            });
        }

        private void chooseSinaWeiboFriend(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SinaWeibo/SelectSinaFollower.xaml", UriKind.Relative));
        }
        private void refreshModelSinaWeibo()
        {
            refreshMySinaAccount();
            App.ViewModel.SinaWeiboPicItems.Clear();
            App.ViewModel.SinaWeiboItems.Clear();
            LoadSinaWeiboContent();
        }

        private void LoadSinaWeiboContent()
        {
            if (String.IsNullOrEmpty(PreferenceHelper.GetPreference("SinaWeibo_FollowerID")))
            {
                MessageBox.Show("尚未设置新浪微博关注对象");
                m_progressIndicatorHelper.PopTask();
                return;
            }
            // Define a new net engine
            netEngine = new SdkNetEngine();

            // Define a new command base
            cmdBase = new cdmUserTimeline
            {
                acessToken = App.SinaWeibo_AccessToken,
                userId = App.ViewModel.SinaWeiboCareID,
                count = "20"
            };
            // Request server, the last parameter is set as default (".xml")
            netEngine.RequestCmd(SdkRequestType.USER_TIMELINE, cmdBase,
                // Requeset callback
                delegate(SdkRequestType requestType, SdkResponse response)
                {
                    if (response.errCode == SdkErrCode.SUCCESS)
                    {
                        WStatuses statuses = null;
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(WStatuses));
                        statuses = ser.ReadObject(response.stream) as WStatuses;
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            foreach (WStatus status in statuses.statuses)
                            {
                                App.ViewModel.SinaWeiboItems.Add(SinaWeiboModelConverter.ConvertSinaWeiboToCommon(status));
                            }
                            m_progressIndicatorHelper.PopTask();
                        }
                        );
                    }
                    else
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            MessageBox.Show(response.content, response.errCode.ToString(), MessageBoxButton.OK);
                            m_progressIndicatorHelper.PopTask();
                        });

                    }
                });
        }
        private void refreshModelRssFeed()
        {
            string url = "http://blog.sina.com.cn/rss/1713845420.xml";
            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri(url));

        }

        private void client_DownloadStringCompleted(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            XmlReader reader;
            try
            {
                reader = XmlReader.Create(new StringReader(e.Result));
            }
            catch (System.Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    m_progressIndicatorHelper.PopTask("Rss");
                });
                return;
            }
           
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                App.ViewModel.RssItems.Clear();
                foreach (SyndicationItem item in feed.Items)
                {
                    App.ViewModel.RssItems.Add(FeedModelConverter.ConvertFeedToCommon(item));
                }
                m_progressIndicatorHelper.PopTask("Rss");
            }
            );
        }


        private void button2_Click(object sender, RoutedEventArgs e)
        {
            refreshMainViewModel();
        }

        private void hubRenren_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void Grid_TextInput(object sender, TextCompositionEventArgs e)
        {

        }


        private void MainListBoxSelectionChanged(object sender, EventArgs e)
        {
            if (MainList.SelectedIndex != 0)
            {
                ItemViewModel item = App.ViewModel.Items[MainList.SelectedIndex];
                if (item.Type == ItemViewModel.EntryType.Feed)
                {
                    NavigationService.Navigate(new Uri("/Views/Rss/RssDetails.xaml?item=" + MainList.SelectedIndex, UriKind.Relative));
                }
            }
        }

        private void Test(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Lab/TimeSpan.xaml", UriKind.Relative));
        }

        private void SinaAcount_Click(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SinaWeibo/SinaAcount.xaml", UriKind.Relative));
        }

        private void RssAcount_Click(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Rss/RssAcount.xaml", UriKind.Relative));
        }


        private void toggleUsePassword_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)toggleUsePassword.IsChecked)
            {
                if (string.IsNullOrEmpty(PreferenceHelper.GetPreference("Global_Password")))
                {
                    NavigationService.Navigate(new Uri("/Views/Password/EditPassWord.xaml", UriKind.Relative));
                }
            }
            else
            {
                NavigationService.Navigate(new Uri("/Views/Password/PassWord.xaml?Type=DeletePassword", UriKind.Relative));
            }
        }

        private void RenrenAcount_Click(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Renren/RenrenAccount.xaml", UriKind.Relative));
        }
    }


    public class TiltableControl : Grid
    {
    }

}