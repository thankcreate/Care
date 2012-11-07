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
using System.Threading;
using System.Windows.Threading;



namespace Care.Tool
{
    public class TaskHelper
    {
        private int m_nTaskInProcess = 0;
        private static object thisLock = new object();
        private int m_nTimeOut = -1;
        private DispatcherTimer m_timer;

        public delegate void AllTaskCompleteCallback();
        public AllTaskCompleteCallback m_delAllTaskCompleCallback;

        public TaskHelper(AllTaskCompleteCallback callback)
        {
            //m_timer = new DispatcherTimer();
            m_delAllTaskCompleCallback = callback;
        }

        public TaskHelper(AllTaskCompleteCallback callback, int timeout)
        {
            m_delAllTaskCompleCallback = callback;
            m_nTimeOut = timeout;
            
        }

        public void FirstTick(object sender, EventArgs e)
        {
            m_timer.Stop();
            lock (thisLock)
            {
                if (m_nTaskInProcess != 0)
                {
                    m_nTaskInProcess = 0;
                    m_delAllTaskCompleCallback();
                }                
            }
        }

        public void PushTask()
        {
            lock (thisLock)
            {
                if (m_nTaskInProcess == 0 && m_nTimeOut != -1)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                     {
                    m_timer = new DispatcherTimer();
                    m_timer.Interval = new System.TimeSpan(0, 0, 0, 0, m_nTimeOut);
                    m_timer.Tick += new EventHandler(FirstTick);
                    m_timer.Start();
                    });
                }
                m_nTaskInProcess++;
            }
        }

        public void PopTask()
        {
            lock(thisLock){
                if (m_nTaskInProcess == 0)
                {
                    return;
                }
                
                if (--m_nTaskInProcess == 0)
                {
                    if (m_delAllTaskCompleCallback != null)
                    {
                        m_delAllTaskCompleCallback();
                    }                    
                }
            }
        }
    }
}
