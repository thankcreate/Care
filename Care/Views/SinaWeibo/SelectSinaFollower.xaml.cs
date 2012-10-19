using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WeiboSdk;
using Hammock;
using Hammock.Web;
using System.Runtime.Serialization.Json;
using System.IO.IsolatedStorage;
using Care.Tool;
using System.Collections;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Care
{
    public partial class SelectSinaFollower : PhoneApplicationPage
    {
        private int m_nPrevois = -1;
        private int m_nNext = -1;
        private int m_nTotalNumber = -1;
        private List<int> m_listPrevios;
        private int m_nCurrentIndex = 0;

        public ObservableCollection<User> Friends { get; private set; }
        public delegate void FetchAllFriendsCallback();
        // 存储了所有朋友的列表，方便查询时使用
        public List<User> AllFriends { get; private set; }

        private ProgressIndicatorHelper m_progressIndicatorHelper;

        public SelectSinaFollower()
        {
            m_listPrevios = new List<int>();

            Friends = new ObservableCollection<User>();
            AllFriends = new List<User>();
            InitializeComponent();
            DataContext = this;
            this.Loaded += new RoutedEventHandler(SelectSinaFollower_Loaded);
        }

        private void SelectSinaFollower_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Shell.SystemTray.ProgressIndicator = new Microsoft.Phone.Shell.ProgressIndicator();
            m_progressIndicatorHelper = new ProgressIndicatorHelper(Microsoft.Phone.Shell.SystemTray.ProgressIndicator, () => { });
            
            GetFriendList(-1);
        }

        

        private SdkCmdBase cmdBase;
        private SdkNetEngine netEngine;

        private void GetFriendList(int cursor)
        {
            m_progressIndicatorHelper.PushTask();
            if (string.IsNullOrEmpty(App.ViewModel.SinaWeiboAccount.id))
                return;
            // Define a new net engine
            netEngine = new SdkNetEngine();
            // Define a new command base
            cmdBase = new SdkCmdBase
            {
                acessToken = App.SinaWeibo_AccessToken,
            };
            RestRequest request = new RestRequest();
            request.Method = WebMethod.Get;
            request.Path = "friendships/friends.json";
            request.AddParameter("access_token", App.SinaWeibo_AccessToken);
            request.AddParameter("uid", App.ViewModel.SinaWeiboAccount.id);
            if (cursor != -1)
            {
                request.AddParameter("cursor", cursor.ToString());
            }            

            // use my uid to get the friends list.
            netEngine.SendRequest(request, cmdBase, (SdkResponse response) =>
            {
                if (response.errCode == SdkErrCode.SUCCESS)
                {
                    Friends friends;
                    try
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Friends));
                        friends = ser.ReadObject(response.stream) as Friends;
                        m_nPrevois = int.Parse(friends.previous_cursor);
                        if (m_nCurrentIndex > m_listPrevios.Count - 1)
                        {
                            m_listPrevios.Add(m_nPrevois);
                        }
                        else
                        {
                            m_listPrevios[m_nCurrentIndex] = m_nPrevois;
                        }                      

                        m_nNext = int.Parse(friends.next_cursor);
                        m_nTotalNumber = int.Parse(friends.total_number);
                        
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            m_progressIndicatorHelper.PopTask();
                            Friends.Clear();
                            foreach (User friend in friends.users)
                            {
                                //App.ViewModel.Friends.Add(friend);
                                Friends.Add(friend);
                            }     
                            ScrollViewer v = VisualTreeHelper.GetChild(this.ResultListBox, 0) as ScrollViewer;
                            v.ScrollToVerticalOffset(0);                            
                        }
                        );
                    }
                    catch (Exception)
                    {
                        m_progressIndicatorHelper.PopTask();
                        throw;
                    }   
                }
                else
                {
                    m_progressIndicatorHelper.PopTask();
                }
            });
        }
        

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string searchName = nameBox.Text;
            //App.ViewModel.SinaWeiboCareID = nameBox.Text;
            //App.ViewModel.IsChanged = true;
            //PreferenceHelper.SetPreference("SinaWeibo_FollowerID", nameBox.Text);
            //PreferenceHelper.SavePreference();

            GetFriendListIterator(-1, () =>
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    Friends.Clear();
                    if (AllFriends.Count == 0)
                    {
                        MessageBox.Show("未能搜索到任何结果");
                        return;
                    }
                    foreach (User user in AllFriends)
                    {
                        if (user.name.ToLower().Contains(searchName.ToLower()))
                        {
                            Friends.Add(user);
                        }
                    }
                });                
            });
            //refreshFollowerSinaAccountInfo();
        }

        private void GetFriendListIterator(int cursor, FetchAllFriendsCallback callback)
        {
            m_progressIndicatorHelper.PushTask();
            if (string.IsNullOrEmpty(App.ViewModel.SinaWeiboAccount.id))
            {
                m_progressIndicatorHelper.PopTask();
                return;
            }
            // Define a new net engine
            netEngine = new SdkNetEngine();
            // Define a new command base
            cmdBase = new SdkCmdBase
            {
                acessToken = App.SinaWeibo_AccessToken,
            };
            RestRequest request = new RestRequest();
            request.Method = WebMethod.Get;
            request.Path = "friendships/friends.json";
            request.AddParameter("access_token", App.SinaWeibo_AccessToken);
            request.AddParameter("uid", App.ViewModel.SinaWeiboAccount.id);
            // curosr = -1 说明是第一次进来
            if (cursor != -1)
            {
                request.AddParameter("cursor", cursor.ToString());
            }
            else
            {
                AllFriends.Clear();
            }

            // use my uid to get the friends list.
            netEngine.SendRequest(request, cmdBase, (SdkResponse response) =>
            {
                if (response.errCode == SdkErrCode.SUCCESS)
                {
                    Friends friends;
                    try
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Friends));
                        friends = ser.ReadObject(response.stream) as Friends;                        
                        int next = int.Parse(friends.next_cursor);
                        foreach (User friend in friends.users)
                        {                          
                            AllFriends.Add(friend);
                        }
                        // 如果还没有遍历完，则继续遍历
                        if (next != 0)
                        {
                            GetFriendListIterator(next, callback);  
                        }
                        // 遍历完后执行后续回调（其实就是从得到的名单中搜索，刷新UI)
                        else
                        {
                            callback();
                        }
                        m_progressIndicatorHelper.PopTask();
                    }
                    catch (Exception)
                    {
                        m_progressIndicatorHelper.PopTask();
                        throw;
                    }
                }
                else
                {
                    m_progressIndicatorHelper.PopTask();
                }
            });
        }


        // 得到关注用户信息
        private void refreshFollowerSinaAccountInfo()
        {
            // Define a new net engine
            SdkNetEngine netEngine = new SdkNetEngine();

            // Define a new command base
            SdkCmdBase cmdBase = new SdkCmdBase
            {
                acessToken = App.SinaWeibo_AccessToken,
            };
            RestRequest request = new RestRequest();
            request.Method = WebMethod.Get;

            request.Path = "/users/show.json";
            request.AddParameter("access_token", App.SinaWeibo_AccessToken);
            request.AddParameter("uid", PreferenceHelper.GetPreference("SinaWeibo_FollowerID"));
            netEngine.SendRequest(request, cmdBase, (SdkResponse e1) =>
            {
                if (e1.errCode == SdkErrCode.SUCCESS)
                {
                    User user = null;
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(User));
                    user = ser.ReadObject(e1.stream) as User;
                    String nickName = user.screen_name;
                    PreferenceHelper.SetPreference("SinaWeibo_FollowerNickName", nickName);
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        NavigationService.Navigate(new Uri("/Views/SinaWeibo/SinaAcount.xaml", UriKind.Relative));
                    });
                }
                else
                {
                    PreferenceHelper.RemovePreference("SinaWeibo_FollowerNickName");
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            MessageBox.Show("此人不存在");
                        });
                        NavigationService.Navigate(new Uri("/Views/SinaWeibo/SinaAcount.xaml", UriKind.Relative));
                    });
                }
            });
        }

        private void Help_Click(object sender, EventArgs e)
        {    
            MessageBox.Show("由于渣浪暂不开放全局搜索API，目前只能在当前用户的关注者范围内搜索 T_T..");
        }

        private void ListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ResultListBox.SelectedIndex;
            User item = Friends[index];

            String prefID = PreferenceHelper.GetPreference("SinaWeibo_FollowerID");
            if (prefID != item.id)
            {
                PreferenceHelper.SetPreference("SinaWeibo_FollowerID", item.id);
                PreferenceHelper.SetPreference("SinaWeibo_FollowerNickName", item.name);
                PreferenceHelper.SetPreference("SinaWeibo_FollowerAvatar", item.profile_image_url);
                PreferenceHelper.SetPreference("SinaWeibo_FollowerAvatar2", item.avatar_large);
                PreferenceHelper.SavePreference();

                App.ViewModel.IsChanged = true;
            }
           
            NavigationService.Navigate(new Uri("/Views/SinaWeibo/SinaAcount.xaml", UriKind.Relative));
        }

        private void Previos_Click(object sender, EventArgs e)
        {
            if (m_nPrevois == -1)
            {
                return;
            }
            if (m_nPrevois == 0)
            {
                MessageBox.Show("已经是第一页");
                return;
            }
            m_nCurrentIndex -= 1;
            GetFriendList(m_listPrevios[m_nCurrentIndex]);
        }

        private void Next_Click(object sender, EventArgs e)
        {
            if (m_nNext == -1)
            {
                return;
            }
            if (m_nNext == 0)
            {
                MessageBox.Show("已经是最后一页");
                return;
            }
            m_nCurrentIndex += 1;
            GetFriendList(m_nNext);
        }

      

    }
}