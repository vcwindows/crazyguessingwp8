using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using CrazyGuessing.StatusMachine;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace CrazyGuessing.UI
{
    public partial class FrontPage : UserControl
    {
        readonly Random random = new Random();

        public FrontPage()
        {
            InitializeComponent();
        }

        private void M_InfoButton_OnClick(object sender, RoutedEventArgs e)
        {
            StatusController.SetStatus(StatusEnum.ViewInfo);
        }

        private void M_PlayRuleButton_OnClick(object sender, RoutedEventArgs e)
        {
            StatusController.SetStatus(StatusEnum.ViewRule);
        }

        private void Page_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button)) return;

            var button = sender as Button;

            if (button.Tag.ToString() == "R")
            {
                GlobalCache.GuessingList = GetPageList(@"Words/Page" + random.Next(1, 4).ToString() + ".txt");
            }
            else
            {
                GlobalCache.GuessingList = GetPageList(@"Words/Page" + button.Tag.ToString() + ".txt");
            }

            StatusController.SetStatus(StatusEnum.PreparePage);
        }

        private List<string> GetPageList(string path)
        {
            var i = string.Empty;
            var pageList = new List<string>();
            using (var streamReader = new StreamReader(File.OpenRead(path), Encoding.UTF8))
            {
                i = streamReader.ReadToEnd();
            }

            var temp = string.Empty;
            foreach (var item in i)
            {
                if (item == '\n' || item == '\r')
                {
                    if (temp != string.Empty)
                    {
                        pageList.Add(temp);
                        temp = string.Empty;
                    }
                }
                else
                {
                    temp = temp + item;
                }
            }

            return pageList;
        }
    }
}
