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
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Media.Imaging;
using Care.Tool;
using System.Text;

namespace Care.Views.Lab
{
    public partial class PotentialEnemyWrapper : PhoneApplicationPage
    {
        #region LogoSourceProperty
        public static readonly DependencyProperty LogoSourceProperty =
            DependencyProperty.Register("LogoSource", typeof(string), typeof(PotentialEnemyWrapper), new PropertyMetadata((string)""));

        public string LogoSource
        {
            get { return (string)GetValue(LogoSourceProperty); }
            set { SetValue(LogoSourceProperty, value); }
        }
        #endregion

        #region NameProperty
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(PotentialEnemyWrapper), new PropertyMetadata((string)""));

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        #endregion

        private string HerID;
        private EntryType m_type;

        private string name1 = "";
        private string name2 = "";
        private string name3 = "";

        private string id1 = "";
        private string id2 = "";
        private string id3 = "";


        private int value1 = 0;
        private int value2 = 0;
        private int value3 = 0;

        List<CommentMan> m_listMan;
        Dictionary<String, String> m_mapNameToID;
        Dictionary<String, int> m_mapMan;

        private ProgressIndicatorHelper m_progressIndicatorHelper;

        public PotentialEnemyWrapper()
        {
            m_mapMan = new Dictionary<String, int>();
            m_mapNameToID = new Dictionary<string, string>();
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Page_Loaded);
            Microsoft.Phone.Shell.SystemTray.ProgressIndicator = new Microsoft.Phone.Shell.ProgressIndicator();
            m_progressIndicatorHelper = new ProgressIndicatorHelper(Microsoft.Phone.Shell.SystemTray.ProgressIndicator, () => { });      
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Shell.SystemTray.ProgressIndicator = new Microsoft.Phone.Shell.ProgressIndicator();
            m_progressIndicatorHelper = new ProgressIndicatorHelper(Microsoft.Phone.Shell.SystemTray.ProgressIndicator, () => { });

