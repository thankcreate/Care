using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.ComponentModel;
using Care.Tool;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Resources;
using System.Windows.Media.Imaging;
namespace Care.Views.Common
{
    public partial class StatuesView : PhoneApplicationPage
    {
        int m_nIndex = -1;
        ItemViewModel m_statusModel;

        public StatuesView()
        {
           
            InitializeComponent();
            InitHeaderHeight();
            this.Loaded += new RoutedEventHandler(StatuesView_Loaded);
        }

        private void StatuesView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadImage();
        }

        private void InitHeaderHeight()
        {
            double perc = ((double)230) / ((double)800);             
            headerGrid.Height = Application.Current.RootVisual.RenderSize.Height * perc;
        }     

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IDictionary<string, string> queryString = this.NavigationContext.QueryString;
            if (queryString.ContainsKey("Index"))
            {
                m_nIndex = int.Parse(queryString["Index"]);                
                m_statusModel = App.ViewModel.Items[m_nIndex];
                this.DataContext = m_statusModel;
            }

            base.OnNavigatedTo(e);
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

        private BitmapImage _thumbSource;
        private BitmapImage _forwardThumbSource;
        private void LoadImage()
        {
            if (m_statusModel != null && !String.IsNullOrEmpty(m_statusModel.FullImageURL))
            {
                _thumbSource = new BitmapImage(new Uri(m_statusModel.FullImageURL, UriKind.Absolute));
                _thumbSource.ImageOpened += new EventHandler<RoutedEventArgs>(ThumbImageOpened);
                _thumbSource.CreateOptions = BitmapCreateOptions.BackgroundCreation | BitmapCreateOptions.DelayCreation | BitmapCreateOptions.IgnoreImageCache;
                ThumbImage.Source = _thumbSource;                
            }

            if (m_statusModel.ForwardItem != null && !String.IsNullOrEmpty(m_statusModel.ForwardItem.FullImageURL))
            {
                _forwardThumbSource = new BitmapImage(new Uri(m_statusModel.ForwardItem.FullImageURL, UriKind.Absolute));
                _forwardThumbSource.ImageOpened += new EventHandler<RoutedEventArgs>(ForwardThumbImageOpened);
                _forwardThumbSource.CreateOptions = BitmapCreateOptions.BackgroundCreation | BitmapCreateOptions.DelayCreation | BitmapCreateOptions.IgnoreImageCache;
                RetweetThumbImage.Source = _forwardThumbSource;
            }
        }

        private void ThumbImageOpened(object sender, RoutedEventArgs e)
        {
            if (_thumbSource == null)
                return;
            double width = _thumbSource.PixelWidth;
            double height = _thumbSource.PixelHeight;
            if (width < 460)
            {
                ThumbImage.Stretch = Stretch.None;
                ThumbImage.HorizontalAlignment = HorizontalAlignment.Center;
            }
            else
            {
                ThumbImage.Stretch = Stretch.UniformToFill;
            }
        }

        private void ForwardThumbImageOpened(object sender, RoutedEventArgs e)
        {
            if (_forwardThumbSource == null)
                return;
            double width = _forwardThumbSource.PixelWidth;
            double height = _forwardThumbSource.PixelHeight;
            if (width < 460)
            {
                RetweetThumbImage.Stretch = Stretch.None;
                RetweetThumbImage.HorizontalAlignment = HorizontalAlignment.Center;
            }
            else
            {
                ThumbImage.Stretch = Stretch.UniformToFill;
            }
        }

        private void Comment_Click(object sender, EventArgs e)
        {            
            StringBuilder sb = new StringBuilder();                        
            // 因为人人很BT，权限问题比较多，所以要带上FeedType和OwnerID
            sb.AppendFormat("/Views/Common/CommentView.xaml?ID={0}&Type={1}&RenrenFeedType={2}&RenrenOwnerID={3}"
               , m_statusModel.ID, m_statusModel.Type, m_statusModel.RenrenFeedType, m_statusModel.OwnerID);
            NavigationService.Navigate(new Uri(sb.ToString(), UriKind.Relative));            
        }
    }
}