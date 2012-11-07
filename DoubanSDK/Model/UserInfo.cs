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
    public class UserInfo
    {
        public String id { get; set; }
        public String uid { get; set; }
        public String name { get; set; }
        public String avatar { get; set; }
        public String alt { get; set; }
        public String relation { get; set; }
        public String created { get; set; }
        public String loc_id { get; set; }
        public String loc_name { get; set; }
        public String desc { get; set; }
    }
}
