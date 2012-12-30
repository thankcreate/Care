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
using System.Collections.ObjectModel;

namespace Care.Tool
{
    public class RenrenModelConverter
    {
        public static CommentViewModel ConvertCommentToCommon(RenrenNews.Comments.Comment comment)
        {
            try
            {
                if (comment == null)
                    return null;
                CommentViewModel commentViewModel = new CommentViewModel();
                commentViewModel.Title = comment.name;
                // 人人的评论头像有可能来自headurl，也可能来自tinyurl，都要试一下
                commentViewModel.IconURL = string.IsNullOrEmpty(comment.headurl) ? comment.tinyurl : comment.headurl;
                commentViewModel.Content = MiscTool.RemoveHtmlTag(comment.text);
                commentViewModel.ID = comment.comment_id;
                commentViewModel.UID = comment.uid;
                commentViewModel.Type = EntryType.Renren;
                commentViewModel.TimeObject = ExtHelpers.GetRenrenTimeFullObject(comment.time);
                return commentViewModel;
            }
            catch (System.Exception ex)
            {
                return null;
            }
          
        }

        // 分享的评论格式不一样，所以要再单独写个转换类
        public static CommentViewModel ConvertShareCommentToCommon(RenrenShareGetCommentsResult.Comment comment)
        {
            try
            {
                CommentViewModel model = new CommentViewModel();
                if (model == null)
                    return null;
                model.Title = comment.name;
                model.IconURL = comment.headurl;
                model.Content = MiscTool.RemoveHtmlTag(comment.content);
                model.ID = comment.id;
                model.TimeObject = ExtHelpers.GetRenrenTimeFullObject(comment.time);
                model.Type = EntryType.Renren;
                return model;
            }
            catch (System.Exception ex)
            {
                return null;
            }
            
        }

        public static ItemViewModel ConvertRenrenNewsToCommon(RenrenNews news)
        {
            try
            {
                if (news.feed_type == null)
                    return null;
                if (news.feed_type == RenrenNews.FeedTypeStatus)
                {
                    return ConvertStatus(news);
                }
                else if (news.feed_type == RenrenNews.FeedTypeUploadPhoto)
                {
                    return ConvertUploadPhoto(news);
                }
                else if (news.feed_type == RenrenNews.FeedTypeSharePhoto)
                {
                    return ConvertSharePhoto(news);
                }
                return null;
            }
            catch (System.Exception ex)
            {
                return null;
            }          
        }

        public static ItemViewModel ConvertStatus(RenrenNews news)
        {
            if (news == null)
            {
                return null;
            }

            ItemViewModel model = new ItemViewModel();
            model.IconURL = news.headurl;
            model.LargeIconURL = PreferenceHelper.GetPreference("Renren_FollowerAvatar2");
            model.Title = news.name;
            model.Content = MiscTool.RemoveHtmlTag(news.prefix);
            model.TimeObject = ExtHelpers.GetRenrenTimeFullObject(news.update_time);
            model.Type = EntryType.Renren;
            model.ID = news.source_id;
            model.OwnerID = news.actor_id;
            model.RenrenFeedType = RenrenNews.FeedTypeStatus;
            model.CommentCount = news.comments.count;
            model.SharedCount = "";
            // 检查是否有转发
            if (news.attachment != null)
            {
                foreach (RenrenNews.Attachment attach in news.attachment)
                {
                    // 转发状态
                    if (attach.media_type == RenrenNews.Attachment.AttachTypeStatus)
                    {
                        model.ForwardItem = new ItemViewModel();
                        ItemViewModel forwardItem = model.ForwardItem;
                        forwardItem.Title = attach.owner_name;
                        forwardItem.Content = MiscTool.RemoveHtmlTag(attach.content);
                        break;
                    }
                }
            }
            return model;
        }

