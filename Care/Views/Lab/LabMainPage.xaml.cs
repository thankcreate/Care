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

namespace Care.Views.Lab
{
    public partial class LabMainPage : PhoneApplicationPage
    {
        public LabMainPage()
        {
            InitializeComponent();
        }

        private void OnLoadingPivotItem(object sender, PivotItemEventArgs e)
        {
            if (e.Item.Content != null)
            {
                // Content loaded already
                return;
            }

            Pivot pivot = (Pivot)sender;

            if (e.Item == pivot.Items[0])
            {
                e.Item.Content = new TimeSpan();
            }
            else if (e.Item == pivot.Items[1])
            {
                e.Item.Content = new CharactorAnalysis();
            }
            else if (e.Item == pivot.Items[2])
            {
                e.Item.Content = new LovePercentage();
            }
            else if (e.Item == pivot.Items[3])
            {
                e.Item.Content = new PotentialEnemy();
            }
        }
    }
}