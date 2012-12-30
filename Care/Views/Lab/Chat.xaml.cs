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
using Care.ViewModels;
using System.Collections.ObjectModel;
using Care.Tool;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.Text;
using Microsoft.Phone.Controls;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Threading;

namespace Care.Views.Lab
{
  
    public partial class Chat : PhoneApplicationPage, INotifyPropertyChanged
    {
        private String[] herSentece = { "^_^ 然后呢?", "呵呵..", "嗯嗯，这样~~" };
        public ObservableCollection<ChatItemViewModel> Items { get; private set; }
        public Chat()
        {
            InitializeComponent();
            Items = new ObservableCollection<ChatItemViewModel>();
            this.DataContext = this;            
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


        bool m_bHaveInited = false;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_bHaveInited)
            {                
                return;
            }
            m_bHaveInited = true;
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            ObservableCollection<ChatItemViewModel> outItem = null;
            if (!settings.TryGetValue<ObservableCollection<ChatItemViewModel>>("Global_ChatRecord", out outItem))
            {
                ChatItemViewModel item1 = new ChatItemViewModel();
                item1.Icon = MiscTool.GetHerIconUrl();
                item1.Title = MiscTool.GetHerName();
                item1.Text = "^_^";
                item1.Type = "Her";
                Items.Add(item1);
            }
            else
            {
                if (outItem != null && outItem.Count > 0)
                {
                    foreach (ChatItemViewModel model in outItem)
                    {
                        Items.Add(model);
                    }
                    ScrollViewer v = VisualTreeHelper.GetChild(this.MainList, 0) as ScrollViewer;
                    v.ScrollToVerticalOffset(100000);
                }
            }
        }

        private void send_click(object sender, RoutedEventArgs e)
        {
            ChatItemViewModel item1 = new ChatItemViewModel();
            item1.Icon = MiscTool.GetMyIconUrl();
            item1.Title = MiscTool.GetMyName();
            item1.Text = txtInput.Text;
            item1.Type = "Me";
            Items.Add(item1);
            txtInput.Text = "";
            ScrollViewer v = VisualTreeHelper.GetChild(this.MainList, 0) as ScrollViewer;
            v.ScrollToVerticalOffset(100000);

            ChatItemViewModel item2 = new ChatItemViewModel();
            Perform(() =>
            {                
                item2.Icon = MiscTool.GetHerIconUrl();
                item2.Title = MiscTool.GetHerName();
                item2.Text = ".";   
                item2.Type = "Her";
                Items.Add(item2);
                ScrollViewer v1 = VisualTreeHelper.GetChild(this.MainList, 0) as ScrollViewer;
                v1.ScrollToVerticalOffset(100000); 
            }, 400);

            Perform(() =>
            {
                item2.Text = "..";           
            }, 800);
            Perform(() =>
            {
                item2.Text = "...";
            }, 1200);
            Perform(() =>
            {
                Random random = new Random();
                int index = random.Next(herSentece.Length);
                item2.Text = herSentece[index];
            }, 1600);
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
                String myname = MiscTool.GetMyName();
                String award = "";
                sentence.Append(string.Format(
                    "我活了20多年，@{0} 是我见过最无聊的一个",
                     myname));
                StringBuilder sb = new StringBuilder();
                sb.Append("/Views/Common/CommitSelectPage.xaml");
                sb.Append(string.Format("?Content={0}&PicURL={1}", sentence, filename));
                NavigationService.Navigate(new Uri(sb.ToString(), UriKind.Relative));
            }
            catch (Exception)
            {
            }
        }

        private void Page_UnLoaded(object sender, RoutedEventArgs e)
        {

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (!settings.Contains("Global_ChatRecord"))
            {
                settings.Add("Global_ChatRecord", Items);
            }
            else
            {
                settings["Global_ChatRecord"] = Items;
            }        
        }

        private void Perform(Action myMethod, int delayInMilliseconds)
        {
            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (s, e) => Thread.Sleep(delayInMilliseconds);

            worker.RunWorkerCompleted += (s, e) => myMethod.Invoke();

            worker.RunWorkerAsync();
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                send_click(null, null);
            }
        }
    }


    public abstract class DataTemplateSelector : ContentControl
    {
        public virtual DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return null;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            ContentTemplate = SelectTemplate(newContent, this);
        }
    }

    public class ChatTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Her
        {
            get;
            set;
        }

        public DataTemplate Me
        {
            get;
            set;
        }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ChatItemViewModel chatItem = item as ChatItemViewModel;
            if (chatItem != null)
            {
                if (chatItem.Type == "Her")
                {
                    return Her;
                }
                else if (chatItem.Type == "Me")
                {
                    return Me;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}