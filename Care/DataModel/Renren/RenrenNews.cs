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
        public string post_id { get; set; }
        public string source_id { get; set; }
        public string feed_type { get; set; }
        public string update_time { get; set; }
        public string actor_id { get; set; }


        public string name { get; set; }
        public string headurl { get; set; }
        public string title { get; set; }
        public string href { get; set; }
        public string description { get; set; }

        // 这玩意儿相当于ItemViewModel里的content
        public string message { get; set; }

        // public string attachment { get; set; }
        //public string content { get; set; }
        //public string feed_type { get; set; }
        //public string update_time { get; set; }
        //public string actor_id { get; set; }
    }
}
