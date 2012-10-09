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
    public enum EntryType{
        SinaWeibo,
        Feed,
        Renren,
        Douban,
    };

    public class CommonEntryModel
    {
        public string _imageURL;
        public DateTime _time;
        public EntryType _type;
        public string _content;
        public string _originalURL;
    }
}
