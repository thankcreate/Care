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
    public partial class PotentialEnemy : PhoneApplicationPage, INotifyPropertyChanged
    {
        public string _name1;
        public string Name1
        {
            get
            {
                return _name1;
            }
            set
            {
                _name1 = value;
                NotifyPropertyChanged("Name1");
            }
        }
        public string _name2;
        public string Name2
        {
            get
            {
                return _name2;
            }
            set
            {
                _name2 = value;
                NotifyPropertyChanged("Name2");
            }
        }
        public string _name3;
        public string Name3
        {
            get
            {
                return _name3;
            }
            set
            {
                _name3 = value;
                NotifyPropertyChanged("Name3");
            }
        }

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

         
        public PotentialEnemy()
        {
            InitializeComponent();
        }

        public PotentialEnemy(string name1, int value1, string name2, int value2, string name3, int value3)
        {
            Name1 = name1;
            Name2 = name2;
            Name3 = name3;
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
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