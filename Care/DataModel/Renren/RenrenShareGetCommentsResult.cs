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
    // 分享的评论格式与其它不同，所以要单独写个类
    // 状态的评论和原创图片的评论见RenrenNews里面的Comment
    // 不同的评论都叫Comment，但是要注意的是他们的前缀不一样
    public class RenrenShareGetCommentsResult
    {
        public class Comment
        {
            public string uid { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string headurl { get; set; }            
            public string time { get; set; }            
            public string content { get; set; }

            public CommentViewModel ToCommentViewModel()
            {
                CommentViewModel model = new CommentViewModel();
                model.Title = this.name;                
                model.IconURL = this.headurl;
                model.Content = this.content;
                model.ID = this.id;
                model.TimeObject = ExtHelpers.GetRenrenTimeFullObject(this.time);
                return model;
            }
        }

        public string total { get; set; }

        // 注意这儿有个s，别忘了
        public Comment[] comments { get; set; }
    }
}
