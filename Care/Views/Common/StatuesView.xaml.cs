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

namespace Care.Views.Common
{
    public partial class StatuesView : PhoneApplicationPage
    {
        int m_nIndex = -1;
        ItemViewModel m_statusModel;

        public StatuesView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(StatuesView_Loaded);
        }

        private void StatuesView_Loaded(object sender, RoutedEventArgs e)
        {           
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