        public static ItemViewModel ConvertUploadPhoto(RenrenNews news)
        {
            if (news == null || news.attachment == null)
            {
                return null;
            }

            ItemViewModel model = new ItemViewModel();
            model.IconURL = news.headurl;
            model.LargeIconURL = PreferenceHelper.GetPreference("Renren_FollowerAvatar2");
            model.Title = news.name;
            model.TimeObject = ExtHelpers.GetRenrenTimeFullObject(news.update_time);
            model.Type = EntryType.Renren;
            
            model.OwnerID = news.actor_id;
            model.RenrenFeedType = RenrenNews.FeedTypeUploadPhoto;
            model.CommentCount = news.comments.count;
            model.SharedCount = "";
            foreach (RenrenNews.Attachment attach in news.attachment)
            {
                // 原创图片上传
                if (attach.media_type == RenrenNews.Attachment.AttachTypePhoto)
                {
                    model.Content = MiscTool.RemoveHtmlTag(attach.content);
                    model.ImageURL = MiscTool.MakeFriendlyImageURL(attach.src);
                    model.MidImageURL = MiscTool.MakeFriendlyImageURL(attach.raw_src);
                    model.FullImageURL = MiscTool.MakeFriendlyImageURL(attach.raw_src);
                    // 对于图片上传而言，其id应该是attach中的id，而不是source_id
                    model.ID = attach.media_id;

                    // 创建图片项
                    PictureItem picItem = new PictureItem();
                    picItem.Url = MiscTool.MakeFriendlyImageURL(attach.raw_src);
                    picItem.FullUrl = MiscTool.MakeFriendlyImageURL(attach.raw_src);
                    picItem.Id = attach.media_id;
                    picItem.Type = EntryType.Renren;
                    picItem.Title = news.name;
                    picItem.Content = MiscTool.RemoveHtmlTag(attach.content);
                    picItem.TimeObject =  ExtHelpers.GetRenrenTimeFullObject(news.update_time);
                    
                    // 之所以这里还要检测，是因为有gif图的情况需要过滤掉                    
                    if (!string.IsNullOrEmpty(picItem.Url))
                    {
                        App.ViewModel.RenrenPicItems.Add(picItem);
                    }     
                    break;
                }
            }
            return model;            
        }

        public static ItemViewModel ConvertSharePhoto(RenrenNews news)
        {
            if (news == null || news.attachment == null)
            {
                return null;
            }

            ItemViewModel model = new ItemViewModel();
            model.IconURL = news.headurl;
            model.LargeIconURL = PreferenceHelper.GetPreference("Renren_FollowerAvatar2");
            model.Title = news.name;
            model.Content = news.message;
            model.TimeObject = ExtHelpers.GetRenrenTimeFullObject(news.update_time);
            model.Type = EntryType.Renren;
            model.ID = news.source_id;
            model.OwnerID = news.actor_id;
            model.RenrenFeedType = RenrenNews.FeedTypeSharePhoto;
            model.CommentCount = news.comments.count;
            model.SharedCount = "";
            foreach (RenrenNews.Attachment attach in news.attachment)
            {
                // 分享图片上传
                if (attach.media_type == RenrenNews.Attachment.AttachTypePhoto)
                {
                    model.ForwardItem = new ItemViewModel();
                    ItemViewModel forwardItem = model.ForwardItem;
                    forwardItem.Title = attach.owner_name;
                    forwardItem.Content = MiscTool.RemoveHtmlTag(news.description);

                    forwardItem.ImageURL = MiscTool.MakeFriendlyImageURL(attach.src);
                    forwardItem.MidImageURL = MiscTool.MakeFriendlyImageURL(attach.raw_src);
                    forwardItem.FullImageURL = MiscTool.MakeFriendlyImageURL(attach.raw_src);               


                    // 创建图片项
                    PictureItem picItem = new PictureItem();
                    picItem.Url = MiscTool.MakeFriendlyImageURL(attach.raw_src);
                    picItem.FullUrl = MiscTool.MakeFriendlyImageURL(attach.raw_src);
                    picItem.Id = attach.media_id;
                    picItem.Type = EntryType.Renren;
                    picItem.Title = news.name;
                    picItem.Content = MiscTool.RemoveHtmlTag(news.message);
                    picItem.TimeObject = ExtHelpers.GetRenrenTimeFullObject(news.update_time);

                    // 之所以这里还要检测，是因为有gif图的情况需要过滤掉
                    if (!string.IsNullOrEmpty(picItem.Url))
                    {
                        App.ViewModel.RenrenPicItems.Add(picItem);
                    }
                    break;
                }
            }
            return model;     

        }

        public static bool MakeRenrenComments(RenrenNews news, ItemViewModel model)
        {
            if (news == null || model == null || news.comments == null || news.comments.comment == null)
                return false;
            model.Comments = new ObservableCollection<CommentViewModel>();
            foreach (RenrenNews.Comments.Comment comment in news.comments.comment)
            {
                CommentViewModel commentModel = ConvertCommentToCommon(comment);
                if (commentModel != null)
                {
                    model.Comments.Add(commentModel);
                }                
            }
            return true;
        }
    }
}
