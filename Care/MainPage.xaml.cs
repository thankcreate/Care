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
using WeiboSdk;
using WeiboSdk.PageViews;
using System.Diagnostics;
using System.Runtime.Serialization.Json;

namespace Care
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            // 此处使用自己 AppKey 和 AppSecret，未经
            //审核的应用只支持用申请该Appkey的帐号来获取数据
            SdkData.AppKey = "1385220836";
            SdkData.AppSecret = "6c8a1cd05efe4ff69839de695c68e8f0";
            App.AccessToken = "2.00HRERzBYNPkVB04461a57ach9z8lB";

            // 您app设置的重定向页,必须一致
            SdkData.RedirectUri = "www.thankcreate.com";
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void sinaAccount_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void weiboLogin(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SdkData.AppKey) || string.IsNullOrEmpty(SdkData.AppSecret)
                || string.IsNullOrEmpty(SdkData.RedirectUri))
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("请在中MainPage.xmal.cs的构造函数中设置自己的appkey、appkeysecret、RedirectUri.");
                });
                return;
            }
            AuthenticationView.OAuth2VerifyCompleted = (e1, e2, e3) => VerifyBack(e1, e2, e3);
            AuthenticationView.OBrowserCancelled = new EventHandler(cancleEvent);
            //其它通知事件...

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/WeiboSdk;component/PageViews/AuthenticationView.xaml"
                    , UriKind.Relative));
            });
        
        }

        private void VerifyBack(bool isSucess, SdkAuthError errCode, SdkAuth2Res response)
        {
            if (errCode.errCode == SdkErrCode.SUCCESS)
            {
                if (null != response)
                {
                    App.AccessToken = response.accesssToken;
                    App.RefleshToken = response.refleshToken;
                }

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MainPanorama.DefaultItem = MainPanorama.Items[2];
                    refreshMainViewModel();
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                    //NavigationService.Navigate(new Uri("/PageViews/SampleTimeline.xaml",
                    //    UriKind.Relative));
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


        private void refreshMainViewModel()
        {
            App.ViewModel.Items.Clear();
            refreshModelSinaWeibo();
        }

        // Weibo logic
        private SdkCmdBase cmdBase;
        private SdkNetEngine netEngine;
        private void refreshModelSinaWeibo()
        {
            // Define a new net engine
            netEngine = new SdkNetEngine();

            // Define a new command base
            cmdBase = new cmdNormalMessages
            {
                acessToken = App.AccessToken,
                count = "20"
            };
            // Request server, the last parameter is set as default (".xml")
            netEngine.RequestCmd(SdkRequestType.FRIENDS_TIMELINE, cmdBase,
                // Requeset callback
                delegate(SdkRequestType requestType, SdkResponse response)
                {
                    //Deployment.Current.Dispatcher.BeginInvoke(() => ProgressIndicatorIsVisible = false);
                    if (response.errCode == SdkErrCode.SUCCESS)
                    {
                        WStatuses statuses = null;
                        try
                        {
                            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(WStatuses));
                            statuses = ser.ReadObject(response.stream) as WStatuses;
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                                {
                                    foreach (WStatus status in statuses.statuses)
                                    {
                                       App.ViewModel.Items.Add(SinaWeiboModelConverter.ConvertSinaWeiboToCommon(status));
                                    }
                                }
                            );
                        }
                        catch (Exception e)
                        {
                            throw;
                        }                       
                        
                    }
                    else
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show(response.content, response.errCode.ToString(), MessageBoxButton.OK));
                    }
                });
        }

        private void refreshModelRssFeed()
        {

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            refreshMainViewModel();
        }
    }
}