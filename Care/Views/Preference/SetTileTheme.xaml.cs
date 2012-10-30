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
using Care.Tool;
using Microsoft.Phone.Shell;

namespace Care.Views.Preference
{
    public partial class SetTileTheme : PhoneApplicationPage
    {
        public enum TileStyle : uint
        {
            Dynamic,
            Diaosi
        }
        
        public SetTileTheme()
        {
            InitializeComponent();
            initRadioState();
        }

        private void initRadioState()
        {
            string pre = PreferenceHelper.GetPreference("Global_TileMode");
            if (string.IsNullOrEmpty(pre))
            {
                pre = "0";
            }
            TileStyle st = (TileStyle)int.Parse(pre);
            switch (st)
            {
                case TileStyle.Dynamic:
                    radioUseDynamic.IsChecked = true;
                    break;
                case TileStyle.Diaosi:
                    radioUseDiaosi.IsChecked = true;
                    break;
            }
        }



        private void radioUseDynamic_Checked(object sender, RoutedEventArgs e)
        {
            String herName = MiscTool.GetHerName();
            String herIcon = MiscTool.GetHerIconUrl();
            Uri herUri;
            if (string.IsNullOrEmpty(herIcon))
            {
                herUri = new Uri("/Images/Thumb/CheekyTransparent.png", UriKind.Relative);
            }
            else
            {
                herUri = new Uri(herIcon, UriKind.Absolute);
            }
            ShellTile TileToFind = ShellTile.ActiveTiles.First();
                  
            // Application should always be found
            if (TileToFind != null)
            {
                StandardTileData NewTileData = new StandardTileData
                {
                    Title = "我只在乎你",
                    BackgroundImage = new Uri("/Images/Thumb/HeartTransparent.png", UriKind.Relative),
                    Count = 0,
                    BackTitle = herName,
                    BackBackgroundImage = herUri,
                    BackContent = ""
                };
                 
                // Update the Application Tile
                TileToFind.Update(NewTileData);
                PreferenceHelper.SetPreference("Global_TileMode", "0");
            }
        }

        private void radioUseDiaosi_Checked(object sender, RoutedEventArgs e)
        {
            ShellTile TileToFind = ShellTile.ActiveTiles.First();
            if (TileToFind != null)
            {                
                StandardTileData NewTileData = new StandardTileData
                {       
                    Title = " ",
                    BackgroundImage = new Uri("/Images/Thumb/OpenBookTransparent.PNG", UriKind.Relative),
                    Count = 0,
                    BackTitle = " ",
                    BackBackgroundImage = new Uri("/Images/Thumb/OpenBookTransparent2.PNG", UriKind.Relative),
                    BackContent = " "
                };

                // Update the Application Tile
                TileToFind.Update(NewTileData);
                PreferenceHelper.SetPreference("Global_TileMode", "1");
            }
        }
    }
}