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
using System.IO.IsolatedStorage;

namespace DoubanSDK
{
    public class DoubanAppInfo
    {
        public DoubanTokenInfo tokenInfo = null;

        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        string tokenInfoKey = "DoubanTokenInfo";

        public DoubanAppInfo()
        {
            if (!settings.TryGetValue<DoubanTokenInfo>(tokenInfoKey, out tokenInfo))
            {
                tokenInfo = new DoubanTokenInfo();
            }           
        }

        public void SetTokenInfo(DoubanTokenInfo info)
        {
            if (info == null)
                return;
            tokenInfo = info;
            if (!settings.Contains(tokenInfoKey))
            {
                settings.Add(tokenInfoKey, tokenInfo);
            }
            else
            {
                settings[tokenInfoKey] = tokenInfo;
            }
            settings.Save();
        }

        public void CleanUp()
        {
            tokenInfo.CleanUp();            
            if (settings.Contains(tokenInfoKey))
                settings.Remove(tokenInfoKey);         
        }
    }



    public class DoubanTokenInfo
    {
        public string access_token { get; set; }
        public DateTime expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }

        /// <summary>
        /// 清除信息
        /// </summary>
        public void CleanUp()
        {
            this.access_token = null;
            this.expires_in = DateTime.Now;
            this.refresh_token = null;
            this.scope = null;
        }
    }
}
