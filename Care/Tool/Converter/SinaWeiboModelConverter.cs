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
using Care.Tool;
using System.Collections.ObjectModel;
namespace Care
{
    public class SinaWeiboModelConverter
    {
        public static ItemViewModel ConvertItemToCommon(WStatus status)
        {
            try
            {
                FiltPicture(status);
                ItemViewModel model = new ItemViewModel();
                model.IconURL = status.user.profile_image_url;
                model.LargeIconURL = status.user.avatar_large;
                model.Title = status.user.name;
                model.Content = status.text;
                model.ImageURL = MiscTool.MakeFriendlyImageURL(status.thumbnail_pic);
                model.MidImageURL = MiscTool.MakeFriendlyImageURL(status.bmiddle_pic);
                model.FullImageURL = MiscTool.MakeFriendlyImageURL(status.original_pic);
                model.TimeObject = status.CreatedAt;
                model.Type = EntryType.SinaWeibo;
                model.ID = status.id;
                model.SharedCount = status.reposts_count.ToString();
                model.CommentCount = status.comments_count.ToString();
                model.Comments = new ObservableCollection<CommentViewModel>();


                if (status.IsRetweetedStatus == "Visible")
                {
                    model.ForwardItem = new ItemViewModel();
                    if (status.retweeted_status.user != null)
                    {
                        model.ForwardItem.Title = status.retweeted_status.user.name;
                    }
                    model.ForwardItem.Content = status.retweeted_status.text;
                    model.ForwardItem.ImageURL = MiscTool.MakeFriendlyImageURL(status.retweeted_status.thumbnail_pic);
                    model.ForwardItem.MidImageURL = MiscTool.MakeFriendlyImageURL(status.retweeted_status.bmiddle_pic);
                    model.ForwardItem.FullImageURL = MiscTool.MakeFriendlyImageURL(status.retweeted_status.original_pic);
                    model.ForwardItem.TimeObject = status.retweeted_status.CreatedAt;
                    model.ForwardItem.Type = EntryType.SinaWeibo;
                    model.ForwardItem.SharedCount = status.retweeted_status.reposts_count.ToString();
                    model.ForwardItem.CommentCount = status.retweeted_status.comments_count.ToString();
                    model.ForwardItem.ID = status.id;
                    model.ForwardItem.IconURL = status.retweeted_status.user.profile_image_url;
                    model.ForwardItem.LargeIconURL = status.retweeted_status.user.avatar_large;
                }
                return model;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        public static void FiltPicture(WStatus status)
        {
            if (status.retweeted_status != null && PreferenceHelper.GetPreference("Global_NeedFetchImageInRetweet") != "False")
            {
                FiltPicture(status.retweeted_status);
            }
            if (string.IsNullOrEmpty(status.thumbnail_pic))
            {
                return;
            }
            PictureItem picItem = new PictureItem();
            picItem.Url = MiscTool.MakeFriendlyImageURL(status.bmiddle_pic);
            picItem.FullUrl = MiscTool.MakeFriendlyImageURL(status.original_pic);
            picItem.Id = status.id;
            picItem.Type = EntryType.SinaWeibo;
            picItem.Title = status.user.name;
            picItem.Content = status.text;
            picItem.TimeObject = status.CreatedAt;

            // 之所以要检测两次是因为如果是gif，在这里也会被赋为空值
            if (string.IsNullOrEmpty(picItem.Url))
            {
                return;
            }

            App.ViewModel.SinaWeiboPicItems.Add(picItem); 
        }



        public static CommentViewModel ConvertCommentToCommon(Comment comment)
        {
            try
            {
                CommentViewModel model = new CommentViewModel();
                model.Content = comment.text;
                model.Title = comment.user.name;
                model.IconURL = comment.user.profile_image_url;
                model.ID = comment.id;
                model.TimeObject = string.IsNullOrEmpty(comment.created_at) ? new DateTimeOffset() : ExtHelpers.GetSinaTimeFullObject(comment.created_at);
                model.Type = EntryType.SinaWeibo;
                return model;
            }
            catch (System.Exception ex)
            {
                return null;
            }           
        }
    }
}
