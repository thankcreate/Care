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
    public class RefreshViewHelper
    {
        public static void RefreshViewItems()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                App.ViewModel.ListItems.Clear();
                App.ViewModel.ListItems.AddRange(App.ViewModel.SinaWeiboItems);
                App.ViewModel.ListItems.AddRange(App.ViewModel.RssItems);
                App.ViewModel.ListItems.AddRange(App.ViewModel.RenrenItems);
                App.ViewModel.ListItems.AddRange(App.ViewModel.DoubanItems);
                App.ViewModel.ListItems.Sort(
                    delegate(ItemViewModel a, ItemViewModel b)
                    {
                        return (a.TimeObject < b.TimeObject ? 1 : a.TimeObject == b.TimeObject ? 0 : -1);
                    }
                    );
                App.ViewModel.Items.Clear();
                if (App.ViewModel.ListItems.Count == 0)
                {
                    ItemViewModel model = new ItemViewModel();
                    model.Title = "没有得到任何结果的说~~~";
                    model.Content = "请到帐号设置页中登陆并设置关注人";
                    App.ViewModel.ListItems.Add(model);
                }
                App.ViewModel.ListItems.ForEach(p => App.ViewModel.Items.Add(p));
               
                RefreshPicturePage();
                SaveToCache();               
            });
        }

        public static void SaveToCache()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (!settings.Contains("Global_TimelineCache"))
            {
                settings.Add("Global_TimelineCache", App.ViewModel.Items);
            }
            else
            {
                settings["Global_TimelineCache"] = App.ViewModel.Items;
            }
            settings.Save();
        }

        public static void RefreshPicturePage()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                App.ViewModel.ListPictureItems.Clear();
                App.ViewModel.ListPictureItems.AddRange(App.ViewModel.SinaWeiboPicItems);
                App.ViewModel.ListPictureItems.AddRange(App.ViewModel.RenrenPicItems);
                App.ViewModel.ListPictureItems.AddRange(App.ViewModel.DoubanPicItems);
                App.ViewModel.ListPictureItems.Sort(
                    delegate(PictureItem a, PictureItem b)
                    {
                        return (a.TimeObject < b.TimeObject ? 1 : a.TimeObject == b.TimeObject ? 0 : -1);
                    });

                // 目前最多只需要9个
                int count = App.ViewModel.ListPictureItems.Count > 9 ? 9 : App.ViewModel.ListPictureItems.Count;

                App.ViewModel.PictureItems.Clear();
                App.ViewModel.ListPictureItems.GetRange(0, count).ForEach(p => 
                   {
                       string test = p.Url;
                       App.ViewModel.PictureItems.Add(p);
                   });
            });
        }
    }
}
