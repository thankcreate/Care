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

namespace Care.Tool
{
    public class PreferenceHelper
    {
        public static IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        // 若不存在返回String.Empty
        public static String GetPreference(String key)
        {            
            if (settings.Contains(key))
            {
                return settings[key] as String;
            }
            return string.Empty;             
        }

        public static Boolean SetPreference(String key, String value)
        { 
            try
            {
                settings.Add(key, value);                
            }
            catch (ArgumentException ex)
            {
                settings[key] = value;
            }
            return true;
        }

        public static void RemovePreference(String key)
        {            
            if (settings.Contains(key))
            {
                settings.Remove(key);
            }
        }

        public static void SavePreference()
        {
            settings.Save();            
        }

        public static void RemoveSinaWeiboPreference()
        {
            RemovePreference("SinaWeibo_NickName");
            RemovePreference("SinaWeibo_ID");
            RemovePreference("SinaWeibo_FollowerID");
            RemovePreference("SinaWeibo_FollowerNickName");
            RemovePreference("SinaWeibo_Token");            
        }

        public static void RemoveRenrenPreference()
        {
            RemovePreference("Renren_NickName");
            RemovePreference("Renren_ID");
            RemovePreference("Renren_FollowerID");
            RemovePreference("Renren_FollowerNickName");
            RemovePreference("Renren_Token");
        }

        public static void RemovePasswordPreference()
        {
            RemovePreference("Global_Password");
        }
    }
}
