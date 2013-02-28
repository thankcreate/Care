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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RenrenSDKLibrary;
using WeiboSdk;
using DoubanSDK;
using Care.Tool;
using SmartMad.Ads.WindowsPhone7.WPF;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Care
{
    public partial class App : Application
    {
        public static int Test1 = 0;
        public static int Test2 = 0;
        public static int Test3 = 0;
        private static MainViewModel viewModel = null;
        public static RenrenAPI RenrenAPI;
        public static DoubanAPI DoubanAPI;
        public static bool NeedChangeStartPage = true;
        /// <summary>
        /// A static ViewModel used by the views to bind against.
        /// </summary>
        /// <returns>The MainViewModel object.</returns>
        public static MainViewModel ViewModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (viewModel == null)
                    viewModel = new MainViewModel(); 
                return viewModel;
            }
        }

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = false;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
                     
            SinaWeiboInit();
            RenrenAPIInit();
            DoubanInit();
            LoadCache();
            // 启动页相关
            RootFrame.Navigating += new NavigatingCancelEventHandler(RootFrame_Navigating);
            // 广告参数设置
            AdView.SetApplicationID("fc955e087f89a189");


            String test = PreferenceHelper.GetPreference("FirstTimeAfterUpdateTo1.1");
            if (String.IsNullOrEmpty(test))
            {
                PreferenceHelper.RemoveSinaWeiboPreference();
                PreferenceHelper.RemoveRenrenPreference();
                RenrenAPI.LogOut();
                PreferenceHelper.RemoveDoubanPreference();
                DoubanAPI.LogOut();
                PreferenceHelper.SetPreference("FirstTimeAfterUpdateTo1.1", "whatever");
                PreferenceHelper.SavePreference();
            }   

        }

        private void LoadCache()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            ObservableCollection<ItemViewModel> outItem = null;
            if (settings.TryGetValue<ObservableCollection<ItemViewModel>>("Global_TimelineCache", out outItem))
            {
                if (outItem != null && outItem.Count != 0)
                {
                    foreach (ItemViewModel item in outItem)
                    {
                        App.ViewModel.Items.Add(item);
                    }
                }             
            }
        }

        private void RootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            // 如果不是到MainPage页，或者现在根本就不是启动过程中，就不管
            // 因我我们只想在启动过程中将转向MainPage的请求重定向到PassWord页
            // NeedChangeStartPage被初始化为True
            // 只要加载了任何一个页面就会被更改为False
            // 目前可能作为启动页的有Password.xaml和MainPage.xaml
            
            if (e.Uri.ToString().Contains("/MainPage.xaml") != true || !NeedChangeStartPage)
                return;
            NeedChangeStartPage = false;
            // 如果是到MainPage的话：
            bool needPassWord = PreferenceHelper.GetPreference("Global_UsePassword") == "True";
            //bool useBlessingPage = PreferenceHelper.GetPreference("Global_UseBlessingPage") != "False";
            e.Cancel = true;
            RootFrame.Dispatcher.BeginInvoke(delegate
            {
                
                if (needPassWord)
                {
                    RootFrame.Navigate(new Uri("/Views/Password/PassWord.xaml", UriKind.Relative));
                }                   
                else
                {
                    RootFrame.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                    
                }
            });
        }

        private void SinaWeiboInit()
        {
            SdkData.AppKey = "466921770";
            SdkData.AppSecret = "548cb1a27cf896d304a9704e2be0e62e";
            SdkData.RedirectUri = "http://thankcreate.github.com/Care";
        }

        private void RenrenAPIInit()
        {            
            // 三个参数分别是：应用ID， API Key, Secret Key
            RenrenAPI = new RenrenAPI("214071", "0b434803c2c7435691bd398eaf44d4fc", "172d2ba967924bc9b457983e1dba1127");
        }

        private void DoubanInit()
        {
            DoubanAPI = new DoubanAPI();
            DoubanSdkData.AppKey = "01ac4907dbc3c4590504db17934b4d0b";
            DoubanSdkData.RedirectUri = "http://thankcreate.github.com/Care/";
            DoubanSdkData.AppSecret = "3a781c654ea41560";
            DoubanSdkData.Scope = "shuo_basic_r,shuo_basic_w,douban_basic_common";
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            #if DEBUG
                UmengSDK.UmengAnalytics.setDebug(true);
                // caution : Don't change the default session continue interval unless you have known the rule !
                UmengSDK.UmengAnalytics.setSessionContinueInterval(TimeSpan.FromMilliseconds(5000));
            #endif

            UmengSDK.UmengAnalytics.onLaunching("50a76c2b5270156d3d0000f3");
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            UmengSDK.UmengAnalytics.onActivated("50a76c2b5270156d3d0000f3");
            // Ensure that application state is restored appropriately
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
            if (PreferenceHelper.GetPreference("Global_UsePassword") == "True")
            {

            }
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            // Ensure that required application state is persisted here.
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            UmengSDK.UmengAnalytics.reportError(e.ExceptionObject);
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }


        public static string SinaWeibo_AccessToken
        {
            get;
            set;
        }

        public static string SinaWeibo_AccessTokenSecret
        {
            get;
            set;
        }
        public static string SinaWeibo_RefleshToken
        {
            get;
            set;
        }




        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}