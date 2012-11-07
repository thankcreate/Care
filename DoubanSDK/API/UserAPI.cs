using System;
using System.Net;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Hammock.Web;
using Hammock;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;

namespace DoubanSDK
{
    public delegate void GetUserInfoCompletedHandler(GetUserInfoEventArgs e);
    public delegate void GetFollowingUserListCompleteHandler(GetFollowingUserListEventArgs e);

    public class UserAPI
    {
        DoubanNetEngine m_netEngine;

        public void GetMyUserInfo(GetUserInfoCompletedHandler handler)
        {
            if (m_netEngine == null)
                m_netEngine = new DoubanNetEngine();
            
            RestRequest request = new RestRequest();
            request.Method = WebMethod.Get;
            request.Path = "v2/user/~me";

            m_netEngine.SendRequest(request, (DoubanSdkResponse response) =>
            {
                if (response.errCode == DoubanSdkErrCode.SUCCESS)
                {
                    GetUserInfoEventArgs args = new GetUserInfoEventArgs();
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(UserInfo));
                    UserInfo userinfo = ser.ReadObject(response.stream) as UserInfo;
                    args.userInfo = userinfo;
                    args.errorCode = response.errCode;
                    args.specificCode = response.specificCode;
                    handler(args);
                }
                else
                {
                    GetUserInfoEventArgs args = new GetUserInfoEventArgs();
                    args.userInfo = null;
                    args.errorCode = response.errCode;
                    args.specificCode = response.specificCode;
                    handler(args);
                }
            });   
        }

        public void GetFollowingUserList(string id, GetFollowingUserListCompleteHandler handler)
        {
            String url = String.Format("https://api.douban.com/shuo/v2/users/{0}/following",id);
            WebClient client = new WebClient();

            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler((e1, e2) =>
            {
                try
                {
                    String result = e2.Result;
                    GetFollowingUserListEventArgs args = new GetFollowingUserListEventArgs();
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<FollowingUserInfo>));
                    List<FollowingUserInfo> list = ser.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(result))) as List<FollowingUserInfo>;
                    args.userList = list;
                    args.errorCode = DoubanSdkErrCode.SUCCESS;                    
                    handler(args);
                }
                catch (System.Exception ex)
                {
                    GetFollowingUserListEventArgs args = new GetFollowingUserListEventArgs();
                    args.userList = null;
                    args.errorCode = DoubanSdkErrCode.SERVER_ERR;                    
                    handler(args);
                }                
            });
            client.DownloadStringAsync(new Uri(url));

            //if (m_netEngine == null)
            //    m_netEngine = new DoubanNetEngine();

            //RestRequest request = new RestRequest();
            //request.Method = WebMethod.Get;
            ////request.Path = String.Format("shuo/v2/users/{0}/followers", id);
            ////request.Path = String.Format("shuo/v2/statuses/home_timeline");
            //request.Path = String.Format("shuo/v2/users/{0}/followers", id);

            //m_netEngine.SendRequest(request, (DoubanSdkResponse response) =>
            //{
            //    if (response.errCode == DoubanSdkErrCode.SUCCESS)
            //    {
            //        GetFollowingUserListEventArgs args = new GetFollowingUserListEventArgs();
            //        //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(UserInfo));
            //        //UserInfo userinfo = ser.ReadObject(response.stream) as UserInfo;
            //        //args.userInfo = userinfo;
            //        //args.errorCode = response.errCode;
            //        handler(args);
            //    }
            //    else
            //    {
            //        //GetUserInfoEventArgs args = new GetUserInfoEventArgs();
            //        //args.userInfo = null;
            //        //args.errorCode = response.errCode;
            //        //handler(args);
            //    }
            //});
        }

    }
}
