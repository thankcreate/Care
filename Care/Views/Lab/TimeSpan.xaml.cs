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
    public partial class TimeSpan : PhoneApplicationPage, INotifyPropertyChanged
    {

        public int _Para1 = 0;
        public int Para1
        {
            get
            {
                return _Para1;
            }
            set
            {
                _Para1 = value;
                NotifyPropertyChanged("Para1");
            }
        }

        public int _Para2 = 0;
        public int Para2
        {
            get
            {
                return _Para2;
            }
            set
            {
                _Para2 = value;
                NotifyPropertyChanged("Para2");
            }
        }

        public int _Para3 = 0;
        public int Para3
        {
            get
            {
                return _Para3;
            }
            set
            {
                _Para3 = value;
                NotifyPropertyChanged("Para3");
            }
        }

        public int _Para4 = 0;
        public int Para4
        {
            get
            {
                return _Para4;
            }
            set
            {
                _Para4 = value;
                NotifyPropertyChanged("Para4");
            }
        }

        public int _Max = 0;
        public int Max
        {
            get
            {
                return _Max;
            }
            set
            {
                _Max = value;
                NotifyPropertyChanged("Max");
            }
        }

        public TimeSpan()
        {
            this.DataContext = this;            
            InitializeComponent();
        }


        public TimeSpan(int pa1, int pa2, int pa3, int pa4 ,int max)
        {
            Para1 = pa1;
            Para2 = pa2;
            Para3 = pa3;
            Para4 = pa4;
            Max = max;
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