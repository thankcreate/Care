using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using Microsoft.Phone.Controls;
using Care.Tool;
using Microsoft.Xna.Framework.Media;

namespace Care.Views.Common
{
    public partial class ImageView : PhoneApplicationPage,  INotifyPropertyChanged
    {
        int m_nIndex = -1;
        bool m_bTextVisable = true;

        public ImageView()
        {
            InitializeComponent();
            var gl = GestureService.GetGestureListener(img);
            gl.DragCompleted += GestureListener_DragCompleted;
        }

        private void GestureListener_DragCompleted(object sender, DragCompletedGestureEventArgs e)
        {
            double horizonChange = e.HorizontalChange;
            // horizonChange < 0 表示手向左滑
            if (horizonChange < 0)
            {
                if (m_nIndex == 8)
                {
                    MessageBox.Show("已经是最后一页哦~");
                    return;
                }
                else
                {
                    this.DataContext = PictureMap.StaticPictureMap[++m_nIndex];
                }
            }
            else
            {
                if (m_nIndex < 1)
                {
                    MessageBox.Show("已经是第一页了哦~");
                    return;
                }
                else
                {
                    this.DataContext = PictureMap.StaticPictureMap[--m_nIndex];
                }
            }
            
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IDictionary<string, string> queryString = this.NavigationContext.QueryString;
            if (queryString.ContainsKey("Index"))
            {
                m_nIndex = int.Parse(queryString["Index"]);
                this.DataContext = App.ViewModel._pictureItem4;
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

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            String url = PictureMap.StaticPictureMap[m_nIndex].FullUrl;
            WebClient client = new WebClient();
            client.OpenReadCompleted += (s, e1) =>
            {
                if (e1.Error == null)
                {
                    MediaLibrary library = new MediaLibrary();
                    library.SavePicture("Image_From_Care", e1.Result);
                }
            };
            client.OpenReadAsync(new Uri(url, UriKind.Absolute));
        }
    }
}