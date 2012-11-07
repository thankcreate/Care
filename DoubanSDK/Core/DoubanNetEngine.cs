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
using Hammock.Silverlight.Compat;
using Hammock.Authentication.OAuth;
using Hammock.Authentication.Basic;
using Hammock.Authentication;
using Hammock.Web;
using Hammock;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DoubanSDK
{
    public delegate void RequestBack(DoubanSdkResponse response);

    public class DoubanNetEngine
    {
        private RestClient m_Client = new RestClient();

        public void SendRequest(RestRequest request, RequestBack callBack)
        {
            Action<string> errAction = (e1) =>
            {
                if (null != callBack)
                {
                    DoubanSdkErrCode sdkErr = DoubanSdkErrCode.XPARAM_ERR;
                    callBack(new DoubanSdkResponse
                    {
                        //requestID = null != data ? data.requestId : "",
                        errCode = sdkErr,
                        specificCode = "",
                        content = e1,
                        stream = null
                    });
                }
            };

            if (null == request)
            {
                errAction("request should`t be null.");
                return;
            }
            
            m_Client.Authority = ConstDefine.PublicApiUrl;
            m_Client.HasElevatedPermissions = true;

            //添加鉴权
            request.DecompressionMethods = DecompressionMethods.GZip;
            request.Encoding = Encoding.UTF8;            

            IWebCredentials credentials = null;

            if (null != DoubanAPI.DoubanInfo.tokenInfo && !string.IsNullOrEmpty(DoubanAPI.DoubanInfo.tokenInfo.access_token))
            {
                request.AddHeader("Authorization", string.Format("Bearer {0}", DoubanAPI.DoubanInfo.tokenInfo.access_token));
            }
            else
            {
                request.AddParameter("source", DoubanSdkData.AppKey);
            }
            request.AddParameter("curtime", DateTime.Now.ToString());
            request.Credentials = credentials;

            m_Client.BeginRequest(request, (e1, e2, e3) => AsyncCallback(e1, e2, callBack));

        }


        private void AsyncCallback(RestRequest request, RestResponse response, RequestBack callBack)
        {
            DoubanSdkResponse sdkRes = new DoubanSdkResponse();
            try
            {
                if (true == response.TimedOut)
                {
                    sdkRes.errCode = DoubanSdkErrCode.TIMEOUT;
                    sdkRes.content = "连接超时";

                }

                //网络异常(WebException)
                else if (null != response.InnerException || HttpStatusCode.OK != response.StatusCode)
                {
                    bool isUserCanceled = false;
                    if (response.InnerException is WebException)
                    {
                        WebException ex = response.InnerException as WebException;
                        if (WebExceptionStatus.RequestCanceled == ex.Status)
                        {
                            sdkRes.errCode = DoubanSdkErrCode.USER_CANCEL;
                            sdkRes.content = "Web Request is cancled.";
                            isUserCanceled = true;

                        }
                    }

                    if (!isUserCanceled)
                    {
                        try
                        {
                            DoubanErrorRes resObject = null;
                            //if (state.dataType == DataType.XML)
                            if (request.Path.Contains(".xml") || request.Path.Contains(".XML"))
                            {
                                XElement xmlSina = XElement.Parse(response.Content);
                                if (null != xmlSina.Element("error_code"))
                                {
                                    //得到服务器标准错误信息
                                    XmlSerializer serializer = new XmlSerializer(typeof(DoubanErrorRes));
                                    resObject = serializer.Deserialize(response.ContentStream) as DoubanErrorRes;
                                }
                            }
                            else
                            {
                                DataContractJsonSerializer ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(DoubanErrorRes));
                                resObject = ser.ReadObject(response.ContentStream) as DoubanErrorRes;

                            }

                            if (null != resObject && resObject is DoubanErrorRes)
                            {
                                sdkRes.errCode = DoubanSdkErrCode.SERVER_ERR;
                                sdkRes.specificCode = resObject.code;
                                sdkRes.content = resObject.msg;
                            }
                            else
                                throw new Exception();
                        }
                        catch//如果没有error_code这个节点...
                        {
                            //不是xml
                            //网络异常时统一错误：NET_UNUSUAL
                            if (response.StatusCode == HttpStatusCode.NotFound)
                            {
                                sdkRes.errCode = DoubanSdkErrCode.NET_UNUSUAL;
                                sdkRes.content = "网络状况异常";
                            }
                            else
                            {
                                sdkRes.errCode = DoubanSdkErrCode.SERVER_ERR;
                                sdkRes.specificCode = response.StatusCode.ToString();
                                sdkRes.content = response.Content;
                            }
                        }
                    }

                }
                // 正常情况
                else
                {
                    sdkRes.errCode = DoubanSdkErrCode.SUCCESS;
                    sdkRes.content = response.Content;
                    sdkRes.stream = response.ContentStream;
                }
            }
            catch (Exception e)
            {  
                sdkRes.errCode = DoubanSdkErrCode.SERVER_ERR;
                sdkRes.content = "服务器返回信息异常";
            }
            if (null != callBack)
                callBack(sdkRes);
        }
    }
}
