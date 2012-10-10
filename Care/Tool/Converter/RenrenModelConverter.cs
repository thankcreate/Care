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

namespace Care.Tool
{
    public class RenrenModelConverter
    {
        public static ItemViewModel ConvertRenrenNewsToCommon(RenrenNews news)
        {
            ItemViewModel model = new ItemViewModel();
            model.IconURL = news.headurl;
            model.Title = news.name;
            model.Content = news.message;
            model.TimeObject = ExtHelpers.GetRenrenTimeFullObject(news.update_time);
            model.Type = EntryType.Renren;
            return model;
        }
    }
}
