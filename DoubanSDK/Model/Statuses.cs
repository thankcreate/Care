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
using System.Collections.Generic;

namespace DoubanSDK
{
    public class Statuses
    {
        public class User
        {
            public String uid { get; set; }
            public String description { get; set; }
            public String small_avatar { get; set; }
            public String type { get; set; }
            public String id { get; set; }
            public String screen_name { get; set; }
        }

        public class Attachment
        {
            public class Media
            {
                public String src { get; set; }
                public String href { get; set; }
                public String type { get; set; }
                public String original_src { get; set; }
            }

            public String description { get; set; }
            public String title { get; set; }
            public List<Media> media { get; set; }
            public String expaned_href { get; set; }
            public String caption { get; set; }
            public String href { get; set; }
            public String type { get; set; }
        }

        public String category { get; set; }
        public String reshared_count { get; set; }
        public List<Attachment> attachments { get; set; }
        //public String source { get; set; }
        public String text { get; set; }
        public String created_at { get; set; }
        public String title { get; set; }
        public String can_reply { get; set; }
        public String liked { get; set; }
        //public String entities { get; set; }
        public String like_count { get; set; }
        public String comments_count { get; set; }
        public User user { get; set; }
        public String is_follow { get; set; }
        public String has_photo { get; set; }
        public String type { get; set; }
        public String id { get; set; }

        public Statuses reshared_status { get; set; }
    }
}
