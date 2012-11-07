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
    public partial class CharactorAnalysisWrapper : PhoneApplicationPage
    {
        #region LogoSourceProperty
        public static readonly DependencyProperty LogoSourceProperty =
            DependencyProperty.Register("LogoSource", typeof(string), typeof(CharactorAnalysisWrapper), new PropertyMetadata((string)""));

        public string LogoSource
        {
            get { return (string)GetValue(LogoSourceProperty); }
            set { SetValue(LogoSourceProperty, value); }
        }
        #endregion

        #region NameProperty
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(CharactorAnalysisWrapper), new PropertyMetadata((string)""));

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        #endregion

        int side1;
        int side2;
        int side3;
        int side4;
        int side5;

        string m_myName ="";
        string m_herName = "";
        string m_award = "";

        public CharactorAnalysisWrapper()
        {
            InitializeComponent(); 
            this.Loaded += new RoutedEventHandler(Page_Loaded);
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Name = MiscTool.GetHerName();
            LogoSource = MiscTool.GetHerIconUrl();
            Refresh();
        }

        private void Refresh()
        {
            bool bReturn = GetData();
            if (bReturn)
            {
                ContentPanel.Children.Clear();
                ContentPanel.Children.Add(new CharactorAnalysis(side1, side2, side3, side4, side5));
            }
          
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
            Analysis();
            return true;
        }

        private void Analysis()
        {
            uint herN = 0;
            char[] herArray = m_herName.ToCharArray();
            foreach (char c in herArray)
            {
                uint n = (uint)c;
                herN += n;
            }
            side1 = (int) (herN * 575 % 50 + 50);
            side2 = (int) (herN * herN % 50 + 50);
            side3 = (int) (herN * 250 % 50 + 50);
            side4 = (int) (herN * 337 % 50 + 50);
            side5 = (int) (herN * 702 % 50 + 50);
            if (side1 >= side2 && side1 >= side3 && side1 >= side4 && side1 >= side5)
                m_award = "极品萝莉";
            else if (side2 >= side1 && side2 >= side3 && side2 >= side4 && side2 >= side5)
                m_award = "盖世女王";
            else if (side3 >= side1 && side3 >= side2 && side3 >= side4 && side3 >= side5)
                m_award = "超萌天然呆";
            else if (side4 >= side1 && side4 >= side2 && side4 >= side3 && side4 >= side5)
                m_award = "吃货去死去死";
            else if (side5 >= side1 && side5 >= side2 && side5 >= side3 && side5 >= side4)
                m_award = "没有你们这帮伪娘世界早就清静了";
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
                if (String.IsNullOrEmpty(m_award))
                    m_award = "这是各种bug乱入的节奏么？";
                sentence.Append(string.Format(
                    "@{0} 获得了成就【{1}】",
                     hername, m_award));
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