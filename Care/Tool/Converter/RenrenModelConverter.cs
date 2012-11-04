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
            model.Content = news.prefix;
            model.TimeObject = ExtHelpers.GetRenrenTimeFullObject(news.update_time);
            model.Type = EntryType.Renren;
            model.ID = news.post_id;
            model.OwnerID = news.actor_id;
            // 检查是否有转发
            if (news.attachment != null)
            {
                foreach (RenrenNews.Attachment attach in news.attachment)
                {
                    if (attach.media_type == RenrenNews.Attachment.TypeStatus)
                    {
                        model.ForwardItem = new ItemViewModel();
                        ItemViewModel forwardItem = model.ForwardItem;
                        forwardItem.Title = attach.owner_name;
                        forwardItem.Content = attach.content;
                    }
                }
            }
            return model;
        }
    }
}
