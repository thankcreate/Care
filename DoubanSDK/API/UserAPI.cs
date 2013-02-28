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
        private int FETCH_COUNT = 3;
        List<FollowingUserInfo> m_friendList = new List<FollowingUserInfo>();

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

        // 豆瓣的好友API虽然没说start和count的事，但是其实他们是启作用的
        public void GetFollowingUserList(string id, GetFollowingUserListCompleteHandler handler)
        {
            m_friendList.Clear();
            GetFollowingUserListInternal(id, 0, FETCH_COUNT, handler);
        }

        public void GetFollowingUserListInternal(string id, int start, int count, GetFollowingUserListCompleteHandler handler)
        {
            String url = String.Format("https://api.douban.com/shuo/v2/users/{0}/following?start={1}&count={2}", id, start, count);
            WebClient client = new WebClient();

            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler((e1, e2) =>
            {
                try
                {
                    String result = e2.Result;
                    GetFollowingUserListEventArgs args = new GetFollowingUserListEventArgs();
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<FollowingUserInfo>));
                    List<FollowingUserInfo> list = ser.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(result))) as List<FollowingUserInfo>;
                    if (list != null && list.Count != 0)
                    {
                        foreach (FollowingUserInfo info in list)
                        {
                            if (info.screen_name != "[已注销]")
                            {
                                m_friendList.Add(info);
                            }
                        }
                        GetFollowingUserListInternal(id, start + FETCH_COUNT, FETCH_COUNT, handler);
                    }
                    else
                    {
                        args.userList = m_friendList;
                        args.errorCode = DoubanSdkErrCode.SUCCESS;
                        handler(args);
                    }
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
        }

    }
}
