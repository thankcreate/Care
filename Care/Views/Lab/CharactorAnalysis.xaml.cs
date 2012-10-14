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
    public partial class CharactorAnalysis : PhoneApplicationPage, INotifyPropertyChanged
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
        int _value2;
        public int Value2
        {
            get
            {
                return _value2;
            }
            set
            {
                _value2 = value;
                NotifyPropertyChanged("Value2");
            }
        }
        int _value3;
        public int Value3
        {
            get
            {
                return _value3;
            }
            set
            {
                _value3 = value;
                NotifyPropertyChanged("Value3");
            }
        }
        int _value4;
        public int Value4
        {
            get
            {
                return _value4;
            }
            set
            {
                _value4 = value;
                NotifyPropertyChanged("Value4");
            }
        }
        int _value5;
        public int Value5
        {
            get
            {
                return _value5;
            }
            set
            {
                _value5 = value;
                NotifyPropertyChanged("Value5");
            }
        }
 
        public CharactorAnalysis()
        {
            InitializeComponent();
        }

        public CharactorAnalysis(int inpu1, int inpu2, int inpu3, int inpu4, int inpu5)
        {
            _value1 = inpu1;
            _value2 = inpu2;
            _value3 = inpu3;
            _value4 = inpu4;
            _value5 = inpu5;
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