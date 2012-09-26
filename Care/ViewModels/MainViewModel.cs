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

            this.PictureItems = new ObservableCollection<PicureItem>();
            this.ListPictureItems = new List<PicureItem>();
            this.SinaWeiboPicItems = new List<PicureItem>();
          

            this.ItemsNeedRefresh = 2;
            this.SinaWeiboAccount = new User();
            this.IsChanged = true;
        }


        // Main items
        public ObservableCollection<ItemViewModel> Items { get; private set; }
        public List<ItemViewModel> ListItems { get; private set; }
        public List<ItemViewModel> SinaWeiboItems { get; private set; }
        public List<ItemViewModel> RssItems { get; private set; }
        public int ItemsNeedRefresh;
        public User SinaWeiboAccount;
        public string SinaWeiboCareID;

        public bool IsChanged;

        // Pic items
        public ObservableCollection<PicureItem> PictureItems { get; private set; }
        public List<PicureItem> ListPictureItems { get; private set; }
        public List<PicureItem> SinaWeiboPicItems { get; private set; }
        public List<PicureItem> RssPicItems { get; private set; }

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
    }
}