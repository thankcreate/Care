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
    public partial class EditPassWord : PhoneApplicationPage, INotifyPropertyChanged
    {

        #region 无聊的Get&Set
        public String _OriginalPassword;
        public String OriginalPassword
        {
            get
            {
                return _OriginalPassword;
            }
            set
            {
                _OriginalPassword = value;
                NotifyPropertyChanged("OriginalPassword");
            }
        }

        public String _NewPassword;
        public String NewPassword
        {
            get
            {
                return _NewPassword;
            }
            set
            {
                _NewPassword = value;
                NotifyPropertyChanged("NewPassword");
            }
        }

        public String _ConfrimPassword;
        public String ConfrimPassword
        {
            get
            {
                return _ConfrimPassword;
            }
            set
            {
                _ConfrimPassword = value;
                NotifyPropertyChanged("ConfrimPassword");
            }
        }
        #endregion

        private bool IsNeverSetBefore = false;
        public EditPassWord()
        {
            DataContext = this;
            OriginalPassword = "123";
            ConfrimPassword = "123";
            NewPassword = "123";
            InitializeComponent();
            if (string.IsNullOrEmpty(PreferenceHelper.GetPreference("Global_Password")))
            {
                txtOriginal.Visibility = Visibility.Collapsed;
                textBlockOriginal.Visibility = Visibility.Collapsed;
                IsNeverSetBefore = true;
            }
            else
            {
                txtOriginal.Visibility = Visibility.Visible;
                textBlockOriginal.Visibility = Visibility.Visible;
                IsNeverSetBefore = true;
            }
        }

        private void Confirm_Click(object sender, EventArgs e)
        {

            if (!IsNeverSetBefore && txtOriginal.Text != PreferenceHelper.GetPreference("Global_Password"))
            {
                MessageBox.Show("原始密码输入错误");
                return;
            }
            if (txtNew.Text != txtConfirm.Text)
            {
                MessageBox.Show("两次密码输入不一致");
                return;
            }
            if (txtNew.Text.Length < 4)
            {
                MessageBox.Show("新密码至少4位");
                return;
            }
            PreferenceHelper.SetPreference("Global_Password", txtNew.Text);
            PreferenceHelper.SetPreference("Global_UsePassword", "True");
            // refresh the butotn state
            App.ViewModel.UsingPassword = "True";
            NavigationService.GoBack();            
        }

        private void Close_Click(object sender, EventArgs e)
        {
            App.ViewModel.UsingPassword = "False";
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

    }
}