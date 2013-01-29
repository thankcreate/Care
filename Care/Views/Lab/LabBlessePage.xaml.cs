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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Care.Tool;

namespace Care.Views.Lab
{
    public partial class LabBlessePage : PhoneApplicationPage, INotifyPropertyChanged
    {
        public ObservableCollection<BlessItem> Items { get; private set; }
        
        private ProgressIndicatorHelper m_progressIndicatorHelper;
        private BlessHelper blessHelper;

        public LabBlessePage()
        {
            InitializeComponent();
            Items = new ObservableCollection<BlessItem>();
            this.DataContext = this;
            this.Loaded += new RoutedEventHandler(LabBlessePage_Loaded);
        }

        void LabBlessePage_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Shell.SystemTray.ProgressIndicator = new Microsoft.Phone.Shell.ProgressIndicator();
            m_progressIndicatorHelper = new ProgressIndicatorHelper(Microsoft.Phone.Shell.SystemTray.ProgressIndicator, () =>
            {

                String firstLoad = PreferenceHelper.GetPreference("Global_FirstLoadBlessList");
                if (String.IsNullOrEmpty(firstLoad))
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        PreferenceHelper.SetPreference("Global_FirstLoadBlessList", "WhatEver");
                        
                        MessageBox.Show("发表在心语墙上的内容，写得比较好的会显示在软件启动页上哦~", "^_^", MessageBoxButton.OK);
                    });
                }
            });

            if (blessHelper == null)
                blessHelper = new BlessHelper();
            
            

            m_progressIndicatorHelper.PushTask();
            blessHelper.FetchBlessItem(25, false, (list) =>
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    Items.Clear();
                    if (list != null)
                    {
                        int i = 0;
                        foreach (BlessItem item in list)
                        {
                            item.index = i++;
                            Items.Add(item);
                        }
                    }
                    m_progressIndicatorHelper.PopTask();
                });
            });
        }



        #region 你还真烦啊！到处都少不了你！！！！！
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private void Post_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Lab/LabBlessPostPage.xaml", UriKind.Relative));
        }

    }


    public class BlessTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Left
        {
            get;
            set;
        }

        public DataTemplate Right
        {
            get;
            set;
        }

        
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            BlessItem chatItem = item as BlessItem;
           
            if (chatItem != null)
            {
                int remain = chatItem.index % 2;
                if (remain == 0)
                {                    
                    return Left;
                }
                else
                {                    
                    return Right;
                }   
            }
            return base.SelectTemplate(item, container);
        }
    }
}