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

namespace DoubanSDK
{
    public delegate void CompleteHandler(DoubanEventArgs e);
    public class DoubanAPI
    {
        public static DoubanAppInfo DoubanInfo = new DoubanAppInfo();

        AuthorityAPI m_authorityAPI;
        ShuoAPI m_shuoAPI;        
        UserAPI m_userAPI;

        // 这里已经将过期这种情况包括在内
        public bool IsAccessTokenValid()
        {
            return (DoubanInfo.tokenInfo.access_token != null && DoubanInfo.tokenInfo.expires_in != null
                && (DateTime.Now.CompareTo(DoubanInfo.tokenInfo.expires_in) < 0));
        }

        // 是否过期
        public bool IsAccessTokenOutOfDate()
        {
            return (DateTime.Now.CompareTo(DoubanInfo.tokenInfo.expires_in) >= 0);
        }

        public void LogOut()
        {
            DoubanInfo.CleanUp();
        }

        public void GetMyUserInfo(GetUserInfoCompletedHandler handler)
        {
            if (m_userAPI == null)
                m_userAPI = new UserAPI();
            m_userAPI.GetMyUserInfo(handler);
        }

        public void GetFollowingUserList(String id, GetFollowingUserListCompleteHandler handler)
        {
            if (m_userAPI == null)
                m_userAPI = new UserAPI();
            m_userAPI.GetFollowingUserList(id, handler);
        }

        public void GetUserTimeLine(String id, int count, GetUserTimeLineCompleteHandler handler)
        {
            if(m_shuoAPI == null)
                m_shuoAPI = new ShuoAPI();
            m_shuoAPI.GetUserTimeLine(id, count, handler);
        }

        public void GetComments(String id, GetCommentsCompleteHandler handler)
        {
            if (m_shuoAPI == null)
                m_shuoAPI = new ShuoAPI();
            m_shuoAPI.GetComments(id, handler);
        }

        public void AddComments(String id, String text, CompleteHandler hanlder)
        {
            if (m_shuoAPI == null)
                m_shuoAPI = new ShuoAPI();
            m_shuoAPI.AddComments(id, text, hanlder);
           
        }

        public void PostStatusesWithPic(String text, String path, CompleteHandler hanlder)
        {
            if (m_shuoAPI == null)
                m_shuoAPI = new ShuoAPI();
            new System.Threading.Thread(() =>
            {
                m_shuoAPI.PostStatusesWithPic(text, path, hanlder);
            }).Start();  
            
        }

        public void RefreshToken(OAuth2LoginBack hander)
        {
            if (m_authorityAPI == null)
                m_authorityAPI = new AuthorityAPI();
            m_authorityAPI.RefreshToken(hander);
        }
    }
}
