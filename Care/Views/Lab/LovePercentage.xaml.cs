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
namespace Care.Views.Lab
{
    public partial class LovePercentage : PhoneApplicationPage, INotifyPropertyChanged
    {
        int _value1;
        public int Value1
        {
            get
            {
                return _value1;
            }
            set
            {
                _value1 = value;
                NotifyPropertyChanged("Value1");
            }
        }

        public LovePercentage()
        {
            InitializeComponent();
        }


        public LovePercentage(int input)
        {
            Value1 = input;
            this.DataContext = this;
            InitializeComponent();
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
    }
}