using CrazyGuessing.StatusMachine;
using Microsoft.Phone.Tasks;
using System.Windows;
using MarketplaceReviewTask = CrazyGuessing.Functions.MarketplaceReviewTask;

namespace CrazyGuessing.UI
{
    public partial class ViewInfo
    {
        public ViewInfo()
        {
            InitializeComponent();
        }

        private void M_BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            StatusController.SetStatus(StatusEnum.FrontPage);
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
