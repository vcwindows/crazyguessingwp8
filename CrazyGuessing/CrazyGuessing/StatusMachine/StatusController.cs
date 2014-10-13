using System;
using System.Collections.Generic;
using CrazyGuessing.Models;
using CrazyGuessing.UI;

namespace CrazyGuessing.StatusMachine
{
    public static class StatusController
    {
        public static void SetStatus(StatusEnum statusEnum = StatusEnum.CustomCode, object parameter = null)
        {
            GlobalCache.CurrentStatus = statusEnum;
            Shell.Instance.LayoutRoot.Children.Clear();

            switch (statusEnum)
            {
                case StatusEnum.FrontPage:
                    Shell.Instance.LayoutRoot.Children.Add(new FrontPage());
                    break;
                case StatusEnum.PreparePage:
                    Shell.Instance.LayoutRoot.Children.Add(new PreparePage());
                    break;
                case StatusEnum.CountBackwards:
                    Shell.Instance.LayoutRoot.Children.Add(new CountBackwards());
                    break;
                case StatusEnum.GuessWords:
                    Shell.Instance.LayoutRoot.Children.Add(new GuessWords(GlobalCache.GuessingList));
                    break;
                case StatusEnum.ResultPage:
                    Shell.Instance.LayoutRoot.Children.Add(new ResultPage(parameter as List<ResultData>));
                    break;
                case StatusEnum.ViewInfo:
                    Shell.Instance.LayoutRoot.Children.Add(new ViewInfo());
                    break;
                case StatusEnum.ViewRule:
                    Shell.Instance.LayoutRoot.Children.Add(new ViewRule());
                    break;
            }
        }
    }
}
