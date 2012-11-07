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
using Care.Tool;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.Text;

namespace Care.Views.Lab.Test
{
    public partial class TimeSpanWrapper : PhoneApplicationPage
    {
        #region LogoSourceProperty
        public static readonly DependencyProperty LogoSourceProperty =
            DependencyProperty.Register("LogoSource", typeof(string), typeof(TimeSpanWrapper), new PropertyMetadata((string)""));

        public string LogoSource
        {
            get { return (string)GetValue(LogoSourceProperty); }
            set { SetValue(LogoSourceProperty, value); }
        }
        #endregion

        #region NameProperty
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(TimeSpanWrapper), new PropertyMetadata((string)""));

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        #endregion

        int param1 = 0;
        int param2 = 0;
        int param3 = 0;
        int param4 = 0;
        int max = 10;

        public TimeSpanWrapper()
        {            
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Page_Loaded);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Name = MiscTool.GetHerName();
            LogoSource = MiscTool.GetHerIconUrl();
            GetData();
            ContentPanel.Children.Clear();
            ContentPanel.Children.Add(new TimeSpan(param1, param2, param3, param4, max));            
        }



        private void GetData()
        {
             foreach (ItemViewModel item in App.ViewModel.Items)
            {
                int hour = item.TimeObject.Hour;
                if( hour >= 8 && hour  < 12  )
                {
                    param1++;
                }
                else if (hour >= 12 && hour < 18)
                {
                    param2++;
                }
                else if (hour >= 18 && hour < 24)
                {
                    param3++;
                }
                else if (hour >= 0 && hour < 8)
                {
                    param4++;
                }
                int big1 = param1 > param2 ? param1 : param2;
                int big2 = param3 > param4 ? param3 : param4;
                max = big1 > big2 ? big1 : big2;
            }
        }        
 
        private void refresh_click(object sender, EventArgs e)
        {
            GetData();
            ContentPanel.Children.Clear();
            ContentPanel.Children.Add(new TimeSpan(param1, param2, param3, param4, max));
        }
        private void share_Click(object sender, EventArgs e)
        {
            var ui = Application.Current.RootVisual;
            string filename = "";
            try
            {
                if (ui != null)
                {
                    FrameworkElement fe = ui as FrameworkElement;
                    if (fe != null)
                    {
                        var width = fe.ActualWidth;
                        var height = fe.ActualHeight;

                        WriteableBitmap wb = new WriteableBitmap(ui,
                            new TranslateTransform());
                        wb.Render(ui, new TranslateTransform());
                        byte[] bb = MiscTool.EncodeToJpeg(wb);

                        filename =
                             DateTime.Now.Ticks
                            + ".jpg";
                        IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
                        using (var st = isf.CreateFile(filename))
                        {
                            st.Write(bb, 0, bb.Length);
                        }
                    }
                }

                StringBuilder sentence = new StringBuilder();
                String hername = MiscTool.GetHerName();
                String award = GenerateAward();
                sentence.Append(string.Format(
                    "@{0} 获得了成就【{1}】",
                     hername, award));
                StringBuilder sb = new StringBuilder();
                sb.Append("/Views/Common/CommitSelectPage.xaml");
                sb.Append(string.Format("?Content={0}&PicURL={1}", sentence, filename));
                NavigationService.Navigate(new Uri(sb.ToString(), UriKind.Relative));
            }
            catch (Exception)
            {
            }
        }

        private String GenerateAward()
        {
            if (param1 >= param2 && param1 >= param3 && param1 >= param4)
                return "这么正常的活动规律我要是你我都不好意思出门";
            else if (param2 >= param1 && param2 >= param3 && param2 >= param4)
                return "睡完午觉就无所事事的家伙";
            else if (param3 >= param1 && param3 >= param2 && param2 >= param4)
                return "月色下的呤游者";
            else if (param4 >= param1 && param4 >= param2 && param4 >= param3)
                return "程序员";

            return "你妹的程序出BUG了啊~~~~~~~~~";
        }
    }
}