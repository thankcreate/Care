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
using Hammock.Authentication.OAuth;
using Hammock.Web;
using Hammock;
using Hammock.Authentication;
using Hammock.Silverlight.Compat;
using System.Text;
using System.Runtime.Serialization.Json;

namespace DoubanSDK
{
    public class AuthorityAPI
    {
        DoubanNetEngine m_netEngine;
        private DoubanSdkAuthError m_Error = new DoubanSdkAuthError { errCode = DoubanSdkErrCode.SUCCESS };

        public void RefreshToken(OAuth2LoginBack OAuth2VerifyCompleted)
        {
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
            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("redirect_uri", DoubanSdkData.RedirectUri);
            request.AddParameter("refresh_token", DoubanAPI.DoubanInfo.tokenInfo.refresh_token);

            client.BeginRequest(request, (e1, e2, e3) =>
            {
                if (null != e2.InnerException)
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
