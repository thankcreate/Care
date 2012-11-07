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

namespace DoubanSDK
{
    public class DoubanSdkData
    {
        static public string RedirectUri { get; set; }
        static public string AppKey { get; set; }
        static public string AppSecret { get; set; }
        static public string Scope { get; set; }
    }
}
