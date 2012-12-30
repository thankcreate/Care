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
using Microsoft.Phone.Shell;
using System.Collections.Generic;
using System.Linq;


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
            Filt(key, value);
            return true;
        }

        public static void Filt(String key, String value)
        {
            if(key == "SinaWeibo_FollowerAvatar" ||
                key == "SinaWeibo_FollowerAvatar2"||
                key == "SinaWeibo_FollowerNickName" ||
                key == "Renren_FollowerNickName" ||
                key == "Renren_FollowerAvatar"  ||
                key == "Renren_FollowerAvatar2" ||
                key == "Douban_FollowerNickName" ||
                key == "Douban_FollowerAvatar"  ||
                key == "Douban_FollowerAvatar2"
                 )
            {
                if (PreferenceHelper.GetPreference("Global_TileMode") != "1")
                {
                    String herName = MiscTool.GetHerName();
                    String herIcon = MiscTool.GetHerIconUrlInDesktopTile();
                    Uri herUri;
                    if (string.IsNullOrEmpty(herIcon))
                    {
                        herUri = new Uri("/Images/Thumb/CheekyTransparent.png", UriKind.Relative);
                    }
                    else
                    {
                        herUri = new Uri(herIcon, UriKind.Absolute);
                    }
                    ShellTile TileToFind = ShellTile.ActiveTiles.First();

                    // Application should always be found
                    if (TileToFind != null)
                    {
                        StandardTileData NewTileData = new StandardTileData
                        {
                            Title = "我只在乎你",
                            BackgroundImage = new Uri("/Images/Thumb/HeartTransparent.png", UriKind.Relative),
                            Count = 0,
                            BackTitle = herName,
                            BackBackgroundImage = herUri,
                            BackContent = " "
                        };

                        // Update the Application Tile
                        TileToFind.Update(NewTileData);
                        PreferenceHelper.SetPreference("Global_TileMode", "0");
                    }
                }
            }
        }

        public static void RemovePreference(String key)
        {            
            if (settings.Contains(key))
            {
                settings.Remove(key);
            }
            Filt(key, null);
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
            RemovePreference("SinaWeibo_Avatar");
            RemovePreference("SinaWeibo_FollowerAvatar");
            RemovePreference("SinaWeibo_FollowerAvatar2"); 
        }

        public static void RemoveSinaWeiboLoginAccountPreference()
        {
            RemovePreference("SinaWeibo_NickName");
            RemovePreference("SinaWeibo_ID");
            RemovePreference("SinaWeibo_Token");
            RemovePreference("SinaWeibo_Avatar");
        }

        public static void RemoveRenrenPreference()
        {
            RemovePreference("Renren_NickName");
            RemovePreference("Renren_ID");
            RemovePreference("Renren_FollowerID");
            RemovePreference("Renren_FollowerNickName");
            RemovePreference("Renren_Token");
            RemovePreference("Renren_Avatar");
            RemovePreference("Renren_FollowerAvatar");
            RemovePreference("Renren_FollowerAvatar2");
        }


        public static void RemoveRenrenLoginAccountPreference()
        {
            RemovePreference("Renren_NickName");
            RemovePreference("Renren_ID");
            RemovePreference("Renren_Token");
            RemovePreference("Renren_Avatar");
        }

        public static void RemovePasswordPreference()
        {
            RemovePreference("Global_Password");
        }

        public static void RemoveRssPreference()
        {
            RemovePreference("RSS_FollowerPath");
            RemovePreference("RSS_FollowerSite");
        }

        public static void RemoveDoubanPreference()
        {
            RemovePreference("Douban_NickName");
            RemovePreference("Douban_ID");
            RemovePreference("Douban_FollowerID");
            RemovePreference("Douban_FollowerNickName");
            RemovePreference("Douban_Token");
            RemovePreference("Douban_Avatar");
            RemovePreference("Douban_FollowerAvatar");
            RemovePreference("Douban_FollowerAvatar2");
        }

        public static void RemoveDoubanLoginAccountPreference()
        {
            RemovePreference("Douban_NickName");
            RemovePreference("Douban_ID");
            RemovePreference("Douban_Token");
            RemovePreference("Douban_Avatar");
        }
    }
}
