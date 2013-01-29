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
using Care.Tool;
namespace Care.Views.Lab
{
    public partial class LabBlessPostPage : PhoneApplicationPage
    {
        public static int MAX_COUNT = 60;
        BlessHelper blessHelper;
        public LabBlessPostPage()
        {
            InitializeComponent();
            InitUI();
            
        }

        private void InitUI()
        {
            lblName.Text = MiscTool.GetMyName();
            lblCountLeft.Text = MAX_COUNT.ToString();
            lblContent.MaxLength = MAX_COUNT;
        }

        private void Send_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(lblContent.Text))
            {
                MessageBox.Show("据说要智商超过250才能看到您写的字？", ">_<", MessageBoxButton.OK);
                return;
            }

            if (blessHelper == null)
                blessHelper = new BlessHelper();
            blessHelper.PostBlessItem(lblName.Text, lblContent.Text, (suc) =>
            {
                if (suc)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show("发送成功", "^_^", MessageBoxButton.OK);
                        NavigationService.GoBack();
                    });
                }
                else
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show("发送失败，请保持网络连接通畅", ">_<", MessageBoxButton.OK);
                    });
                }
            });
        }

        private void Content_Changed(object sender, TextChangedEventArgs e)
        {
            lblCountLeft.Text = (MAX_COUNT - lblContent.Text.Length).ToString();
        }
    }
}