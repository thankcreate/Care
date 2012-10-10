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
using System.ServiceModel.Syndication;
namespace Care
{
    public class FeedModelConverter
    {
        public static ItemViewModel ConvertFeedToCommon(SyndicationItem feedItem)
        {
            ItemViewModel model = new ItemViewModel();
            model.Title = feedItem.Title.Text;
            model.TimeObject = feedItem.PublishDate;
            model.RssSummary = feedItem.Summary.Text;
            model.IconURL = "/Care;component/Images/RSS feeds.png";
            
            model.Type = EntryType.Feed;
            if( feedItem.Links.Count != 0)
            {
                model.OriginalURL = feedItem.Links[0].Uri.ToString();
            }
            return model;
        }
    }
}
