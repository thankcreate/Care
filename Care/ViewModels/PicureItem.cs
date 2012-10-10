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
using Care.ViewModels;

namespace Care
{
    public class PictureItem
    {
        public PictureItem()
        {
            Url = "";
            FullUrl = "";
            Id = "";
            Title = "";
            Content = "";
            OriginUrl = "";
            Type = EntryType.SinaWeibo;
        }
        public string Url { get; set; }
        public string FullUrl { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string OriginUrl { get; set; }
        public EntryType Type { get; set; }
        public String TypeString
        {
            get
            {
                switch (Type)
                {
                    case EntryType.Renren:
                        return "   来自人人";
                    case EntryType.Douban:
                        return "   来自豆瓣";
                    case EntryType.SinaWeibo:
                        return "   来自新浪微博";
                    case EntryType.Feed:
                        return "   来自RSS订阅";
                }
                return "来自火星";
            }
        }

        public string _time;
        public string Time 
        {
            get
            {
                return _time = ExtHelpers.TimeObjectToString(TimeObject);
            }
            set
            {
                _time = value;
            }
        }
        public DateTimeOffset TimeObject { get; set; }

        public string IsImageExists
        {
            get
            {
                return string.IsNullOrEmpty(Url) ? "Collapsed" : "Visible";
            }
        }
    }
}
