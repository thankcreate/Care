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
using System.Windows.Navigation;

namespace Care.Views.Common
{
    public partial class CommentView : PhoneApplicationPage
    {
        public ObservableCollection<CommentViewModel> Comments{ get; private set; }

        public ItemViewModel m_itemViewModel;


        #region HeaderHeightProperty
        public static readonly DependencyProperty HeaderHeightProperty =
            DependencyProperty.Register("HeaderHeight", typeof(string), typeof(CommentView), new PropertyMetadata((string)"180"));

        public string HeaderHeight
        {
            get { return (string)GetValue(HeaderHeightProperty); }
            set { SetValue(HeaderHeightProperty, value); }
        }
        #endregion

        public CommentView()
        {
            this.DataContext = this;
            Comments = new ObservableCollection<CommentViewModel>();
            
            InitializeComponent();
            InitHeaderHeight();
            this.Loaded += new RoutedEventHandler(Page_Loaded);
        }

        private void InitHeaderHeight()
        {
            double perc =( (double) 180) / ( (double) 800);
            HeaderHeight = ((int)(Application.Current.RootVisual.RenderSize.Height * perc)).ToString();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (m_itemViewModel == null)
            {
                NavigationService.GoBack();
            }
            RefreshComments();
            base.OnNavigatedTo(e);
        }

        private void RefreshComments()
        {
            EntryType tp = m_itemViewModel.Type;
            if (tp == EntryType.SinaWeibo)
            {
                RefreshCommentsForSinaWeibo();
            }
            else if (tp == EntryType.Renren)
            {
                RefreshCommentsForRenren();
            }
            else if (tp == EntryType.Douban)
            {
                RefreshCommentsForDouban();
            }
        }

        private void RefreshCommentsForDouban()
        {
            String finalID = m_itemViewModel.ID;
            if (m_itemViewModel.ForwardItem != null)
                finalID = m_itemViewModel.ForwardItem.ID;
            if (String.IsNullOrEmpty(finalID))
                return;
            App.DoubanAPI.GetComments(finalID, 100, (GetCommentsEventArgs args) =>
            {
                if (args.errorCode == DoubanSdkErrCode.SUCCESS && args.comments != null)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        Comments.Clear();
                        List<CommentViewModel> sortList = new List<CommentViewModel>();
                        args.comments.ForEach(p =>
                        {
                            CommentViewModel model = DoubanModelConverter.ConvertCommentToCommon(p);
                            if (model != null)
                            {
                                sortList.Add(model);
                            }
                        });
                        var sorted = from m in sortList orderby m.TimeObject descending select m;
                        if (sorted != null)
                        {
                            foreach (CommentViewModel model in sorted)
                            {
                                Comments.Add(model);
                            }
                        }
                        UpdateCommentCount(sortList.Count);
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
            String renrenFeedType = m_itemViewModel.RenrenFeedType;
            String renrenOwnerID = m_itemViewModel.OwnerID;
            String statusID = m_itemViewModel.ID;
            if (renrenFeedType == RenrenNews.FeedTypeStatus)
            {
                List<APIParameter> param = new List<APIParameter>();
                param.Add(new APIParameter("method", "status.getComment"));

                param.Add(new APIParameter("status_id", statusID));
                param.Add(new APIParameter("owner_id", renrenOwnerID));
                param.Add(new APIParameter("order", "1"));
                param.Add(new APIParameter("count", "100"));

                App.RenrenAPI.RequestAPIInterface(RenrenCommentGetCallback, param);
            }
            // 图片
            else if (renrenFeedType == RenrenNews.FeedTypeUploadPhoto)
            {
                List<APIParameter> param = new List<APIParameter>();
                param.Add(new APIParameter("method", "photos.getComments"));

                param.Add(new APIParameter("pid", statusID));
                param.Add(new APIParameter("uid", renrenOwnerID));
                param.Add(new APIParameter("order", "1"));
                param.Add(new APIParameter("count", "100"));

                App.RenrenAPI.RequestAPIInterface(RenrenCommentGetCallback, param);
            }
            else if (renrenFeedType == RenrenNews.FeedTypeSharePhoto)
            {
                List<APIParameter> param = new List<APIParameter>();
                param.Add(new APIParameter("method", "share.getComments"));
                param.Add(new APIParameter("share_id", statusID));
                param.Add(new APIParameter("user_id", renrenOwnerID));
                param.Add(new APIParameter("count", "100"));

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
                        List<CommentViewModel> sortList = new List<CommentViewModel>();
                        foreach (RenrenShareGetCommentsResult.Comment comment in commentsResult.comments)
                        {
                            CommentViewModel model = RenrenModelConverter.ConvertShareCommentToCommon(comment);
                            if (model != null)
                                sortList.Add(model);
                        }
                        var sorted = from m in sortList orderby m.TimeObject descending select m;
                        if (sorted != null)
                        {
                            foreach (CommentViewModel model in sorted)
                            {
                                Comments.Add(model);
                            }
                        }
                        UpdateCommentCount(Comments.Count);
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
                    UpdateCommentCount(Comments.Count);
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
                    Content = "尚无评论",
                    Title = "呃~",
                    Type = EntryType.NotSet                    
                };
                Comments.Add(tipComment);
            }
        }

        private void RefreshCommentsForSinaWeibo()
        {
            if (string.IsNullOrEmpty(m_itemViewModel.ID))
                return;
            SinaWeiboFetcher fetcher = new SinaWeiboFetcher();
            fetcher.LoadSinaWeiboCommentByStatusID(m_itemViewModel.ID, (comments) =>
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
                    UpdateCommentCount(comments.comments.Length);
                    AddEmptyTipCommentCleverly();
                });
            }); 
        }

        // 刷新相应条目的comment值
        private void UpdateCommentCount(int num)
        {
            if (m_itemViewModel != null)
            {
                int nCount = 0;
                try
                {
                    nCount = int.Parse(m_itemViewModel.CommentCount);
                }
                catch (System.Exception ex)
                {
                    nCount = 0;	
                }
                if (num > nCount)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        m_itemViewModel.CommentCount = num.ToString();
                    });
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.Content is PostCommentView)
            {
                PostCommentView postCommentView = e.Content as PostCommentView;
                postCommentView.m_itemViewModel = m_itemViewModel; ;
            }
            //else if (e.Content is StatuesView)
            //{
            //    StatuesView statusView = e.Content as StatuesView;
            //    statusView.m_itemViewModel = m_itemViewModel; ;
            //}
        }

        private void WriteComment_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Common/PostCommentView.xaml", UriKind.Relative));
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            RefreshComments();
        }

        private void CommentToComment_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            FrameworkElement control = sender as FrameworkElement;
            if (control == null)
                return;
            CommentViewModel model = control.DataContext as CommentViewModel;
            if (model == null)
                return;

            String content = null;
            EntryType type = model.Type;
            if (type == EntryType.SinaWeibo)
            {
                content = String.Format("回复@{0}: ", model.Title);
            }
            else if (type == EntryType.Renren)
            {
                content = String.Format("回复{0}: ", model.Title);
            }
            else if (type == EntryType.Douban)
            {
                content = String.Format("@{0}: ", model.DoubanUID);
            }
            else if (type == EntryType.NotSet)
            {
                return;
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("/Views/Common/PostCommentView.xaml?Content={0}",
                content);
            NavigationService.Navigate(new Uri(sb.ToString(), UriKind.Relative));
        }
    }
}