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

namespace Care.Views.Preference
{
    public partial class SetFreshItemCount : PhoneApplicationPage, INotifyPropertyChanged
    {
        public String SinaWeibo_RecentCount
        {
            get
            {
                string pre =  PreferenceHelper.GetPreference("SinaWeibo_RecentCount");
                if (string.IsNullOrEmpty(pre))
                {
                    return "30";
                }
                return pre;
            }
            set
            {
                PreferenceHelper.SetPreference("SinaWeibo_RecentCount", value);
                NotifyPropertyChanged("SinaWeibo_RecentCount");
            }
        }

        public String Renren_RecentCount
        {
            get
            {
                string pre = PreferenceHelper.GetPreference("Renren_RecentCount");
                if (string.IsNullOrEmpty(pre))
                {
                    return "30";
                }
                return pre;
            }
            set
            {
                PreferenceHelper.SetPreference("Renren_RecentCount", value);
                NotifyPropertyChanged("Renren_RecentCount");
            }
        }

        private static String[] source = { "20", "30", "50" };
        private bool m_bInitFinished = false;

        public SetFreshItemCount()
        {
            this.DataContext = this;
            InitializeComponent();
            sinaPicker.ItemsSource = new List<String>(source);
            renrenPicker.ItemsSource = new List<String>(source);
            doubanPicker.ItemsSource = new List<String>(source);   
            this.Loaded += new RoutedEventHandler(Page_Loaded);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {                     
            int sinaIndex = ValueToIndex(PreferenceHelper.GetPreference("SinaWeibo_RecentCount"));
            sinaPicker.SelectedIndex = sinaIndex;

            int renrenIndex = ValueToIndex(PreferenceHelper.GetPreference("Renren_RecentCount"));
            renrenPicker.SelectedIndex = renrenIndex;
            m_bInitFinished = true;
        }

        private int ValueToIndex(string value)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (value == source[i])
                    return i;
            }
            // 失败了返回0，这里其实是默认值的说~
            return 0;
        }

        private String IndexToValue(int index)
        {
            if(index >= 0 && index < source.Length )
                return source[index];
            // 失败了返回20
            return source[0];
        }

        // TODO: 其实应该加上限制，比如新浪好像最多一次50条
        // 最好是换成下拉列表控件
        private void Confirm_Click(object sender, EventArgs e)
        {       
            NavigationService.GoBack();
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

        private void SinaSelection_Changed(object sender, SelectionChangedEventArgs e)
        {   
            int index = sinaPicker.SelectedIndex;
            String recentCount = IndexToValue(index);
            // Selection_Changed会比Init发生得早，若此时SerPreference
            // Init过程中得到的那个Preference就被覆盖了
            if (m_bInitFinished)
            {
                PreferenceHelper.SetPreference("SinaWeibo_RecentCount", recentCount);
            }         
        }

        private void RenrenSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            int index = renrenPicker.SelectedIndex;
            String recentCount = IndexToValue(index);
            // Selection_Changed会比Init发生得早，若此时SerPreference
            // Init过程中得到的那个Preference就被覆盖了
            if (m_bInitFinished)
            {
                PreferenceHelper.SetPreference("Renren_RecentCount", recentCount);
            }            
        }

        private void DoubanSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            int index = doubanPicker.SelectedIndex;
            String doubanCount = IndexToValue(index);
            // Selection_Changed会比Init发生得早，若此时SerPreference
            // Init过程中得到的那个Preference就被覆盖了
            if (m_bInitFinished)
            {
                PreferenceHelper.SetPreference("Douban_RecentCount", doubanCount);
            }
        }        
    }
}