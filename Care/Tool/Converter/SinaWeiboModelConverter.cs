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
using Care.ViewModels;

namespace Care
{
    public class SinaWeiboModelConverter
    {
        public static ItemViewModel ConvertSinaWeiboToCommon(WStatus status)
        {
            FiltPicture(status);
            ItemViewModel model = new ItemViewModel();
            model.IconURL = status.user.profile_image_url;
            model.Title = status.user.name;
            model.Content = status.text;
            model.ImageURL = status.thumbnail_pic;
            model.MidImageURL = status.bmiddle_pic;
            model.FullImageURL = status.original_pic;
            model.TimeObject = status.CreatedAt;
            model.Type = EntryType.SinaWeibo;

            if (status.IsRetweetedStatus == "Visible")
            {
                model.ForwardItem = new ItemViewModel();
                if (status.retweeted_status.user != null)
                {
                    model.ForwardItem.Title = status.retweeted_status.user.name;
                }
                model.ForwardItem.Content = status.retweeted_status.text;
                model.ForwardItem.ImageURL = status.retweeted_status.thumbnail_pic;
                model.ForwardItem.MidImageURL = status.retweeted_status.bmiddle_pic;
                model.ForwardItem.FullImageURL = status.retweeted_status.original_pic;
                model.ForwardItem.TimeObject = status.retweeted_status.CreatedAt;
            }
            return model;
        }

        public static void FiltPicture(WStatus status)
        {
            if (string.IsNullOrEmpty(status.thumbnail_pic))
            {
                return;
            }
            PictureItem picItem = new PictureItem();
            picItem.Url = status.bmiddle_pic;
            picItem.FullUrl = status.original_pic;
            picItem.Id = status.id;
            picItem.Type = EntryType.SinaWeibo;
            picItem.Title = status.user.name;
            picItem.Content = status.text;
            picItem.TimeObject = status.CreatedAt;
            App.ViewModel.SinaWeiboPicItems.Add(picItem); 
        }
    }
}
