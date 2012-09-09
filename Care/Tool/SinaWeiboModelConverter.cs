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

namespace Care
{
    public class SinaWeiboModelConverter
    {
        public static ItemViewModel ConvertSinaWeiboToCommon(WStatus status)
        {
            ItemViewModel model = new ItemViewModel();
            model.IconURL = status.user.profile_image_url;
            model.Title = status.user.name;
            model.Content = status.text;
            model.ImageURL = status.thumbnail_pic;
            model.Time = status.CreatedAt;

            if (status.IsRetweetedStatus == "Visible")
            {
                model.ForwardItem = new ItemViewModel();
                if (status.retweeted_status.user != null)
                {
                    model.ForwardItem.Title = status.retweeted_status.user.name;
                }
                model.ForwardItem.Content = status.retweeted_status.text;
                model.ForwardItem.ImageURL = status.retweeted_status.thumbnail_pic;
                model.ForwardItem.Time = status.retweeted_status.CreatedAt;
            }
            return model;
        }
    }
}
