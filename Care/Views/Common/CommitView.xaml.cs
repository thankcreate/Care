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
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Phone;
using RenrenSDKLibrary;
using DoubanSDK;

namespace Care.Views.Common
{
    public partial class CommitView : PhoneApplicationPage
    {

        #region WordCountProperty
        public static readonly DependencyProperty WordCountProperty =
            DependencyProperty.Register("WordCount", typeof(string), typeof(CommitView), new PropertyMetadata((string)"140"));

        public string WordCount
        {
            get { return (string)GetValue(WordCountProperty); }
            set { SetValue(WordCountProperty, value); }
        }
        #endregion

        #region LogoSourceProperty
        public static readonly DependencyProperty LogoSourceProperty =
            DependencyProperty.Register("LogoSource", typeof(string), typeof(CommitView), new PropertyMetadata((string)""));

        public string LogoSource
        {
            get { return (string)GetValue(LogoSourceProperty); }
            set { SetValue(LogoSourceProperty, value); }
        }
        #endregion

        #region WordMaxLength
        public static readonly DependencyProperty WordMaxLengthProperty =
            DependencyProperty.Register("WordMaxLength", typeof(string), typeof(CommitView), new PropertyMetadata((string)""));

        public string WordMaxLength
        {
            get { return (string)GetValue(WordMaxLengthProperty); }
            set { SetValue(WordMaxLengthProperty, value); }
        }
        #endregion

        private SdkCmdBase cmdBase;
        private SdkNetEngine netEngine;
        RenrenAPI api = App.RenrenAPI;

        private string m_content = "";
        private string m_picURL = "";
        private EntryType m_type = EntryType.SinaWeibo;

        private int m_maxLenth;
       
        private WriteableBitmap m_writeableBitmap = new WriteableBitmap(200, 200);

        public CommitView()
        {
            DataContext = this;            
            InitializeComponent();
            InitHeaderHeight();
        }

        private void InitHeaderHeight()
        {
            double perc = ((double)140) / ((double)800);
            TitlePanel.Height = Application.Current.RootVisual.RenderSize.Height * perc;
        }     

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IDictionary<string, string> queryString = this.NavigationContext.QueryString;

            try
            {
                if (queryString.ContainsKey("Content"))
                    m_content = queryString["Content"];
                if (queryString.ContainsKey("PicURL"))
                    m_picURL = queryString["PicURL"];
                if (queryString.ContainsKey("Type"))
                    m_type = (EntryType)Enum.Parse(typeof(EntryType), queryString["Type"], true);
                txtInput.Text = m_content;
            }
            catch (System.Exception ex)
            {
                int a = 1;
                a++;
            }

            if (!String.IsNullOrEmpty(m_picURL))
            {
                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(m_picURL, FileMode.Open, FileAccess.Read))
                    {
                        // Decode the JPEG stream.
                        m_writeableBitmap = PictureDecoder.DecodeJpeg(fileStream);
                    }
                }
                this.imgPreview.Source = m_writeableBitmap;
            }          
            ChangeUIByInputType();
        }

        private void ChangeUIByInputType()
        {
            if (m_type == EntryType.SinaWeibo)
            {
                m_maxLenth = 140;
                WordMaxLength = "140";
                WordCount = "140";
                LogoSource = "../../Images/Thumb/weibologo.png";
            }
            else if (m_type == EntryType.Renren)
            {
                m_maxLenth = 280;
                WordMaxLength = "280";
                WordCount = "280";
                LogoSource = "../../Images/Thumb/renren_logo.png";
            }
            else if (m_type == EntryType.Douban)
            {
                m_maxLenth = 140;
                WordMaxLength = "140";
                WordCount = "140";
                LogoSource = "../../Images/Thumb/douban_logo.png";
            }
        }

        private void txtInput_Changed(object sender, TextChangedEventArgs e)
        {
            WordCount = (m_maxLenth - txtInput.Text.Length).ToString();
        }

        private void send_Click(object sender, EventArgs e)
        {

            if (String.IsNullOrEmpty(txtInput.Text))
            {
                MessageBox.Show("据说要智商超过250才能看到您写的字？", ">_<", MessageBoxButton.OK);
                return;
            }
            // 注意，此处要分两种情况 
            // 1.带有图片的发送
            // 2.不带图片的发送
            // 我知道多年以后看到这段注释我一定会嫌自己啰嗦
            // 但是还是要说明，有的api会把带图片的和不带图片的放在两种api路径里，要注意分别处理！
            if (m_type == EntryType.SinaWeibo)
            {               
                SinaWeiboSend();
            }
            else if (m_type == EntryType.Renren)
            {
                RenrenSend();
            }
            else if (m_type == EntryType.Douban)
            {
                DoubanSend();
            }
        }

        private void DoubanSend()
        {
            if (m_writeableBitmap == null)
                return;
            // 豆瓣的发图api和不发图的都是同一个，所以不用理会
            App.DoubanAPI.PostStatusesWithPic(txtInput.Text, m_picURL, (DoubanEventArgs args) =>
            {
                if (args.errorCode == DoubanSdkErrCode.SUCCESS)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show("发送成功");
                    });
                }
                else
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                         MessageBox.Show("发送失败");
                    });
                }
            });
        }

        private void RenrenSend()
        {
            if (api == null || m_writeableBitmap == null)
                return;
            // m_picURL为空说明发送纯文本
            if (String.IsNullOrEmpty(m_picURL))
            {
                List<APIParameter> param = new List<APIParameter>();
                param.Add(new APIParameter("method", "status.set"));
                param.Add(new APIParameter("status", txtInput.Text));
                App.RenrenAPI.RequestAPIInterface(RenrenUphotStatus_DownloadStringCompleted, param);
            }
            else
            {
                api.PublishPhoto(m_picURL, RenrenUphotPhoto_DownloadStringCompleted,
                txtInput.Text);
            }
        }

        private void RenrenUphotStatus_DownloadStringCompleted(object sender, APIRequestCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(e.Error.Message);
                });
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("发送成功");
                });
            }
        }

        private void RenrenUphotPhoto_DownloadStringCompleted(object sender,
           RenrenSDKLibrary.UploadPhotoCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(e.Error.Message);
                });               
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("发送成功");
                });  
            }
        }

        private void SinaWeiboSend()
        {
            txtInput.IsReadOnly = true;
            netEngine = new SdkNetEngine();

            netEngine = new SdkNetEngine();

            // m_picURL为空说明发送纯文本
            if (String.IsNullOrEmpty(m_picURL))
            {
                cmdBase = new cmdUploadMessage
                {
                    status = txtInput.Text,
                    acessToken = App.SinaWeibo_AccessToken
                };
                netEngine.RequestCmd(SdkRequestType.UPLOAD_MESSAGE, cmdBase, weiboRequestCompleted);
            }
            else
            {
                cmdBase = new cmdUploadPic
                {
                    messageText = txtInput.Text,
                    picPath = m_picURL,
                    acessToken = App.SinaWeibo_AccessToken
                };
                netEngine.RequestCmd(SdkRequestType.UPLOAD_MESSAGE_PIC, cmdBase, weiboRequestCompleted);
            }
     
        }

        void weiboRequestCompleted(SdkRequestType requestType, SdkResponse response)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                txtInput.IsReadOnly = false;
                if (response.errCode == SdkErrCode.SUCCESS)
                {
                    MessageBox.Show("发送成功");
                }
                else
                {
                    MessageBox.Show("发送失败");
                }                
            });
        }
    }
}