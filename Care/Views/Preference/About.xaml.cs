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
using Microsoft.Phone.Tasks;

namespace Care.Views.Preference
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
        }
        
        
        private void SendMail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask emailcomposer = new EmailComposeTask();
            emailcomposer.To = "thankcreate@gmail.com";
            emailcomposer.Subject = "论改进一下的必要性";
            emailcomposer.Body = "请喷得温柔些哦  ^_^  ~~";
            emailcomposer.Show();
        }


        private void GotoSite_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask brTask = new WebBrowserTask();
            brTask.Uri = new Uri("http://thankcreate.github.com/Care/");
            brTask.Show();
        }

        private void Mark_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
                marketplaceReviewTask.Show();
            }
            catch (System.Exception ex)
            {
            }
        }

        private void Refresh_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();

                marketplaceDetailTask.ContentIdentifier = "380eeee6-2040-4a3a-bae2-0f09939aea77";
                marketplaceDetailTask.ContentType = MarketplaceContentType.Applications;

                marketplaceDetailTask.Show();
            }
            catch (System.Exception ex)
            {
            }
        } 
    }
}