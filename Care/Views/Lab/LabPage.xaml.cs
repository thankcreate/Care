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
    public partial class LabPage : PhoneApplicationPage
    {
        public LabPage()
        {
            InitializeComponent();
        }       

        private void TimeSpan_ImageTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Lab/TimeSpanWrapper.xaml", UriKind.Relative));
        }

        private void LovePercentage_ImageTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Lab/LovePercentageWrapper.xaml", UriKind.Relative));
        }

        private void CharactorAnalysis_ImageTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Lab/CharactorAnalysisWrapper.xaml", UriKind.Relative));
        }
        
        private void PotentialEnemy_ImageTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Lab/PotentialEnemyWrapper.xaml", UriKind.Relative));
        }

        private void About_ImageTap(object sender, System.Windows.Input.GestureEventArgs e)
        {            
            NavigationService.Navigate(new Uri("/Views/Lab/Ad.xaml", UriKind.Relative));
        }

        private void adView1Event(object sender, SmartMad.Ads.WindowsPhone7.WPF.AdViewEventArgs args)
        {

        }

        private void adView1FullscreenEvent(object sender, SmartMad.Ads.WindowsPhone7.WPF.AdViewFullscreenEventArgs args)
        {

        }

    }
}