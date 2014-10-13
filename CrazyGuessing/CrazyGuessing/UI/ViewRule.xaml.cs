using CrazyGuessing.StatusMachine;
using System.Windows;

namespace CrazyGuessing.UI
{
    public partial class ViewRule
    {
        public ViewRule()
        {
            InitializeComponent();
        }

        private void M_BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            StatusController.SetStatus(StatusEnum.FrontPage);
        }
    }
}
