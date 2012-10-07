using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.ComponentModel;

namespace Care
{
    public class StatusItem
    {

    }

    public class User
    {
        public string id { get; set; }
        public string idstr { get; set; }
        public string screen_name { get; set; }
        public string name { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string profile_image_url { get; set; }
        public string profile_url { get; set; }
        public string domain { get; set; }
        public string weihao { get; set; }
        public string gender { get; set; }
        public string followers_count { get; set; }
        public string friends_count { get; set; }
        public string statuses_count { get; set; }
        public string favourites_count { get; set; }
        public string created_at { get; set; }
        public bool following { get; set; }
        public bool allow_all_act_msg { get; set; }
        public bool geo_enabled { get; set; }
        public bool verified { get; set; }
        public string IsVerified
        {
            get
            {
                return verified ? "Visible" : "Collapsed";
            }
        }
        public string verified_type { get; set; }
        public string remark { get; set; }
        public bool allow_all_comment { get; set; }
        public string avatar_large { get; set; }
        public string verified_reason { get; set; }
        public bool follow_me { get; set; }
        public string online_status { get; set; }
        public string bi_followers_count { get; set; }
        public string lang { get; set; }
    }


    public class Visible
    {
        public string type { get; set; }
        public string list_id { get; set; }
    }


    public class WStatus
    {
        public string created_at { get; set; }
        public DateTimeOffset CreatedAt
        {
            get
            {
                return string.IsNullOrEmpty(created_at) ? new DateTimeOffset() : ExtHelpers.GetSinaTimeFullObject(created_at);
            }
        }

        public string id { get; set; }
        public string mid { get; set; }
        public string idstr { get; set; }
        public string text { get; set; }
        public string source { get; set; }
        public string Source
        {
            get
            {
                string so = string.Empty;
                if (!string.IsNullOrEmpty(source))
                {
                    int pos = source.IndexOf(">");
                    if (-1 != pos)
                    {
                        int end = source.IndexOf("</a>", pos, source.Length - pos);
                        so = source.Substring(pos + 1, end - pos - 1);
                    }
                }
                return so;
            }
        }
        public bool favorited { get; set; }
        public bool truncated { get; set; }
        public string in_reply_to_status_id { get; set; }
        public string in_reply_to_user_id { get; set; }
        public string in_reply_to_screen_name { get; set; }
        public string thumbnail_pic { get; set; }
        public string IsThumbnail
        {
            get
            {
                return string.IsNullOrEmpty(thumbnail_pic) ? "Collapsed" : "Visible";
            }
        }
        public string bmiddle_pic { get; set; }
        public string original_pic { get; set; }
        public object geo { get; set; }
        public User user { get; set; }
        public WStatus retweeted_status { get; set; }
        public string IsRetweetedStatus
        {
            get
            {
                return retweeted_status == null ? "Collapsed" : "Visible";
            }
        }
        public int reposts_count { get; set; }
        public int comments_count { get; set; }
        public int mlevel { get; set; }
        public Visible visible { get; set; }
    }

    public class WStatuses
    {
        public WStatus[] statuses { get; set; }

        //public string hasvisible { get; set; }

        //public string previous_cursor { get; set; }

        //public string next_cursor { get; set; }

        //public string total_number { get; set; }

    }

    public class Friends
    {
        public User[] users { get; set; }
        public string next_cursor { get; set; }
        public string previous_cursor { get; set; }
        public string total_number { get; set; }
    }
}
