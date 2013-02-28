using System;
using System.Net;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WeiboSdk;
using System.ServiceModel.Syndication;
using System.Runtime.Serialization.Json;
using Care.Tool;
using Hammock;
using Hammock.Web;

namespace Care.Tool
{
    public class SinaWeiboFetcher : BaseFetcher
    {
        private SdkCmdBase m_cmdBase;
        private SdkNetEngine m_netEngine;
        private TaskHelper m_taskHelper;

        public delegate void CompleteDelegate();
        public delegate void LoadSinaWeiboStatusesCompleteDelegate(WStatuses statuses);
        public delegate void LoadSinaWeiboCommentsCompleteDelegate(Comments comments);
        

        public event LoadCommmentManCompleteHandler m_fetchCommentManListCompleted;

        public SinaWeiboFetcher()
        {           
        }

        public override void FetchCommentManList(LoadCommmentManCompleteHandler callback)
        {
            if (callback == null)
            {               
                return;
            }
            m_fetchCommentManListCompleted = callback;
            LoadSinaWeiboItems(LoadSinaWeiboCommentMen);
        }
        
        private void LoadSinaWeiboCommentMen(WStatuses statuses)
        {
            List<CommentMan> manList = new List<CommentMan>();
            m_taskHelper = new TaskHelper(
                delegate()
                {
                    m_fetchCommentManListCompleted(manList);
                }, 5000);     
            App.Test1 = 0;
            App.Test2 = 0;
            App.Test3 = 0;
            foreach (WStatus status in statuses.statuses)
            {
                m_taskHelper.PushTask();
                List<String> commentIDList = new List<String>();
                
                ++App.Test1;
                System.Diagnostics.Debug.WriteLine("Test1: " + App.Test1);
                LoadSinaWeiboCommentByStatusID(status.id, 
                    delegate(Comments comments)
                    {
                        if (comments != null)
                        {
                            foreach (Comment comment in comments.comments)
                            {
                                // 微博版要区分姓别
                                // 要去掉她自己啊！！！！你个2货
                                if (comment.user.id != status.user.id
                                    && comment.user.id != PreferenceHelper.GetPreference("SinaWeibo_ID")
                                    && comment.user.gender != status.user.gender)
                                {
                                    CommentMan man = new CommentMan
                                    {
                                        name = comment.user.screen_name,
                                        id = comment.user.id
                                    };
                                    manList.Add(man);
                                }                                    
                            }     
                        }                                         
                        m_taskHelper.PopTask();
                    });

            }            
        }
        #region ID方式加载，已被注释
        //private string GernerateIDString(List<String> idList)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    foreach (String id in idList)
        //    {
        //        sb.Append(id + ",");
        //    }
        //    return sb.ToString();
        //}

        //private void LoadSinaWeiboCommentByCommentIDArray(List<String> idList, LoadSinaWeiboCommentsCompleteDelegate dele)
        //{
        //    if (string.IsNullOrEmpty(App.ViewModel.SinaWeiboAccount.id))
        //        return;
        //    // Define a new net engine
        //    m_netEngine = new SdkNetEngine();
        //    // Define a new command base
        //    m_cmdBase = new SdkCmdBase
        //    {
        //        acessToken = App.SinaWeibo_AccessToken,
        //    };
        //    RestRequest request = new RestRequest();
        //    request.Method = WebMethod.Get;
        //    request.Path = "comments/show_batch.json";
        //    request.AddParameter("access_token", App.SinaWeibo_AccessToken);
        //    request.AddParameter("cids", GernerateIDString(idList));

        //    // use my uid to get the friends list.
        //    m_netEngine.SendRequest(request, m_cmdBase, (SdkResponse response) =>
        //    {
        //        ++App.Test2;
        //        System.Diagnostics.Debug.WriteLine("Test2: " + App.Test2);
        //        if (response.errCode == SdkErrCode.SUCCESS)
        //        {
        //            Comments comments;
        //            try
        //            {
        //                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Comments));
        //                comments = ser.ReadObject(response.stream) as Comments;
        //                dele(comments);
        //            }
        //            catch (Exception)
        //            {
        //                dele(null);
        //            }
        //        }
        //        else
        //        {
        //            dele(null);
        //        }
        //    });
        //}
#endregion       

        public void LoadSinaWeiboCommentByStatusID(String id, LoadSinaWeiboCommentsCompleteDelegate dele)
        {
            String MyID = PreferenceHelper.GetPreference("SinaWeibo_ID");
            if (string.IsNullOrEmpty(MyID))
                return;
            // Define a new net engine
            m_netEngine = new SdkNetEngine();
            // Define a new command base
            m_cmdBase = new SdkCmdBase
            {
                acessToken = App.SinaWeibo_AccessToken,
            };
            RestRequest request = new RestRequest();
            request.Method = WebMethod.Get;
            request.Path = "comments/show.json";
            request.AddParameter("access_token", App.SinaWeibo_AccessToken);
            request.AddParameter("id", id);
            //request.CacheOptions.Mode = Hammock.Caching.CacheMode.AbsoluteExpiration;

            
            ++App.Test3;
            System.Diagnostics.Debug.WriteLine("Test3: " + App.Test3);
            m_netEngine.SendRequest(request, m_cmdBase, (SdkResponse response) =>
            {
                ++App.Test2;
                System.Diagnostics.Debug.WriteLine("Test2: " + App.Test2);
                if (response.errCode == SdkErrCode.SUCCESS)
                {
                    Comments comments;
                    try
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Comments));
                        comments = ser.ReadObject(response.stream) as Comments;
                        dele(comments);
                    }
                    catch (Exception)
                    {
                        dele(null);
                    }
                }
                else
                {
                    dele(null);
                }
            });
        }

        private bool LoadSinaWeiboItems(LoadSinaWeiboStatusesCompleteDelegate complete)
        {
            String followerID = PreferenceHelper.GetPreference("SinaWeibo_FollowerID");
            if (String.IsNullOrEmpty(followerID))
            {
                return false;
            }
            // Define a new net engine
            m_netEngine = new SdkNetEngine();

            // Define a new command base
            m_cmdBase = new cdmUserTimeline
            {
                acessToken = App.SinaWeibo_AccessToken,
                userId = followerID,
                count = "40"
            };
            // Request server, the last parameter is set as default (".xml")
            m_netEngine.RequestCmd(SdkRequestType.USER_TIMELINE, m_cmdBase,
                // Requeset callback
                delegate(SdkRequestType requestType, SdkResponse response)
                {
                    if (response.errCode == SdkErrCode.SUCCESS)
                    {
                        WStatuses statuses = null;
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(WStatuses));
                        statuses = ser.ReadObject(response.stream) as WStatuses;
                        complete(statuses);
                    }
                    else
                    {
                        return;
                    }
                });
            return true;
        }
    }
}
