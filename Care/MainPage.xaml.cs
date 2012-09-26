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
using WeiboSdk;
using WeiboSdk.PageViews;
using Hammock;
using Hammock.Web;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.IO.IsolatedStorage;
using System.Xml;
using System.ServiceModel.Syndication;

namespace Care
{

    public partial class MainPage : PhoneApplicationPage
    {
        DoubanHelper m_doubanHelper;
        // Constructor
        public MainPage()
        {
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
            if (settings.Contains("Token"))
            {
                App.AccessToken = settings["Token"] as string;
            }
            if (settings.Contains("SinaWeiboCareID"))
            {
                App.ViewModel.SinaWeiboCareID = settings["SinaWeiboCareID"] as string;
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
                    App.AccessToken = response.accesssToken;
                    App.RefleshToken = response.refleshToken;
                }

                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                try
                {
                    settings.Add("Token", response.accesssToken);
                }
                catch (ArgumentException ex)
                {
                    settings["Token"] = response.accesssToken;
                }
                settings.Save();

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MainPanorama.DefaultItem = MainPanorama.Items[2];
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

        private void refreshMainViewModel()
        {
            refreshNewsPage();
            // refreshPicturePage();
        }

        private void refreshNewsPage()
        {
            Microsoft.Phone.Shell.SystemTray.ProgressIndicator = new Microsoft.Phone.Shell.ProgressIndicator();
            Microsoft.Phone.Shell.SystemTray.ProgressIndicator.Text = "数据传输中";
            Microsoft.Phone.Shell.SystemTray.ProgressIndicator.IsIndeterminate = true;
            Microsoft.Phone.Shell.SystemTray.ProgressIndicator.IsVisible = true;
            App.ViewModel.Items.Clear();
            // 1.Weibo
            refreshModelSinaWeibo();
            // 2.Rss
            refreshModelRssFeed();

            //m_doubanHelper.getRequestToken();


            App.ViewModel.IsChanged = false;
        }

        private void refreshPicturePage()
        {
            App.ViewModel.PictureItems.Clear();
            App.ViewModel.ListPictureItems.Clear();
            App.ViewModel.ListPictureItems.AddRange(App.ViewModel.SinaWeiboPicItems);
            App.ViewModel.ListPictureItems.Sort(
                delegate(PicureItem a, PicureItem b)
                {
                    return (a.TimeObject < b.TimeObject ? 1 : a.TimeObject == b.TimeObject ? 0 : -1);
                });
            int count = App.ViewModel.ListPictureItems.Count;
            if(count < 9)
            {
                int remain = 9 - count;
                for (; remain != 0; --remain)
                {
                    App.ViewModel.ListPictureItems.Add(new PicureItem());
                }
            }
            for (int i = 0; i < 9; i++)
                App.ViewModel.PictureItems.Add(App.ViewModel.ListPictureItems[i]);
        }

        // Aggregation logic
        private void refreshViewItems()
        {
            App.ViewModel.ListItems.Clear();
            App.ViewModel.ListItems.AddRange(App.ViewModel.SinaWeiboItems);
            App.ViewModel.ListItems.AddRange(App.ViewModel.RssItems);
            App.ViewModel.ListItems.Sort(
                delegate(ItemViewModel a, ItemViewModel b)
                {
                    return (a.TimeObject < b.TimeObject ? 1 : a.TimeObject ==  b.TimeObject ? 0 : -1);
                }
                );
            App.ViewModel.Items.Clear();
            App.ViewModel.ListItems.ForEach(p => App.ViewModel.Items.Add(p));
            refreshPicturePage();
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
                acessToken = App.AccessToken,
            };
            RestRequest request = new RestRequest();
            request.Method = WebMethod.Get;

            request.Path = "/account/get_uid.json";
            request.AddParameter("access_token", App.AccessToken);
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
            NavigationService.Navigate(new Uri("/SelectSinaFollower.xaml", UriKind.Relative));
        }
        private void refreshModelSinaWeibo()
        {
            refreshMySinaAccount();
            LoadSinaWeiboContent();
        }

        private void LoadSinaWeiboContent()
        {
            if (App.ViewModel.SinaWeiboCareID == null)
            {
                MessageBox.Show("尚未设置新浪微博关注对象");
                Microsoft.Phone.Shell.SystemTray.ProgressIndicator.Text = "";
                Microsoft.Phone.Shell.SystemTray.ProgressIndicator.IsIndeterminate = false;
                return;
            }
            // Define a new net engine
            netEngine = new SdkNetEngine();

            // Define a new command base
            cmdBase = new cdmUserTimeline
            {
                acessToken = App.AccessToken,
                userId = App.ViewModel.SinaWeiboCareID,
                count = "20"
            };
            // Request server, the last parameter is set as default (".xml")
            netEngine.RequestCmd(SdkRequestType.USER_TIMELINE, cmdBase,
                // Requeset callback
                delegate(SdkRequestType requestType, SdkResponse response)
                {
                    //Deployment.Current.Dispatcher.BeginInvoke(() => ProgressIndicatorIsVisible = false);
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
                            Microsoft.Phone.Shell.SystemTray.ProgressIndicator.Text = "";
                            Microsoft.Phone.Shell.SystemTray.ProgressIndicator.IsIndeterminate = false;
                            refreshViewItems();
                        }
                        );
                    }
                    else
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show(response.content, response.errCode.ToString(), MessageBoxButton.OK));
                        Microsoft.Phone.Shell.SystemTray.ProgressIndicator.Text = "";
                        Microsoft.Phone.Shell.SystemTray.ProgressIndicator.IsIndeterminate = false;
                        refreshViewItems();
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

        private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            XmlReader reader = XmlReader.Create(new StringReader(e.Result));
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                App.ViewModel.RssItems.Clear();
                foreach (SyndicationItem item in feed.Items)
                {
                    App.ViewModel.RssItems.Add(FeedModelConverter.ConvertFeedToCommon(item));
                }
                refreshViewItems();
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
                NavigationService.Navigate(new Uri("/RssDetails.xaml?item=" + MainList.SelectedIndex, UriKind.Relative));
            }
        }

        private void Test(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SelectOnly.xaml", UriKind.Relative));
        }
    }


    public class TiltableControl : Grid
    {
    }

}