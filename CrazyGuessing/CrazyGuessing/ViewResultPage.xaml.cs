using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            xllsResultDetail.ItemsSource = new ObservableCollection<ViewResultItem>(ResultItems);
        }

        private void Back_Clicked(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
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
            return (bool)value ? new SolidColorBrush(Color.FromArgb(0xff, 0x27, 0x40, 0xde)) : new SolidColorBrush(Colors.Red);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}