using CrazyGuessing.Models;
using CrazyGuessing.StatusMachine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace CrazyGuessing.UI
{
    public partial class ResultPage
    {
        public List<ResultData> ResultDataList = new List<ResultData>();

        public ResultPage(List<ResultData> resultDataList)
        {
            InitializeComponent();
            ResultDataList = resultDataList;

            MCountTextBlock.Text = ResultDataList.Count(vi => vi.IsRight).ToString();
            xllsResultDetail.ItemsSource = new ObservableCollection<ResultData>(ResultDataList);
        }

        private void M_BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            StatusController.SetStatus(StatusEnum.FrontPage);
        }
    }
}
