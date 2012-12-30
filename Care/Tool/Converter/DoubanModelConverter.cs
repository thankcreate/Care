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
using DoubanSDK;
using System.Collections.ObjectModel;

namespace Care.Tool
{
    public class DoubanModelConverter
    {
        public static CommentViewModel ConvertCommentToCommon(DoubanSDK.Comment comment)
        {
            try
            {
                if (comment == null)
                    return null;
                CommentViewModel commentViewModel = new CommentViewModel();
                commentViewModel.Title = comment.user.screen_name;
                commentViewModel.IconURL = MiscTool.MakeFriendlyImageURL(comment.user.small_avatar);
                commentViewModel.Content = comment.text;
                commentViewModel.ID = comment.id;
                commentViewModel.UID = comment.user.id;
                commentViewModel.DoubanUID = comment.user.uid;
                commentViewModel.TimeObject = ExtHelpers.GetRenrenTimeFullObject(comment.created_at);
                commentViewModel.Type = EntryType.Douban;
                return commentViewModel;
            }
            catch (System.Exception ex)
            {
                return null;	
            }          
        }

        public static ItemViewModel ConvertDoubanStatuesToCommon(Statuses statues)
        {
            try
            {
                if (statues.title.Contains("关注")  // 新关注了某个人
                   || statues.title.Contains("加入")    // 加入小组
                   || statues.title.Contains("活动")    // 对某活动感兴趣
                   || statues.title.Contains("歌曲")    // 某2添加了某歌曲
                   || statues.title.Contains("试读")    // 正在试读
                   || statues.title.Contains("豆瓣阅读")    // 豆瓣阅读
                   || statues.title.Contains("使用")   // 开始使用
                   || statues.title.Contains("日记"))   // 写了日记
                {
                    return null;
                }

                if (statues.type == "collect_book") // 不硬编码不舒服斯基
                {
                    return ConvertBookStatus(statues);
                }
                else if (statues.type == "collect_movie")
                {
                    return ConvertMovieStatus(statues);
                }
                else if (statues.type == "collect_music")
                {
                    return ConvertMusicStatus(statues);
                }
                else if (statues.title == "说：" || statues.type == "text")  // 豆瓣现在抽风，纯文字状态有时候type是null
                {
                    return ConvertTextStatus(statues);
                }
               
                // should never got here
                return null;
            }
            catch (System.Exception ex)
            {
                return null;
            }        
        }

        public static ItemViewModel ConvertMusicStatus(Statuses statues)
        {
            ItemViewModel model = new ItemViewModel();
            model.IconURL = statues.user.small_avatar;
            model.LargeIconURL = PreferenceHelper.GetPreference("Douban_FollowerAvatar2");
            model.Title = statues.user.screen_name;
            String MovieTitle = "";
            if (statues.attachments != null && statues.attachments.Count > 0)
            {
                foreach (Statuses.Attachment attach in statues.attachments)
                {
                    if (attach.type == "music")
                    {
                        MovieTitle = attach.title;
                    }
                }
            }
            model.Content = TrimMark(statues.title) + " “" + MovieTitle + "”  " + statues.text;
            //model.ImageURL = MiscTool.MakeFriendlyImageURL(statues.thumbnail_pic);
            //model.MidImageURL = MiscTool.MakeFriendlyImageURL(status.bmiddle_pic);
            //model.FullImageURL = MiscTool.MakeFriendlyImageURL(status.original_pic);
            model.TimeObject = ExtHelpers.GetDoubanTimeFullObject(statues.created_at);
            model.Type = EntryType.Douban;
            model.ID = statues.id;
            model.Comments = new ObservableCollection<CommentViewModel>();
            model.CommentCount = statues.comments_count;
            model.SharedCount = statues.reshared_count;
            FiltPicture(statues, model);
            return model;
        }

        public static ItemViewModel ConvertMovieStatus(Statuses statues)
        {
            ItemViewModel model = new ItemViewModel();
            model.IconURL = statues.user.small_avatar;
            model.LargeIconURL = PreferenceHelper.GetPreference("Douban_FollowerAvatar2");
            model.Title = statues.user.screen_name;            
            String MovieTitle = "";
            if (statues.attachments != null && statues.attachments.Count > 0)
            {
                foreach (Statuses.Attachment attach in statues.attachments)
                {
                    if (attach.type == "movie")
                    {
                        MovieTitle = attach.title;
                    }
                }
            }
            model.Content = TrimMark(statues.title) + " “" + MovieTitle + "”  " + statues.text;
            //model.ImageURL = MiscTool.MakeFriendlyImageURL(statues.thumbnail_pic);
            //model.MidImageURL = MiscTool.MakeFriendlyImageURL(status.bmiddle_pic);
            //model.FullImageURL = MiscTool.MakeFriendlyImageURL(status.original_pic);
            model.TimeObject = ExtHelpers.GetDoubanTimeFullObject(statues.created_at);
            model.Type = EntryType.Douban;
            model.ID = statues.id;
            model.Comments = new ObservableCollection<CommentViewModel>();
            model.CommentCount = statues.comments_count;
            model.SharedCount = statues.reshared_count;
            FiltPicture(statues, model);
            return model;
        }

