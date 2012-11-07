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
using System.Xml.Serialization;

using System.Runtime.Serialization;
using System.IO;


namespace DoubanSDK
{
    public enum DoubanSdkErrCode
    {
        //参数错误
        XPARAM_ERR = -1,
        //成功
        SUCCESS = 0,
        //网络不可用
        NET_UNUSUAL,
        //服务器返回异常
        SERVER_ERR,
        //访问超时
        TIMEOUT,
        //用户请求被取消
        USER_CANCEL
        //TOKEN过期
    }


    public class DoubanSdkAuthError
    {
        public DoubanSdkErrCode errCode;
        public string specificCode = "";
        public string errMessage = "";
    }

    public class DoubanSdkAuthRes
    {
        public string userId = "";
        public string acessToken = "";
        public string acessTokenSecret = "";

        //refleshToken
    }

    [DataContract]
    public class DoubanSdkAuth2Res
    {
        [DataMember(Name = "access_token")]
        public string accesssToken { get; set; }

        [DataMember(Name = "refresh_token")]
        public string refleshToken { get; set; }

        [DataMember(Name = "expires_in")]
        public string expriesIn { get; set; }
    }

    [DataContract]
    public class DoubanOAuthErrRes
    {
        [DataMember(Name = "error")]
        public string Error { get; set; }

        [DataMember(Name = "error_code")]
        public string ErrorCode { get; set; }

        [DataMember(Name = "error_description")]
        public string errDes { get; set; }
    }

    public class DoubanSdkResponse
    {
        public DoubanSdkErrCode errCode;
        public string specificCode;

        //public string requestID = "";
        public string content = "";
        public Stream stream = null;
    }

    public class DoubanErrorRes
    {
        [XmlElement("request")]
        [DataMember(Name = "request")]
        public string request { get; set; }

        [XmlElement("code")]
        [DataMember(Name = "code")]
        public string code { get; set; }

        [XmlElement("msg")]
        [DataMember(Name = "msg")]
        public string msg { get; set; }
    }
    
    public delegate void OAuth2LoginBack(bool isSucess, DoubanSdkAuthError err, DoubanSdkAuth2Res response);    
}
