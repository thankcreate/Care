using System;
using System.Net;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;

namespace Care.Tool
{
    public class MiscTool
    {
        public static String GetMyName()
        {
            String myName = PreferenceHelper.GetPreference("SinaWeibo_NickName");
            if (!string.IsNullOrEmpty(myName))
            {
                return myName;
            }
            // sina is null ,than search renren
            myName = PreferenceHelper.GetPreference("Renren_NickName");
            if (!string.IsNullOrEmpty(myName))
            {
                return myName;
            }
            // than search douban
            myName = PreferenceHelper.GetPreference("Douban_NickName");
            if (!string.IsNullOrEmpty(myName))
            {
                return myName;
            }

            return "";
        }

        public static String GetHerName()
        {
            String herName = PreferenceHelper.GetPreference("SinaWeibo_FollowerNickName");
            if (!string.IsNullOrEmpty(herName))
            {
                return herName;
            }
            // sina is null
            herName = PreferenceHelper.GetPreference("Renren_FollowerNickName");
            if (!string.IsNullOrEmpty(herName))
            {
                return herName;
            }
            // renren is null
            herName = PreferenceHelper.GetPreference("Douban_FollowerNickName");
            if (!string.IsNullOrEmpty(herName))
            {
                return herName;
            }
            return "";
        }

        // 现在其实没有区别了
        public static String GetHerIconUrl()
        {  
            String herIcon = PreferenceHelper.GetPreference("SinaWeibo_FollowerAvatar2");
            if (!string.IsNullOrEmpty(herIcon))
            {
                return herIcon;
            }

             
            herIcon = PreferenceHelper.GetPreference("Renren_FollowerAvatar2");
            if (!string.IsNullOrEmpty(herIcon))
            {
                return herIcon;
            }

             
            herIcon = PreferenceHelper.GetPreference("Douban_FollowerAvatar2");
            if (!string.IsNullOrEmpty(herIcon))
            {
                return herIcon;
            }

            herIcon = PreferenceHelper.GetPreference("SinaWeibo_FollowerAvatar");
            if (!string.IsNullOrEmpty(herIcon))
            {
                return herIcon;
            }

            herIcon = PreferenceHelper.GetPreference("Renren_FollowerAvatar");
            if (!string.IsNullOrEmpty(herIcon))
            {
                return herIcon;
            }

            herIcon = PreferenceHelper.GetPreference("Douban_FollowerAvatar");
            if (!string.IsNullOrEmpty(herIcon))
            {
                return herIcon;
            }
            return "";
        }

        // 桌面上必须要用高清图，不论这个图是不是规则的四方形
        public static String GetHerIconUrlInDesktopTile()
        {
            String herIcon = PreferenceHelper.GetPreference("SinaWeibo_FollowerAvatar2");
            if (!string.IsNullOrEmpty(herIcon))
            {
                return herIcon;
            }
                         
            herIcon = PreferenceHelper.GetPreference("Renren_FollowerAvatar2");
            if (!string.IsNullOrEmpty(herIcon))
            {
                return herIcon;
            }

            herIcon = PreferenceHelper.GetPreference("Douban_FollowerAvatar2");
            if (!string.IsNullOrEmpty(herIcon))
            {
                return herIcon;
            }

            herIcon = PreferenceHelper.GetPreference("SinaWeibo_FollowerAvatar");
            if (!string.IsNullOrEmpty(herIcon))
            {
                return herIcon;
            }

            herIcon = PreferenceHelper.GetPreference("Renren_FollowerAvatar");
            if (!string.IsNullOrEmpty(herIcon))
            {
                return herIcon;
            }

            herIcon = PreferenceHelper.GetPreference("Douban_FollowerAvatar");
            if (!string.IsNullOrEmpty(herIcon))
            {
                return herIcon;
            }
            return "";
        }

        public static String GetMyIconUrl()
        {
            String myIcon = PreferenceHelper.GetPreference("SinaWeibo_Avatar");
            if (!string.IsNullOrEmpty(myIcon))
            {
                return myIcon;
            }

            myIcon = PreferenceHelper.GetPreference("Renren_Avatar");
            if (!string.IsNullOrEmpty(myIcon))
            {
                return myIcon;
            }

            myIcon = PreferenceHelper.GetPreference("Douban_Avatar");
            if (!string.IsNullOrEmpty(myIcon))
            {
                return myIcon;
            }
            return "";           
        }

        // >_< 一点也不Friendly吧....这种设定~~~哦哈哈哈哈~~~~
        // 这是主要是为了滤掉gif图
        public static String MakeFriendlyImageURL(String url)
        {
            if (url == null)
            {
                return null;
            }
            if (url.EndsWith("gif"))
            {
                return "";
            }
            return url;
        }


        public static byte[] EncodeToJpeg(WriteableBitmap wb)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                wb.SaveJpeg(
                    stream,
                    wb.PixelWidth,
                    wb.PixelHeight,
                    0,
                    85);
                return stream.ToArray();
            }
        }

        public static string RemoveHtmlTag(String input)
        {
            try
            {
                String withoutHtml = Regex.Replace(input, "<.+?>", string.Empty);
                return withoutHtml;
            }
            catch (System.Exception ex)
            {
                return input;
            }
            
        }
    }
}
