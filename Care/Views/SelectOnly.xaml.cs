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
using Care;
namespace Care.Views
{
    public partial class SelectOnly : PhoneApplicationPage
    {
        public SelectOnly()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(PageLoaded);
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {       
            selector.DataSource = new Care.Views.IntLoopingDataSource() { MinValue = 1, MaxValue = 60, SelectedItem = 1 };
        }
    }
}