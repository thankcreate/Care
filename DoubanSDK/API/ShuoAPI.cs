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
using Hammock.Web;
using Hammock;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using System.IO.IsolatedStorage;

namespace DoubanSDK
{
    public delegate void GetUserTimeLineCompleteHandler(GetUserTimeLineEventArgs e);
    public delegate void GetCommentsCompleteHandler(GetCommentsEventArgs e);
    public class ShuoAPI
    {
        DoubanNetEngine m_netEngine;

        public void GetUserTimeLine(String id, int count, GetUserTimeLineCompleteHandler handler)
        {
            if (m_netEngine == null)
                m_netEngine = new DoubanNetEngine();
            if (count > 200)
                count = 200;            
            RestRequest request = new RestRequest();
            request.Method = WebMethod.Get;
            request.Path = String.Format("shuo/v2/statuses/user_timeline/{0}", id);
            request.AddParameter("count",count.ToString());

            m_netEngine.SendRequest(request, (DoubanSdkResponse response) =>
            {
                if (response.errCode == DoubanSdkErrCode.SUCCESS)
                {
                    GetUserTimeLineEventArgs args = new GetUserTimeLineEventArgs();
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<Statuses>));
                    List<Statuses> list = ser.ReadObject(response.stream) as List<Statuses>;
                    args.statues = list;
                    args.errorCode = DoubanSdkErrCode.SUCCESS;
                    args.specificCode = response.specificCode;
                    handler(args);
                }
                else
                {
                    GetUserTimeLineEventArgs args = new GetUserTimeLineEventArgs();
                    args.statues = null;
                    args.errorCode = response.errCode;
                    args.specificCode = response.specificCode;
                    handler(args);
                }
            });  
        }

        public void GetComments(String id, GetCommentsCompleteHandler handler)
        {
            if (m_netEngine == null)
                m_netEngine = new DoubanNetEngine();
            RestRequest request = new RestRequest();
            request.Method = WebMethod.Get;
            request.Path = String.Format("shuo/v2/statuses/{0}/comments", id);            

            m_netEngine.SendRequest(request, (DoubanSdkResponse response) =>
            {
                if (response.errCode == DoubanSdkErrCode.SUCCESS)
                {
                    GetCommentsEventArgs args = new GetCommentsEventArgs();
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<Comment>));
                    List<Comment> list = ser.ReadObject(response.stream) as List<Comment>;
                    args.comments = list;
                    args.errorCode = DoubanSdkErrCode.SUCCESS;
                    args.specificCode = response.specificCode;
                    handler(args);
                }
                else
                {
                    GetCommentsEventArgs args = new GetCommentsEventArgs();
                    args.comments = null;
                    args.errorCode = response.errCode;
                    args.specificCode = response.specificCode;
                    handler(args);
                }
            });  
        }
        public void AddComments(String id, String text, CompleteHandler handler)
        {
            if (m_netEngine == null)
                m_netEngine = new DoubanNetEngine();
            RestRequest request = new RestRequest();
            request.Method = WebMethod.Post;
            request.Path = String.Format("shuo/v2/statuses/{0}/comments", id);
            request.AddParameter("text", text);
            request.AddParameter("source", DoubanSdkData.AppKey);

            m_netEngine.SendRequest(request, (DoubanSdkResponse response) =>
            {
                if (response.errCode == DoubanSdkErrCode.SUCCESS)
                {
                    DoubanEventArgs args = new DoubanEventArgs();
                    args.errorCode = DoubanSdkErrCode.SUCCESS;
                    args.specificCode = response.specificCode;
                    handler(args);
                }
                else
                {
                    DoubanEventArgs args = new DoubanEventArgs();
                    args.errorCode = response.errCode;
                    args.specificCode = response.specificCode;
                    handler(args);                 
                }
            });  
        }

        public void PostStatusesWithPic(String text, String path, CompleteHandler handler)
        {
            if (m_netEngine == null)
                m_netEngine = new DoubanNetEngine();
            RestRequest request = new RestRequest();
            request.Method = WebMethod.Post;
            request.Path = "shuo/v2/statuses/";
            request.AddParameter("text", text);
            request.AddParameter("source", DoubanSdkData.AppKey);

            //path = "";
            if (!String.IsNullOrEmpty(path))
            {
                IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                if (!file.FileExists(path))
                {
                    file.Dispose();
                    DoubanEventArgs args = new DoubanEventArgs();
                    args.errorCode = DoubanSdkErrCode.XPARAM_ERR;
                    handler(args);
                    return;
                }
                file.Dispose();
                string picType = System.IO.Path.GetExtension(path);
                string picName = System.IO.Path.GetFileName(path);
                if ("png" == picType)
                {
                    request.AddFile("image", picName, path, "image/png");
                }
                else
                {
                    request.AddFile("image", picName, path, "image/jpeg");
                }
            } 

            m_netEngine.SendRequest(request, (DoubanSdkResponse response) =>
            {
                if (response.errCode == DoubanSdkErrCode.SUCCESS)
                {
                    DoubanEventArgs args = new DoubanEventArgs();
                    args.errorCode = DoubanSdkErrCode.SUCCESS;
                    args.specificCode = response.specificCode;
                    handler(args);
                }
                else
                {
                    DoubanEventArgs args = new DoubanEventArgs();
                    args.errorCode = response.errCode;
                    args.specificCode = response.specificCode;
                    handler(args);
                }
            });
        }

    }
}
