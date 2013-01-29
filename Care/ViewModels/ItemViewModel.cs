using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace Care
{
    public class ItemViewModel : INotifyPropertyChanged
    {

        private string _iconURL;
        private string _largeIconURL;
        private string _imageURL;
        private string _midImageURL;
        private string _fullImageURL;

        private DateTimeOffset _timeObject;
        private string _time;
        private EntryType _type;        
        private string _content;
        private string _rssSummary;
        private string _title;
        private string _originalURL;
        private string _description;
        private string _ownerID;
        private string _commentCount;
        private string _sharedCount;

        public ObservableCollection<CommentViewModel> Comments { get; set; }

        public string ID { get; set; }

        // 这是只针对人人的，因为人人的新鲜事分很多类型
        public string RenrenFeedType { get; set; }

        public ItemViewModel ForwardItem{ get; set; }

        public string IsForwardItemExists
        {
            get
            {
                return ForwardItem == null ? "Collapsed" : "Visible";
            }
        }

        public string IsContentExists
        {
            get
            {
                return Content == null ? "Collapsed" : "Visible";
            }
        }

        public string IsImageExists
        {
            get
            {
                return string.IsNullOrEmpty(_imageURL) ? "Collapsed" : "Visible";
            }
        }

        public string IsCommentExists
        {
            get
            {
                return _type == EntryType.Feed ? "Collapsed" : "Visible";
            }
        }

        public string IconURL
        {
            get
            {
                return _iconURL;
            }
            set
            {
                if (value != _iconURL)
                {
                    _iconURL = value;
                    NotifyPropertyChanged("IconURL");
                }
            }
        }

        public string LargeIconURL
        {
            get
            {
                if(String.IsNullOrEmpty(_largeIconURL))
                    return _iconURL;
                return _largeIconURL;
            }
            set
            {
                if (value != _largeIconURL)
                {
                    _largeIconURL = value;
                    NotifyPropertyChanged("LargeIconURL");
                }
            }
        }        

        public string ImageURL
        {
            get
            {
                return _imageURL;
            }
            set
            {
                if (value != _imageURL)
                {
                    _imageURL = value;
                    NotifyPropertyChanged("ImageURL");
                }
            }
        }

        public string MidImageURL
        {
            get
            {
                if (String.IsNullOrEmpty(_midImageURL))
                    return ImageURL;
                return _midImageURL;
            }
            set
            {
                if (value != _midImageURL)
                {
                    _midImageURL = value;
                    NotifyPropertyChanged("MidImageURL");
                }
            }
        }

        public string FullImageURL
        {
            get
            {
                if (String.IsNullOrEmpty(_fullImageURL))
                    return MidImageURL;
                return _fullImageURL;
            }
            set
            {
                if (value != _fullImageURL)
                {
                    _fullImageURL = value;
                    NotifyPropertyChanged("FullImageURL");
                }
            }
        }

        public EntryType Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (value != _type)
                {
                    _type = value;
                    NotifyPropertyChanged("Type");
                }
            }
        }
        
        public DateTimeOffset TimeObject
        {
            get
            {
                return _timeObject;
            }
            set
            {
                if (value != _timeObject)
                {
                    _timeObject = value;
                    NotifyPropertyChanged("TimeObject");
                }
            }
        }

        public string Time
        {
            get
            {
                string timePart = ExtHelpers.TimeObjectToString(_timeObject);
                string fromPart = "";
                //switch (Type)
                //{
                //    case EntryType.Renren:
                //        fromPart = "   来自人人";
                //        break;
                //    case EntryType.Douban:
                //        fromPart = "   来自豆瓣";
                //        break;
                //    case EntryType.SinaWeibo:
                //        fromPart = "   来自新浪微博";
                //        break;
                //    case EntryType.Feed:
                //        fromPart = "   来自RSS订阅";
                //        break;
                //}
                return _time = timePart + fromPart;
            }
            set
            {
                if (value != _time)
                {
                    _time = value;
                    NotifyPropertyChanged("Time");
                }
            }
        }

        public string From
        {
            get
            {                
                string fromPart = "";
                switch (Type)
                {
                    case EntryType.Renren:
                        fromPart = "来自人人";
                        break;
                    case EntryType.Douban:
                        fromPart = "来自豆瓣";
                        break;
                    case EntryType.SinaWeibo:
                        fromPart = "来自新浪微博";
                        break;
                    case EntryType.Feed:
                        fromPart = "来自RSS订阅";
                        break;
                    default:
                        fromPart = "来自火星";
                        break;
                }
                return fromPart;
            }
        }

        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                if (value != _content)
                {
                    _content = value;
                    NotifyPropertyChanged("Content");
                }
            }
        }

        public string RssSummary
        {
            get
            {
                return _rssSummary;
            }
            set
            {
                if (value != _rssSummary)
                {
                    _rssSummary = value;
                    NotifyPropertyChanged("RssSummary");
                }
            }
        }

        public string OriginalURL
        {
            get
            {
                return _originalURL;
            }
            set
            {
                if (value != _originalURL)
                {
                    _originalURL = value;
                    NotifyPropertyChanged("OriginalURL");
                }
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (value != _description)
                {
                    _title = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        public string OwnerID
        {
            get
            {
                return _ownerID;
            }
            set
            {
                if (value != _ownerID)
                {
                    _ownerID = value;
                    NotifyPropertyChanged("OwnerID");
                }
            }
        }

        public string CommentCount
        {
            get
            {
                return _commentCount;
            }
            set
            {
                if (value != _commentCount)
                {
                    _commentCount = value;
                    NotifyPropertyChanged("CommentCount");
                }
            }
        }

        public string SharedCount
        {
            get
            {
                return _sharedCount;
            }
            set
            {
                if (value != _sharedCount)
                {
                    _sharedCount = value;
                    NotifyPropertyChanged("SharedCount");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}