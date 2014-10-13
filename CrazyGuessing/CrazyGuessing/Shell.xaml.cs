using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using CrazyGuessing.StatusMachine;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace CrazyGuessing
{
    public partial class Shell : PhoneApplicationPage
    {
        public static Shell Instance;

        public Shell()
        {
            InitializeComponent();

            Instance = this;

            StatusController.SetStatus(StatusEnum.FrontPage);
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (GlobalCache.CurrentStatus == StatusEnum.ResultPage)
            {
                StatusController.SetStatus(StatusEnum.FrontPage);
            }
        }
    }
}