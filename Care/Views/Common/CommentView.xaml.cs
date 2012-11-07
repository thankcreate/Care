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
using System.Collections.ObjectModel;
using Care.Tool;
using System.ComponentModel;
using System.Text;
using RenrenSDKLibrary;
using DoubanSDK;
using System.Runtime.Serialization.Json;

namespace Care.Views.Common
{
    public partial class CommentView : PhoneApplicationPage
    {
        public ObservableCollection<CommentViewModel> Comments{ get; private set; }
        private string m_id = "";
        private string m_renrenFeedType;
        private string m_renrenOwnerID;
        private EntryType m_type = EntryType.SinaWeibo;

        public CommentView()
        {
            this.DataContext = this;
            Comments = new ObservableCollection<CommentViewModel>();
            
            InitializeComponent();            
            this.Loaded += new RoutedEventHandler(Page_Loaded);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
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
            RefreshComments();
            base.OnNavigatedTo(e);
        }

        private void RefreshComments()
        {
            if (m_type == EntryType.SinaWeibo)
            {
                RefreshCommentsForSinaWeibo();
            }
            else if (m_type == EntryType.Renren)
            {
                RefreshCommentsForRenren();
            }
            else if (m_type == EntryType.Douban)
            {
                RefreshCommentsForDouban();
            }
        }

        private void RefreshCommentsForDouban()
        {
            if (String.IsNullOrEmpty(m_id))
                return;
            App.DoubanAPI.GetComments(m_id, (GetCommentsEventArgs args) =>
            {
                if (args.errorCode == DoubanSdkErrCode.SUCCESS && args.comments != null)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        Comments.Clear();
                        args.comments.ForEach(p =>
                        {
                            CommentViewModel model = DoubanModelConverter.ConvertCommentToCommon(p);
                            if (model != null)
                            {
                                Comments.Add(model);
                            }
                        });
                        AddEmptyTipCommentCleverly();
                    });
                }
                else
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        if (!String.IsNullOrEmpty(PreferenceHelper.GetPreference("Renren_ID")))
                        {
                            MessageBox.Show("豆瓣评论获取失败，可能是网络问题，也可能是帐号过期", "失败", MessageBoxButton.OK);
                        }
                    });
                }
                
            });

        }

        private void RefreshCommentsForRenren()
        {
            // 普通文字状态
            if (m_renrenFeedType == RenrenNews.FeedTypeStatus)
            {
                List<APIParameter> param = new List<APIParameter>();
                param.Add(new APIParameter("method", "status.getComment"));

                param.Add(new APIParameter("status_id", m_id));
                param.Add(new APIParameter("owner_id", m_renrenOwnerID));                

                App.RenrenAPI.RequestAPIInterface(RenrenCommentGetCallback, param);
            }
            // 图片
            else if (m_renrenFeedType == RenrenNews.FeedTypeUploadPhoto)
            {
                List<APIParameter> param = new List<APIParameter>();
                param.Add(new APIParameter("method", "photos.getComments"));

                param.Add(new APIParameter("pid", m_id));
                param.Add(new APIParameter("uid", m_renrenOwnerID));

                App.RenrenAPI.RequestAPIInterface(RenrenCommentGetCallback, param);
            }
            else if (m_renrenFeedType == RenrenNews.FeedTypeSharePhoto)
            {
                List<APIParameter> param = new List<APIParameter>();
                param.Add(new APIParameter("method", "share.getComments"));

                param.Add(new APIParameter("share_id", m_id));
                param.Add(new APIParameter("user_id", m_renrenOwnerID));

                App.RenrenAPI.RequestAPIInterface(RenrenShareCommentGetCallback, param);
            }
          
        }

        private void RenrenShareCommentGetCallback(object sender, APIRequestCompletedEventArgs e)
        {
            // Success
            if (e.Error == null)
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RenrenShareGetCommentsResult));                
                RenrenShareGetCommentsResult commentsResult = serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(e.ResultJsonString))) as RenrenShareGetCommentsResult;
                if (commentsResult != null && commentsResult.comments != null)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        Comments.Clear();
                        foreach (RenrenShareGetCommentsResult.Comment comment in commentsResult.comments)
                        {
                            CommentViewModel model = comment.ToCommentViewModel();
                            if (model != null)
                                Comments.Add(model);
                        }
                        AddEmptyTipCommentCleverly();
                    });
                }
            }
            // Fail
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (!String.IsNullOrEmpty(PreferenceHelper.GetPreference("Renren_ID")))
                    {
                        MessageBox.Show("人人评论获取失败，可能是网络问题，也可能是帐号过期", "失败", MessageBoxButton.OK);
                    }
                });
            }
        }

        private void RenrenCommentGetCallback(object sender, APIRequestCompletedEventArgs e)
        {
            // Success
            if (e.Error == null)
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<RenrenNews.Comments.Comment>));                
                List<RenrenNews.Comments.Comment> commentsResult = serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(e.ResultJsonString))) as List<RenrenNews.Comments.Comment>;
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    Comments.Clear();
                    commentsResult.ForEach(p =>
                    {
                        CommentViewModel model = RenrenModelConverter.ConvertCommentToCommon(p);
                        if (model != null)
                        {
                            Comments.Add(model);
                        }
                    });
                    AddEmptyTipCommentCleverly();
                });
            }
            // Fail
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (!String.IsNullOrEmpty(PreferenceHelper.GetPreference("Renren_ID")))
                    {
                        MessageBox.Show("人人评论获取失败，可能是网络问题，也可能是帐号过期", "失败", MessageBoxButton.OK);
                    }                   
                });
            }
        }

        private void AddEmptyTipCommentCleverly()
        {
            if (Comments.Count == 0)
            {
                CommentViewModel tipComment = new CommentViewModel()
                {
                    Content = "尚无评论"
                };
                Comments.Add(tipComment);
            }
        }

        private void RefreshCommentsForSinaWeibo()
        {
            if (string.IsNullOrEmpty(m_id))
                return;
            SinaWeiboFetcher fetcher = new SinaWeiboFetcher();
            fetcher.LoadSinaWeiboCommentByStatusID(m_id, (comments) =>
            {
                if (comments == null)
                {
                    return;
                }
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    Comments.Clear();
                    foreach (Comment comment in comments.comments)
                    {
                        CommentViewModel model = SinaWeiboModelConverter.ConvertCommentToCommon(comment);
                        if (model != null)
                        {
                            Comments.Add(model);
                        }                        
                    }
                    AddEmptyTipCommentCleverly();
                });
            }); 
        }

        private void WriteComment_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("/Views/Common/PostCommentView.xaml?ID={0}&Type={1}&RenrenFeedType={2}&RenrenOwnerID={3}", 
                m_id, 
                m_type,
                m_renrenFeedType,
                m_renrenOwnerID);
            NavigationService.Navigate(new Uri(sb.ToString(), UriKind.Relative));
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            RefreshComments();
        }
    }
}