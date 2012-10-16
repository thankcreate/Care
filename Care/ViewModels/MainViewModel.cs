using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Care.Tool;

namespace Care
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Items = new ObservableCollection<ItemViewModel>();
            this.ListItems = new List<ItemViewModel>();
            this.SinaWeiboItems = new List<ItemViewModel>();
            this.RssItems = new List<ItemViewModel>();
            this.RenrenItems = new List<ItemViewModel>();

            this.PictureItems = new ObservableCollection<PictureItem>();
            this.PictureItems2 = new ObservableCollection<PictureItem>();
            this.ListPictureItems = new List<PictureItem>();
            this.SinaWeiboPicItems = new List<PictureItem>();
            this.RssPicItems = new List<PictureItem>();
          

            this.ItemsNeedRefresh = 2;
            this.SinaWeiboAccount = new User();
            this.IsChanged = true;

            #region PicItems by hard code.
            this.PictureItem0 = new PictureItem();
            this.PictureItem1 = new PictureItem();
            this.PictureItem2 = new PictureItem();
            this.PictureItem3 = new PictureItem();
            this.PictureItem4 = new PictureItem();
            this.PictureItem5 = new PictureItem();
            this.PictureItem6 = new PictureItem();
            this.PictureItem7 = new PictureItem();
            this.PictureItem8 = new PictureItem();
            #endregion
        }


        // Main items
        public ObservableCollection<ItemViewModel> Items { get; private set; }
        public List<ItemViewModel> ListItems { get; private set; }
        public List<ItemViewModel> SinaWeiboItems { get; private set; }
        public List<ItemViewModel> RssItems { get; private set; }
        public List<ItemViewModel> RenrenItems { get; private set; }
        public int ItemsNeedRefresh;
        public User SinaWeiboAccount;
        public string SinaWeiboCareID;

        public bool IsChanged;

        public ObservableCollection<PictureItem> PictureItems2 { get; private set; }

       #region PictureItems by hard code.

        public PictureItem _pictureItem0;
        public PictureItem PictureItem0
        {
            get
            {
                return _pictureItem0;
            }
            set
            {
                _pictureItem0 = value;
                NotifyPropertyChanged("PictureItem0");
            }
        }

        public PictureItem _pictureItem1;
        public PictureItem PictureItem1
        {
            get
            {
                return _pictureItem1;
            }
            set
            {
                _pictureItem1 = value;
                NotifyPropertyChanged("PictureItem1");
            }
        }

        public PictureItem _pictureItem2;
        public PictureItem PictureItem2
        {
            get
            {
                return _pictureItem2;
            }
            set
            {
                _pictureItem2 = value;
                NotifyPropertyChanged("PictureItem2");
            }
        }

        public PictureItem _pictureItem3;
        public PictureItem PictureItem3
        {
            get
            {
                return _pictureItem3;
            }
            set
            {
                _pictureItem3 = value;
                NotifyPropertyChanged("PictureItem3");
            }
        }

        public PictureItem _pictureItem4;
        public PictureItem PictureItem4
        {
            get
            {
                return _pictureItem4;
            }
            set
            {
                _pictureItem4 = value;
                NotifyPropertyChanged("PictureItem4");
            }
        }

        public PictureItem _pictureItem5;
        public PictureItem PictureItem5
        {
            get
            {
                return _pictureItem5;
            }
            set
            {
                _pictureItem5 = value;
                NotifyPropertyChanged("PictureItem5");
            }
        }

        public PictureItem _pictureItem6;
        public PictureItem PictureItem6
        {
            get
            {
                return _pictureItem6;
            }
            set
            {
                _pictureItem6 = value;
                NotifyPropertyChanged("PictureItem6");
            }
        }
        public PictureItem _pictureItem7;
        public PictureItem PictureItem7
        {
            get
            {
                return _pictureItem7;
            }
            set
            {
                _pictureItem7 = value;
                NotifyPropertyChanged("PictureItem7");
            }
        }

        public PictureItem _pictureItem8;
        public PictureItem PictureItem8
        {
            get
            {
                return _pictureItem8;
            }
            set
            {
                _pictureItem8 = value;
                NotifyPropertyChanged("PictureItem8");
            }
        }

#endregion

        // Pic items
        public ObservableCollection<PictureItem> PictureItems { get; private set; }
        public List<PictureItem> ListPictureItems { get; private set; }
        public List<PictureItem> SinaWeiboPicItems { get; private set; }
        public List<PictureItem> RssPicItems { get; private set; }

        // Setting
        public String UsingPassword
        {
            // 只有False和True两种可能值
            get
            {
                String ifUsePassword = PreferenceHelper.GetPreference("Global_UsePassword");
                if (String.IsNullOrEmpty(ifUsePassword))
                {
                    return "False";
                }
                return PreferenceHelper.GetPreference("Global_UsePassword");
            }
            set
            {
                PreferenceHelper.SetPreference("Global_UsePassword", value);
                NotifyPropertyChanged("UsingPassword");
                NotifyPropertyChanged("UsingPasswordText");
            }
        }
       
        public String UsingPasswordText
        {
            // 直接依赖于UsingPassword的值
            get
            {
                return UsingPassword == "True" ? "是" : "否";
            }
            set
            {
                NotifyPropertyChanged("UsingPasswordText");
            }
        }
         
        public String NeedFetchImageInRetweet
        {
            // 只有False和True两种可能值
            get
            {
                String ifUsePassword = PreferenceHelper.GetPreference("Global_NeedFetchImageInRetweet");
                if (String.IsNullOrEmpty(ifUsePassword))
                {
                    return "True";
                }
                return PreferenceHelper.GetPreference("Global_NeedFetchImageInRetweet");
            }
            set
            {
                PreferenceHelper.SetPreference("Global_NeedFetchImageInRetweet", value);
                NotifyPropertyChanged("NeedFetchImageInRetweet");
                NotifyPropertyChanged("NeedFetchImageInRetweetText");
            }
        }

        public String NeedFetchImageInRetweetText
        {
            // 直接依赖于NeedFetchImageInRetweet的值
            get
            {
                return NeedFetchImageInRetweet == "True" ? "是" : "否";
            }
            set
            {
                NotifyPropertyChanged("NeedFetchImageInRetweetText");
            }
        }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            // Sample data; replace with real data
            this.Items.Add(new ItemViewModel() { Title = "runtime one", Content = "Maecenas praesent accumsan bibendum"});
            this.Items.Add(new ItemViewModel() { Title = "runtime two", Content = "Dictumst eleifend facilisi faucibus"});
            //this.Friends.Add(new User() { name = "xiechuang", profile_image_url = "love wxh" });
            //this.Friends.Add(new User() { name = "wuxihui", profile_image_url = "don't love tron T_T" });
           this.IsDataLoaded = true;
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

        public void test()
        {
            NotifyPropertyChanged("UsingPassword");
        }
    }
}