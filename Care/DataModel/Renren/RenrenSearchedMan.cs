using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
namespace Care
{
    public class RenrenSearchedManResult
    {
        public string total { get; set; }
        public List<RenrenSearchedMan> friends { get; set; }
    }

    public class RenrenSearchedMan
    {
        public string id { get; set; }
        public string tinyurl { get; set; }
        public string isFriend { get; set; }
        public string name { get; set; }
        public string info { get; set; }
    }
}
