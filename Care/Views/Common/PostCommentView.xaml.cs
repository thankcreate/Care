using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using WeiboSdk;
using Hammock.Web;
using Hammock;
using RenrenSDKLibrary;
using System.Runtime.Serialization.Json;
using DoubanSDK;

namespace Care.Views.Common
{
    public partial class PostCommentView : PhoneApplicationPage
    {

        #region LogoSourceProperty
        public static readonly DependencyProperty LogoSourceProperty =
            DependencyProperty.Register("LogoSource", typeof(string), typeof(PostCommentView), new PropertyMetadata((string)""));

        public string LogoSource
        {
            get { return (string)GetValue(LogoSourceProperty); }
            set { SetValue(LogoSourceProperty, value); }
        }
        #endregion

        #region WordCountProperty
        public static readonly DependencyProperty WordCountProperty =
            DependencyProperty.Register("WordCount", typeof(string), typeof(PostCommentView), new PropertyMetadata((string)""));

        public string WordCount
        {
            get { return (string)GetValue(WordCountProperty); }
            set { SetValue(WordCountProperty, value); }
        }
        #endregion

        #region WordMaxLength
        public static readonly DependencyProperty WordMaxLengthProperty =
            DependencyProperty.Register("WordMaxLength", typeof(string), typeof(PostCommentView), new PropertyMetadata((string)""));

        public string WordMaxLength
        {
            get { return (string)GetValue(WordMaxLengthProperty); }
            set { SetValue(WordMaxLengthProperty, value); }
        }
        #endregion

        private EntryType m_type = EntryType.SinaWeibo;
        private string m_id;
        private string m_renrenFeedType;
        private string m_renrenOwnerID;
        private int m_maxLenth;

        private SdkCmdBase cmdBase;
        private SdkNetEngine netEngine;        

        public PostCommentView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IDictionary<string, string> queryString = this.NavigationContext.QueryString;

            if (queryString.ContainsKey("ID")
                && queryString.ContainsKey("Type"))
            {
                m_id = queryString["ID"];
                m_type = (EntryType)Enum.Parse(typeof(EntryType), queryString["Type"], true);
            }
            m_renrenFeedType = queryString["RenrenFeedType"];
            m_renrenOwnerID = queryString["RenrenOwnerID"];
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

        private void SinaWeiboSend()
        {
            if (String.IsNullOrEmpty(m_id))
                return;

            String commentText = StatusMessageBox.Text;

            netEngine = new SdkNetEngine();
            // Define a new command base
            cmdBase = new SdkCmdBase
            {
                acessToken = App.SinaWeibo_AccessToken,
            };

            RestRequest request = new RestRequest();
            request.Method = WebMethod.Post;
            request.Path = "comments/create.json";
            request.AddParameter("access_token", App.SinaWeibo_AccessToken);
            request.AddParameter("id", m_id);
            request.AddParameter("comment", commentText);
            request.AddParameter("comment_ori", "0");

            netEngine.SendRequest(request, cmdBase, (SdkResponse response) =>
            {
                if (response.errCode == SdkErrCode.SUCCESS)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show("发送成功！");
                        // go back to CommentView.
                        // CommentView should refresh itself to show the newly added comment
                        NavigationService.GoBack();
                    }
                    );
                }
                else
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show("发送失败！");
                        // go back to CommentView.
                        // CommentView should refresh itself to show the newly added comment
                        NavigationService.GoBack();
                    });
                }
            });
        }

        private void RenrenSend()
        {
            // 对普通状态的评论
            if (m_renrenFeedType == RenrenNews.FeedTypeStatus)
            {
                List<APIParameter> param = new List<APIParameter>();
                param.Add(new APIParameter("method", "status.addComment"));
                param.Add(new APIParameter("status_id", m_id));
                param.Add(new APIParameter("owner_id", m_renrenOwnerID));
                param.Add(new APIParameter("content", StatusMessageBox.Text));

                App.RenrenAPI.RequestAPIInterface(RenrenAddCommentGetCallback, param);
            }
            // 对原创上传照片的评论
            else if (m_renrenFeedType == RenrenNews.FeedTypeUploadPhoto)
            {
                List<APIParameter> param = new List<APIParameter>();
                param.Add(new APIParameter("method", "photos.addComment"));
                param.Add(new APIParameter("pid", m_id));
                // 靠靠靠，有的地方是owner_id，有的地方是uid，谨记谨记~
                param.Add(new APIParameter("uid", m_renrenOwnerID));
                param.Add(new APIParameter("content", StatusMessageBox.Text));

                App.RenrenAPI.RequestAPIInterface(RenrenAddCommentGetCallback, param);
            }
            // 对照片分享的评论
            else if (m_renrenFeedType == RenrenNews.FeedTypeSharePhoto)
            {
                List<APIParameter> param = new List<APIParameter>();
                param.Add(new APIParameter("method", "share.addComment"));
                param.Add(new APIParameter("share_id", m_id));
                // （呃~这里又变成user_id了么？好吧，一旦接受了这种设定，仿佛也挺有意思的呢..）   <=====  作废
                //  虽然api文档里写的是user_id，但是其实应该写uid
                param.Add(new APIParameter("user_id", m_renrenOwnerID));
                param.Add(new APIParameter("content", StatusMessageBox.Text));

                App.RenrenAPI.RequestAPIInterface(RenrenAddCommentGetCallback, param);
            }
        }

        private void RenrenAddCommentGetCallback(object sender, APIRequestCompletedEventArgs e)
        {
            // Success
            if (e.Error == null)
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RenrenAddCommentResult));                
                RenrenAddCommentResult addCommentsResult = serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(e.ResultJsonString))) as RenrenAddCommentResult;
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (addCommentsResult.result == RenrenAddCommentResult.SUCCESS)
                    {
                        MessageBox.Show("发送成功！");
                        // go back to CommentView.
                        // CommentView should refresh itself to show the newly added comment
                        NavigationService.GoBack();
                    }
                });
            }
            // Fail
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("发送失败！");
                    // go back to CommentView.
                    // CommentView should refresh itself to show the newly added comment
                    NavigationService.GoBack();
                });
            }
        }

        private void DoubanSend()
        {
             if (String.IsNullOrEmpty(m_id))
                return;

            String commentText = StatusMessageBox.Text;
            if(String.IsNullOrEmpty(commentText))
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("内容为空！");                    
                });
                return;
            }

            App.DoubanAPI.AddComments(m_id, commentText, (DoubanEventArgs args) =>
            {
                if (args.errorCode == DoubanSdkErrCode.SUCCESS)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        if (args.errorCode == DoubanSdkErrCode.SUCCESS)
                        {
                            MessageBox.Show("发送成功！");
                            // go back to CommentView.
                            // CommentView should refresh itself to show the newly added comment
                            NavigationService.GoBack();
                        }
                    });
                }
                else
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show("发送失败！");
                        // go back to CommentView.
                        // CommentView should refresh itself to show the newly added comment
                        NavigationService.GoBack();
                    });
                }
            });
        }

        private void send_Click(object sender, EventArgs e)
        {
            String commentText = StatusMessageBox.Text;
            if (String.IsNullOrEmpty(commentText))
            {
                MessageBox.Show("内容为空！");
                return;
            }
            else if (commentText.Length > m_maxLenth)
            {
                MessageBox.Show("内容超出最大长度！");
                return;
            }

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

        private void StatusMessageBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            WordCount = (m_maxLenth - StatusMessageBox.Text.Length).ToString();
        }
    }
}