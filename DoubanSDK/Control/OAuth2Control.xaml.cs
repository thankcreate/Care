using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Hammock.Authentication.OAuth;
using Hammock.Web;
using Hammock;
using Hammock.Authentication;
using Hammock.Silverlight.Compat;
// from System.Servicemodel.Web
using System.Runtime.Serialization.Json;

namespace DoubanSDK
{

    public partial class OAuth2Control : UserControl
    {
        private bool m_bIsCompleted = false;

        private DoubanSdkAuthError m_Error = new DoubanSdkAuthError { errCode = DoubanSdkErrCode.SUCCESS };

        public EventHandler OBrowserCancelled { get; set; }
        public OAuth2LoginBack OAuth2VerifyCompleted { get; set; }

        public OAuth2Control()
        {
            InitializeComponent();
            string accredit = string.Format("{0}?client_id={1}&response_type=code&redirect_uri={2}&scope={3}&display=mobile"
                    , ConstDefine.OAuth2_0Url, DoubanSdkData.AppKey, DoubanSdkData.RedirectUri, DoubanSdkData.Scope);
            Dispatcher.BeginInvoke(() =>
            {
                OAuthBrowser.Navigate(new Uri(accredit));
            });
        }

        private void BrowserNavigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            // 嗯嗯，什么事儿都不做
            // 大部分事儿都在BrowserNavigating里了
        }

        private void BrowserNavigating(object sender, NavigatingEventArgs e)
        {
            if (m_bIsCompleted)
                return;

            if (e.Uri.AbsoluteUri.Contains("error=access_denied"))
            {
                if (null != OBrowserCancelled)
                    OBrowserCancelled.Invoke(sender, e);
                m_bIsCompleted = true;
                return;
            }

            if (!e.Uri.AbsoluteUri.Contains("code="))
            {
                return;
            }

            // 下面是拿code换token的部分喽

            m_bIsCompleted = true;
            var arguments = e.Uri.AbsoluteUri.Split('?');
            if (0 == arguments.Length)
            {
                m_Error.errCode = DoubanSdkErrCode.SERVER_ERR;
                
                if (null != OAuth2VerifyCompleted)
                    OAuth2VerifyCompleted(false, m_Error, null);
                
                return;
            }
            
            GetOAuth2AccessToken(arguments[1]);
                
        }

        private void GetOAuth2AccessToken(string uri)
        {
            String requestVerifier = "";
            foreach (string item in uri.Split('&'))
            {
                string[] parts = item.Split('=');
                if (parts[0] == "code")
                {
                    requestVerifier = parts[1];
                    break;
                }
            }
            
            if (string.IsNullOrEmpty(requestVerifier))
            {
                m_Error.errCode = DoubanSdkErrCode.NET_UNUSUAL;
                if (null != OAuth2VerifyCompleted)
                    OAuth2VerifyCompleted(false, m_Error, null);
                return;
            }

            RestClient client = new RestClient();
            client.Authority = ConstDefine.ServerUrl2_0;
            client.HasElevatedPermissions = true;

            RestRequest request = new RestRequest();
            request.Path = "/auth2/token";
            request.Method = WebMethod.Post;

            request.DecompressionMethods = DecompressionMethods.GZip;
            request.Encoding = Encoding.UTF8;

            request.AddParameter("client_id", DoubanSdkData.AppKey);
            request.AddParameter("client_secret", DoubanSdkData.AppSecret);
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("redirect_uri", DoubanSdkData.RedirectUri);
            request.AddParameter("code", requestVerifier);


            client.BeginRequest(request, (e1, e2, e3) =>
            {                
                if ( null != e2.InnerException)
                {
                    m_Error.errCode = DoubanSdkErrCode.NET_UNUSUAL;
                    if (null != OAuth2VerifyCompleted)
                        OAuth2VerifyCompleted(false, m_Error, null);
                }
                else if (e2.StatusCode != HttpStatusCode.OK)
                {
                    if (null == e2.ContentStream || e2.ContentStream.Length == 0)
                    {
                        m_Error.errCode = DoubanSdkErrCode.NET_UNUSUAL;
                        if (null != OAuth2VerifyCompleted)
                            OAuth2VerifyCompleted(false, m_Error, null);
                        return;
                    }
                    DataContractJsonSerializer ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(DoubanOAuthErrRes));
                    DoubanOAuthErrRes errRes = ser.ReadObject(e2.ContentStream) as DoubanOAuthErrRes;

                    m_Error.errCode = DoubanSdkErrCode.SERVER_ERR;
                    m_Error.specificCode = errRes.ErrorCode;
                    m_Error.errMessage = errRes.errDes;
                    if (null != OAuth2VerifyCompleted)
                        OAuth2VerifyCompleted(true, m_Error, null);
                }
                else
                {
                    m_Error.errCode = DoubanSdkErrCode.SUCCESS;
                    //isCompleted = true;
                    //解析
                    DataContractJsonSerializer ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(DoubanSdkAuth2Res));
                    var oauthRes = ser.ReadObject(e2.ContentStream) as DoubanSdkAuth2Res;

                    // 存到本地
                    DoubanTokenInfo tokenInfo = new DoubanTokenInfo();
                    tokenInfo.access_token = oauthRes.accesssToken;
                    tokenInfo.refresh_token = oauthRes.refleshToken;
                    tokenInfo.expires_in = DateTime.Now.AddSeconds(Int32.Parse(oauthRes.expriesIn));
                    DoubanAPI.DoubanInfo.SetTokenInfo(tokenInfo);

                    if (null != OAuth2VerifyCompleted)
                        OAuth2VerifyCompleted(true, m_Error, oauthRes);
                }
            });


        }
    }
}