            Refersh(m_type);
        }

        private void Refersh(EntryType type)
        {
            Name = MiscTool.GetHerName();
            LogoSource = MiscTool.GetHerIconUrl();
            m_progressIndicatorHelper.PushTask();
            BaseFetcher fetcher;

            switch (type)
            {
                case EntryType.SinaWeibo:
                    fetcher = new SinaWeiboFetcher();
                    LogoSource = PreferenceHelper.GetPreference("SinaWeibo_FollowerAvatar2");
                    Name = PreferenceHelper.GetPreference("SinaWeibo_FollowerNickName");
                    HerID = PreferenceHelper.GetPreference("SinaWeibo_FollowerID");
                    break;
                case EntryType.Renren:
                    fetcher = new RenrenFetcher();
                    LogoSource = PreferenceHelper.GetPreference("Renren_FollowerAvatar2");
                    Name = PreferenceHelper.GetPreference("Renren_FollowerNickName");
                    HerID = PreferenceHelper.GetPreference("Renren_FollowerID");
                    break;
                case EntryType.Douban:
                    fetcher = new DoubanFetcher();
                    LogoSource = PreferenceHelper.GetPreference("Douban_FollowerAvatar");
                    Name = PreferenceHelper.GetPreference("Douban_FollowerNickName");
                    HerID = PreferenceHelper.GetPreference("Douban_FollowerID");
                    break;
                default:
                    fetcher = SelectDefaultFetcher();
                    break;
            }
            if (fetcher == null)
            {                
                m_progressIndicatorHelper.PopTask();                
                return;
            }
            fetcher.FetchCommentManList((List<CommentMan> list) =>
            {
                m_listMan = list;
                if (list == null)
                {
                    m_progressIndicatorHelper.PopTask();
                    return;
                }
                GetData();
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    ContentPanel.Children.Clear();
                    ContentPanel.Children.Add(new PotentialEnemy(name1, value1, name2, value2, name3, value3));
                });
                m_progressIndicatorHelper.PopTask();
            }); 

        }       

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IDictionary<string, string> queryString = this.NavigationContext.QueryString;

            if (queryString.ContainsKey("Type"))
            {
                try
                {
                    m_type = (EntryType)Enum.Parse(typeof(EntryType), queryString["Type"], true);
                }
                catch (System.Exception ex)
                {                	
                }                
            }
            //Refersh(m_type);
        }
        private void refresh_click(object sender, EventArgs e)
        {
            Refersh(m_type);
        }

        private void GetData()
        {
            ConvertListToMap();
            GetTop3();
        }

        private void ConvertListToMap()
        {
            m_mapMan.Clear();
            foreach (CommentMan man in m_listMan)
            {
                if (m_mapMan.ContainsKey(man.name))
                {
                    m_mapMan[man.name]++;
                }
                else
                {
                    m_mapMan.Add(man.name, 1);
                }

                if (!m_mapNameToID.ContainsKey(man.name))
                {
                    m_mapNameToID.Add(man.name, man.id);
                }
            }
        }

        private void GetTop3()
        {
            var sortedDict = 
                (from entry in m_mapMan
                orderby entry.Value
                descending
                select entry.Key).Take(3);
            String[] nameTop3 = sortedDict.ToArray();
            if (nameTop3.Length == 3)
            {
                name1 = nameTop3[0];
                name2 = nameTop3[1];
                name3 = nameTop3[2];

                id1 = m_mapNameToID[name1];
                id2 = m_mapNameToID[name2];
                id3 = m_mapNameToID[name3];

                value1 = m_mapMan[name1];
                value2 = m_mapMan[name2];
                value3 = m_mapMan[name3];
            }
            else if (nameTop3.Length == 2)
            {
                name1 = nameTop3[0];
                name2 = nameTop3[1];

                id1 = m_mapNameToID[name1];
                id2 = m_mapNameToID[name2];

                value1 = m_mapMan[name1];
                value2 = m_mapMan[name2];
            }
            else if (nameTop3.Length == 1)
            {
                name1 = nameTop3[0];

                id1 = m_mapNameToID[name1];                

                value1 = m_mapMan[name1];
            } 
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
                // 人人的@方式不一样，需要在名字后面带ID
                if (m_type == EntryType.Renren)
                {
                    sentence.Append(string.Format(
                    "收取了可观小的小费后，酒馆老板小声道："
                    + "看在你对@{0}({1}) 一片痴情的份上，我可以告诉你@{2}({3}) 似乎在做些小动作，而@{4}({5}) 更值得注意，当然了，比起@{6}({7}) 可就略逊一筹了~~",
                     Name, HerID, name3, id3, name2, id2, name1, id1));
                }
                else
                {
                    sentence.Append(string.Format(
                    "收取了可观小的小费后，酒馆老板小声道："
                    + "看在你对@{0} 一片痴情的份上，我可以告诉你@{1} 似乎在做些小动作，而@{2} 更值得你注意，当然了，比起@{3} 可就略逊一筹了~~",
                     Name, name3, name2, name1));
                }
                StringBuilder sb = new StringBuilder();
                sb.Append("/Views/Common/CommitSelectPage.xaml");
                sb.Append(string.Format("?Content={0}&PicURL={1}", sentence, filename));
                NavigationService.Navigate(new Uri(sb.ToString(), UriKind.Relative));
            }
            catch (Exception)
            {
            }
        }

        private void AnalyseWithSinaWeibo_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(PreferenceHelper.GetPreference("SinaWeibo_FollowerID")))
            {
                MessageBox.Show("尚未设置新浪微博关注对象");                
                return;
            }
            LogoSource = PreferenceHelper.GetPreference("SinaWeibo_FollowerAvatar2");
            m_type = EntryType.SinaWeibo;
            Refersh(EntryType.SinaWeibo);
        }

        private void AnalyseWithRenren_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(PreferenceHelper.GetPreference("Renren_FollowerID")))
            {
                MessageBox.Show("尚未设置人人关注对象");
                
                return;
            }
            LogoSource= PreferenceHelper.GetPreference("Renren_FollowerAvatar2");
            m_type = EntryType.Renren;
            Refersh(EntryType.Renren);
        }

        private void AnalyseWithDouban_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(PreferenceHelper.GetPreference("Douban_FollowerID")))
            {
                MessageBox.Show("尚未设置豆瓣关注对象");                
                return;
            }
            LogoSource = PreferenceHelper.GetPreference("Douban_FollowerAvatar2");
            m_type = EntryType.Douban;
            Refersh(EntryType.Douban);
        }

        private BaseFetcher SelectDefaultFetcher()
        {
            BaseFetcher fetcher = null;
            if(!String.IsNullOrEmpty(PreferenceHelper.GetPreference("SinaWeibo_ID"))
                && !String.IsNullOrEmpty(PreferenceHelper.GetPreference("SinaWeibo_FollowerID"))
                && !String.IsNullOrEmpty(PreferenceHelper.GetPreference("SinaWeibo_Token")))
            {
                LogoSource = PreferenceHelper.GetPreference("SinaWeibo_FollowerAvatar2");
                Name = PreferenceHelper.GetPreference("SinaWeibo_FollowerNickName");
                HerID = PreferenceHelper.GetPreference("SinaWeibo_FollowerID");
                fetcher = new SinaWeiboFetcher();
                m_type = EntryType.SinaWeibo;
            }
            else if (!String.IsNullOrEmpty(PreferenceHelper.GetPreference("Renren_ID"))
                && !String.IsNullOrEmpty(PreferenceHelper.GetPreference("Renren_FollowerID")))
            {
                // 因为人人的avatar2 很可能是不规则的，所以这里用低清的
                LogoSource = PreferenceHelper.GetPreference("Renren_FollowerAvatar");
                Name = PreferenceHelper.GetPreference("Renren_FollowerNickName");
                HerID = PreferenceHelper.GetPreference("Renren_FollowerID");
                fetcher = new RenrenFetcher();
                m_type = EntryType.Renren;
            }
            else if (!String.IsNullOrEmpty(PreferenceHelper.GetPreference("Douban_ID"))
                && !String.IsNullOrEmpty(PreferenceHelper.GetPreference("Douban_FollowerID"))
                && !String.IsNullOrEmpty(PreferenceHelper.GetPreference("Douban_Token")))
            {
                LogoSource = PreferenceHelper.GetPreference("Douban_FollowerAvatar2");
                Name = PreferenceHelper.GetPreference("Douban_FollowerNickName");
                HerID = PreferenceHelper.GetPreference("Renren_FollowerID");
                fetcher = new DoubanFetcher();
                m_type = EntryType.Douban;
            }
            return fetcher;
        }

    }
}