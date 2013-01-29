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
using System.Windows.Threading;
using System.Windows.Media.Imaging;

using Care.Tool;
namespace Care.Views.Startup
{
    public partial class BlessingPage : PhoneApplicationPage
    {
        private int PER_SHOW_TIME = 8; // 每张图显示的总时间
        private int MIX_SHOW_TIME = 2; // 两张图一起显示的时间(通过alpha混合在一起)
        private int activeFlag = 1;
        private DispatcherTimer timer;

        private int mBkgIndex = 0;
        private int mItemIndex = 0;
        private List<WriteableBitmap> listBitmap;
        private List<BlessItem> listItems;
        private BlessHelper blessHelper;
        public BlessingPage()
        {
            InitializeComponent();


            this.Loaded += new RoutedEventHandler(BlessingPage_Loaded);
        }

        private void InitTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval =TimeSpan.FromSeconds(PER_SHOW_TIME - MIX_SHOW_TIME);
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (activeFlag == 1)
            {
                image2.Source = listBitmap[mBkgIndex];
                Image2FadeIn.Begin();
                activeFlag = 2;

                mBkgIndex = ++mBkgIndex % listBitmap.Count;
            }
            else
            {
                image1.Source = listBitmap[mBkgIndex];
                Image1FadeIn.Begin();
                activeFlag = 1;

                mBkgIndex = ++mBkgIndex % listBitmap.Count;
            }
            RefreshItemText();
        }

        void RefreshItemText()
        {
            BlessItem item = listItems[mItemIndex];
            lblContent.Text = item.Content;
            lblName.Text = "— " + item.Name;
            TextFadeIn.Begin();
            mItemIndex = ++mItemIndex % listItems.Count;
        }

        private void BlessingPage_Loaded(object sender, RoutedEventArgs e)
        {
            ArrowFadeIn.Begin();
            blessHelper = new BlessHelper();
            listItems = blessHelper.GetCachedBlessItem();
            if (listItems == null || listItems.Count == 0)
                GotoNextPage();

            blessHelper.GetBlessImages((List<WriteableBitmap> map) =>
            {
                listBitmap = map;
                if (map == null || map.Count == 0)
                    GotoNextPage();
                
                InitTimer();
                Timer_Tick(null, null);
            });

          
            BlessHelper helper = new BlessHelper();
            helper.CacheBlessImages();
            helper.CacheBlessItem();
        }

        private void GotoNextPage()
        {
            bool needPassWord = PreferenceHelper.GetPreference("Global_UsePassword") == "True";
            if (needPassWord)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    NavigationService.Navigate(new Uri("/Views/Password/PassWord.xaml", UriKind.Relative));
                });  
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                });                  
            }
        }

        private void Enter_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            GotoNextPage();
        }
    }
}