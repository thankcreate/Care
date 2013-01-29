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

        public static ItemViewModel ConvertDoubanUnionStatues(Statuses statues)
        {
            ItemViewModel model = new ItemViewModel();
            model.IconURL = statues.user.small_avatar;
            model.LargeIconURL = PreferenceHelper.GetPreference("Douban_FollowerAvatar2");
            model.Title = statues.user.screen_name;
            String attachTitle = "";
            if (statues.attachments != null && statues.attachments.Count > 0)
            {
                foreach (Statuses.Attachment attach in statues.attachments)
                {
                    attachTitle = attach.title;
                }
            }
            model.Content = TrimMark(statues.title) + " " + attachTitle + " " + statues.text;
            model.TimeObject = ExtHelpers.GetDoubanTimeFullObject(statues.created_at);
            model.Type = EntryType.Douban;
            model.ID = statues.id;
            model.Comments = new ObservableCollection<CommentViewModel>();
            model.CommentCount = statues.comments_count;
            model.SharedCount = statues.reshared_count;


            if (statues.reshared_status != null)
            {
                Statuses forwardStatus = statues.reshared_status;
                ItemViewModel forwardModel = new ItemViewModel();
                forwardModel.IconURL = forwardStatus.user.small_avatar;
                forwardModel.LargeIconURL = PreferenceHelper.GetPreference("Douban_FollowerAvatar2");
                forwardModel.Title = forwardStatus.user.screen_name;
                String forwardAttachTitle = "";
                if (forwardStatus.attachments != null && forwardStatus.attachments.Count > 0)
                {
                    foreach (Statuses.Attachment attach in forwardStatus.attachments)
                    {
                        forwardAttachTitle = attach.title;
                    }
                }
                forwardModel.Content = TrimMark(forwardStatus.title) + " " + forwardAttachTitle + " " + forwardStatus.text;
                forwardModel.TimeObject = ExtHelpers.GetDoubanTimeFullObject(forwardStatus.created_at);
                forwardModel.Type = EntryType.Douban;
                forwardModel.ID = forwardStatus.id;
                forwardModel.Comments = new ObservableCollection<CommentViewModel>();
                forwardModel.CommentCount = forwardStatus.comments_count;
                forwardModel.SharedCount = forwardStatus.reshared_count;

                if (PreferenceHelper.GetPreference("Global_NeedFetchImageInRetweet") != "False")
                {
                    FiltPicture(forwardStatus, forwardModel);
                }
                // 如果是转播的话，把model的text改成“转播”两字，不然空在那里很奇怪
                model.Content = "转播";
                model.CommentCount = forwardModel.CommentCount;
                model.SharedCount = forwardModel.SharedCount;
                model.ForwardItem = forwardModel;
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
