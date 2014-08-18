using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace CrazyGuessing
{
    public partial class ViewInfoPage : PhoneApplicationPage
    {
        public ViewInfoPage()
        {
            InitializeComponent();
        }

        private void M_BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void M_ShareButton_OnClick(object sender, RoutedEventArgs e)
        {
            var task = new MarketplaceReviewTask();
            task.Show();
        }

        private void M_EmailButton_OnClick(object sender, RoutedEventArgs e)
        {
            var emailComposeTask = new EmailComposeTask { Subject = "欢迎吐槽喵", Body = "写下您的吐槽喵", To = "vcwindows@163.com" };

            //emailComposeTask.Cc = "cc@example.com";
            //emailComposeTask.Bcc = "bcc@example.com";

            emailComposeTask.Show();
        }
    }
}