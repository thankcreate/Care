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
using System.ComponentModel;
using Microsoft.Phone.Controls;
using Care.Tool;
namespace Care.Views.Common
{
    public partial class ImageView : PhoneApplicationPage,  INotifyPropertyChanged
    {
        int m_nIndex = -1;
        bool m_bTextVisable = true;

        public ImageView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IDictionary<string, string> queryString = this.NavigationContext.QueryString;
            if (queryString.ContainsKey("Index"))
            {
                m_nIndex = int.Parse(queryString["Index"]);
                this.DataContext = PictureMap.StaticPictureMap[m_nIndex];
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

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (m_bTextVisable)
            {
                innerTextControl.Visibility = Visibility.Collapsed;
                ApplicationBar.IsVisible = false;
                m_bTextVisable = false;
            }
            else
            {
                innerTextControl.Visibility = Visibility.Visible;
                ApplicationBar.IsVisible = true;
                m_bTextVisable = true;
            }
        }
    }
}