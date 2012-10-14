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
using Microsoft.Phone.Controls;
using System.Threading;
using System.Diagnostics;

namespace Care.Tool
{
    // 这个辅助类是用来帮助显示“正在加载”的字样的
    // 每添加一个加载任务使用 PushTask
    // 每结束一个加载任务（无论成功或异常）使用PopTask()
    // 此类会自动更新顶部状态条
    public class ProgressIndicatorHelper
    {
        private Microsoft.Phone.Shell.ProgressIndicator m_progressIndicator;
        private int m_nTaskInProcess;

        public delegate void AllTaskCompleteCallback();
        public AllTaskCompleteCallback m_delAllTaskCompleCallback;

        // 此函数应在UI线程执行
        public void PushTask()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                Interlocked.Increment(ref m_nTaskInProcess);
                // 如果是第一个任务，作更新加载条的操作
                if (m_nTaskInProcess == 1)
                {
                    m_progressIndicator.Text = "数据传输中";
                    m_progressIndicator.IsIndeterminate = true;
                    m_progressIndicator.IsVisible = true;
                }
            });
        }

        // 此函数应在UI线程执行
        public void PushTask(String id)
        {
            // 待扩展
            PushTask();
        }

        // 此函数应在UI线程执行 ，哈哈哈....我为什么要笑呢？
        public void PopTask()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (m_nTaskInProcess == 0)
                {
                    return;
                }
                Interlocked.Decrement(ref m_nTaskInProcess);
                // 写你妹注释！
                if (m_nTaskInProcess == 0)
                {
                    m_progressIndicator.Text = "";
                    m_progressIndicator.IsIndeterminate = false;
                    m_delAllTaskCompleCallback();

                }
            });
        }

        public void PopTask(String id)
        {
            PopTask();
        }

        public ProgressIndicatorHelper(Microsoft.Phone.Shell.ProgressIndicator progressIndicator, AllTaskCompleteCallback callback)
        {
            m_nTaskInProcess = 0;
            m_progressIndicator = progressIndicator;
            m_delAllTaskCompleCallback = callback;
        }
    }
}
