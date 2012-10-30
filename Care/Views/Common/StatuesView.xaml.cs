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

namespace Care.Views.Common
{
    public partial class StatuesView : PhoneApplicationPage, INotifyPropertyChanged
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
            if (m_statusModel == null)
                return;
            if (m_statusModel.Comments == null)
                m_statusModel.Comments = new ObservableCollection<CommentViewModel>();
            
            m_statusModel.Comments.Clear();
            SinaWeiboFetcher fetcher = new SinaWeiboFetcher();
            fetcher.LoadSinaWeiboCommentByStatusID(m_statusModel.ID, (comments) =>
            {
                if (comments == null)
                {
                    return;
                }
                Comments tempComments = comments;
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    foreach (Comment comment in comments.comments)
                    {
                        m_statusModel.Comments.Add(SinaWeiboModelConverter.ConvertCommentToCommon(comment));
                    }
                });
            });
            
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
    }
}