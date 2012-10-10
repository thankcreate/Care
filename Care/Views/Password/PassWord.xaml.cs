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

namespace Care.Views
{
    public partial class PassWord : PhoneApplicationPage, INotifyPropertyChanged
    {
        // 真实的密码
        private string RealPassWord;
        private bool IsDeletePasswordPage;

        // 输入框中的密码
        public string _InputPassWord;
        public string InputPassWord
        {
            get
            {
                return _InputPassWord;
            }
            set
            {
                if (value.Length > 6)
                {
                    return;
                }
                _InputPassWord = value;
                NotifyPropertyChanged("InputPassWord");
            }
        }
        public PassWord()
        {
            DataContext = this;
            IsDeletePasswordPage = false;
            RealPassWord = PreferenceHelper.GetPreference("Global_Password");            
            InitializeComponent();

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

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            int length = InputPassWord.Length;
            if(length > 0)
            {
                InputPassWord = InputPassWord.Substring(0, InputPassWord.Length - 1);
            }      
        }

        private void buttonUnlock_Click(object sender, RoutedEventArgs e)
        {
            if(RealPassWord == InputPassWord)
            {
                if (IsDeletePasswordPage)
                {
                    App.ViewModel.UsingPassword = "False";
                    PreferenceHelper.RemovePasswordPreference();
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative)); 
                }
                else
                {
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));    
                }
                  
            }
            else 
            {
                MessageBox.Show("窥视别人的隐私是不对的哦，亲~");
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IDictionary<string, string> queryString = this.NavigationContext.QueryString;
            if (queryString.ContainsKey("Type"))
            {
                String type = queryString["Type"];
                if (type == "DeletePassword")
                {
                    IsDeletePasswordPage = true;
                }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            InputPassWord += "1";
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            InputPassWord += "2";
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            InputPassWord += "3";
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            InputPassWord += "4";
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            InputPassWord += "5";
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            InputPassWord += "6";
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            InputPassWord += "7";
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            InputPassWord += "8";
        }

        private void button9_Click(object sender, RoutedEventArgs e)
        {
            InputPassWord += "9";
        }

        private void button0_Click(object sender, RoutedEventArgs e)
        {
            InputPassWord += "0";
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (RealPassWord == InputPassWord)
            {
                if (IsDeletePasswordPage)
                {
                    App.ViewModel.UsingPassword = "False";
                    PreferenceHelper.RemovePasswordPreference();
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative)); 
                }
                else
                {
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }
            }
        }

       
    }
}