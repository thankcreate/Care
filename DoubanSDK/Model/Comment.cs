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

namespace DoubanSDK
{
    public class Comment
    {
        public class User
        {
            public String city { get; set; }
            public String icon_avatar { get; set; }
            public String statuses_count { get; set; }
            public String screen_name { get; set; }
            public String url { get; set; }
            public String created_at { get; set; }
            public String description { get; set; }
            public String is_first_visit { get; set; }
            public String new_site_to_vu_count { get; set; }
            public String location { get; set; }
            public String small_avatar { get; set; }
            public String following { get; set; }
            public String verified { get; set; }
            public String large_avatar { get; set; }
            public String type { get; set; }
            public String id { get; set; }
            public String uid { get; set; }            
        }
        //public String entities { get; set; }
        public String text { get; set; }
        public String created_at { get; set; }
        public String source { get; set; }
        public User user { get; set; }
        public String id { get; set; }
    }
}
