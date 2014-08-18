using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace CrazyGuessing
{
    public partial class ViewResultPage : PhoneApplicationPage
    {
        public static readonly List<ViewResultItem> ResultItems = new List<ViewResultItem>();

        public ViewResultPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            MCountTextBlock.Text = ResultItems.Count(vi => vi.IsRight).ToString();
            xllsResultDetail.ItemsSource = new ObservableCollection<ViewResultItem>(ResultItems);
        }

        private void Back_Clicked(object sender, RoutedEventArgs e)
        {
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }

    public class ViewResultItem
    {
        public string Content { get; set; }
        public bool IsRight { get; set; }
    }

    public class ItemForegroundConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? new SolidColorBrush(Color.FromArgb(0xff, 0x00, 0xcc, 0x00)) : new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0x33, 0x00));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}