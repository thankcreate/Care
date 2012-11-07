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
using System.Runtime.Serialization.Json;
using System.Text;
using System.IO;
using DoubanSDK;
using System.Collections.Generic;

namespace Care.Tool
{
    public class DoubanFetcher : BaseFetcher
    {
        public event LoadCommmentManCompleteHandler m_fetchCommentManListCompleted;

        private TaskHelper m_taskHelper;

        public override void FetchCommentManList(LoadCommmentManCompleteHandler handler)
        {
            m_fetchCommentManListCompleted = handler;
            if (handler == null)
            {
                return;
            }
            if (App.DoubanAPI.IsAccessTokenOutOfDate())
            {
                MessageBox.Show("豆瓣授权已过期，请重新登陆", "温馨提示", MessageBoxButton.OK);
            }

            String doubanFollowID = PreferenceHelper.GetPreference("Douban_FollowerID");
            if (String.IsNullOrEmpty(doubanFollowID))
            {               
                return;
            }
            String strCount = PreferenceHelper.GetPreference("Douban_RecentCount");
            if (string.IsNullOrEmpty(strCount))
            {
                strCount = "40";
            }

            App.DoubanAPI.GetUserTimeLine(doubanFollowID, int.Parse(strCount), (args) =>
            {
                if (args.errorCode == DoubanSdkErrCode.SUCCESS && args.statues != null)
                {
                    FetchCommentsInStatuesList(args.statues);              
                }
                else
                {
                    handler(null);
                }
            });
        }

        private void FetchCommentsInStatuesList(List<Statuses> statuesList)
        {
            List<CommentMan> manList = new List<CommentMan>();
            m_taskHelper = new TaskHelper(
                delegate()
                {
                    m_fetchCommentManListCompleted(manList);
                }, 5000);
            foreach (Statuses statues in statuesList)
            {
                m_taskHelper.PushTask();
                App.DoubanAPI.GetComments(statues.id, (args) =>
                {
                    if (args.errorCode == DoubanSdkErrCode.SUCCESS && args.comments != null)
                    {
                        foreach (DoubanSDK.Comment comment in args.comments)
                        {
                            // 去掉关注对象自己
                            if (comment.user.id != PreferenceHelper.GetPreference("Douban_FollowerID"))
                            {
                                CommentMan man = new CommentMan
                                {
                                    name = comment.user.screen_name,
                                    id = comment.user.id
                                };
                                manList.Add(man);                                
                            }                            
                        }
                        m_taskHelper.PopTask();
                    }
                    else
                    {
                        m_taskHelper.PopTask();
                    }
                });
            }
        }
    }
}
