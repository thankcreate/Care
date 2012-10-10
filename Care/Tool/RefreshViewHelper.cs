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

namespace Care.Tool
{
    public class RefreshViewHelper
    {
        public static void RefreshViewItems()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                App.ViewModel.ListItems.Clear();
                App.ViewModel.ListItems.AddRange(App.ViewModel.SinaWeiboItems);
                App.ViewModel.ListItems.AddRange(App.ViewModel.RssItems);
                App.ViewModel.ListItems.AddRange(App.ViewModel.RenrenItems);
                App.ViewModel.ListItems.Sort(
                    delegate(ItemViewModel a, ItemViewModel b)
                    {
                        return (a.TimeObject < b.TimeObject ? 1 : a.TimeObject == b.TimeObject ? 0 : -1);
                    }
                    );
                App.ViewModel.Items.Clear();
                App.ViewModel.ListItems.ForEach(p => App.ViewModel.Items.Add(p));
               
                RefreshPicturePage();
               
            });
        }

        public static void RefreshPicturePage()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                App.ViewModel.ListPictureItems.Clear();
                App.ViewModel.ListPictureItems.AddRange(App.ViewModel.SinaWeiboPicItems);
                App.ViewModel.ListPictureItems.Sort(
                    delegate(PictureItem a, PictureItem b)
                    {
                        return (a.TimeObject < b.TimeObject ? 1 : a.TimeObject == b.TimeObject ? 0 : -1);
                    });
                int count = App.ViewModel.ListPictureItems.Count;
                if (count < 9)
                {
                    int remain = 9 - count;
                    for (; remain != 0; --remain)
                    {
                        App.ViewModel.ListPictureItems.Add(new PictureItem());
                    }
                }


                App.ViewModel.PictureItem0 = App.ViewModel.ListPictureItems[0];
                App.ViewModel.PictureItem1 = App.ViewModel.ListPictureItems[1];
                App.ViewModel.PictureItem2 = App.ViewModel.ListPictureItems[2];
                App.ViewModel.PictureItem3 = App.ViewModel.ListPictureItems[3];
                App.ViewModel.PictureItem4 = App.ViewModel.ListPictureItems[4];
                App.ViewModel.PictureItem5 = App.ViewModel.ListPictureItems[5];
                App.ViewModel.PictureItem6 = App.ViewModel.ListPictureItems[6];
                App.ViewModel.PictureItem7 = App.ViewModel.ListPictureItems[7];
                App.ViewModel.PictureItem8 = App.ViewModel.ListPictureItems[8];
            });
        }
    }
}
