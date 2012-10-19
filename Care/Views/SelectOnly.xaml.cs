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
        public static string SHOWTYPE_NEWS = "动态";
        public static string SHOWTYPE_PICTURES = "照片";
        public static string DATASOURCE_SINAWEIBO = "新浪微博";
        public static string DATASOURCE_RSS = "RSS源";
        public static string DATASOURCE_DOUBAN = "豆瓣社区";
        public static string DATASOURCE_RENREN = "人人网";
        public static string DATASOURCE_ALL = "全部";

        LoopingArrayDataSource<string> sourceLeft;
        LoopingArrayDataSource<string> sourceRight;       

        public SelectOnly()
        {
            sourceLeft = new LoopingArrayDataSource<string>(new string[] { SHOWTYPE_NEWS, SHOWTYPE_PICTURES }, 0);
            sourceRight = new LoopingArrayDataSource<string>(new string[] { DATASOURCE_SINAWEIBO, DATASOURCE_RSS, DATASOURCE_DOUBAN, DATASOURCE_RENREN, DATASOURCE_ALL }, 0);
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(PageLoaded);
        }


        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            selectorLeft.DataSource = sourceLeft;
            selectorRight.DataSource = sourceRight;
        }

        private void Confirm_Click(object sender, EventArgs e)
        {
            // 这里showType实际上是显示的String值，不是索引值之类的
            string target = "/MainPage.xaml";
            string showType = sourceLeft.SelectedItem as String;
            string dataSource = sourceRight.SelectedItem as String;
            target += string.Format("?ShowType={0}&DataSource={1}", showType, dataSource);
            App.ViewModel.IsChanged = true;
            NavigationService.Navigate(new Uri(target,UriKind.Relative));

        }
        private void Close_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

    }
}