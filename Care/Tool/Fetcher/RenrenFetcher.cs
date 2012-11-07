using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using RenrenSDKLibrary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.IO;


namespace Care.Tool
{    
    public class RenrenFetcher : BaseFetcher
    {        
        public delegate void LoadRenrenStatusesCompleteHandler(List<RenrenNews> manNameList);


        public override void FetchCommentManList(LoadCommmentManCompleteHandler handler)
        {
            if (handler == null)
            {
                return;
            }
            LoadRenrenNews((List<RenrenNews> listNews) =>
            {
                List<CommentMan> resultList = new List<CommentMan>();
                if (listNews != null)
                {                    
                    foreach (RenrenNews news in listNews)
                    {
                        if (news.comments != null && news.comments.comment != null)
                        {
                            foreach (RenrenNews.Comments.Comment comment in news.comments.comment)
                            {
                                // 要去掉自己
                                if (comment.uid != PreferenceHelper.GetPreference("Renren_FollowerID"))
                                {
                                    CommentMan man = new CommentMan
                                    {
                                        name = comment.name,
                                        id = comment.uid
                                    };
                                    resultList.Add(man);
                                }
                            }
                        }
                    }
                    handler(resultList);
                }
                else
                {
                    handler(resultList);
                }
            });            
        }

        private void LoadRenrenNews(LoadRenrenStatusesCompleteHandler handler)
        {
            String renrenFollowID = PreferenceHelper.GetPreference("Renren_FollowerID");
            if (String.IsNullOrEmpty(renrenFollowID))
            {
                return;
            }

            if (!App.RenrenAPI.IsAccessTokenValid())
            {
                // 有值说明之前登陆过，须提示过期
                if (!String.IsNullOrEmpty(PreferenceHelper.GetPreference("Renren_ID")))
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show("人人帐号授权已过期，请重新登陆", "温馨提示", MessageBoxButton.OK);
                    });                    
                }
                return;
            }

            List<APIParameter> param = new List<APIParameter>();
            param.Add(new APIParameter("method", "feed.get"));
            // 当前只获取     
            // 10:更新状态
            // 20:发表日志,先不做了，太麻烦 
            // 30:上传照片
            // 32:分享照片
            // 的新鲜事，以逗号分隔
            param.Add(new APIParameter("type", "10,30,32"));
            param.Add(new APIParameter("uid", renrenFollowID));            
            String strCount = PreferenceHelper.GetPreference("Renren_RecentCount");
            if (string.IsNullOrEmpty(strCount))
            {
                strCount = "40";
            }
            param.Add(new APIParameter("count", strCount));
            App.RenrenAPI.RequestAPIInterface((sender, e) =>
            {
                // Success
                if (e.Error == null)
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<RenrenNews>));
                    List<RenrenNews> searchResult = serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(e.ResultJsonString))) as List<RenrenNews>;
                    handler(searchResult);
                }
                // Fail
                else
                {
                    handler(null);
                }
            }, param);            
        }



    }
}