        public static ItemViewModel ConvertBookStatus(Statuses statues)
        {
            ItemViewModel model = new ItemViewModel();
            model.IconURL = statues.user.small_avatar;
            model.LargeIconURL = PreferenceHelper.GetPreference("Douban_FollowerAvatar2");
            model.Title = statues.user.screen_name;
            String bookTitle = "";
            if (statues.attachments != null && statues.attachments.Count > 0)
            {                
                foreach (Statuses.Attachment attach in statues.attachments)
                {
                    if (attach.type == "book")
                    {
                        bookTitle = attach.title;
                    }                                   
                }
            }
            model.Content = TrimMark(statues.title) + " “" + bookTitle + "”  " + statues.text;
            //model.ImageURL = MiscTool.MakeFriendlyImageURL(statues.thumbnail_pic);
            //model.MidImageURL = MiscTool.MakeFriendlyImageURL(status.bmiddle_pic);
            //model.FullImageURL = MiscTool.MakeFriendlyImageURL(status.original_pic);
            model.TimeObject = ExtHelpers.GetDoubanTimeFullObject(statues.created_at);
            model.Type = EntryType.Douban;
            model.ID = statues.id;
            model.Comments = new ObservableCollection<CommentViewModel>();
            model.CommentCount = statues.comments_count;
            model.SharedCount = statues.reshared_count;
            FiltPicture(statues, model);
            return model;
        }

        public static ItemViewModel ConvertTextStatus(Statuses statues)
        {
            ItemViewModel model = new ItemViewModel();
            model.IconURL = statues.user.small_avatar;
            model.LargeIconURL = PreferenceHelper.GetPreference("Douban_FollowerAvatar2");
            model.Title = statues.user.screen_name;            
            model.Content = statues.text;
            //model.ImageURL = MiscTool.MakeFriendlyImageURL(statues.thumbnail_pic);
            //model.MidImageURL = MiscTool.MakeFriendlyImageURL(status.bmiddle_pic);
            //model.FullImageURL = MiscTool.MakeFriendlyImageURL(status.original_pic);
            model.TimeObject = ExtHelpers.GetDoubanTimeFullObject(statues.created_at);
            model.Type = EntryType.Douban;
            model.ID = statues.id;
            model.Comments = new ObservableCollection<CommentViewModel>();
            model.CommentCount = statues.comments_count;
            model.SharedCount = statues.reshared_count;

            // 如果是转发
            // Caution : 对于豆瓣而言，对转发的评论其实就是对原作评论
            if (statues.reshared_status != null)
            {
                ItemViewModel shareModel = ConvertDoubanStatuesToCommon(statues.reshared_status);
                if (shareModel == null)
                    return null;
                // 如果是转播的话，把model的text改成“转播”两字，不然空在那里很奇怪
                model.Content = "转播"/*你妹*/;
                model.ForwardItem = shareModel;
            }
            FiltPicture(statues, model);
            return model;
        }


        // 因为读过的内容会在Title里写个如下打分字样 ，要把它去掉
        // 读过[score]0[/score]
        public static String TrimMark(String input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '[')
                {
                    String result = input.Substring(0, i);
                    return result;
                }
            }
            return input;
        }

        public static void FiltPicture(Statuses statues, ItemViewModel model)
        {
            try
            {
                if (statues.attachments != null && statues.attachments.Count > 0)
                {
                    foreach (Statuses.Attachment attach in statues.attachments)
                    {
                        if (attach.media != null && attach.media.Count > 0)
                        {
                            foreach (Statuses.Attachment.Media media in attach.media)
                            {
                                if (media.type == "image")
                                {
                                    model.ImageURL = media.src;
                                    model.MidImageURL = GenerateDoubanSrc(model.ImageURL, "median");
                                    model.FullImageURL = GenerateDoubanSrc(model.ImageURL, "raw");

                                    PictureItem picItem = new PictureItem();
                                    picItem.Url = model.MidImageURL;
                                    picItem.FullUrl = model.FullImageURL;
                                    picItem.Id = model.ID;
                                    picItem.Type = model.Type;
                                    picItem.Title = model.Title;
                                    picItem.Content = model.Content;
                                    picItem.TimeObject = model.TimeObject;
                                    App.ViewModel.DoubanPicItems.Add(picItem);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
            	
            }
           
        }

        public static String GenerateDoubanSrc(String input, String convertTo)
        {
            if (input == null)
                return null;
            return input.Replace("small", convertTo);
        }

    }
}
