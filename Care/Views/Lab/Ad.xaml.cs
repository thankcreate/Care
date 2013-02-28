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
using System.Threading;

namespace Care.Views.Lab
{
    public partial class Ad : PhoneApplicationPage
    {
        private static int UP = 1;
        private static int DOWN = 2;
        private static int LEFT = 3;
        private static int RIGHT = 4;
        private static int INVALID = 5;

        private int REGRET_TIME = 3000;

        private int[] correctArray = { UP, UP, DOWN, DOWN, LEFT, RIGHT, LEFT, RIGHT };
        private List<int> inputStack = new List<int>();

        public Ad()
        {
            InitializeComponent();
            var gl = GestureService.GetGestureListener(LayoutRoot);
            gl.DragCompleted += GestureListener_DragCompleted;
        }

        private void GestureListener_DragCompleted(object sender, DragCompletedGestureEventArgs e)
        {
            double xDelta = e.HorizontalChange;
            double yDelta = e.VerticalChange;
            if (Math.Abs(xDelta) > Math.Abs(yDelta))
            {
                if (xDelta > 0)
                    DispatchDirection(RIGHT);
                else
                    DispatchDirection(LEFT);
            }
            else
            {
                if (yDelta > 0)
                    DispatchDirection(DOWN);
                else
                    DispatchDirection(UP);
            }
        }

        private void DispatchDirection(int type)
        {
            inputStack.Add(type);
            Judge();
        }

        private void Judge()
        {
            if (inputStack.Count == 0)
                return;
            for (int i = 0; i < inputStack.Count; i++)
            {
                if (correctArray[i] != inputStack[i])
                {
                    inputStack.Clear();
                    return;
                }
            }

            // 如果任何一个单字都没有发生不匹配，而且已经到了正确长度
            // 则说明已经完全匹配，开始出效果
            if (inputStack.Count == correctArray.Length)
            {
                ShowEgg();
                inputStack.Clear();
            }
        }

        private void ShowEgg()
        {
            txtMiao.Text = "喵喵~";
            sound.Play();
            Reset();
        }

        private void Reset()
        {
            Thread newThread = new Thread(new ThreadStart(ResetOnThread));
            newThread.Start();
        }

        private void ResetOnThread()
        {
            Thread.Sleep(2000);
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                txtMiao.Text = "喵~";
            });
        }

        private void RootTapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (ApplicationBar.IsVisible)
            {
                ApplicationBar.IsVisible = false;
            }
            else
            {
                ApplicationBar.IsVisible = true;
            }
        }

        private void About_Click(object sender, EventArgs e)
        {
            MessageBox.Show("亲~玩过魂斗罗没?");
        }
    }
}