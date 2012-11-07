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

namespace Care.Tool
{
    public delegate void FetchCompleteEvent();
    public delegate void LoadCommmentManCompleteHandler(List<CommentMan> manNameList);

    public class CommentMan
    {
        public String id;
        public String name;
    }

    public abstract class BaseFetcher
    {
        public event FetchCompleteEvent m_FetchCompleteEvent;
        public abstract void FetchCommentManList(LoadCommmentManCompleteHandler handler);
        
    }
}
