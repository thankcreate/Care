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
    public class RenrenNews
    {
        public class Attachment
        {
            public static String TypePhoto = "photo";
            public static String TypeAlbum = "album";
            public static String TypeLink = "link";
            public static String TypeVideo = "video";
            public static String TypeAudio = "audio";
            public static String TypeStatus = "status";

            public string href { get; set; }
            public string media_type { get; set; }
            //public string scr { get; set; }
            public string media_id { get; set; }
            public string owner_id { get; set; }
            public string owner_name { get; set; }
            public string content { get; set; }
        }

        public class Likes
        {
            public string total_count { get; set; }
            public string friend_count { get; set; }
            public string user_like { get; set; }
            public string like { get; set; }
        }

        public class Comments
        {
            public class Comment
            {
                public string uid { get; set; }
                public string name { get; set; }
                public string headurl { get; set; }
                public string time { get; set; }
                public string comment_id { get; set; }
                public string text { get; set; }
            }

            public string count { get; set; }
            public Comment[] comment { get; set; }
        }

        public string post_id { get; set; }
        public string source_id { get; set; }
        public string feed_type { get; set; }
        public string update_time { get; set; }
        public string actor_id { get; set; }

        // 实际上可以看原是转发内容的前面那一段儿
        public string prefix { get; set; }  

        public string name { get; set; }
        public string headurl { get; set; }
        public string title { get; set; }
        public string href { get; set; }
        public string description { get; set; }

        // 这玩意儿相当于ItemViewModel里的content 
        // 但是如果是转发,它会把转发内容和原始内容杂到一起
        public string message { get; set; }

        public Comments comments { get; set; }
        public Attachment[] attachment { get; set; }
        public Likes likes { get; set; }

        // public string attachment { get; set; }
        //public string content { get; set; }
        //public string feed_type { get; set; }
        //public string update_time { get; set; }
        //public string actor_id { get; set; }
    }
}
