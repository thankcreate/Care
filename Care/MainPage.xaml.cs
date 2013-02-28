using System;
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
using System.Windows.Threading;
using Microsoft.Phone.Shell;
using Care.Views;
using Care.Tool;
using RenrenSDKLibrary;
using DoubanSDK;
using System.Windows.Navigation;
using Care.Views.Common;
using Care.Controls;
namespace Care
{

    public partial class MainPage : PhoneApplicationPage
    {   
        ProgressIndicatorHelper m_progressIndicatorHelper;
        bool m_bIsNavigateFromSelectPage;
        string m_strShowType = "";
        string m_strDataSource = "";

        private DispatcherTimer timer;
        private int HUBTILE_INTERVAL = 2;

        MaskViewHelper maskViewHelper;

        // Constructor
        public MainPage()
        {
            App.NeedChangeStartPage = false;
            m_bIsNavigateFromSelectPage = false;

            TiltEffect.TiltableItems.Add(typeof(TiltableControl));

            InitializeComponent();


            ApplicationBar.IsVisible = false;
            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);            
            InitSinaWeiboInfo();
            InitTimer();
        }

        private void InitTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(HUBTILE_INTERVAL);
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("确定退出程序？", "提示", MessageBoxButton.OKCancel);
            // 如果确实退出, 则清空back stack，退出
            if (result == MessageBoxResult.OK)
            {
                while (NavigationService.BackStack.Any())
                {
                    NavigationService.RemoveBackEntry();
                }
                // 因为这里本质上是通过抛异常退出的，所以应该提前收尾
                // 比如保存本地存储
                PreferenceHelper.SavePreference();
                NavigationService.GoBack();
            }
            // 否则取消退出过程
            e.Cancel = true;            
        }

        private void InitSinaWeiboInfo()
        {
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
            /*
           * 必须要把BlessingPage放到这里作为一个遮罩层
           * 不然的话会导致从BlessingPage到MainPage的跳转时间过长
           */
            bool useBlessingPage = PreferenceHelper.GetPreference("Global_UseBlessingPage") != "False";
            if (useBlessingPage)
            {
                if (!App.ViewModel.HasLoadBless)
                {
                    if (maskViewHelper == null)
                        maskViewHelper = new MaskViewHelper();
                    maskViewHelper.InitView(this, bsControl);
                    App.ViewModel.HasLoadBless = true;
                }
                else
                {   
                    ApplicationBar.IsVisible = true;
                }
            }
            else
            {                
                ApplicationBar.IsVisible = true;
            }

            if (App.ViewModel.IsChanged)
            {
                refreshMainViewModel();
            }
        }


        Control bsControl;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            /*
             * 必须要把BlessingPage放到这里作为一个遮罩层
             * 不然的话会导致从BlessingPage到MainPage的跳转时间过长
             * 这里先把底层alpha设为0,否则会显示1秒左右的底层再跳转到bless层
            */
            bool useBlessingPage = PreferenceHelper.GetPreference("Global_UseBlessingPage") != "False";
            if (useBlessingPage)
            {
                if (!App.ViewModel.HasLoadBless)
                {  
                    this.Content.Opacity = 0;
                }
            }
            else
            {
                ApplicationBar.IsVisible = true;
            }


            toggleUsePassword.IsChecked = App.ViewModel.UsingPassword == "True" ? true : false;
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

      

        private void cancleEvent(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.GoBack();
            });
        }
        private void Refresh_Click(object sender, EventArgs e)
        {
            // 目前对Refresh的设定是直接刷新所有源
            // 不理会过滤器发过来的东西
            refreshNewsPage();
        }

        private void Filt_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SelectOnly.xaml", UriKind.Relative));
        }

        private void SendStatus_Click(object sender, EventArgs e)
        {            
            NavigationService.Navigate(new Uri("/Views/Common/CommitSelectPage.xaml", UriKind.Relative));
        }

        

        private void Microscope_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Lab/LabPage.xaml", UriKind.Relative));
        }

        private void refreshMainViewModel()
        {
            // 如果是由SelectOnly页面转发过来，且不是选则“所有”，则不进行网络数据请求，只更新UI
            if (m_bIsNavigateFromSelectPage && m_strDataSource != SelectOnly.DATASOURCE_ALL)
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
            else if (m_strDataSource == SelectOnly.DATASOURCE_RENREN)
            {
                App.ViewModel.ListItems.AddRange(App.ViewModel.RenrenItems);
            }
            else if (m_strDataSource == SelectOnly.DATASOURCE_DOUBAN)
            {
                App.ViewModel.ListItems.AddRange(App.ViewModel.DoubanItems);
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
            else if (m_strDataSource == SelectOnly.DATASOURCE_RENREN)
            {
                App.ViewModel.ListPictureItems.AddRange(App.ViewModel.RenrenPicItems);
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
            m_progressIndicatorHelper.PushTask("Douban");
            
            //App.ViewModel.Items.Clear();
            // 1.Weibo
            refreshModelSinaWeibo();
            // 2.Rss
            refreshModelRssFeed();
            // 3.Renren
            refreshModelRenren();
            // 4:Douban
            refreshModelDouban();

            App.ViewModel.IsChanged = false;
        }

        private void refreshModelDouban()
        {
            App.ViewModel.DoubanItems.Clear();
            App.ViewModel.DoubanPicItems.Clear();

            // 如果检测到过期，则直接调用API的RefreshToken，重新来一次
            if (!String.IsNullOrEmpty(PreferenceHelper.GetPreference("Douban_Token"))
                && App.DoubanAPI.IsAccessTokenOutOfDate())
            {
                App.DoubanAPI.RefreshToken((bool isSucess, DoubanSdkAuthError errCode, DoubanSdkAuth2Res response) =>
                {
                    if (errCode.errCode == DoubanSdkErrCode.SUCCESS)
                    {
                        PreferenceHelper.SetPreference("Douban_Token", response.accesssToken);
                        PreferenceHelper.SavePreference();
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            refreshModelDouban();
                        });
                        return;
                    }
                    else
                    {

                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            if (!String.IsNullOrEmpty(PreferenceHelper.GetPreference("Douban_ID")))
                            {
                                // 这里不能直接remove掉，因为有可能只是网络连接异常
                                //PreferenceHelper.RemoveDoubanLoginAccountPreference();
                                MessageBox.Show("网络连接异常或豆瓣登陆已过期~", ">_<", MessageBoxButton.OK);
                            }
                            m_progressIndicatorHelper.PopTask("Douban");
                        });
                    }
                });                
            }
            else
            {
                if (String.IsNullOrEmpty(PreferenceHelper.GetPreference("Douban_ID")))
                {
                    m_progressIndicatorHelper.PopTask("Douban");
                    return;
                }       

                String doubanFollowID = PreferenceHelper.GetPreference("Douban_FollowerID");
                if (String.IsNullOrEmpty(doubanFollowID))
                {
                    m_progressIndicatorHelper.PopTask("Douban");
                    return;
                }
                String strCount = PreferenceHelper.GetPreference("Douban_RecentCount");
                if (string.IsNullOrEmpty(strCount))
                {
                    strCount = "30";
                }

                App.DoubanAPI.GetUserTimeLine(doubanFollowID, int.Parse(strCount), DoubanFeedGetCallback);
            }           
        }

        private void DoubanFeedGetCallback(GetUserTimeLineEventArgs args)
        {
            if (args.errorCode == DoubanSdkErrCode.SUCCESS && args.statues != null)
            {                
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    foreach (DoubanSDK.Statuses statues in args.statues)
                    {
                        ItemViewModel model = DoubanModelConverter.ConvertDoubanUnionStatues(statues);
                        if (model != null)
                        {
                            App.ViewModel.DoubanItems.Add(model);
                        }      
                    }
                    m_progressIndicatorHelper.PopTask("Douban");
                });
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (args.specificCode == "106")
                    {
                        MessageBox.Show("豆瓣授权已过期，请重新登陆", ">_<", MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageBox.Show("豆瓣信息获取发生未知错误，请确保网络连接正常", ">_<", MessageBoxButton.OK);
                    }
                    m_progressIndicatorHelper.PopTask("Douban");
                });
            }
        }

        private void refreshModelRenren()
        {
            App.ViewModel.RenrenItems.Clear();
            App.ViewModel.RenrenPicItems.Clear();
            
            if (!App.RenrenAPI.IsAccessTokenValid())
            {
                // 有值说明之前登陆过，须提示过期
                if (!String.IsNullOrEmpty(PreferenceHelper.GetPreference("Renren_ID")))
                {
                    MessageBox.Show("人人帐号授权已过期，请重新登陆", ">_<", MessageBoxButton.OK);
                }
                PreferenceHelper.RemoveRenrenLoginAccountPreference();
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
            // 20:发表日志,先不做了，太麻烦 
            // 30:上传照片
            // 32:分享照片
            // 的新鲜事，以逗号分隔
            param.Add(new APIParameter("type", "10,30,32"));
            param.Add(new APIParameter("uid", renrenFollowID));                        
            String strCount = PreferenceHelper.GetPreference("Renren_RecentCount");
            if (string.IsNullOrEmpty(strCount))
            {
                strCount = "30";
            }
            param.Add(new APIParameter("count", strCount));
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
                    searchResult.ForEach(p =>
                    {
                        ItemViewModel model = RenrenModelConverter.ConvertRenrenNewsToCommon(p);
                        if (model != null)
                        {
                            App.ViewModel.RenrenItems.Add(model);
                        }                        
                    });                                       
                    m_progressIndicatorHelper.PopTask("Renren");
                });   
            }
            // Fail
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (!String.IsNullOrEmpty(PreferenceHelper.GetPreference("Renren_ID")))
                    {
                        MessageBox.Show("人人信息源获取失败，可能是网络问题，也可能是帐号过期", ">_<", MessageBoxButton.OK);
                    } 
                    m_progressIndicatorHelper.PopTask("Renren");
                });
            }   
        }

        // Weibo logic
        private SdkCmdBase cmdBase;
        private SdkNetEngine netEngine;
        
        private void chooseSinaWeiboFriend(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SinaWeibo/SelectSinaFollower.xaml", UriKind.Relative));
        }
        private void refreshModelSinaWeibo()
        {
            App.ViewModel.SinaWeiboPicItems.Clear();
            App.ViewModel.SinaWeiboItems.Clear();
            if (String.IsNullOrEmpty(PreferenceHelper.GetPreference("SinaWeibo_ID")))
            {
                m_progressIndicatorHelper.PopTask("Sina");
                return;
            }            
            LoadSinaWeiboContent();
        }

        private void LoadSinaWeiboContent()
        {
            try
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
                String strCount = PreferenceHelper.GetPreference("SinaWeibo_RecentCount");
                if (string.IsNullOrEmpty(strCount))
                {
                    strCount = "30";
                }
                String careID = PreferenceHelper.GetPreference("SinaWeibo_FollowerID");
                cmdBase = new cdmUserTimeline
                {
                    acessToken = App.SinaWeibo_AccessToken,
                    userId = careID,
                    count = strCount
                };
                // Request server, the last parameter is set as default (".xml")
                netEngine.RequestCmd(SdkRequestType.USER_TIMELINE, cmdBase,
                    // Requeset callback
                    LoadSinaWeiboContentComplete);
            }
            catch (System.Exception ex)
            {
                UmengSDK.UmengAnalytics.reportError(ex);
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("新浪微博加载过程中发生未知错误", "悲剧了>_<", MessageBoxButton.OK);
                    m_progressIndicatorHelper.PopTask();
                });
            }         
        }

        private void LoadSinaWeiboContentComplete(SdkRequestType requestType, SdkResponse response)
        {
            try
            {
                if (response.errCode == SdkErrCode.SUCCESS)
                {
                    WStatuses statuses = null;
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(WStatuses));
                    statuses = ser.ReadObject(response.stream) as WStatuses;
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        if (statuses.statuses != null)
                        {
                            foreach (WStatus status in statuses.statuses)
                            {
                                ItemViewModel model = SinaWeiboModelConverter.ConvertItemToCommon(status);
                                if (model != null)
                                {
                                    App.ViewModel.SinaWeiboItems.Add(model);
                                }
                            }
                        }
                        m_progressIndicatorHelper.PopTask();
                    }
                    );
                }
                // 失败
                else
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        // 21327 expired_token Token 过期
                        if (response.specificCode == "21327")
                        {
                            MessageBox.Show("新浪微博帐号已过期，请重新登陆", ">_<", MessageBoxButton.OK);
                            // 清掉保存的当前帐号信息
                            // 但是关注人信息还保留着
                            PreferenceHelper.RemoveSinaWeiboLoginAccountPreference();
                        }

                        m_progressIndicatorHelper.PopTask();
                    });
                }
            }
            catch (System.Exception ex)
            {
                UmengSDK.UmengAnalytics.reportError(ex);
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("新浪微博加载过程中发生未知错误", "悲剧了>_<", MessageBoxButton.OK);
                    m_progressIndicatorHelper.PopTask();
                });
            }
        }


        private void refreshModelRssFeed()
        {
            App.ViewModel.RssItems.Clear();
            string url = PreferenceHelper.GetPreference("RSS_FollowerPath");
            if (string.IsNullOrEmpty(url))
            {
                m_progressIndicatorHelper.PopTask();
                return;                
            }
            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri(url));
        }

        private void client_DownloadStringCompleted(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            XmlReader reader;
            SyndicationFeed feed;
            try
            {
                reader = XmlReader.Create(new StringReader(e.Result));
                feed = SyndicationFeed.Load(reader);                
            }
            catch (System.Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    m_progressIndicatorHelper.PopTask("Rss");
                });
                return;
            }           
            
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {                
                foreach (SyndicationItem item in feed.Items)
                {
                    ItemViewModel model = RSSFeedModelConverter.ConvertFeedToCommon(item);
                    if (model != null)
                    {
                        App.ViewModel.RssItems.Add(model);
                    }                    
                }
                m_progressIndicatorHelper.PopTask("Rss");
            });
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


        // V1.4版后重构了传值方法，直接在OnNavigatedFrom里把整个ItemViewModel传过去了
        private void MainListBoxSelectionChanged(object sender, EventArgs e)
        {
            if (MainList.SelectedIndex != -1)
            {
                ItemViewModel item = App.ViewModel.Items[MainList.SelectedIndex];
                m_lastSelectedIndex = MainList.SelectedIndex;
                if (item.Type == EntryType.Feed)
                {                    
                    NavigationService.Navigate(new Uri("/Views/Rss/RssDetails.xaml", UriKind.Relative));
                }
                if (item.Type == EntryType.SinaWeibo
                    || item.Type == EntryType.Renren
                    || item.Type == EntryType.Douban)
                {
                    NavigationService.Navigate(new Uri("/Views/Common/StatuesView.xaml", UriKind.Relative));
                }
            }
            MainList.SelectedIndex = -1;
        }

        private int m_lastSelectedIndex = 0;
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.Content is StatuesView)
            {
                StatuesView statuesView = e.Content as StatuesView;
                statuesView.m_itemViewModel = App.ViewModel.Items[m_lastSelectedIndex];
            }
            else if (e.Content is RssDetails)
            {
                RssDetails rssDetailsView = e.Content as RssDetails;
                rssDetailsView.m_itemViewModel = App.ViewModel.Items[m_lastSelectedIndex];
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



        private void toggleUseBlessingPage_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)toggleUseBlessingPage.IsChecked)
            {
                App.ViewModel.UseBlessingPage = "True";
            }
            else
            {
                App.ViewModel.UseBlessingPage = "False";
            }
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

        private void NavigateImageView(int index)
        {
            StringBuilder url = new StringBuilder();
            url.Append("/Views/Common/ImageView.xaml");
            url.AppendFormat("?Index={0}", index);
            NavigationService.Navigate(new Uri(url.ToString(), UriKind.Relative));
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Lab/Test/TimeSpanWrapper.xaml", UriKind.Relative));
        }

        private void toggleFetchImageInRetweet_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)toggleFetchImageInRetweet.IsChecked)
            {
                App.ViewModel.NeedFetchImageInRetweet = "True";
            }
            else
            {
                App.ViewModel.NeedFetchImageInRetweet = "False";
            }
        }

        private void textSeletRefreshItemCount_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Preference/SetFreshItemCount.xaml", UriKind.Relative));
        }

        private void TiltableControl_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Preference/SetTileTheme.xaml", UriKind.Relative));
        }

        private void About_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Preference/About.xaml", UriKind.Relative));
        }

        private void DoubanAcount_Click(object sender, System.Windows.Input.GestureEventArgs e)
        {            
            NavigationService.Navigate(new Uri("/Views/Douban/DoubanAccount.xaml", UriKind.Relative));         
        }

        private void PicList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PicList.SelectedIndex != -1)
            {
                NavigateImageView(PicList.SelectedIndex);
            }
            PicList.SelectedIndex = -1;
        }

        private List<HubTile> hubtiles = new List<HubTile>();
        
        private void hubTileLoaded(object sender, RoutedEventArgs e)
        {
            HubTile hub = sender as HubTile;                   
            hub.IsFrozen = true;
            hub.Tag = "Expanded";
            if (!hubtiles.Contains(hub))
            {
                hubtiles.Add(hub);
            }            
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (hubtiles == null || hubtiles.Count == 0)
                return;
            Random r = new Random();
            foreach(HubTile hub in hubtiles)
            {
                int result = r.Next(5);

                String tag = hub.Tag as String;
                if (tag == "Expanded")
                {
                    if (result <= 3)
                    {
                        hub.Tag = "Expanded";
                        VisualStateManager.GoToState(hub, "Expanded", true);
                    }
                    else if (result <= 4)
                    {
                        hub.Tag = "Flipped";
                        VisualStateManager.GoToState(hub, "Flipped", true);
                    }
                }
                else if (tag == "Flipped")
                {
                    hub.Tag = "Expanded";
                    VisualStateManager.GoToState(hub, "Expanded", true);
                }
            }
        }
    }

    public class TiltableControl : Grid
    {
    }
}