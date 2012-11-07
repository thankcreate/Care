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


namespace Care.Views.Lab
{
    public partial class LovePercentageWrapper : PhoneApplicationPage
    {
        #region LogoSourceProperty
        public static readonly DependencyProperty LogoSourceProperty =
            DependencyProperty.Register("LogoSource", typeof(string), typeof(LovePercentageWrapper), new PropertyMetadata((string)""));

        public string LogoSource
        {
            get { return (string)GetValue(LogoSourceProperty); }
            set { SetValue(LogoSourceProperty, value); }
        }
        #endregion

        #region NameProperty
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(LovePercentageWrapper), new PropertyMetadata((string)""));

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        #endregion

        #region LogoSourceProperty1
        public static readonly DependencyProperty LogoSourceProperty1 =
            DependencyProperty.Register("LogoSource1", typeof(string), typeof(LovePercentageWrapper), new PropertyMetadata((string)""));

        public string LogoSource1
        {
            get { return (string)GetValue(LogoSourceProperty1); }
            set { SetValue(LogoSourceProperty1, value); }
        }
        #endregion

        #region NameProperty1
        public static readonly DependencyProperty NameProperty1 =
            DependencyProperty.Register("Name1", typeof(string), typeof(LovePercentageWrapper), new PropertyMetadata((string)""));

        public string Name1
        {
            get { return (string)GetValue(NameProperty1); }
            set { SetValue(NameProperty1, value); }
        }
        #endregion


        int m_percentage = 50;
        string m_myName = "";
        string m_herName = "";

        public LovePercentageWrapper()
        {
            InitializeComponent(); 
            this.Loaded += new RoutedEventHandler(Page_Loaded);
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Name = MiscTool.GetHerName();
            LogoSource = MiscTool.GetHerIconUrl();
            Name1 = MiscTool.GetMyName();
            LogoSource1 = MiscTool.GetMyIconUrl();
            Refresh();
        }

        private bool GetData()
        {
            m_myName = MiscTool.GetMyName();
            if (String.IsNullOrEmpty(m_myName))
            {
                MessageBox.Show("请至少先登陆一个帐户");
                return false;
            }
            m_herName = MiscTool.GetHerName();
            if (String.IsNullOrEmpty(m_herName))
            {
                MessageBox.Show("请至少先关注她/他的一个帐户");
                return false;
            }            
            m_percentage = AnalysisLovePercentage();
            return true;
        }

       
        private int AnalysisLovePercentage()
        {
            char[] myArray = m_myName.ToCharArray();
            int myN = 0;
            foreach (char c in myArray)
            {
                int n = (int)c;
                myN += n;
            }
            int herN = 0;
            char[] herArray = m_herName.ToCharArray();
            foreach (char c in herArray)
            {
                int n = (int)c;
                herN += n;
            }
            int result = (myN + herN) * 575 % 49 + 50;
            return result;
        }

        private void Refresh()
        {
            GetData();
            ContentPanel.Children.Clear();
            ContentPanel.Children.Add(new LovePercentage(m_percentage));
        }

        private void refresh_click(object sender, EventArgs e)
        {
            Refresh();
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
                String myname = MiscTool.GetMyName();
                String award = "";
                sentence.Append(string.Format(
                    "经某不靠谱的分析仪测算，@{0} 与@{1} 的姻缘指数达到惊人的{2}。去死去死团众，不管你们信不信，我反正不信了",
                     hername, myname, m_percentage));
                StringBuilder sb = new StringBuilder();
                sb.Append("/Views/Common/CommitSelectPage.xaml");
                sb.Append(string.Format("?Content={0}&PicURL={1}", sentence, filename));
                NavigationService.Navigate(new Uri(sb.ToString(), UriKind.Relative));
            }
            catch (Exception)
            {
            }
        }
    }
}